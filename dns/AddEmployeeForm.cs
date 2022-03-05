using System;
using System.Windows.Forms;

namespace dns
{
    public partial class AddEmployeeForm : Form
    {
        private EmployeesForm parentForm;
        private string action;

        public AddEmployeeForm(EmployeesForm f, string ac)
        {
            InitializeComponent();

            parentForm = f;
            action = ac;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            if (surnameTextBox.Text.Length < 2 || nameTextBox.Text.Length < 2 ||
                patronymicTextBox.Text.Length < 2 || adressTextBox.Text.Length < 2 ||
                phoneTextBox.Text.Length < 2 || educationTextBox.Text.Length < 2 ||
                passportTextBox.Text.Length < 2 || positionTextBox.Text.Length < 2)
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
            string passport = passportTextBox.Text;
            string education = educationTextBox.Text;
            string phone = phoneTextBox.Text;
            string position = positionTextBox.Text;

            string query;

            switch (action)
            {
                case "adding":
                    query = $"INSERT INTO сотрудники (фамилия, имя, отчество, дата_рождения, образование, должность," +
                        $" адрес, телефон, паспорт) " +
                        $"VALUES ('{surname}', '{name}', '{patronymic}', '{date}', '{education}', " +
                        $"'{position}', '{adress}', '{phone}', '{passport}')";
                    QueriesClass.ApplyQuery_ReturnNone(parentForm.myConnection, query);
                    break;
                case "updating":
                    query = $"UPDATE сотрудники SET адрес='{adress}', " +
                        $"телефон='{phone}', должность='{position}', образование='{education}', паспорт='{passport}' " +
                        $"WHERE фамилия='{surname}' AND имя='{name}' and отчество='{patronymic}'";
                    QueriesClass.ApplyQuery_ReturnNone(parentForm.myConnection, query);
                    break;
            }
            parentForm.TableRefresh();
            this.Close();
        }
    }
}
