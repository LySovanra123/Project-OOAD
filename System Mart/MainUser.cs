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
    public partial class MainUser : Form
    {

        String stringConnection = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=MartDB;Integrated Security=True";
        public MainUser()
        {
            InitializeComponent();
            this.Load += MainUser_Load;
            pnlMessage.Hide();
        }

        private void MainUser_Load(object sender, EventArgs e)
        {
            lblName.Text = Session.SessionName;
        }
        private void LoadProductShortList()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(stringConnection))
                {
                    String query = "SELECT barCode, pName, pPrice FROM Products WHERE pStatus=@status";
                    using(SqlCommand cmd = new SqlCommand(query,conn))
                    {
                        cmd.Parameters.AddWithValue("@status", "SaleShortList");

                        conn.Open();
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        dgvProductOrder.DataSource = dt;
                        conn.Close();

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

        private void SelectData_Click(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dgvProductOrder.Rows[e.RowIndex];
                    lblbarCode.Text = row.Cells["barCode"].Value.ToString();
                    lblNameProduct.Text = row.Cells["pName"].Value.ToString();
                    lblPrice.Text = row.Cells["pPrice"].Value.ToString();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty(lblbarCode.Text))
            {
                MessageBox.Show("Please enter both username and password.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                if (!int.TryParse(lblbarCode.Text, out int barcode))
                {
                    MessageBox.Show("Please enter a valid number for barcode.",
                        "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(stringConnection))
                {
                    conn.Open();
                    String query = "UPDATE Products SET pStatus=@status WHERE barCode=@barCode";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@barCode", barcode);
                        cmd.Parameters.AddWithValue("@status", "Available");
                        
                        cmd.ExecuteNonQuery();
                        conn.Close();

                        LoadProductShortList();

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

        private void btnAddBuy_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtInputBarcode.Text))
            {
                MessageBox.Show("Please enter barCode of Product.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {

                int barcode = int.Parse(txtInputBarcode.Text);

                using (SqlConnection conn = new SqlConnection(stringConnection))
                {
                    conn.Open();

                    string querySelect = "SELECT barCode, pName, pPrice FROM Products WHERE barCode = @barcode AND pStatus=@status";
                    using (SqlCommand cmdSelect = new SqlCommand(querySelect, conn))
                    {
                        cmdSelect.Parameters.AddWithValue("@barcode", barcode);
                        cmdSelect.Parameters.AddWithValue("@status", "Available");
                        using (SqlDataReader reader = cmdSelect.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                String barCode = reader["barCode"].ToString();
                                String productName = reader["pName"].ToString();
                                String price = reader["pPrice"].ToString();

                                lblbarCode.Text = barCode;
                                lblNameProduct.Text = productName;
                                lblPrice.Text = price;
                            }
                            else
                            {
                                MessageBox.Show("Product not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                lblbarCode.Text = "";
                                lblPrice.Text = "";
                                lblNameProduct.Text = "";
                                return;
                            }
                        }

                        string queryUpdate = "UPDATE Products SET pStatus = @status WHERE barCode = @barcode";
                        using (SqlCommand cmdUpdate = new SqlCommand(queryUpdate, conn))
                        {
                            cmdUpdate.Parameters.AddWithValue("@barcode", barcode);
                            cmdUpdate.Parameters.AddWithValue("@status", "SaleShortList");
                            cmdUpdate.ExecuteNonQuery();
                        }

                        string queryImage = "SELECT pImage FROM Products WHERE barCode = @barcode";
                        using (SqlCommand cmdImage = new SqlCommand(queryImage, conn))
                        {
                            cmdImage.Parameters.AddWithValue("@barcode", barcode);

                            object result = cmdImage.ExecuteScalar();

                            if (result != null && result != DBNull.Value)
                            {
                                byte[] imageData = (byte[])result;
                                using (MemoryStream ms = new MemoryStream(imageData))
                                {
                                    picbProduct.Image = Image.FromStream(ms);
                                }
                            }
                            else
                            {
                                picbProduct.Image = null;
                            }
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
            pnlMessage.Hide();
            LoadProductShortList();
        }

        private void btnPay_Click(object sender, EventArgs e)
        {

            if (dgvProductOrder.Rows.Count <= 0)
            {
                MessageBox.Show("No products for sale.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string employeeName = lblName.Text;

                using (SqlConnection conn = new SqlConnection(stringConnection))
                {
                    string query = "SELECT eId FROM Employees WHERE eName = @name";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", employeeName);
                        conn.Open();

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            Session.SessioneId = Convert.ToInt32(result);
                        }
                        else
                        {
                            MessageBox.Show($"Employee '{employeeName}' not found in database.");
                            return;
                        }
                    }
                }

                List<int> barcodeList = new List<int>();

                if (dgvProductOrder.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgvProductOrder.Rows)
                    {
                        if (row.Cells["BarCode"].Value != null)
                        {
                            int barcode;
                            if (int.TryParse(row.Cells["barCode"].Value.ToString(), out barcode))
                            {
                                barcodeList.Add(barcode);
                            }
                        }
                    }
                }

                    Dictionary<string, (int count, decimal totalPrice)> productSummary = new Dictionary<string, (int, decimal)>();

                foreach (DataGridViewRow row in dgvProductOrder.Rows)
                {
                    string name = row.Cells["pName"].Value?.ToString();
                    decimal price = Convert.ToDecimal(row.Cells["pPrice"].Value);

                    if (!string.IsNullOrEmpty(name))
                    {
                        if (productSummary.ContainsKey(name))
                        {
                            var current = productSummary[name];
                            productSummary[name] = (current.count + 1, current.totalPrice + price);
                        }
                        else
                        {
                            productSummary[name] = (1, price);
                        }
                    }
                }
                Payment payment = new Payment(productSummary,barcodeList);
                payment.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void resetForm()
        {
            txtInputBarcode.Clear();
            lblbarCode.Text = "";
            lblNameProduct.Text = "";
            lblPrice.Text = "";
            dgvProductOrder.DataSource = null;
            pnlMessage.Show();
            picbProduct.Image = null;
            lblMessage.Text = "Payment Sucessful! Thank you.";
        }

        private void btnAccount_Click(object sender, EventArgs e)
        {
            Setting setting = new Setting();
            setting.Show();
        }

        private void picbProduct_Click(object sender, EventArgs e)
        {

        }
    }
}
