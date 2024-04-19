using Newtonsoft.Json;
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
using static FLManager.ProductUpdateForm;

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

            buttonUpdate.Visible = false;
            buttonDelete.Visible = false;
            buttonProductDownload.Visible = false;
            separator.Visible = false;
        }

        private void labelProducts_Click(object sender, EventArgs e)
        {
            panel_Indicator.Location = new Point(333, 128);
            panel_Indicator.Size = new Size(66, 3);
            panelLicense.Visible = false;

            buttonUpdate.Visible = true;
            buttonDelete.Visible= true;
            buttonProductDownload.Visible = true;
            separator.Visible = true;

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
            licenseGrid.Rows.Add(name,  licenseCode, licenseKey, experienceDays, selectedPlan, expiryDate.ToString("yyyy-MM-dd"), email);

            // Save data to JSON file
            SaveDataToJsonFile(name, experienceDays, licenseCode, licenseKey, selectedPlan, expiryDate, email);

            // Clear input fields
            ClearInputFields();
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

            // Initialize productList to store existing data
            List<ProductData> productList = new List<ProductData>();

            // Check if the JSON file exists
            if (File.Exists(filePath))
            {
                // Read existing data from the JSON file
                string existingData = File.ReadAllText(filePath);

                // Deserialize existing JSON data to list of ProductData objects
                productList = JsonConvert.DeserializeObject<List<ProductData>>(existingData);
            }

            // Add new product data to the productList
            productList.Add(new ProductData
            {
                product_Name = name,
                product_ExperienceDays = experienceDays,
                product_LicenseCode = licenseCode,
                product_LicenseKey = licenseKey,
                product_Plan = selectedPlan,
                product_Expiry = expiryDate,
                product_Email = email,
                DateGenerated = dateGenerated
            });

            // Serialize updated data to JSON
            string updatedData = JsonConvert.SerializeObject(productList, Formatting.Indented);

            // Write updated data to the JSON file
            File.WriteAllText(filePath, updatedData);
        }


        private void ClearInputFields()
        {
            textBoxName.Clear();
            textBoxExperienceDays.Clear();
            textBoxEmail.Clear();
            comboBoxPlan.SelectedIndex = -1;
        }
      

        //Loading the json file from product list datagrid
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
                        status,
                        product.product_LicenseCode,
                        product.product_LicenseKey,
                        product.product_Plan,
                        product.product_ExperienceDays,
                        product.product_Expiry.ToString("yyyy-MM-dd"),
                        product.product_Email,
                        product.DateGenerated.ToString("yyyy-MM-dd")
                       
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


        //For updating the json file in product list datagrid
        private void UpdateDataToJsonFile()
        {
            // Get AppData directory path
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Create directory if it doesn't exist
            string directoryPath = Path.Combine(appDataPath, "FLManager");
            Directory.CreateDirectory(directoryPath);

            // Create file path
            string filePath = Path.Combine(directoryPath, "generated_data.json");

            // Initialize productList to store existing data
            List<ProductData> productList = new List<ProductData>();

            // Check if the JSON file exists
            if (File.Exists(filePath))
            {
                // Read existing data from the JSON file
                string existingData = File.ReadAllText(filePath);

                // Deserialize existing JSON data to list of ProductData objects
                productList = JsonConvert.DeserializeObject<List<ProductData>>(existingData);

                // Check if a row is selected in the productGrid
                if (productGrid.SelectedRows.Count > 0)
                {
                    // Get the index of the selected row
                    int selectedRowIndex = productGrid.SelectedRows[0].Index;

                    // Update the corresponding entry in the productList
                    if (selectedRowIndex >= 0 && selectedRowIndex < productList.Count)
                    {
                        // Get the selected row from the productGrid
                        DataGridViewRow selectedRow = productGrid.Rows[selectedRowIndex];

                        // Find the corresponding product in the productList and update its properties
                        string licenseCode = selectedRow.Cells["product_LicenseCode"].Value.ToString();
                        var productToUpdate = productList.Find(product => product.product_LicenseCode == licenseCode);
                        if (productToUpdate != null)
                        {
                            productToUpdate.product_Name = selectedRow.Cells["product_Name"].Value.ToString();
                            productToUpdate.product_ExperienceDays = Convert.ToInt32(selectedRow.Cells["product_ExperienceDays"].Value);
                            productToUpdate.product_Plan = selectedRow.Cells["product_Plan"].Value.ToString();
                            productToUpdate.product_Email = selectedRow.Cells["product_Email"].Value.ToString();
                        }

                        // Serialize updated data to JSON
                        string updatedData = JsonConvert.SerializeObject(productList, Formatting.Indented);

                        // Write updated data to the JSON file
                        File.WriteAllText(filePath, updatedData);
                    }
                    else
                    {
                        MessageBox.Show("Invalid selected row index.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a row in the productGrid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("JSON file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            // Check if a row is selected
            if (productGrid.SelectedRows.Count > 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = productGrid.SelectedRows[0];

                // Create a new instance of ProductUpdateForm
                ProductUpdateForm updateForm = new ProductUpdateForm();

                // Pass the selected row's data to the ProductUpdateForm
                updateForm.ProductNameValue = selectedRow.Cells["product_Name"].Value.ToString();
                updateForm.ExperienceDays = Convert.ToInt32(selectedRow.Cells["product_ExperienceDays"].Value);
                updateForm.Email = selectedRow.Cells["product_Email"].Value.ToString();
                updateForm.SelectedPlan = selectedRow.Cells["product_Plan"].Value.ToString();

                // Subscribe to the SaveButtonClicked event
                updateForm.SaveButtonClicked += UpdateForm_SaveButtonClicked;

                // Show the ProductUpdateForm
                updateForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a row to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void UpdateForm_SaveButtonClicked(object sender, ProductUpdateEventArgs e)
        {
            // Update the selected row in productGrid with the updated values
            DataGridViewRow selectedRow = productGrid.SelectedRows[0];
            selectedRow.Cells["product_Name"].Value = e.UpdatedName;
            selectedRow.Cells["product_ExperienceDays"].Value = e.UpdatedExperienceDays;
            selectedRow.Cells["product_Email"].Value = e.UpdatedEmail;
            selectedRow.Cells["product_Plan"].Value = e.UpdatedSelectedPlan;

            UpdateDataToJsonFile();

            // Close the ProductUpdateForm
            ((ProductUpdateForm)sender).Close();
        }


        private void buttonDelete_Click(object sender, EventArgs e)
        {
            // Check if a row is selected
            if (productGrid.SelectedRows.Count > 0)
            {
                // Get the license code of the selected product
                string licenseCode = productGrid.SelectedRows[0].Cells["product_LicenseCode"].Value.ToString();

                // Get the selected row
                DataGridViewRow selectedRow = productGrid.SelectedRows[0];

                // Remove the selected row from the DataGridView
                productGrid.Rows.Remove(selectedRow);

                // Update the JSON file after removing the row
                DeleteDataFromJsonFile(licenseCode);
            }
            else
            {
                MessageBox.Show("Please select a row to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Method to delete data from the JSON file
        private void DeleteDataFromJsonFile(string licenseCode)
        {
            // Get AppData directory path
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Create directory if it doesn't exist
            string directoryPath = Path.Combine(appDataPath, "FLManager");
            Directory.CreateDirectory(directoryPath);

            // Create file path
            string filePath = Path.Combine(directoryPath, "generated_data.json");

            // Check if the JSON file exists
            if (File.Exists(filePath))
            {
                // Read existing data from the JSON file
                string existingData = File.ReadAllText(filePath);

                // Deserialize existing JSON data to list of ProductData objects
                List<ProductData> productList = JsonConvert.DeserializeObject<List<ProductData>>(existingData);

                // Find the index of the product to delete
                int indexToDelete = productList.FindIndex(product => product.product_LicenseCode == licenseCode);

                if (indexToDelete != -1)
                {
                    // Remove the product from the list
                    productList.RemoveAt(indexToDelete);

                    // Serialize updated data to JSON
                    string updatedData = JsonConvert.SerializeObject(productList, Formatting.Indented);

                    // Write updated data to the JSON file
                    File.WriteAllText(filePath, updatedData);
                }
                else
                {
                    MessageBox.Show("Product not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

