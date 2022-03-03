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

        public UpdateDataForm1(ItemsForm f, string item)
        {
            InitializeComponent();
            form = f;

            // Запись данных в typeComboBox
            QueriesClass.SetDataIntoList(form.myConnection, typeComboBox);
            typeComboBox.SelectedItem = item;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            // Вызов метода UpdateData из главной формы
            form.UpdateData(nameTextBox.Text, typeComboBox.SelectedItem.ToString(), (int)countTextBox.Value, (double)priceTextBox.Value);
            this.Close();
        }
    }
}
