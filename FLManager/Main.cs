using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FLManager
{
    public partial class Main : MetroFramework.Forms.MetroForm
    {
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
    }
}
