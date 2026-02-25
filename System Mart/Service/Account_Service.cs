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
using System_Mart.Pattern;

namespace System_Mart.Service
{
    public class Account_Service
    {
        private IPosition manager = new ManagerPosition();
        private IPosition cashier = new CashierPosition();
        private IPosition stocker = new StockerPosition();
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
            if (account_Model.Position.ToLower() == "manager")
            {
                AdminPosition enableManager = new AdminPosition(manager);
                enableManager.DisableAccount(account_Model);
            }
            else if (account_Model.Position.ToLower() == "stocker")
            {
                AdminPosition enableStocker = new AdminPosition(stocker);
                enableStocker.DisableAccount(account_Model);
            }
            else
            {
                AdminPosition enableCashier = new AdminPosition(cashier);
                enableCashier.DisableAccount(account_Model);
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
            return "disable";
        }

        public void enableAccount(Account_Model account_Model)
        {

            if (account_Model.Position.ToLower() == "manager")
            {
                AdminPosition enableManager = new AdminPosition(manager);
                enableManager.EnableAccount(account_Model);
            }
            else if (account_Model.Position.ToLower() == "stocker")
            {
                AdminPosition enableStocker = new AdminPosition(stocker);
                enableStocker.EnableAccount(account_Model);
            }
            else
            {
                AdminPosition enableCashier = new AdminPosition(cashier);
                enableCashier.EnableAccount(account_Model);
            }
        }

        //==============login================
        public bool Login(Account_Model account_Model)
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            string Query = "SELECT adminPosition FROM AccountAdmins " +
                           "WHERE adminName = @name AND adminPassword = @password AND adminStatus=@status";

            using (SqlCommand cmd = new SqlCommand(Query, conn))
            {
                cmd.Parameters.AddWithValue("@name", account_Model.Name);
                cmd.Parameters.AddWithValue("@password", account_Model.Password);
                cmd.Parameters.AddWithValue("@status", "enable");

                Object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    String position = result.ToString().Trim();

                    if (position.Equals("Manager"))
                    {
                        EmployeeAccount employeeManager = new EmployeeAccount(manager);
                        employeeManager.employeeLoginAccount();
                    }
                    else if (position.Equals("Stocker"))
                    {
                        EmployeeAccount employeeStocker = new EmployeeAccount(stocker);
                        employeeStocker.employeeLoginAccount();
                    }
                    else
                    {
                        EmployeeAccount employeeCashier = new EmployeeAccount(cashier);
                        employeeCashier.employeeLoginAccount();
                    }

                    return true;

                }
                else
                {
                    return false;
                }
            }
        }
        //=======================Logout===========================
        public void LogoutAccount(string position)
        {
            if (position == "manager")
            {
                EmployeeAccount employeeManager = new EmployeeAccount(manager);
                employeeManager.employeeLogout();
            }
            else if (position == "stocker")
            {
                EmployeeAccount employeeStocker = new EmployeeAccount(stocker);
                employeeStocker.employeeLogout();
            }
            else
            {
                EmployeeAccount employeeCashier = new EmployeeAccount(cashier);
                employeeCashier.employeeLogout();
            }
        }
    }
}
