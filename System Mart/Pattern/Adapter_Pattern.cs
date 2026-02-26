using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace System_Mart.Pattern
{
    public interface IPaymentMethod
    {
        void ProccessPayment(int employeeId, List<int> listBarCode);
    }
    public class ABABankAPI
    {
        public void MakePaymentABA(int employeeId, List<int> listBarCode)
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

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
                    string queryCashierProduct = "INSERT INTO CashierProducts(eId, barCode, paymentType, assignDate) " +
                                                 "VALUES(@eId, @barCode, @paymentType, @assignDate)";
                    using (SqlCommand cmdCashierProduct = new SqlCommand(queryCashierProduct, conn, transaction))
                    {
                        cmdCashierProduct.Parameters.Add("@eId", SqlDbType.Int);
                        cmdCashierProduct.Parameters.Add("@barCode", SqlDbType.Int);
                        cmdCashierProduct.Parameters.Add("@paymentType", SqlDbType.VarChar, 225);
                        cmdCashierProduct.Parameters.Add("@assignDate", SqlDbType.DateTime);

                        foreach (var barcode in listBarCode)
                        {
                            cmdCashierProduct.Parameters["@eId"].Value = employeeId;
                            cmdCashierProduct.Parameters["@barCode"].Value = barcode;
                            cmdCashierProduct.Parameters["@paymentType"].Value = "ABA Bank";
                            cmdCashierProduct.Parameters["@assignDate"].Value = DateTime.Now;

                            cmdCashierProduct.ExecuteNonQuery();
                        }
                    }

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
    }

    public class ACLEDABankAPI
    {
        public void MakePaymentACLEDA(int employeeId, List<int> listBarCode)
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

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
                    string queryCashierProduct = "INSERT INTO CashierProducts(eId, barCode, paymentType, assignDate) " +
                                                 "VALUES(@eId, @barCode, @paymentType, @assignDate)";
                    using (SqlCommand cmdCashierProduct = new SqlCommand(queryCashierProduct, conn, transaction))
                    {
                        cmdCashierProduct.Parameters.Add("@eId", SqlDbType.Int);
                        cmdCashierProduct.Parameters.Add("@barCode", SqlDbType.Int);
                        cmdCashierProduct.Parameters.Add("@paymentType", SqlDbType.VarChar, 225);
                        cmdCashierProduct.Parameters.Add("@assignDate", SqlDbType.DateTime);

                        foreach (var barcode in listBarCode)
                        {
                            cmdCashierProduct.Parameters["@eId"].Value = employeeId;
                            cmdCashierProduct.Parameters["@barCode"].Value = barcode;
                            cmdCashierProduct.Parameters["@paymentType"].Value = "ACLEDA Bank";
                            cmdCashierProduct.Parameters["@assignDate"].Value = DateTime.Now;

                            cmdCashierProduct.ExecuteNonQuery();
                        }
                    }

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
    }

    public class ABAAdapter : IPaymentMethod
    {
        protected ABABankAPI api;

        public ABAAdapter(ABABankAPI api)
        {
            this.api = api;
        }

        public void ProccessPayment(int employeeId, List<int> listBarCode)
        {
            api.MakePaymentABA(employeeId, listBarCode);
        }
    }

    public class ACLEDAAdapter : IPaymentMethod
    {
        protected ACLEDABankAPI api;

        public ACLEDAAdapter(ACLEDABankAPI api)
        {
            this.api = api;
        }

        public void ProccessPayment(int employeeId, List<int> listBarCode)
        {
            api.MakePaymentACLEDA(employeeId, listBarCode);
        }
    }
}
