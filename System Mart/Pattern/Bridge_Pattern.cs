using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System_Mart.Model;

namespace System_Mart.Pattern
{
    public interface IPosition
    {
        void Login();
        void Logout();
        void disable(Account_Model account_Model);
        void enable(Account_Model account_Model);
    }
    public class ManagerPosition : IPosition
    {
        public void disable(Account_Model account_Model)
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            String query = "UPDATE AccountAdmins SET adminStatus=@status WHERE adminName=@name AND adminPassword=@password";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", account_Model.Name);
                cmd.Parameters.AddWithValue("@status", "disable");
                cmd.Parameters.AddWithValue("@password", account_Model.Password);

                cmd.ExecuteNonQuery();
                conn.Close();

            }
        }

        public void enable(Account_Model account_Model)
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            String query = "UPDATE AccountAdmins SET adminStatus=@status WHERE adminName=@name AND adminPassword=@password";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", account_Model.Name);
                cmd.Parameters.AddWithValue("@status", "enable");
                cmd.Parameters.AddWithValue("@password", account_Model.Password);

                cmd.ExecuteNonQuery();
                conn.Close();

            }
        }

        public void Login()
        {
            MainAdmin mainAdmin = new MainAdmin();
            mainAdmin.Show();
        }

        public void Logout()
        {
            Login login = Application.OpenForms.OfType<Login>().FirstOrDefault();
            if (login != null)
            {
                login.clearForm();
                login.Show();
            }
        }
    }
    public class CashierPosition : IPosition
    {
        public void disable(Account_Model account_Model)
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            String query = "UPDATE AccountAdmins SET adminStatus=@status WHERE adminName=@name AND adminPassword=@password";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", account_Model.Name);
                cmd.Parameters.AddWithValue("@status", "disable");
                cmd.Parameters.AddWithValue("@password", account_Model.Password);

                cmd.ExecuteNonQuery();
                conn.Close();

            }
        }

        public void enable(Account_Model account_Model)
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            String query = "UPDATE AccountAdmins SET adminStatus=@status WHERE adminName=@name AND adminPassword=@password";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", account_Model.Name);
                cmd.Parameters.AddWithValue("@status", "enable");
                cmd.Parameters.AddWithValue("@password", account_Model.Password);

                cmd.ExecuteNonQuery();
                conn.Close();

            }
        }

        public void Login()
        {
            MainUser mainUser = new MainUser();
            mainUser.Show();
        }

        public void Logout()
        {
            Login login = Application.OpenForms.OfType<Login>().FirstOrDefault();
            if (login != null)
            {
                login.clearForm();
                login.Show();
            }
            MainUser mainUser = Application.OpenForms.OfType<MainUser>().FirstOrDefault();
            if (mainUser != null)
            {
                mainUser.resetForm();
                mainUser.Close();
            }
        }
    }
    public class StockerPosition : IPosition
    {
        public void disable(Account_Model account_Model)
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            String query = "UPDATE AccountAdmins SET adminStatus=@status WHERE adminName=@name AND adminPassword=@password";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", account_Model.Name);
                cmd.Parameters.AddWithValue("@status", "disable");
                cmd.Parameters.AddWithValue("@password", account_Model.Password);

                cmd.ExecuteNonQuery();
                conn.Close();

            }
        }

        public void enable(Account_Model account_Model)
        {
            SqlConnection conn = DataBaseConnection.Instance.GetConnection();

            String query = "UPDATE AccountAdmins SET adminStatus=@status WHERE adminName=@name AND adminPassword=@password";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", account_Model.Name);
                cmd.Parameters.AddWithValue("@status", "enable");
                cmd.Parameters.AddWithValue("@password", account_Model.Password);

                cmd.ExecuteNonQuery();
                conn.Close();

            }
        }

        public void Login()
        {
            Product product = new Product();
            product.StockerShowLogout();
            product.Show();
        }

        public void Logout()
        {
            Login login = Application.OpenForms.OfType<Login>().FirstOrDefault();
            if (login != null)
            {
                login.clearForm();
                login.Show();
            }
        }
    }
    public class EmployeeAccount
    {
        protected IPosition _position;

        public EmployeeAccount(IPosition position)
        {
            _position = position;
        }
        public void employeeLoginAccount()
        {
            _position.Login();
        }
        public void employeeLogout()
        {
            _position.Logout();
        }
    }
    public class AdminPosition : EmployeeAccount
    {
        public AdminPosition(IPosition position) : base(position)
        {
        }
        public void EnableAccount(Account_Model account_Model)
        {
            _position.enable(account_Model);
        }

        public void DisableAccount(Account_Model account_Model)
        {
            _position.disable(account_Model);
        }
    }
}
