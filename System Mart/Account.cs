using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using System_Mart.Model;
using System_Mart.Service;

namespace System_Mart
{
    public partial class Account : Form
    {

        private Manager manager = new Manager();
        private Account_Service service = new Account_Service();
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
            Account_Model admin = new Account_Model()
            {
                Name = Session.SessionName,
                Password = Session.SessionPassword,
                Position = "Manager",
                Status = "Enable"
            };
            manager = new Manager(admin);
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

                Account_Model account = new Account_Model()
                {
                    Name = name,
                    Password = pass,
                    Position = position
                };

                IEmployee employee;
                if (account.Position.ToLower() == "manager")
                    employee = new Manager(account);
                else
                    employee = new Cashier(account);

                manager.Add(employee);

                employee.saveAccount();

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
                dgvAccount.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                // Run on UI thread to avoid race condition
                this.BeginInvoke(new MethodInvoker(() =>
                {
                    dgvAccount.DataSource = service.getAllAccount();
                    dgvAccount.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }));

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
            if (String.IsNullOrEmpty(txtSearchName.Text))
            {
                MessageBox.Show("Please enter name.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {

                if (service.searchAccount(txtSearchName.Text).Rows.Count > 0)
                {
                    dgvAccount.DataSource = service.searchAccount(txtSearchName.Text);
                }
                else
                {
                    MessageBox.Show("No account found with that name.", "Not Found",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dgvAccount.DataSource = null;
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
            service.LogoutAccount("manager");
            
        }

        private void SelectData_Click(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Ignore header clicks
                if (e.RowIndex < 0) return;

                DataGridViewRow row = dgvAccount.Rows[e.RowIndex];

                // Helper method to safely get cell value
                string GetCellValue(string columnName) =>
                    dgvAccount.Columns.Contains(columnName)
                        ? row.Cells[columnName].Value?.ToString() ?? ""
                        : "";

                // Fill admin info
                txtName.Text = GetCellValue("adminName");
                txtPassword.Text = GetCellValue("adminPassword");
                txtConfirmPassword.Text = txtPassword.Text; // Mirror password
                txtPosition.Text = GetCellValue("adminPosition");

                DisplayButtun(); // Update buttons

                // Fallback to employee info if admin info is empty
                if (string.IsNullOrEmpty(txtName.Text))
                    txtName.Text = GetCellValue("eName");

                if (string.IsNullOrEmpty(txtPosition.Text))
                    txtPosition.Text = GetCellValue("ePosition");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Load Row Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                dgvAccount.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                // Assign DataSource safely on UI thread
                this.BeginInvoke(new MethodInvoker(() =>
                {
                    dgvAccount.DataSource = service.getAllEmployeeCashierANDStock();
                    dgvAccount.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }));
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

                Account_Model account_Model = new Account_Model()
                {
                    Name = Name,
                    Position = position,
                    Password = password
                };

                service.disableAccount(account_Model);

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

                string status = service.displayButton(name);

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

                Account_Model model = new Account_Model()
                {
                    Name = Name,
                    Position = position,
                    Password = password
                };
                service.enableAccount(model);
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
