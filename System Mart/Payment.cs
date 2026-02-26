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
using System_Mart.Pattern;

namespace System_Mart
{
    public partial class Payment : Form
    {
        List<int> barcodeList = new List<int>();
        private IPaymentMethod paymentMethod;
        string bankType;
        public Payment(Dictionary<string, (int count , decimal totalPrice)> productSummary, List<int> barcodeList, string BankType)
        {
            InitializeComponent();

            this.barcodeList = barcodeList;
            bankType = BankType;

            dgvPayment.Columns.Clear();
            dgvPayment.Columns.Add("ProductName", "Product Name");
            dgvPayment.Columns.Add("Count", "Count");
            dgvPayment.Columns.Add("TotalPrice", "Total Price");
            dgvPayment.Columns.Add("Bank", "Bank");

            dgvPayment.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPayment.Columns["TotalPrice"].DefaultCellStyle.Format = "N2";

            decimal grandTotal = 0;
            foreach (var item in productSummary)
            {
                dgvPayment.Rows.Add(item.Key, item.Value.count, item.Value.totalPrice,"");
                grandTotal += item.Value.totalPrice;
            }
            dgvPayment.Rows.Add("Grand Total", "", grandTotal,BankType);
            dgvPayment.Rows[dgvPayment.Rows.Count - 1].DefaultCellStyle.Font =
                new Font(dgvPayment.Font, FontStyle.Bold);
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

                ABABankAPI apiABA = new ABABankAPI();
                ACLEDABankAPI apiACLEDA = new ACLEDABankAPI();

                if(bankType.Equals("ABA Bank")){
                    paymentMethod = new ABAAdapter(apiABA);
                }
                else
                {
                    paymentMethod = new ACLEDAAdapter(apiACLEDA);
                }

                paymentMethod.ProccessPayment(eId,barcodeList);
                
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

    }
}
