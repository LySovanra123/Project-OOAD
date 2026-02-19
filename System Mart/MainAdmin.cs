using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace System_Mart
{
    public partial class MainAdmin : Form
    {
        String stringConnection = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=MartDB;Integrated Security=True";
        public MainAdmin()
        {
            InitializeComponent();
            setupChartProduct();
            setupChartEmployee();

            Product product = new Product();
            product.ManagerShowBack();
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            this.Close();
            Product product = new Product();
            product.Show();
        }

        private void chartProduct_Click(object sender, EventArgs e)
        {

        }
        private void setupChartEmployee()
        {
            try
            {
                chartEmployee.Series.Clear();

                using (SqlConnection conn = new SqlConnection(stringConnection))
                {
                    conn.Open();

                    String query = @"SELECT ePosition , COUNT(*) as EmployeeCount FROM Employees GROUP BY ePosition";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataReader sdr = cmd.ExecuteReader();

                        Series series = new Series("Employees");    
                        series.ChartType = SeriesChartType.Column;
                        series.Color = Color.FromArgb(52, 152, 219);
                        series.Font = new Font("Arial", 10);

                        while (sdr.Read())
                        {
                            String dept = sdr["ePosition"].ToString();
                            int count = Convert.ToInt32(sdr["EmployeeCount"]);
                            series.Points.AddXY(dept, count);
                        }
                        chartEmployee.Series.Add(series);

                        chartEmployee.Legends[0].Docking = Docking.Top;
                        chartEmployee.ChartAreas[0].AxisY.Interval = 1;
                        chartEmployee.ChartAreas[0].AxisY.LabelStyle.Format = "0";
                        chartEmployee.ChartAreas[0].AxisX.Title = "Position";
                        chartEmployee.ChartAreas[0].AxisY.Title = "Number of Employees";

                        conn.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading chart: " + ex.Message);
            }
        }
        private void setupChartProduct() 
        {
            try
            {
                chartProduct.Series.Clear();

                using (SqlConnection conn = new SqlConnection(stringConnection))
                {
                    conn.Open();

                    String query = @"SELECT pStatus , COUNT(*) as ProductCount FROM Products GROUP BY pStatus";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataReader sdr = cmd.ExecuteReader();

                        Series series = new Series("Products");
                        series.ChartType = SeriesChartType.Column;
                        series.Color = Color.FromArgb(52, 152, 219);
                        series.Font = new Font("Arial", 10);

                        while (sdr.Read())
                        {
                            String dept = sdr["pStatus"].ToString();
                            int count = Convert.ToInt32(sdr["ProductCount"]);
                            series.Points.AddXY(dept, count);
                        }
                        chartProduct.Series.Add(series);

                        chartProduct.Legends[0].Docking = Docking.Top;
                        chartProduct.ChartAreas[0].AxisY.Interval = 1;
                        chartProduct.ChartAreas[0].AxisY.LabelStyle.Format = "0";
                        chartProduct.ChartAreas[0].AxisX.Title = "Status";
                        chartProduct.ChartAreas[0].AxisY.Title = "Number of Products";

                        conn.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading chart: " + ex.Message);
            }
        }

        private void btnEmployee_Click(object sender, EventArgs e)
        {
            this.Close();
            EmployeeForm employee = new EmployeeForm();
            employee.Show();
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            this.Close();
            Account account = new Account();
            account.Show();
        }
    }
}
