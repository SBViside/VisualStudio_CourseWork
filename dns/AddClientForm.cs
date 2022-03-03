using System;
using System.Windows.Forms;

namespace dns
{
    public partial class AddClientForm : Form
    {
        private ClientsForm parentForm;

        public AddClientForm(ClientsForm f)
        {
            InitializeComponent();
            parentForm = f;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            if ( surnameTextBox.Text.Length < 2 || nameTextBox.Text.Length < 2 ||
                patronymicTextBox.Text.Length < 2 || adressTextBox.Text.Length < 2 ||
                phoneTextBox.Text.Length < 2 || emailTextBox.Text.Length < 2 )
            {
                MessageBox.Show("Проверьте введённые данные.", "Действие невозможно", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string surname = surnameTextBox.Text;
            string name = nameTextBox.Text;
            string patronymic = patronymicTextBox.Text;
            string date = dateTimePicker.Value.ToString();
            string adress = adressTextBox.Text;
            string phone = phoneTextBox.Text;
            string email = emailTextBox.Text;

            string query = $"INSERT INTO клиенты (фамилия, имя, отчество, дата_рождения, адрес, телефон, эл_почта) " +
                $"VALUES ('{surname}', '{name}', '{patronymic}', '{date}', '{adress}', '{phone}', '{email}')";
            QueriesClass.ApplyQuery_ReturnNone(parentForm.myConnection, query);

            this.Close();
        }
    }
}
