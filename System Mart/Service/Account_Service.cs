using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Office.Interop.Excel;
using System_Mart.Model;

namespace System_Mart.Service
{
    public class Account_Service
    {
        public System.Data.DataTable getAllAccount()
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            String query = "SELECT * FROM AccountAdmins";

            using (SqlDataAdapter sda = new SqlDataAdapter(query, conn))
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                sda.Fill(dt);
                return dt;
            }
        }
        public System.Data.DataTable searchAccount(string name)
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            String query = "SELECT * FROM AccountAdmins WHERE adminName=@name";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", name);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                System.Data.DataTable dt = new System.Data.DataTable();
                sda.Fill(dt);
                return dt;
            }
        }

        public System.Data.DataTable getAllEmployeeCashierANDStock()
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            String query = "SELECT eId,eName,ePosition FROM Employees WHERE (ePosition=@positionC OR ePosition=@positionM OR ePosition=@positionS) AND eStatus=@status";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@positionC", "Cashier");
                cmd.Parameters.AddWithValue("@positionM", "Manager");
                cmd.Parameters.AddWithValue("@positionS", "Stocker");
                cmd.Parameters.AddWithValue("@status", "Not yet");
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    System.Data.DataTable dt = new System.Data.DataTable();
                    sda.Fill(dt);
                    return dt;
                }
            }
        }

        public void disableAccount(Account_Model account_Model)
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            String query = "UPDATE AccountAdmins SET adminStatus=@status WHERE adminName=@name AND adminPassword=@password AND adminPosition=@position";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", account_Model.Name);
                cmd.Parameters.AddWithValue("@status", "disable");
                cmd.Parameters.AddWithValue("@password", account_Model.Password);
                cmd.Parameters.AddWithValue("@position", account_Model.Position);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

            }
        }

        public string displayButton(string name)
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            string query = "SELECT adminStatus FROM AccountAdmins WHERE adminName=@name";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", name);
                Object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    string status = result.ToString().Trim();
                    return status;
                }
            }
            return null;
        }

        public void enableAccount(Account_Model account_Model)
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            String query = "UPDATE AccountAdmins SET adminStatus=@status WHERE adminName=@name AND adminPassword=@password AND adminPosition=@position";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", account_Model.Name);
                cmd.Parameters.AddWithValue("@status", "enable");
                cmd.Parameters.AddWithValue("@password", account_Model.Password);
                cmd.Parameters.AddWithValue("@position", account_Model.Position);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

            }
        }
    }
}
