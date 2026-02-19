using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System_Mart.Model
{
    public class Employee_Model
    {
        private int id;
        private string name;
        private byte[] imageData;
        private string gender;
        private string position;
        private string phoneNumber;
        private string address;
        private DateTime SDW;
        private decimal salary;

        public string Name { get => name; set => name = value; }
        public byte[] ImageData { get => imageData; set => imageData = value; }
        public string Gender { get => gender; set => gender = value; }
        public string Position { get => position; set => position = value; }
        public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }
        public string Address { get => address; set => address = value; }
        public DateTime SDW1 { get => SDW; set => SDW = value; }
        public decimal Salary { get => salary; set => salary = value; }
        public int Id { get => id; set => id = value; }
    }
}
