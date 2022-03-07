using System;
using System.Windows.Forms;

namespace dns
{
    public partial class UpdateItemForm : Form
    {
        private ItemsForm parentForm;

        public UpdateItemForm(ItemsForm f, string item)
        {
            InitializeComponent();
            parentForm = f;

            // Запись данных в typeComboBox
            QueriesClass.SetDataIntoList(parentForm.myConnection, typeComboBox, "название_типа", "типы");
            typeComboBox.SelectedItem = item;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            // Вызов метода UpdateData из главной формы
            parentForm.UpdateData(nameTextBox.Text, typeComboBox.SelectedItem.ToString(), 
                (int)countTextBox.Value, (int)priceTextBox.Value);
            this.Close();
        }
    }
}
