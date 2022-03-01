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
    public partial class UpdateDataForm1 : Form
    {
        private ItemsForm form;

        public UpdateDataForm1(ItemsForm f, int index)
        {
            InitializeComponent();
            form = f;
            // Запись данных в typeComboBox
            form.SetDataIntoList(typeComboBox);
            typeComboBox.SelectedIndex = index;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            // Вызов метода UpdateData из главной формы
            form.UpdateData(nameTextBox.Text, typeComboBox.SelectedIndex + 1, (int)countTextBox.Value, (int)priceTextBox.Value);
            this.Close();
        }

        private void UpdateDataForm1_Load(object sender, EventArgs e)
        {

        }
    }
}
