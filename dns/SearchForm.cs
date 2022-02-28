using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dns
{
    public partial class SearchForm : Form
    {
        private Items form;

        public SearchForm(Items i)
        {
            InitializeComponent();
            form = i;
        }

        private void executeButton_Click(object sender, EventArgs e)
        {
            form.Search(wordTextBox.Text, typeComboBox.SelectedIndex+1, checkRegister.Checked);
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
