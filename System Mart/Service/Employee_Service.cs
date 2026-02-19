using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System_Mart.Model;
using Microsoft.Office.Interop.Excel;
using System.Net;

namespace System_Mart.Service
{
    public class Employee_Service
    {
        public Employee_Service() { }

        private List<Employee_Model> _employees = new List<Employee_Model>();

        public void AddEmployee(Employee_Model employee) 
        {
            try
            {
                SqlConnection conn = DataBaseConnection.Instance.GetConnection();

                // Check if employee already exists
                string queryCheck = "SELECT COUNT(*) FROM Employees WHERE eName=@name AND ePhoneNumber=@phone";
                using (SqlCommand cmdCheck = new SqlCommand(queryCheck, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@name", employee.Name);
                    cmdCheck.Parameters.AddWithValue("@phone", employee.PhoneNumber);

                    int count = (int)cmdCheck.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("Employee with the same name and phone number already exists!", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                string status = "Not yet"; // default status

                string queryInsert = @"INSERT INTO Employees
                               (eName, eImage, eGender, ePosition, ePhoneNumber, eDateStartWork, eSalary, eAddress, eStatus)
                               VALUES
                               (@name, @image, @gender, @position, @phoneNumber, @dateStartWork, @salary, @address, @status)";

                using (SqlCommand cmdInsert = new SqlCommand(queryInsert, conn))
                {
                    cmdInsert.Parameters.AddWithValue("@name", employee.Name);
                    cmdInsert.Parameters.Add("@image", SqlDbType.VarBinary, employee.ImageData.Length).Value = employee.ImageData;
                    cmdInsert.Parameters.AddWithValue("@salary", employee.Salary);
                    cmdInsert.Parameters.AddWithValue("@gender", employee.Gender);
                    cmdInsert.Parameters.AddWithValue("@position", employee.Position);
                    cmdInsert.Parameters.AddWithValue("@phoneNumber", employee.PhoneNumber);
                    cmdInsert.Parameters.AddWithValue("@dateStartWork", employee.SDW1);
                    cmdInsert.Parameters.AddWithValue("@address", employee.Address);
                    cmdInsert.Parameters.AddWithValue("@status", status);

                    cmdInsert.ExecuteNonQuery();

                }
            }
            catch (SqlException se)
            {
                MessageBox.Show("Database error: " + se.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public System.Data.DataTable GetAllEmployee()
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            String query = @"SELECT 
                    eId,
                    eName,
                    eImage,
                    eGender, 
                    ePosition, 
                    ePhoneNumber, 
                    eSalary,
                    eDateStartWork,
                    eAddress 
                FROM Employees";

            using (SqlDataAdapter sda = new SqlDataAdapter(query, conn))
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                sda.Fill(dt);
                return dt;
            }
        }

        public void editEmaployee(Employee_Model employee)
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            using (SqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    // Update Employees table
                    string query = @"UPDATE Employees SET 
                                    eName=@name,
                                    eSalary=@salary,
                                    eGender=@gender,
                                    ePosition=@position,
                                    ePhoneNumber=@phoneNumber,
                                    eAddress=@address,
                                    eDateStartWork=@dateStartWork";

                    if (employee.ImageData != null)
                        query += ", eImage=@image";

                    query += " WHERE eId=@id";

                    using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@name", employee.Name);
                        cmd.Parameters.AddWithValue("@salary", employee.Salary);
                        cmd.Parameters.AddWithValue("@gender", employee.Gender);
                        cmd.Parameters.AddWithValue("@position", employee.Position);
                        cmd.Parameters.AddWithValue("@phoneNumber", employee.PhoneNumber);
                        cmd.Parameters.AddWithValue("@address", employee.Address);
                        cmd.Parameters.AddWithValue("@dateStartWork", employee.SDW1);
                        cmd.Parameters.AddWithValue("@id", employee.Id);

                        if (employee.ImageData != null)
                            cmd.Parameters.Add("@image", SqlDbType.VarBinary, employee.ImageData.Length).Value = employee.ImageData;

                        cmd.ExecuteNonQuery();
                    }

                    // If employee is Cashier or Manager, update AccountAdmins table
                    if (employee.Position.Equals("Cashier") || employee.Position.Equals("Manager") || employee.Position.Equals("Stocker"))
                    {
                        string queryAdmin = @"UPDATE AccountAdmins
                                          SET adminName=@name, adminPosition=@position
                                          WHERE adminID IN (SELECT adminID FROM CashierAdminMapping WHERE eId=@id)";
                        using (SqlCommand cmdAdmin = new SqlCommand(queryAdmin, conn, transaction))
                        {
                            cmdAdmin.Parameters.AddWithValue("@name", employee.Name);
                            cmdAdmin.Parameters.AddWithValue("@position", employee.Position);
                            cmdAdmin.Parameters.AddWithValue("@id", employee.Id);

                            object result = cmdAdmin.ExecuteNonQuery();
                            if (result != null)
                            {
                                MessageBox.Show("Sucessful!");
                            }
                            else
                            {
                                MessageBox.Show("Error");
                            }
                        }
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public bool deleteEmployee(Employee_Model employee) {
            var d_employee = _employees.FirstOrDefault( e => e.Id == employee.Id );

            if (d_employee != null) {
                _employees.Remove( d_employee );

                return true;
            }
            return false;
        }
    }
}
