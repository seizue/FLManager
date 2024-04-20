using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static FLManager.Main;


namespace FLManager
{
    public partial class UserControlDashBoard1 : UserControl
    {
        public UserControlDashBoard1()
        {
            InitializeComponent();
        }

        public void UpdateActivePlanCount(int count)
        {
            textBoxActivePlan.Text = count.ToString();
        }

        public void UpdateInactivePlanCount(int count)
        {
            textBoxInactivePlan.Text = count.ToString();
        }

        public void UpdateOverallCustomerCount(int count)
        {
            textBoxOverallCustomer.Text = count.ToString();
        }

        public void LoadSelectedColumnsFromJson(string filePath)
        {
            // Check if the JSON file exists
            if (File.Exists(filePath))
            {
                // Read the JSON file
                string jsonData = File.ReadAllText(filePath);

                // Deserialize JSON to list of ProductData objects
                var productList = JsonConvert.DeserializeObject<List<ProductData>>(jsonData);

                // Clear existing rows in DataGridView
                DashBoardGrid.Rows.Clear();

                foreach (var product in productList)
                {
                    // Format product_Expiry to display in the desired format
                    string expiryDate = product.product_Expiry.ToString("yyyy-M-dd");

                    // Add data to DataGridView using selected columns
                    DashBoardGrid.Rows.Add(
                        product.product_Name,
                        product.product_ExperienceDays,
                        product.product_Plan,
                        expiryDate,
                        product.product_Email
                    );
                }

            }
            else
            {
                MessageBox.Show("The JSON file does not exist. Current directory: " + Directory.GetCurrentDirectory(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

    }
}
