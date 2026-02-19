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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;


namespace System_Mart
{
    public partial class Payment : Form
    {
        List<int> barcodeList = new List<int>();
        String stringConnection = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=MartDB;Integrated Security=True";
        public Payment(Dictionary<string, (int count , decimal totalPrice)> productSummary, List<int> barcodeList)
        {
            InitializeComponent();

            this.barcodeList = barcodeList;

            dgvPayment.Columns.Clear();
            dgvPayment.Columns.Add("ProductName", "Product Name");
            dgvPayment.Columns.Add("Count", "Count");
            dgvPayment.Columns.Add("TotalPrice", "Total Price");

            dgvPayment.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPayment.Columns["TotalPrice"].DefaultCellStyle.Format = "N2";

            decimal grandTotal = 0;
            foreach (var item in productSummary)
            {
                dgvPayment.Rows.Add(item.Key, item.Value.count, item.Value.totalPrice);
                grandTotal += item.Value.totalPrice;
            }
            dgvPayment.Rows.Add("Grand Total", "", grandTotal);
            dgvPayment.Rows[dgvPayment.Rows.Count - 1].DefaultCellStyle.Font =
                new System.Drawing.Font(dgvPayment.Font, FontStyle.Bold);
            dgvPayment.Rows[dgvPayment.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightYellow;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            try
            {
                int eId = Session.SessioneId;

                using (SqlConnection conn = new SqlConnection(stringConnection))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Update product status
                            string queryUpdate = "UPDATE Products SET pStatus = @newStatus WHERE pStatus = @oldStatus";
                            using (SqlCommand cmdUpdate = new SqlCommand(queryUpdate, conn, transaction))
                            {
                                cmdUpdate.Parameters.AddWithValue("@newStatus", "Saled");
                                cmdUpdate.Parameters.AddWithValue("@oldStatus", "SaleShortList");
                                cmdUpdate.ExecuteNonQuery();
                            }

                            // Insert into CashierProducts
                            string queryCashierProduct = "INSERT INTO CashierProducts(eId, barCode, assignDate) " +
                                                         "VALUES(@eId, @barCode, @assignDate)";
                            using (SqlCommand cmdCashierProduct = new SqlCommand(queryCashierProduct, conn, transaction))
                            {
                                cmdCashierProduct.Parameters.Add("@eId", SqlDbType.Int);
                                cmdCashierProduct.Parameters.Add("@barCode", SqlDbType.Int);
                                cmdCashierProduct.Parameters.Add("@assignDate", SqlDbType.DateTime);

                                foreach (var barcode in barcodeList)
                                {
                                    cmdCashierProduct.Parameters["@eId"].Value = eId;
                                    cmdCashierProduct.Parameters["@barCode"].Value = barcode;
                                    cmdCashierProduct.Parameters["@assignDate"].Value = DateTime.Now;

                                    cmdCashierProduct.ExecuteNonQuery();
                                }
                                
                            }
                            ExportDataGridViewToPDF(dgvPayment, "PaymentInvoice.pdf");

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }

                    conn.Close();
                }

                // Clear the list safely
                barcodeList.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return;
            }

            MainUser mainUser = Application.OpenForms.OfType<MainUser>().FirstOrDefault();
            mainUser?.resetForm();

            this.Close();
        }
        private void ExportDataGridViewToPDF(DataGridView dgv, string fileName)
        {
            try
            {
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "PDF File (*.pdf)|*.pdf";
                save.FileName = fileName;

                if (save.ShowDialog() != DialogResult.OK)
                    return;

                PdfPTable pdfTable = new PdfPTable(dgv.ColumnCount);
                pdfTable.WidthPercentage = 100;
                pdfTable.DefaultCell.Padding = 5;

                // Add column headers
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(col.HeaderText));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    pdfTable.AddCell(cell);
                }

                // Add rows
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            pdfTable.AddCell(cell.Value?.ToString() ?? "");
                        }
                    }
                }

                using (FileStream stream = new FileStream(save.FileName, FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A4, 20, 20, 20, 20);
                    PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();
                    pdfDoc.Add(pdfTable);
                    pdfDoc.Close();
                    stream.Close();
                }

                MessageBox.Show("PDF created successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


    }
}
