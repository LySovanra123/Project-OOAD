using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System_Mart.Pattern;

namespace System_Mart.Model
{
    public class Product_Model : IPropotype<Product_Model>
    {
        private int pro_id;
        private string pro_name;
        private byte[] pro_imageData;
        private double pro_price;
        private DateTime pro_importDate;

        public Product_Model()
        {
        }

        public int Pro_id { get => pro_id; set => pro_id = value; }
        public string Pro_name { get => pro_name; set => pro_name = value; }
        public byte[] Pro_imageData { get => pro_imageData; set => pro_imageData = value; }
        public double Pro_price { get => pro_price; set => pro_price = value; }
        public DateTime Pro_importDate { get => pro_importDate; set => pro_importDate = value; }

        public Product_Model Clone()
        {
            Product_Model cloned = (Product_Model)this.MemberwiseClone();
            
            if(this.pro_imageData != null )
            {
                cloned.pro_imageData = (byte[])this.pro_imageData.Clone();
            }

            return cloned;
        }
    }
}
