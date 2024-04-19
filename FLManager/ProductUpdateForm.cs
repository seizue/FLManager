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
    public partial class ProductUpdateForm : MetroFramework.Forms.MetroForm
    {
        // Properties to hold the data
        public string ProductNameValue { get; set; }
        public int ExperienceDays { get; set; }
        public string Email { get; set; }
        public string SelectedPlan { get; set; }

        public ProductUpdateForm()
        {
            InitializeComponent();
        }

        private void ProductUpdateForm_Load(object sender, EventArgs e)
        {
            // Populate the controls with the passed data
            textBoxName.Text = ProductNameValue;
            textBoxExperienceDays.Text = ExperienceDays.ToString();
            textBoxEmail.Text = Email;
            comboBoxPlan.SelectedItem = SelectedPlan;
        }

        public event EventHandler<ProductUpdateEventArgs> SaveButtonClicked;
        private void buttonSaveUpdateForm_Click(object sender, EventArgs e)
        {
            // Retrieve updated values from the form
            string updatedName = textBoxName.Text;
            int updatedExperienceDays = Convert.ToInt32(textBoxExperienceDays.Text);
            string updatedEmail = textBoxEmail.Text;
            string updatedSelectedPlan = comboBoxPlan.SelectedItem.ToString();

            // Fire the event with the updated values
            SaveButtonClicked?.Invoke(this, new ProductUpdateEventArgs(updatedName, updatedExperienceDays, updatedEmail, updatedSelectedPlan));
        }

        public class ProductUpdateEventArgs : EventArgs
        {
            public string UpdatedName { get; }
            public int UpdatedExperienceDays { get; }
            public string UpdatedEmail { get; }
            public string UpdatedSelectedPlan { get; }

            public ProductUpdateEventArgs(string updatedName, int updatedExperienceDays, string updatedEmail, string updatedSelectedPlan)
            {
                UpdatedName = updatedName;
                UpdatedExperienceDays = updatedExperienceDays;
                UpdatedEmail = updatedEmail;
                UpdatedSelectedPlan = updatedSelectedPlan;
            }
        }

    }
}
