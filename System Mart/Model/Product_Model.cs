using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System_Mart.Model
{
    public class Product_Model
    {
        private int pro_id;
        private string pro_name;
        private byte[] pro_imageData;
        private double pro_price;
        private DateTime pro_importDate;

        public int Pro_id { get => pro_id; set => pro_id = value; }
        public string Pro_name { get => pro_name; set => pro_name = value; }
        public byte[] Pro_imageData { get => pro_imageData; set => pro_imageData = value; }
        public double Pro_price { get => pro_price; set => pro_price = value; }
        public DateTime Pro_importDate { get => pro_importDate; set => pro_importDate = value; }
    }
}
