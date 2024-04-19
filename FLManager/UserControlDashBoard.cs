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
    public partial class UserControlDashBoard : UserControl
    {
        public UserControlDashBoard()
        {
            InitializeComponent();
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
                MessageBox.Show("JSON file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
