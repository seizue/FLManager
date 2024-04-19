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
            textBoxName.Text = ProductName;
            textBoxExperienceDays.Text = ExperienceDays.ToString();
            textBoxEmail.Text = Email;
            comboBoxPlan.SelectedItem = SelectedPlan;
        }
    }
}
