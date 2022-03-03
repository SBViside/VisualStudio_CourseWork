using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace dns
{
    public partial class ClientsForm : Form
    {
        public string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=shopBD.accdb";
        public OleDbConnection myConnection;

        public ClientsForm(string log)
        {
            InitializeComponent();
            currUserLabel.Text = "Вход выполнен: " + log;

            // Подлючение к БД
            myConnection = new OleDbConnection(connectionString);
            myConnection.Open();

            // Заполнение таблицы
            TableRefresh();
        }

        // Метод обновления данных в таблице
        private void TableRefresh()
        {
            try
            {
                // Очистка таблицы на форме
                dataGridView1.Rows.Clear();
                // Строка запроса к БД
                string query = "SELECT фамилия, имя, отчество, возраст, телефон, эл_почта FROM клиенты";
                OleDbCommand command = new OleDbCommand(query, myConnection); // Создаю запрос
                OleDbDataReader dbReader = command.ExecuteReader();   // Считываю данные

                // Загрузка данных в таблицу
                while (dbReader.Read())
                    dataGridView1.Rows.Add(dbReader["фамилия"], dbReader["имя"], dbReader["отчество"], dbReader["возраст"], dbReader["телефон"], dbReader["эл_почта"]);

                dbReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClientsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            myConnection.Close();
        }

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void height_30_Click(object sender, EventArgs e)
        {
            dataGridView1.ColumnHeadersHeight = 30;
            height_30.Checked = true;
            height_40.Checked = false;
            height_50.Checked = false;
        }

        private void height_40_Click_1(object sender, EventArgs e)
        {
            dataGridView1.ColumnHeadersHeight = 40;
            height_30.Checked = false;
            height_40.Checked = true;
            height_50.Checked = false;
        }

        private void height_50_Click(object sender, EventArgs e)
        {
            dataGridView1.ColumnHeadersHeight = 50;
            height_30.Checked = false;
            height_40.Checked = false;
            height_50.Checked = true;
        }

        private void посикToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchForm sf = new SearchForm(this);
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
                sf.typeComboBox.Items.Add(dataGridView1.Columns[i].HeaderText);
            sf.typeComboBox.SelectedIndex = 0;
            sf.ShowDialog();
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить строку?", "Подтверждение действия", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            string query = $"DELETE FROM клиенты WHERE фамилия='{dataGridView1.CurrentRow.Cells[0].Value}' and имя='{dataGridView1.CurrentRow.Cells[1].Value}' and отчество='{dataGridView1.CurrentRow.Cells[2].Value}' and возраст={dataGridView1.CurrentRow.Cells[3].Value}";
            QueriesClass.ApplyQuery_ReturnNone(myConnection, dataGridView1, query);
            TableRefresh();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            TableRefresh();
        }
    }
}
