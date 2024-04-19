﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FLManager
{
    public partial class Main : MetroFramework.Forms.MetroForm
    {

        // Define a class to represent the data structure in the JSON file
        public class ProductData
        {
            public string product_Name { get; set; }
            public int product_ExperienceDays { get; set; }
            public string product_LicenseCode { get; set; }
            public string product_LicenseKey { get; set; }
            public string product_Plan { get; set; }
            public DateTime product_Expiry { get; set; }
            public string product_Email { get; set; }
            public DateTime DateGenerated { get; set; }
        }


        public Main()
        {
            InitializeComponent();
        }

        private void buttonDashboard_Click(object sender, EventArgs e)
        {

        }

        private void buttonProducts_Click(object sender, EventArgs e)
        {

        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {

        }

        private void labelRegister_Click(object sender, EventArgs e)
        {
            panel_Indicator.Location = new Point(239, 128);
            panel_Indicator.Size = new Size(49, 3);
            panelLicense.Visible = true;
        }

        private void labelProducts_Click(object sender, EventArgs e)
        {
            panel_Indicator.Location = new Point(333, 128);
            panel_Indicator.Size = new Size(66, 3);
            panelLicense.Visible = false;

            // Get AppData directory path
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Create directory if it doesn't exist
            string directoryPath = Path.Combine(appDataPath, "FLManager");
            Directory.CreateDirectory(directoryPath);

            // Create file path
            string filePath = Path.Combine(directoryPath, "generated_data.json");

            // Load data from JSON file into productGrid
            LoadDataFromJsonFile(filePath);
        }

        private void button_Generate_Click(object sender, EventArgs e)
        {
            // Validate input fields
            if (string.IsNullOrWhiteSpace(textBoxName.Text) ||
                string.IsNullOrWhiteSpace(textBoxExperienceDays.Text) ||
                comboBoxPlan.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all required fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get input values
            string name = textBoxName.Text;
            int experienceDays = Convert.ToInt32(textBoxExperienceDays.Text);
            string email = textBoxEmail.Text;
            string selectedPlan = comboBoxPlan.SelectedItem.ToString();

            // Generate unique license code and key
            string licenseCode = GenerateUniqueLicenseString();
            string licenseKey;

            // Ensure license key is different from license code
            do
            {
                licenseKey = GenerateUniqueLicenseString();
            } while (licenseKey == licenseCode);

            // Calculate expiry date
            DateTime expiryDate = DateTime.Now.AddDays(experienceDays);

            // Add data to DataGridView
            licenseGrid.Rows.Add(name, experienceDays, licenseCode, licenseKey, selectedPlan, expiryDate.ToString("yyyy-MM-dd"), email);

            // Save data to JSON file
            SaveDataToJsonFile(name, experienceDays, licenseCode, licenseKey, selectedPlan, expiryDate, email);

            // Clear input fields
            ClearInputFields();
        }


        private string GenerateUniqueLicenseString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder builder = new StringBuilder();
            Random random = new Random();

            string generatedString;

            do
            {
                builder.Clear();
                for (int i = 0; i < 25; i++) // Adjust length as needed
                {
                    if (i > 0 && i % 5 == 0) // Add '-' every 5 characters
                        builder.Append('-');

                    builder.Append(chars[random.Next(chars.Length)]);
                }

                generatedString = builder.ToString();

            } while (licenseGrid.Rows.Cast<DataGridViewRow>().Any(row => row.Cells[2].Value?.ToString() == generatedString));

            return generatedString;
        }

        private void SaveDataToJsonFile(string name, int experienceDays, string licenseCode, string licenseKey, string selectedPlan, DateTime expiryDate, string email)
        {
            // Get AppData directory path
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Create directory if it doesn't exist
            string directoryPath = Path.Combine(appDataPath, "FLManager");
            Directory.CreateDirectory(directoryPath);

            // Create file path
            string filePath = Path.Combine(directoryPath, "generated_data.json");

            // Add DateGenerated to ProductData
            DateTime dateGenerated = DateTime.Now;

            // Create list of ProductData objects
            List<ProductData> productList = new List<ProductData>
        {
            new ProductData
            {
                product_Name = name,
                product_ExperienceDays = experienceDays,
                product_LicenseCode = licenseCode,
                product_LicenseKey = licenseKey,
                product_Plan = selectedPlan,
                product_Expiry = expiryDate,
                product_Email = email,
                DateGenerated = dateGenerated
            }
        };

            // Serialize data to JSON and write to file
            string jsonData = JsonConvert.SerializeObject(productList, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
        }

        private void ClearInputFields()
        {
            textBoxName.Clear();
            textBoxExperienceDays.Clear();
            textBoxEmail.Clear();
            comboBoxPlan.SelectedIndex = -1;
        }

        private void buttonUpdateLi_Click(object sender, EventArgs e)
        {
            // Check if a cell is selected
            if (licenseGrid.SelectedCells.Count > 0)
            {
                int selectedRowIndex = licenseGrid.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = licenseGrid.Rows[selectedRowIndex];

                // Populate input fields with values from selected row
                textBoxName.Text = selectedRow.Cells["PName"].Value.ToString();
                textBoxExperienceDays.Text = selectedRow.Cells["PExperienceDays"].Value.ToString();
                textBoxEmail.Text = selectedRow.Cells["PEmail"].Value != null ? selectedRow.Cells["PEmail"].Value.ToString() : "";
                buttonSaveUpdate.Visible = true;
                button_Generate.Visible = false;            

                // Select the corresponding plan in comboBoxPlan
                string selectedPlan = selectedRow.Cells["PPlan"].Value.ToString();
                if (comboBoxPlan.Items.Contains(selectedPlan))
                {
                    comboBoxPlan.SelectedItem = selectedPlan;
                }

                else
                {
                    // If the selected plan is not found in the combobox items, display a message or handle it as appropriate
                    MessageBox.Show("Selected plan not found in the combobox items.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a cell to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSaveUpdate_Click(object sender, EventArgs e)
        {
            button_Generate.Visible = true;
            buttonSaveUpdate.Visible = false;
        }

        private void LoadDataFromJsonFile(string filePath)
        {
            // Check if the JSON file exists
            if (File.Exists(filePath))
            {
                // Read the JSON file
                string jsonData = File.ReadAllText(filePath);

                // Deserialize JSON to list of ProductData objects
                var productList = JsonConvert.DeserializeObject<List<ProductData>>(jsonData);

                // Clear existing rows in productGrid
                productGrid.Rows.Clear();

                // Add data from productList to productGrid
                foreach (var product in productList)
                {
                    // Determine the status based on the expiry date
                    string status = product.product_Expiry >= DateTime.Now ? "ACTIVE" : "INACTIVE";

                    // Determine the fore color based on the status
                    Color foreColor = status == "ACTIVE" ? Color.MediumSeaGreen : Color.Salmon;

                    // Add data to productGrid using column names
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(productGrid,
                        product.product_Name,
                        product.product_ExperienceDays,
                        product.product_LicenseCode,
                        product.product_LicenseKey,
                        product.product_Plan,
                        product.product_Expiry.ToString("yyyy-MM-dd"),
                        product.product_Email,
                        product.DateGenerated.ToString("yyyy-MM-dd"),
                        status
                    );

                    // Set the fore color for the status cell
                    row.Cells[productGrid.Columns["product_Status"].Index].Style.ForeColor = foreColor;

                    // Add the row to productGrid
                    productGrid.Rows.Add(row);
                }
            }
            else
            {
                MessageBox.Show("JSON file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
           
        }
    }
}