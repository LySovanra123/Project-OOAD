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
    public partial class ResetPassword : Form
    {
        private String name;
        String stringConnection = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=MartDB;Integrated Security=True";
        public ResetPassword(String name)
        {
            InitializeComponent();
            this.name = name;
        }

        private void ResetPassword_Load(object sender, EventArgs e)
        {

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtPassword.Text) || String.IsNullOrEmpty(txtComfirm.Text))
            {
                MessageBox.Show("Please enter password and confifm password", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                String pass = txtPassword.Text.Trim();
                String confirm = txtComfirm.Text.Trim();

                if(!pass.Equals(confirm))
                {
                    MessageBox.Show("Password and Confirm Password do not match!", "Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using(SqlConnection conn = new SqlConnection(stringConnection))
                {
                    String query = "UPDATE AccountAdmins SET adminPassword=@password WHERE adminName=@name";

                    using(SqlCommand cmd = new SqlCommand(query,conn))
                    {
                        cmd.Parameters.AddWithValue("@password", pass);
                        cmd.Parameters.AddWithValue("@name", name);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();

                        MessageBox.Show("Reset your password sucessful.","Sucess",MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
