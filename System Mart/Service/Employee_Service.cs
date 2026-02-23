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
using System.Xml.Linq;

namespace System_Mart.Service
{
    public class Employee_Service
    {
        public Employee_Service() { }

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

            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            // Begin transaction to ensure all deletions happen together
            SqlTransaction sqlTransaction = conn.BeginTransaction();

            using (SqlTransaction transaction = sqlTransaction)
            {
                try
                {
                    // Delete from CashierProducts first
                    string queryDeleteProducts = "DELETE FROM CashierProducts WHERE eId=@id";
                    using (SqlCommand cmdProducts = new SqlCommand(queryDeleteProducts, conn, transaction))
                    {
                        cmdProducts.Parameters.AddWithValue("@id", employee.Id);
                        cmdProducts.ExecuteNonQuery();
                    }

                    // Delete from CashierAdminMapping
                    string queryDeleteMapping = "DELETE FROM CashierAdminMapping WHERE eId=@id";
                    using (SqlCommand cmdMapping = new SqlCommand(queryDeleteMapping, conn, transaction))
                    {
                        cmdMapping.Parameters.AddWithValue("@id", employee.Id);
                        cmdMapping.ExecuteNonQuery();
                    }

                    // Finally, delete from Employees
                    string queryDeleteEmployee = "DELETE FROM Employees WHERE eId=@id";
                    using (SqlCommand cmdEmployee = new SqlCommand(queryDeleteEmployee, conn, transaction))
                    {
                        cmdEmployee.Parameters.AddWithValue("@id", employee.Id);
                        int rowsAffected = cmdEmployee.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                           return true;
                        }
                        else
                        {
                            return false;
                        }

                    }
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public System.Data.DataTable searchEmployee(string nameSearch)
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
                    FROM Employees WHERE eName=@name";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", nameSearch);
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    System.Data.DataTable dt = new System.Data.DataTable();
                    int row = sda.Fill(dt);
                    if (row > 0)
                    {
                        return dt;
                    }
                    else
                    {
                        return null;
                    }
                }

            }
        }


        //=========================Save new Admin OR Cashier=======================
        public void saveAccount(Account_Model model)
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            // Check if the account already exists
            string querySelect = "SELECT COUNT(*) FROM AccountAdmins WHERE adminName=@name";
            using (SqlCommand cmdCheck = new SqlCommand(querySelect, conn))
            {
                cmdCheck.Parameters.AddWithValue("@name", model.Name);
                int count = (int)cmdCheck.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("This account already exists!",
                        "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Create a transaction for data consistency
            using (SqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    //Insert the new account
                    string queryInsert = @"INSERT INTO AccountAdmins(adminName, adminPassword, adminPosition, adminStatus)
                                           VALUES(@name, @password, @position, @status);
                                           SELECT SCOPE_IDENTITY();";

                    int newAdminId;
                    using (SqlCommand cmdInsert = new SqlCommand(queryInsert, conn, transaction))
                    {
                        cmdInsert.Parameters.AddWithValue("@name", model.Name);
                        cmdInsert.Parameters.AddWithValue("@password", model.Password);
                        cmdInsert.Parameters.AddWithValue("@position", model.Position);
                        cmdInsert.Parameters.AddWithValue("@status", "enable");

                        // get new adminID
                        newAdminId = Convert.ToInt32(cmdInsert.ExecuteScalar());
                    }

                    //Find corresponding employee ID
                    string queryEmployee = "SELECT eId FROM Employees WHERE eName=@name";
                    int employeeId;
                    using (SqlCommand cmdEmp = new SqlCommand(queryEmployee, conn, transaction))
                    {
                        cmdEmp.Parameters.AddWithValue("@name", model.Name);
                        object result = cmdEmp.ExecuteScalar();
                        if (result == null)
                        {
                            MessageBox.Show("No employee found with this name. Cannot map account.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            transaction.Rollback();
                            return;
                        }
                        employeeId = Convert.ToInt32(result);
                    }

                    //Insert mapping record
                    string queryMap = "INSERT INTO CashierAdminMapping (eId, adminID) VALUES (@eId, @adminID)";
                    using (SqlCommand cmdMap = new SqlCommand(queryMap, conn, transaction))
                    {
                        cmdMap.Parameters.AddWithValue("@eId", employeeId);
                        cmdMap.Parameters.AddWithValue("@adminID", newAdminId);
                        cmdMap.ExecuteNonQuery();
                    }

                    //Update employee status
                    string queryUpdate = "UPDATE Employees SET eStatus=@eStatus WHERE eId=@eId";
                    using (SqlCommand cmdUpdate = new SqlCommand(queryUpdate, conn, transaction))
                    {
                        cmdUpdate.Parameters.AddWithValue("@eStatus", "create");
                        cmdUpdate.Parameters.AddWithValue("@eId", employeeId);
                        cmdUpdate.ExecuteNonQuery();
                    }

                    //Commit all changes
                    transaction.Commit();
                    MessageBox.Show("New admin account created and linked successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception exTrans)
                {
                    transaction.Rollback();
                    MessageBox.Show("Failed to create admin: " + exTrans.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
