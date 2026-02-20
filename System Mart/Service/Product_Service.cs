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

namespace System_Mart.Service
{
    public class Product_Service
    {
        public void addProduct(Product_Model product_Model)
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            String query = "INSERT INTO Products(pName,pImage,pPrice,importDate,pStatus) " +
                    "VALUES(@name,@image,@price,@importDate,@status)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", product_Model.Pro_name);
                cmd.Parameters.Add("@image", SqlDbType.VarBinary, product_Model.Pro_imageData.Length).Value = product_Model.Pro_imageData;
                cmd.Parameters.AddWithValue("@price", product_Model.Pro_price);
                cmd.Parameters.AddWithValue("@importDate", product_Model.Pro_importDate);
                cmd.Parameters.AddWithValue("@status", "Available");
                cmd.ExecuteNonQuery();

            }
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
    }
}
