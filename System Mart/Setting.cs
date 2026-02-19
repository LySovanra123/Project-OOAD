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
    public partial class Setting : Form
    {
        public Setting()
        {
            InitializeComponent();
            txtName.Text = Session.SessionName;
            txtPass.Text = Session.SessionPassword;
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty(txtName.Text) || String.IsNullOrEmpty(txtPass.Text) || String.IsNullOrEmpty(txtConfirm.Text))
            {
                MessageBox.Show("Please enter username, password and confirm password.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                String name = txtName.Text.Trim();
                String password = txtPass.Text.Trim();
                String confirm = txtConfirm.Text.Trim();

                if(!password.Equals(confirm))
                {
                    MessageBox.Show("Please check your password and confirm pasword.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SqlConnection conn = DataBaseConnection.Instance.GetConnection();

                String query = "UPDATE AccountAdmins SET adminName=@name, adminPassword=@password WHERE adminName=@owerName";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@owerName", Session.SessionName);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    MessageBox.Show("Change your information sucessful.", "Changed", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Login login = Application.OpenForms.OfType<Login>().FirstOrDefault();
            if (login != null)
            {
                login.clearForm();
                login.Show();
            }
            MainUser mainUser = Application.OpenForms.OfType<MainUser>().FirstOrDefault();
            if (mainUser != null)
            {
                mainUser.resetForm();
                mainUser.Close();
            }
            this.Close();

        }
    }
}
