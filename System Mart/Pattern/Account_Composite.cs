using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System_Mart.Model;

namespace System_Mart.Service
{
    //================Component==================
    public interface IEmployee
    {
        void saveAccount();
    }

    //====================Leaf===================
    public class Cashier : IEmployee
    {
        private Account_Model account;
        private Employee_Service service = new Employee_Service();

        public Cashier(Account_Model account)
        {
            this.account = account;
        }

        public void saveAccount()
        {
            service.saveAccount(account);
        }
    }

    //================Composite====================
    public class Manager : IEmployee
    {
        private Account_Model account;
        private Employee_Service service = new Employee_Service();
        private List<IEmployee> employees = new List<IEmployee>();

        public Manager(Account_Model account)
        {
            this.account = account;
        }

        public void Add(IEmployee employee)
        {
            employees.Add(employee);
        }

        public void saveAccount()
        {
            service.saveAccount(account);

            foreach (var emp in employees) 
            {
                emp.saveAccount();
            }
        }
    }
}
