using System;
using System.Windows.Forms;
using System.Data.OleDb;

namespace dns
{
    public partial class AddClientForm : Form
    {
        private OleDbConnection myConnection;
        private string action;

        public AddClientForm(OleDbConnection con, string ac)
        {
            InitializeComponent();

            myConnection = con;
            action = ac;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            //this.Close();
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            if ( surnameTextBox.Text.Length < 1 || nameTextBox.Text.Length < 1 ||
                patronymicTextBox.Text.Length < 1 || adressTextBox.Text.Length < 1 ||
                phoneTextBox.Text.Length < 1 || emailTextBox.Text.Length < 1 )
            {
                MessageBox.Show("Проверьте введённые данные", "Действие невозможно", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string surname = surnameTextBox.Text.Replace(' ', '_');
            string name = nameTextBox.Text.Replace(' ', '_');
            string patronymic = patronymicTextBox.Text.Replace(' ', '_');

            string query = $"SELECT * FROM клиенты WHERE фамилия='{surname}' AND имя='{name}' AND отчество='{patronymic}'";
            if (action=="adding" && QueriesClass.HasLinks(myConnection, query))
            {
                MessageBox.Show("Клиент с таким именен уже существует", "Повторите попытку",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string date = dateTimePicker.Value.ToString();
            string adress = adressTextBox.Text;
            string phone = phoneTextBox.Text;
            string email = emailTextBox.Text;

            switch (action)
            {
                case "adding":
                    query = $"INSERT INTO клиенты (фамилия, имя, отчество, дата_рождения, адрес, телефон, эл_почта) " +
                        $"VALUES ('{surname}', '{name}', '{patronymic}', '{date}', '{adress}', '{phone}', '{email}')";
                    QueriesClass.ApplyQuery_ReturnNone(myConnection, query);
                    break;

                case "updating":
                    query = $"UPDATE клиенты SET адрес='{adress}', " +
                        $"телефон='{phone}', эл_почта='{email}' WHERE фамилия='{surname}' " +
                        $"and имя='{name}' and отчество='{patronymic}'";
                    QueriesClass.ApplyQuery_ReturnNone(myConnection, query);
                    break;
            }
            this.Close();
        }
    }
}
