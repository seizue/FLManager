using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
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

            // Get AppData directory path
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Create file path
            string directoryPath = Path.Combine(appDataPath, "FLManager");
            string filePath = Path.Combine(directoryPath, "generated_data.json");

            // Load selected columns from JSON file
            userControlDashBoard1.LoadSelectedColumnsFromJson(filePath);

            // You can add this line in your Main constructor or Form_Load event
            textboxSearch.ButtonClick += textboxSearch_ButtonClick;

        }

        private void buttonDashboard_Click(object sender, EventArgs e)
        {
            buttonDashboard.ForeColor = Color.SaddleBrown;
            buttonDashboard.Font = new Font(buttonDashboard.Font, FontStyle.Bold);
            buttonProducts.ForeColor = Color.FromArgb(64,64, 64);
            buttonProducts.Font = new Font(buttonDashboard.Font, FontStyle.Regular);

            // Get AppData directory path
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Create directory if it doesn't exist
            string directoryPath = Path.Combine(appDataPath, "FLManager");
            Directory.CreateDirectory(directoryPath);

            // Create file path
            string filePath = Path.Combine(directoryPath, "generated_data.json");

            // Load data from JSON file into productGrid
            LoadDataFromJsonFile(filePath);

            userControlDashBoard1.Visible = true;
        }

        private void buttonProducts_Click(object sender, EventArgs e)
        {
            buttonProducts.ForeColor = Color.SaddleBrown;
            buttonProducts.Font = new Font(buttonDashboard.Font,FontStyle.Bold);
            buttonDashboard.ForeColor = Color.FromArgb(64, 64, 64);
            buttonDashboard.Font = new Font(buttonDashboard.Font, FontStyle.Regular);

            userControlDashBoard1.Visible = false;
        }

        private void labelRegister_Click(object sender, EventArgs e)
        {
            panel_Indicator.Location = new Point(232, 127);
            panel_Indicator.Size = new Size(49, 3);
            panelLicense.Visible = true;

            buttonUpdate.Visible = false;
            buttonDelete.Visible = false;
            buttonProductDownload.Visible = false;
            separator.Visible = false;
        }

        private void labelProducts_Click(object sender, EventArgs e)
        {

            panel_Indicator.Location = new Point(326, 127);
            panel_Indicator.Size = new Size(66, 3);
            panelLicense.Visible = false;

            buttonUpdate.Visible = true;
            buttonDelete.Visible= true;
            buttonProductDownload.Visible = true;
            separator.Visible = true;
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

            UpdateStatusCounts();
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
            // Get AppData directory path
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Create directory if it doesn't exist
            string directoryPath = Path.Combine(appDataPath, "FLManager");

            // Create file path
            string filePath = Path.Combine(directoryPath, "generated_data.json");

            // Check and create JSON file if it doesn't exist
            CheckAndCreateJsonFile(filePath);

            // Load data from JSON file into productGrid
            LoadDataFromJsonFile(filePath);
            UpdateStatusCounts();
        }

        private void CheckAndCreateJsonFile(string filePath)
        {
            // Check if the directory exists, if not, create it
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Check if the JSON file exists
            if (!File.Exists(filePath))
            {
                // Create a new JSON file
                File.WriteAllText(filePath, "[]");
            }
        }

        private void textboxSearch_ButtonClick(object sender, EventArgs e)
        {
            // Get the search keyword from the textbox
            string keyword = textboxSearch.Text.ToLower();

            // Iterate through each row in the productGrid
            foreach (DataGridViewRow row in productGrid.Rows)
            {
                // Check if any cell in the row contains the search keyword
                bool rowContainsKeyword = false;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && cell.Value.ToString().ToLower().Contains(keyword))
                    {
                        rowContainsKeyword = true;
                        break;
                    }
                }

                // Show or hide the row based on whether it contains the keyword
                row.Visible = rowContainsKeyword;
            }
        }

        private void checkBoxActive_CheckedChanged(object sender, EventArgs e)
        {
            FilterRowsByStatus();
        }

        private void checkBoxInactive_CheckedChanged(object sender, EventArgs e)
        {
            FilterRowsByStatus();
        }

        private void FilterRowsByStatus()
        {
            foreach (DataGridViewRow row in productGrid.Rows)
            {
                // Get the status value from the "Status" column
                string status = row.Cells["product_Status"].Value.ToString();

                // Check if the row should be visible based on the status and the state of the checkboxes
                if ((checkBoxActive.Checked && status == "ACTIVE") || (checkBoxInactive.Checked && status == "INACTIVE"))
                {
                    row.Visible = true;
                }
                else
                {
                    row.Visible = false;
                }
            }
        }

        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterRowsBySelectedCriteria();
        }

        private void FilterRowsBySelectedCriteria()
        {
            // Get the selected criteria from comboBoxFilter
            string selectedCriteria = comboBoxFilter.SelectedItem.ToString();

            // Determine which column to sort based on the selected criteria
            string columnName = string.Empty;
            switch (selectedCriteria)
            {
                case "Product Name":
                    columnName = "product_Name";
                    break;
                case "Experience Days":
                    columnName = "product_ExperienceDays";
                    break;
                case "License Code":
                    columnName = "product_LicenseCode";
                    break;
                case "License Key":
                    columnName = "product_LicenseKey";
                    break;
                case "Plan":
                    columnName = "product_Plan";
                    break;
                case "Expiry Date":
                    columnName = "product_Expiry";
                    break;
                case "Email":
                    columnName = "product_Email";
                    break;
                case "Date Generated":
                    columnName = "DateGenerated";
                    break;
                default:
                    break;
            }

            // Check if the column name is valid
            if (!string.IsNullOrEmpty(columnName))
            {
                // Sort the DataGridView by the selected column
                productGrid.Sort(productGrid.Columns[columnName], ListSortDirection.Ascending);
            }
        }

        // Export the product list from productGrid
        private void buttonProductDownload_Click(object sender, EventArgs e)
        {
            // Open a SaveFileDialog to choose where to save the CSV file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog.Title = "Export CSV";
            saveFileDialog.FileName = $"product_data_{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv"; // File name with current date and time

            // Set the initial directory for the SaveFileDialog
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Create a StreamWriter to write to the selected file
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        // Write the header row to the CSV file
                        string header = string.Join(",", productGrid.Columns.Cast<DataGridViewColumn>().Select(col => col.HeaderText));
                        writer.WriteLine(header);

                        // Write each row of data to the CSV file
                        foreach (DataGridViewRow row in productGrid.Rows)
                        {
                            string rowData = string.Join(",", row.Cells.Cast<DataGridViewCell>().Select(cell => cell.Value.ToString()));
                            writer.WriteLine(rowData);
                        }
                    }

                    MessageBox.Show("CSV file exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting CSV file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Export license generated from licenseGrid
        private void buttonDownload_Click(object sender, EventArgs e)
        {
            // Check if there are any rows in licenseGrid
            if (licenseGrid.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Open a SaveFileDialog to choose where to save the text file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt";
            saveFileDialog.Title = "Export Text";
            saveFileDialog.FileName = $"license_data_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt"; // Default file name with current date and time

            // Set the initial directory for the SaveFileDialog
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Create a StreamWriter to write to the selected file
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        // Write the header row to the text file
                        string header = string.Join("\t", licenseGrid.Columns.Cast<DataGridViewColumn>().Select(col => col.HeaderText)); // Using tab as delimiter
                        writer.WriteLine(header);

                        // Write each row of data to the text file
                        foreach (DataGridViewRow row in licenseGrid.Rows)
                        {
                            string rowData = string.Join("\t", row.Cells.Cast<DataGridViewCell>().Select(cell => cell.Value.ToString())); // Using tab as delimiter
                            writer.WriteLine(rowData);
                        }
                    }

                    MessageBox.Show("Text file exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting text file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateStatusCounts()
        {
            // Initialize count variables
            int activeCount = 0;
            int inactiveCount = 0;

            // Iterate through each row in productGrid
            foreach (DataGridViewRow row in productGrid.Rows)
            {
                // Get the status value from the "product_Status" column
                string status = row.Cells["product_Status"].Value.ToString();

                // Increment the count based on the status
                if (status == "ACTIVE")
                {
                    activeCount++;
                }
                else if (status == "INACTIVE")
                {
                    inactiveCount++;
                }
            }

            // Update the text boxes with the counts
            textBoxActive.Text = activeCount.ToString();
            textBoxInActive.Text = inactiveCount.ToString();
            textBoxALL.Text = (activeCount + inactiveCount).ToString();

            // Update the UserControl's text boxes with the counts
            userControlDashBoard1.UpdateActivePlanCount(activeCount);
            userControlDashBoard1.UpdateInactivePlanCount(inactiveCount);
            userControlDashBoard1.UpdateOverallCustomerCount(activeCount + inactiveCount);
        }


    }
}

