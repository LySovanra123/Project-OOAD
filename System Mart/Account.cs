using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace System_Mart
{
    public partial class Account : Form
    {
        String stringConnection = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=MartDB;Integrated Security=True";
        public Account()
        {
            InitializeComponent();
            this.Load += MainAccount_Load;
            btnDisable.Enabled = false;
            btnEnable.Enabled = false;
            txtAdminName.Text = Session.SessionName;
        }

        private void MainAccount_Load(object sender, EventArgs e)
        {
            LoadAccountView();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnAddAdmin_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtName.Text) ||
                String.IsNullOrEmpty(txtPassword.Text) ||
                String.IsNullOrEmpty(txtConfirmPassword.Text) ||
                String.IsNullOrEmpty(txtPosition.Text))
            {
                MessageBox.Show("Please enter username, password, confirm password, and position.",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string name = txtName.Text.Trim();
                string pass = txtPassword.Text.Trim();
                string confirm = txtConfirmPassword.Text.Trim();
                string position = txtPosition.Text.Trim();

                if (!pass.Equals(confirm))
                {
                    MessageBox.Show("Password and Confirm Password do not match!",
                        "Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(stringConnection))
                {
                    conn.Open();

                    // Check if the account already exists
                    string querySelect = "SELECT COUNT(*) FROM AccountAdmins WHERE adminName=@name";
                    using (SqlCommand cmdCheck = new SqlCommand(querySelect, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@name", name);
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
                            // 1️⃣ Insert the new account
                            string queryInsert = @"INSERT INTO AccountAdmins(adminName, adminPassword, adminPosition, adminStatus)
                                           VALUES(@name, @password, @position, @status);
                                           SELECT SCOPE_IDENTITY();";

                            int newAdminId;
                            using (SqlCommand cmdInsert = new SqlCommand(queryInsert, conn, transaction))
                            {
                                cmdInsert.Parameters.AddWithValue("@name", name);
                                cmdInsert.Parameters.AddWithValue("@password", pass);
                                cmdInsert.Parameters.AddWithValue("@position", position);
                                cmdInsert.Parameters.AddWithValue("@status", "enable");

                                // get new adminID
                                newAdminId = Convert.ToInt32(cmdInsert.ExecuteScalar());
                            }

                            // 2️⃣ Find corresponding employee ID
                            string queryEmployee = "SELECT eId FROM Employees WHERE eName=@name";
                            int employeeId;
                            using (SqlCommand cmdEmp = new SqlCommand(queryEmployee, conn, transaction))
                            {
                                cmdEmp.Parameters.AddWithValue("@name", name);
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

                            // 3️⃣ Insert mapping record
                            string queryMap = "INSERT INTO CashierAdminMapping (eId, adminID) VALUES (@eId, @adminID)";
                            using (SqlCommand cmdMap = new SqlCommand(queryMap, conn, transaction))
                            {
                                cmdMap.Parameters.AddWithValue("@eId", employeeId);
                                cmdMap.Parameters.AddWithValue("@adminID", newAdminId);
                                cmdMap.ExecuteNonQuery();
                            }

                            // 4️⃣ Update employee status
                            string queryUpdate = "UPDATE Employees SET eStatus=@eStatus WHERE eId=@eId";
                            using (SqlCommand cmdUpdate = new SqlCommand(queryUpdate, conn, transaction))
                            {
                                cmdUpdate.Parameters.AddWithValue("@eStatus", "create");
                                cmdUpdate.Parameters.AddWithValue("@eId", employeeId);
                                cmdUpdate.ExecuteNonQuery();
                            }

                            // 5️⃣ Commit all changes
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
                btnRefresh_Click(sender, e);
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


        private void LoadAccountView()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(stringConnection))
                {
                    String query = "SELECT * FROM AccountAdmins";

                    using (SqlDataAdapter sda = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        dgvAccount.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                        // Run on UI thread to avoid race condition
                        this.BeginInvoke(new MethodInvoker(() =>
                        {
                            dgvAccount.DataSource = dt;
                            dgvAccount.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex);
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
            MainAdmin mainAdmin = new MainAdmin();
            mainAdmin.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty(txtSearchName.Text))
            {
                MessageBox.Show("Please enter name.", "Warning",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                String name = txtName.Text.Trim();
                using (SqlConnection conn = new SqlConnection(stringConnection))
                {
                    String query = "SELECT * FROM AccountAdmins WHERE adminName=@name";
                    using(SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);

                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        if(dt.Rows.Count > 0)
                        {
                            dgvAccount.DataSource = dt;
                        }
                        else
                        {
                            MessageBox.Show("No account found with that name.", "Not Found",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                            dgvAccount.DataSource = null;
                        }
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

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            String name = Session.SessionName;
            ResetPassword resetPassword = new ResetPassword(name);
            resetPassword.Show();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
            Login login = Application.OpenForms.OfType<Login>().FirstOrDefault();
            if (login != null)
            {
                login.clearForm();
                login.Show();
            }
        }

        private void SelectData_Click(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;

                DataGridViewRow row = dgvAccount.Rows[e.RowIndex];

                if (dgvAccount.Columns.Contains("adminName"))
                    txtName.Text = row.Cells["adminName"].Value?.ToString() ?? "";

                if (dgvAccount.Columns.Contains("adminPassword"))
                {
                    txtPassword.Text = row.Cells["adminPassword"].Value?.ToString() ?? "";
                    txtConfirmPassword.Text = txtPassword.Text;
                }

                if (dgvAccount.Columns.Contains("adminPosition"))
                    txtPosition.Text = row.Cells["adminPosition"].Value?.ToString() ?? "";
                DisplayButtun();

                if (string.IsNullOrEmpty(txtName.Text) && dgvAccount.Columns.Contains("eName"))
                    txtName.Text = row.Cells["eName"].Value?.ToString() ?? "";

                if (string.IsNullOrEmpty(txtPosition.Text) && dgvAccount.Columns.Contains("ePosition"))
                    txtPosition.Text = row.Cells["ePosition"].Value?.ToString() ?? "";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtName.Clear();
            txtPassword.Clear();
            txtConfirmPassword.Clear();
            txtPosition.Clear();
            lblTitle.Text = "List Account";
            LoadAccountView();

        }

        private void cbbPosition_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAddEmployee_Click(object sender, EventArgs e)
        {
            txtName.Clear();
            txtPassword.Clear();
            txtConfirmPassword.Clear();
            txtPosition.Clear();
            lblTitle.Text = "List Employee";
            LoadEmployeeCashier();
        }
        private void LoadEmployeeCashier()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(stringConnection))
                {
                    String query = "SELECT eId,eName,ePosition FROM Employees WHERE (ePosition=@positionC OR ePosition=@positionM OR ePosition=@positionS) AND eStatus=@status";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@positionC", "Cashier");
                        cmd.Parameters.AddWithValue("@positionM", "Manager");
                        cmd.Parameters.AddWithValue("@positionS", "Stocker");
                        cmd.Parameters.AddWithValue("@status", "Not yet");
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                            dgvAccount.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                            // Assign DataSource safely on UI thread
                            this.BeginInvoke(new MethodInvoker(() =>
                            {
                                dgvAccount.DataSource = dt;
                                dgvAccount.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex);
            }
        }

        private void btnDisable_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtPosition.Text) || String.IsNullOrEmpty(txtName.Text) || String.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Please enter your data on list", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                String Name = txtName.Text.Trim();
                String position = txtPosition.Text.Trim();
                String password = txtPassword.Text.Trim();

                using (SqlConnection conn = new SqlConnection(stringConnection))
                {
                    String query = "UPDATE AccountAdmins SET adminStatus=@status WHERE adminName=@name AND adminPassword=@password AND adminPosition=@position";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", Name);
                        cmd.Parameters.AddWithValue("@status", "disable");
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@position", position);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();

                    }
                }
                btnRefresh_Click(sender, e);

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

        private void DisplayButtun()
        {
            try
            {
                string name = txtName.Text.Trim();

                using (SqlConnection conn = new SqlConnection(stringConnection))
                {
                    string query = "SELECT adminStatus FROM AccountAdmins WHERE adminName=@name";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        conn.Open();
                        Object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            string status = result.ToString().Trim();
                            if (status.Equals("enable"))
                            {
                                btnEnable.Enabled = false;
                                btnDisable.Enabled = true;
                            }
                            else
                            {
                                btnEnable.Enabled = true;
                                btnDisable.Enabled = false;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex);
            }
        }

        private void btnEnable_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtPosition.Text) || String.IsNullOrEmpty(txtName.Text) || String.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Please enter your data on list", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                String Name = txtName.Text.Trim();
                String position = txtPosition.Text.Trim();
                String password = txtPassword.Text.Trim();

                using (SqlConnection conn = new SqlConnection(stringConnection))
                {
                    String query = "UPDATE AccountAdmins SET adminStatus=@status WHERE adminName=@name AND adminPassword=@password AND adminPosition=@position";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", Name);
                        cmd.Parameters.AddWithValue("@status", "enable");
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@position", position);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();

                    }
                }
                btnRefresh_Click(sender, e);

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
