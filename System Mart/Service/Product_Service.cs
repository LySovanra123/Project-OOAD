using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System_Mart.Model;
using System.Xml.Linq;
using System.Windows.Forms;

namespace System_Mart.Service
{
    public class Product_Service
    {
        public void addProduct(Product_Model product_Model)
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            string query = @"INSERT INTO Products
                            (pName, pImage, pPrice, importDate, pStatus)
                            VALUES
                            (@name, @image, @price, @importDate, @status)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", product_Model.Pro_name);
                cmd.Parameters.AddWithValue("@price", product_Model.Pro_price);
                cmd.Parameters.AddWithValue("@importDate", product_Model.Pro_importDate);
                cmd.Parameters.AddWithValue("@status", "Available");

                // Safely handle null image
                if (product_Model.Pro_imageData != null)
                    cmd.Parameters.Add("@image", SqlDbType.VarBinary)
                       .Value = product_Model.Pro_imageData;
                else
                    cmd.Parameters.Add("@image", SqlDbType.VarBinary)
                       .Value = DBNull.Value;

                cmd.ExecuteNonQuery();
            }
        }

        //==============Clone================
        public void duplicateProduct(Product_Model original)
        {
            Product_Model clone = original.Clone();

            clone.Pro_id = 0; // new ID
            clone.Pro_importDate = DateTime.Now;

            addProduct(clone);
        }

        public bool UpdateProduct(Product_Model product_Model) 
        {
            if (product_Model.Pro_imageData == null)
            {

                SqlConnection conn = DataBaseConnection.Instance.GetConnection();

                String query = "UPDATE Products SET " +
                                    "pName=@name," +
                                    "pPrice=@price," +
                                    "importDate=@importDate " +
                                    "WHERE barCode=@barCode AND pStatus=@status";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", product_Model.Pro_name);
                    cmd.Parameters.AddWithValue("@price", product_Model.Pro_price);
                    cmd.Parameters.AddWithValue("@importDate", product_Model.Pro_importDate);
                    cmd.Parameters.AddWithValue("@barCode", product_Model.Pro_id);
                    cmd.Parameters.AddWithValue("@status", "Available");

                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                return true;
            }
            else
            {
                SqlConnection conn = DataBaseConnection.Instance.GetConnection();

                String query = "UPDATE Products SET " +
                                    "pName=@name," +
                                    "pImage=@image," +
                                    "pPrice=@price," +
                                    "importDate=@importDate " +
                                    "WHERE barCode=@barCode AND pStatus=@status";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", product_Model.Pro_name);
                    cmd.Parameters.Add("@image", SqlDbType.VarBinary, product_Model.Pro_imageData.Length).Value = product_Model.Pro_imageData;
                    cmd.Parameters.AddWithValue("@price", product_Model.Pro_price);
                    cmd.Parameters.AddWithValue("@importDate", product_Model.Pro_importDate);
                    cmd.Parameters.AddWithValue("@barCode", product_Model.Pro_id);
                    cmd.Parameters.AddWithValue("@status", "Available");

                    cmd.ExecuteNonQuery();
                    conn.Close();

                }
                return false;
            }
        }

        public System.Data.DataTable getAllProduct()
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            String query = @"SELECT 
                    barCode, 
                    pName, 
                    pPrice, 
                    pImage,
                    importDate, 
                    pStatus,
                    COUNT(*) OVER (PARTITION BY pName) AS NumberOfProduct
                 FROM Products WHERE pStatus = 'Available'";

            using (SqlDataAdapter sda = new SqlDataAdapter(query, conn))
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                sda.Fill(dt);
                return dt;
            }
        }

        public System.Data.DataTable getAllProductSale()
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            String query = @"SELECT 
                    barCode, 
                    pName, 
                    pPrice, 
                    importDate,
                    pImage,
                    pStatus,
                    COUNT(*) OVER (PARTITION BY pName) AS NumberOfProduct
                 FROM Products WHERE pStatus = 'Saled'";

            using (SqlDataAdapter sda = new SqlDataAdapter(query, conn))
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                sda.Fill(dt);
                return dt;
            }
        }

        //================DELETE PRODUCT======================
        public bool deleteProduct(int barCode)
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

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
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        //==================================SEARCH PRODUCT=================================
        public System.Data.DataTable searchProduct(string barcodeORname)
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            SqlCommand cmd;

            if (int.TryParse(barcodeORname, out int searchBarcode))
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
                cmd.Parameters.AddWithValue("@name", "%" + barcodeORname + "%");
            }

            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                sda.Fill(dt);
                return dt;
            }
        }
    }
}
