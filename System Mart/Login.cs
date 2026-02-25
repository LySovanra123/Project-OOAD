using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using System_Mart.Model;
using System_Mart.Service;

namespace System_Mart
{
    public partial class Login : Form
    {

        public Login()
        {
            InitializeComponent();
        }

        private Account_Service service = new Account_Service();

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtName.Text) || String.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Please enter both username and password.",
                                "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                String name = txtName.Text;
                String password = txtPassword.Text;

                Session.SessionName = name;
                Session.SessionPassword = password;

                Account_Model account_Model = new Account_Model()
                {
                    Name = name,
                    Password = password,
                };

                bool result = service.Login(account_Model);

                if (result)
                {
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.",
                                        "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show("Database error: " + se.Message,
                                "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public void clearForm()
        {
            txtName.Clear();
            txtPassword.Clear();
        }
    }
}
