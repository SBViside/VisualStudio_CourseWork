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
    public partial class OrdersForm : Form
    {
        public string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=shopBD.accdb";
        public OleDbConnection myConnection;

        public OrdersForm(string log)
        {
            InitializeComponent();
            currUserLabel.Text = "Вход выполнен: " + log;

            // Подлючение к БД
            myConnection = new OleDbConnection(connectionString);
            myConnection.Open();

            // Заполнение таблицы
            TableRefresh_1();
        }

        private void OrdersForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            myConnection.Close();
        }

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void шрифтТаблицыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.Cancel) return;
            dataGridView1.Font = fontDialog1.Font;
            dataGridView2.Font = fontDialog1.Font;
            dataGridView3.Font = fontDialog1.Font;
        }

        public void TableRefresh_1()
        {
            try
            {
                dataGridView1.Rows.Clear();
                // Строка запроса к БД
                string query = "SELECT заказы.код_заказа AS [Код заказа], товары.название AS Товар, " +
                    "заказы.статус AS Статус FROM заказы " +
                    "INNER JOIN товары ON заказы.код_товара=товары.код_товара WHERE заказы.статус='активен'";
                OleDbCommand command = new OleDbCommand(query, myConnection); // Создаю запрос
                OleDbDataReader dbReader = command.ExecuteReader();   // Считываю данные

                // Загрузка данных в таблицу
                while (dbReader.Read())
                    dataGridView1.Rows.Add(dbReader["Код заказа"], dbReader["Товар"], dbReader["Статус"]);

                dbReader.Close();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                    row.Height = 30;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    TableRefresh_1();
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }
        }
    }
}
