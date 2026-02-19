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
    public partial class Login : Form
    {

        public Login()
        {
            InitializeComponent();
        }

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

                SqlConnection conn = DataBaseConnection.Instance.GetConnection();

                string Query = "SELECT adminPosition FROM AccountAdmins " +
                               "WHERE adminName = @name AND adminPassword = @password AND adminStatus=@status";

                using (SqlCommand cmd = new SqlCommand(Query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@status", "enable");

                    Object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        String position = result.ToString().Trim();

                        if (position.Equals("Manager"))
                        {
                            MainAdmin mainAdmin = new MainAdmin();
                            mainAdmin.Show();
                        }
                        else if (position.Equals("Stocker"))
                        {
                            Product product = new Product();
                            product.StockerShowLogout();
                            product.Show();
                        }
                        else
                        {
                            MainUser mainUser = new MainUser();
                            mainUser.Show();
                        }

                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password.",
                                        "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
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
