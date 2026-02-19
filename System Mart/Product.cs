using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace System_Mart
{
    public partial class Product : Form
    {
        String stringConnection = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=MartDB;Integrated Security=True";

        private byte[] imageData = null;
        public Product()
        {
            InitializeComponent();
            LoadProductView();
            pnlMessage.Hide();
            btnLogout.Hide();
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

                        lblMessageProduct.Text = "Image imported and resized to 150x150!";
                        pnlMessage.Show();
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
            if (String.IsNullOrEmpty(txtProductName.Text) || String.IsNullOrEmpty(txtPrice.Text) || imageData == null)
            {
                MessageBox.Show("Please enter product name, price, and import an image.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                String name = txtProductName.Text.Trim();
                if (!double.TryParse(txtPrice.Text, out double price))
                {
                    MessageBox.Show("Please enter a valid number for price.",
                        "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DateTime importDate = dtpImportDate.Value;

                using (SqlConnection conn = new SqlConnection(stringConnection))
                {
                    conn.Open();
                    String query = "INSERT INTO Products(pName,pImage,pPrice,importDate,pStatus) " +
                        "VALUES(@name,@image,@price,@importDate,@status)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.Add("@image", SqlDbType.VarBinary, imageData.Length).Value = imageData;
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@importDate", importDate);
                        cmd.Parameters.AddWithValue("@status", "Available");

                        cmd.ExecuteNonQuery();

                        pnlMessage.Show();
                        lblMessageProduct.Text = "Added Product " + name + " Sucessful.";

                        imageData = null;

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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtProductName.Text) || String.IsNullOrEmpty(txtPrice.Text))
            {
                MessageBox.Show("Please enter product name, price, and import an image.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                String name = txtProductName.Text.Trim();
                if (!double.TryParse(txtPrice.Text, out double price))
                {
                    MessageBox.Show("Please enter a valid number for price.",
                        "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DateTime importDate = dtpImportDate.Value;
                int barCode = int.Parse(txtBarCode.Text);

                if (imageData == null)
                {
                    using (SqlConnection conn = new SqlConnection(stringConnection))
                    {
                        conn.Open();
                        String query = "UPDATE Products SET " +
                                        "pName=@name," +
                                        "pPrice=@price," +
                                        "importDate=@importDate " +
                                        "WHERE barCode=@barCode AND pStatus=@status";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@name", name);
                            cmd.Parameters.AddWithValue("@price", price);
                            cmd.Parameters.AddWithValue("@importDate", importDate);
                            cmd.Parameters.AddWithValue("@barCode", barCode);
                            cmd.Parameters.AddWithValue("@status", "Available");

                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(stringConnection))
                    {
                        conn.Open();
                        String query = "UPDATE Products SET " +
                                        "pName=@name," +
                                        "pImage=@image," +
                                        "pPrice=@price," +
                                        "importDate=@importDate " +
                                        "WHERE barCode=@barCode AND pStatus=@status";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@name", name);
                            cmd.Parameters.Add("@image", SqlDbType.VarBinary, imageData.Length).Value = imageData;
                            cmd.Parameters.AddWithValue("@price", price);
                            cmd.Parameters.AddWithValue("@importDate", importDate);
                            cmd.Parameters.AddWithValue("@barCode", barCode);
                            cmd.Parameters.AddWithValue("@status", "Available");

                            cmd.ExecuteNonQuery();
                            conn.Close();

                        }
                        imageData = null;
                    }
                }
                pnlMessage.Show();
                lblMessageProduct.Text = "Updated Product " + barCode + " Sucessful.";
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

        private void LoadProductView()
        {
            using (SqlConnection conn = new SqlConnection(stringConnection))
            {
                String query = @"SELECT 
                    barCode, 
                    pName, 
                    pPrice, 
                    importDate, 
                    pStatus,
                    COUNT(*) OVER (PARTITION BY pName) AS NumberOfProduct
                 FROM Products WHERE pStatus = 'Available'";

                using (SqlDataAdapter sda = new SqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    dgvProduct.DataSource = dt;
                }
            }
        }

        private void SelectAllData_Product(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dgvProduct.Rows[e.RowIndex];
                    txtProductName.Text = row.Cells["pName"].Value.ToString();
                    txtBarCode.Text = row.Cells["barCode"].Value.ToString();
                    txtPrice.Text = row.Cells["pPrice"].Value.ToString();
                    dtpImportDate.Value = Convert.ToDateTime(row.Cells["importDate"].Value);
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void btnViewAllPro_Click(object sender, EventArgs e)
        {
            LoadProductView();
            lblMessageProduct.Text = "";
            pnlMessage.Hide();
        }

        private void LoadProductSale()
        {
            using (SqlConnection conn = new SqlConnection(stringConnection))
            {
                String query = @"SELECT 
                    barCode, 
                    pName, 
                    pPrice, 
                    importDate, 
                    pStatus,
                    COUNT(*) OVER (PARTITION BY pName) AS NumberOfProduct
                 FROM Products WHERE pStatus = 'Saled'";

                using (SqlDataAdapter sda = new SqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    dgvProduct.DataSource = dt;
                }
            }
        }

        private void btnViewSale_Click(object sender, EventArgs e)
        {
            LoadProductSale();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInput();
        }

        private void ClearInput()
        {
            txtProductName.Clear();
            txtBarCode.Clear();
            txtPrice.Clear();
            imageData = null;
            dtpImportDate.Value = DateTime.Now;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtBarCode.Text))
            {
                MessageBox.Show("Please enter the product barcode", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (!int.TryParse(txtBarCode.Text, out int barCode))
                {
                    MessageBox.Show("Please enter a valid number for Barcode.",
                        "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(stringConnection))
                {
                    conn.Open();

                    // Begin transaction to ensure all deletions happen together
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Delete from Products
                            string queryDeleteProduct = "DELETE FROM Products WHERE barCode=@barCode AND pStatus=@status";
                            using (SqlCommand cmdProduct = new SqlCommand(queryDeleteProduct, conn, transaction))
                            {
                                cmdProduct.Parameters.AddWithValue("@barCode", barCode);
                                cmdProduct.Parameters.AddWithValue("@status", "Available");
                                int rowsAffected = cmdProduct.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    pnlMessage.Show();
                                    lblMessageProduct.Text = $"Deleted Product {txtProductName.Text} successfully.";
                                }
                                else
                                {
                                    MessageBox.Show("Product not found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw; // Rethrow exception to catch block
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

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
            MainAdmin mainAdmin = new MainAdmin();
            mainAdmin.Show();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            if (String.IsNullOrEmpty(txtSearch.Text))
            {
                MessageBox.Show("Please enter product barCode or name for search.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(stringConnection))
                {

                    SqlCommand cmd;

                    if (int.TryParse(txtSearch.Text, out int searchBarcode))
                    {
                        String query = @"SELECT 
                                            barCode, 
                                            pName, 
                                            pPrice, 
                                            importDate, 
                                            pStatus,
                                            COUNT(*) OVER (PARTITION BY pName) AS NumberOfProduct
                                            FROM Products WHERE barCode=@barCode";

                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@barCode", searchBarcode);

                    }
                    else
                    {
                        String query = @"SELECT 
                                                barCode, 
                                                pName, 
                                                pPrice, 
                                                importDate, 
                                                pStatus,
                                                COUNT(*) OVER (PARTITION BY pName) AS NumberOfProduct
                                                FROM Products WHERE pName LIKE @name";

                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@name", "%" + txtSearch.Text + "%");
                    }

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        dgvProduct.DataSource = dt;
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

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtBarCode.Text);
            lblMessageProduct.Text = "Copied";
            pnlMessage.Show();
        }

        private void btnViewStock_Click(object sender, EventArgs e)
        {
            if (dgvProduct.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                Excel.Application app = new Excel.Application();
                Excel.Workbook wk = app.Workbooks.Add(Type.Missing);
                Excel.Worksheet ws = wk.ActiveSheet;
                ws.Name = "Products";

                for (int i = 0; i < dgvProduct.Columns.Count; i++)
                    ws.Cells[1, i + 1] = dgvProduct.Columns[i].HeaderText;

                for (int i = 0; i < dgvProduct.Rows.Count; i++)
                {
                    for (int j = 0; j < dgvProduct.Columns.Count; j++)
                    {
                        ws.Cells[i + 2, j + 1] = dgvProduct.Rows[i].Cells[j].Value?.ToString();

                    }
                }
                ws.Columns.AutoFit();
                app.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            Login login = Application.OpenForms.OfType<Login>().FirstOrDefault();
            if (login != null)
            {
                login.clearForm();
                login.Show();
            }
        }
        
        public void ManagerShowBack()
        {
            btnLogout.Hide();
            btnBack.Show();
        }

        public void StockerShowLogout()
        {
            btnLogout.Show();
            btnBack.Hide();
        }
    }
}
