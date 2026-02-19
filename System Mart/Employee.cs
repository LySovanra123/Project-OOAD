using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace System_Mart
{
    public partial class EmployeeForm : Form
    {

        byte[] imageData =  null;
        public EmployeeForm()
        {
            InitializeComponent();
            LoadEmployeeView();
            pnlMessageEmployee.Hide();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "PNG Image|*.png";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        Image img = Image.FromFile(ofd.FileName);
                        Bitmap resized = new Bitmap(img, new Size(150, 150));

                        using (MemoryStream ms = new MemoryStream())
                        {
                            resized.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            imageData = ms.ToArray();
                        }

                        lblMessageEmployee.Text = "Image imported and resized to 180x180!";
                        pnlMessageEmployee.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtEmployeeName.Text) || String.IsNullOrEmpty(cbbGender.Text) || String.IsNullOrEmpty(cbbPosition.Text) ||
    String.IsNullOrEmpty(txtPhoneNumber.Text) || String.IsNullOrEmpty(txtAddress.Text) || String.IsNullOrEmpty(txtSalary.Text) || imageData == null)
            {
                MessageBox.Show("Please enter employee name, gender, position, phone number, salary, address, and import an image.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {

                SqlConnection conn = DataBaseConnection.Instance.GetConnection();

                string name = txtEmployeeName.Text.Trim();
                string phoneNumber = txtPhoneNumber.Text.Trim();

                // Check if employee already exists
                string queryCheck = "SELECT COUNT(*) FROM Employees WHERE eName=@name AND ePhoneNumber=@phone";
                using (SqlCommand cmdCheck = new SqlCommand(queryCheck, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@name", name);
                    cmdCheck.Parameters.AddWithValue("@phone", phoneNumber);

                    int count = (int)cmdCheck.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("Employee with the same name and phone number already exists!", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // If not exists, insert the new employee
                if (!decimal.TryParse(txtSalary.Text, out decimal salary))
                {
                    MessageBox.Show("Please enter a valid number for salary.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string gender = cbbGender.Text;
                string position = cbbPosition.Text;
                string address = txtAddress.Text.Trim();
                DateTime dateStartWork = dtpStartWorkDate.Value;
                string status = "Not yet"; // default status

                string queryInsert = @"INSERT INTO Employees
                               (eName, eImage, eGender, ePosition, ePhoneNumber, eDateStartWork, eSalary, eAddress, eStatus)
                               VALUES
                               (@name, @image, @gender, @position, @phoneNumber, @dateStartWork, @salary, @address, @status)";

                using (SqlCommand cmdInsert = new SqlCommand(queryInsert, conn))
                {
                    cmdInsert.Parameters.AddWithValue("@name", name);
                    cmdInsert.Parameters.Add("@image", SqlDbType.VarBinary, imageData.Length).Value = imageData;
                    cmdInsert.Parameters.AddWithValue("@salary", salary);
                    cmdInsert.Parameters.AddWithValue("@gender", gender);
                    cmdInsert.Parameters.AddWithValue("@position", position);
                    cmdInsert.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                    cmdInsert.Parameters.AddWithValue("@dateStartWork", dateStartWork);
                    cmdInsert.Parameters.AddWithValue("@address", address);
                    cmdInsert.Parameters.AddWithValue("@status", status);

                    cmdInsert.ExecuteNonQuery();

                    pnlMessageEmployee.Show();
                    lblMessageEmployee.Text = "Added Employee " + name + " successfully.";

                    imageData = null; // reset image after insertion
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

        private void btnView_Click(object sender, EventArgs e)
        {
            LoadEmployeeView();
        }

        private void LoadEmployeeView()
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
                DataTable dt = new DataTable();
                sda.Fill(dt);
                dgvEmployee.DataSource = dt;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtEmployeeName.Clear();
            txtAddress.Clear();
            txtPhoneNumber.Clear();
            txtSalary.Clear();
            cbbGender.SelectedIndex = -1;
            cbbPosition.SelectedIndex = -1;
            imageData = null;
            picbProfile.Image = null;
            lblMessageEmployee.Text = "";
            pnlMessageEmployee.Hide();
            lblId.Text = "";
            lblName.Text = "";
            lblPosition.Text = "";
        }

        private void SelectDataEmployee(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row =dgvEmployee.Rows[e.RowIndex];
                    lblId.Text = row.Cells["eId"].Value.ToString();
                    txtEmployeeName.Text = row.Cells["eName"].Value.ToString();
                    txtSalary.Text = row.Cells["eSalary"].Value.ToString();
                    txtPhoneNumber.Text = row.Cells["ePhoneNumber"].Value.ToString();
                    txtAddress.Text = row.Cells["eAddress"].Value.ToString();
                    cbbGender.Text = row.Cells["eGender"].Value.ToString();
                    cbbPosition.Text = row.Cells["ePosition"].Value.ToString();
                    lblName.Text = row.Cells["eName"].Value.ToString();
                    lblPosition.Text = row.Cells["ePosition"].Value.ToString();
                    dtpStartWorkDate.Value = Convert.ToDateTime(row.Cells["eDateStartWork"].Value);

                    if (row.Cells["eImage"].Value != DBNull.Value)
                    {
                        byte[] imageData = (byte[])row.Cells["eImage"].Value;
                        using (MemoryStream ms = new MemoryStream(imageData))
                        {
                            picbProfile.Image = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        picbProfile.Image = null;
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void lblPosition_Click(object sender, EventArgs e)
        {

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
            MainAdmin mainAdmin = new MainAdmin();
            mainAdmin.Show();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtEmployeeName.Text) || String.IsNullOrEmpty(txtSalary.Text) ||
                String.IsNullOrEmpty(txtPhoneNumber.Text) || String.IsNullOrEmpty(txtAddress.Text) ||
                String.IsNullOrEmpty(cbbGender.Text) || String.IsNullOrEmpty(cbbPosition.Text))
            {
                MessageBox.Show("Please enter employee name, salary, gender, position, phone number, address, and optionally import an image.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string name = txtEmployeeName.Text.Trim();
                string gender = cbbGender.Text;
                string position = cbbPosition.Text;
                string phoneNumber = txtPhoneNumber.Text.Trim();
                string address = txtAddress.Text.Trim();
                DateTime dateStartWork = dtpStartWorkDate.Value;

                if (!decimal.TryParse(txtSalary.Text, out decimal salary))
                {
                    MessageBox.Show("Please enter a valid number for salary.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(lblId.Text, out int id))
                {
                    MessageBox.Show("Invalid employee ID.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

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

                        if (imageData != null)
                            query += ", eImage=@image";

                        query += " WHERE eId=@id";

                        using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@name", name);
                            cmd.Parameters.AddWithValue("@salary", salary);
                            cmd.Parameters.AddWithValue("@gender", gender);
                            cmd.Parameters.AddWithValue("@position", position);
                            cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                            cmd.Parameters.AddWithValue("@address", address);
                            cmd.Parameters.AddWithValue("@dateStartWork", dateStartWork);
                            cmd.Parameters.AddWithValue("@id", id);

                            if (imageData != null)
                                cmd.Parameters.Add("@image", SqlDbType.VarBinary, imageData.Length).Value = imageData;

                            cmd.ExecuteNonQuery();
                        }

                        // If employee is Cashier or Manager, update AccountAdmins table
                        if (position.Equals("Cashier") || position.Equals("Manager") || position.Equals("Stocker"))
                        {
                            string queryAdmin = @"UPDATE AccountAdmins
                                          SET adminName=@name, adminPosition=@position
                                          WHERE adminID IN (SELECT adminID FROM CashierAdminMapping WHERE eId=@id)";
                            using (SqlCommand cmdAdmin = new SqlCommand(queryAdmin, conn, transaction))
                            {
                                cmdAdmin.Parameters.AddWithValue("@name", name);
                                cmdAdmin.Parameters.AddWithValue("@position", position);
                                cmdAdmin.Parameters.AddWithValue("@id", id);

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

                        pnlMessageEmployee.Show();
                        lblMessageEmployee.Text = $"Updated Employee ID {id} successfully.";

                        imageData = null; // reset image
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(lblId.Text))
            {
                MessageBox.Show("Please enter Employee id", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (!int.TryParse(lblId.Text, out int id))
                {
                    MessageBox.Show("Please enter a valid number for id.",
                        "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SqlConnection conn = DataBaseConnection.Instance.GetConnection();

                // Begin transaction to ensure all deletions happen together
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Delete from CashierProducts first
                        string queryDeleteProducts = "DELETE FROM CashierProducts WHERE eId=@id";
                        using (SqlCommand cmdProducts = new SqlCommand(queryDeleteProducts, conn, transaction))
                        {
                            cmdProducts.Parameters.AddWithValue("@id", id);
                            cmdProducts.ExecuteNonQuery();
                        }

                        // Delete from CashierAdminMapping
                        string queryDeleteMapping = "DELETE FROM CashierAdminMapping WHERE eId=@id";
                        using (SqlCommand cmdMapping = new SqlCommand(queryDeleteMapping, conn, transaction))
                        {
                            cmdMapping.Parameters.AddWithValue("@id", id);
                            cmdMapping.ExecuteNonQuery();
                        }

                        // Finally, delete from Employees
                        string queryDeleteEmployee = "DELETE FROM Employees WHERE eId=@id";
                        using (SqlCommand cmdEmployee = new SqlCommand(queryDeleteEmployee, conn, transaction))
                        {
                            cmdEmployee.Parameters.AddWithValue("@id", id);
                            int rowsAffected = cmdEmployee.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                pnlMessageEmployee.Show();
                                lblMessageEmployee.Text = $"Deleted Employee id: {id} successfully.";
                            }
                            else
                            {
                                MessageBox.Show("Employee not found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            catch (SqlException se)
            {
                MessageBox.Show("Database error: " + se.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtEmployeeName.Text))
            {
                MessageBox.Show("Please enter barCode of Product.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
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
                    cmd.Parameters.AddWithValue("@name", txtEmployeeName.Text);
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        int row = sda.Fill(dt);
                        if (row > 0)
                        {
                            dgvEmployee.DataSource = dt;
                        }
                        else
                        {
                            dgvEmployee.DataSource = null;
                            pnlMessageEmployee.Show();
                            lblMessageEmployee.Text = "Emplyee not found!";
                        }
                    }

                }

            }
            catch (FormatException fe)
            {
                MessageBox.Show("Input format is invalid: " + fe.Message, "Format Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
    }
}
