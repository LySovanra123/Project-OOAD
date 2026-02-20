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
using Microsoft.Office.Interop.Excel;
using System_Mart.Model;
using System_Mart.Service;

namespace System_Mart
{
    public partial class EmployeeForm : Form
    {

        byte[] imageData =  null;

        private Employee_Service service = new Employee_Service();
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

            // If not exists, insert the new employee
            if (!decimal.TryParse(txtSalary.Text, out decimal salary))
            {
                MessageBox.Show("Please enter a valid number for salary.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string name = txtEmployeeName.Text;
            string phoneNumber = txtPhoneNumber.Text;
            string gender = cbbGender.Text;
            string position = cbbPosition.Text;
            string address = txtAddress.Text.Trim();
            DateTime dateStartWork = dtpStartWorkDate.Value;

            Employee_Model emp_model = new Employee_Model()
            {
                Name = name,
                PhoneNumber = phoneNumber,
                Gender = gender,
                Position = position,
                Address = address,
                Salary = salary,
                ImageData = imageData,
                SDW1 = dateStartWork,
            };

            service.AddEmployee( emp_model );

            pnlMessageEmployee.Show();
            lblMessageEmployee.Text = "Added Employee " + name + " successfully.";

            imageData = null; // reset image after insertion

        }

        private void btnView_Click(object sender, EventArgs e)
        {
            LoadEmployeeView();
        }

        private void LoadEmployeeView()
        {
            dgvEmployee.DataSource = service.GetAllEmployee();
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

                Employee_Model emp_model = new Employee_Model()
                {
                    Name = name,
                    Gender = gender,
                    Position = position,
                    PhoneNumber = phoneNumber,
                    Address = address,
                    Salary = salary,
                    SDW1 = dateStartWork,
                    ImageData = imageData,
                    Id = id,
                };

                service.editEmaployee(emp_model);

                pnlMessageEmployee.Show();
                lblMessageEmployee.Text = $"Updated Employee ID {id} successfully.";

                imageData = null; // reset image

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

                Employee_Model emp_model = new Employee_Model()
                {
                    Id = id,
                };

                bool haveRowAffected;

                haveRowAffected = service.deleteEmployee(emp_model);

                if (haveRowAffected == true)
                {
                    pnlMessageEmployee.Show();
                    lblMessageEmployee.Text = $"Deleted Employee id: {id} successfully.";
                }
                else
                {
                    MessageBox.Show("Employee not found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                dgvEmployee.DataSource = service.searchEmployee(txtEmployeeName.Text);

                if(dgvEmployee.DataSource == null)
                {
                    pnlMessageEmployee.Show();
                    lblMessageEmployee.Text = "Emplyee not found!";
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
