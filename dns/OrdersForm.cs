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
            SetRefresh();
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

        private void TableRefresh(DataGridView dgv, string query)
        {
            try
            {
                dgv.Rows.Clear();
                OleDbCommand command = new OleDbCommand(query, myConnection); // Создаю запрос
                OleDbDataReader dbReader = command.ExecuteReader();   // Считываю данные

                // Загрузка данных в таблицу
                while (dbReader.Read())
                    dgv.Rows.Add(dbReader["Код заказа"], dbReader["Товар"], dbReader["Статус"]);

                dbReader.Close();

                foreach (DataGridViewRow row in dgv.Rows)
                    row.Height = 30;
                dgv.ClearSelection();
                ClearLabels();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SetRefresh()
        {
            string query;
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    query = "SELECT заказы.код_заказа AS [Код заказа], товары.название AS Товар, " +
                    "заказы.статус AS Статус FROM заказы " +
                    "INNER JOIN товары ON заказы.код_товара=товары.код_товара WHERE заказы.статус='активен'";
                    TableRefresh(this.dataGridView1, query);
                    break;
                case 1:
                    query = "SELECT заказы.код_заказа AS [Код заказа], товары.название AS Товар, " +
                    "заказы.статус AS Статус FROM заказы " +
                    "INNER JOIN товары ON заказы.код_товара=товары.код_товара WHERE заказы.статус='выполнен'";
                    TableRefresh(this.dataGridView2, query);
                    break;
                case 2:
                    query = "SELECT заказы.код_заказа AS [Код заказа], товары.название AS Товар, " +
                    "заказы.статус AS Статус FROM заказы INNER JOIN товары ON заказы.код_товара=товары.код_товара";
                    TableRefresh(this.dataGridView3, query);
                    break;
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            SetRefresh();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetRefresh();
        }

        private void GetInfo(DataGridViewRow row)
        {
            string query = $"SELECT клиенты.фамилия AS Фамилия, клиенты.имя AS Имя, " +
                $"клиенты.отчество AS Отчество, клиенты.адрес AS Адрес FROM заказы INNER JOIN клиенты " +
                $"ON заказы.код_клиента=клиенты.код_клиента WHERE заказы.код_заказа={row.Cells[0].Value}";

            OleDbCommand command = new OleDbCommand(query, myConnection);
            OleDbDataReader dbReader = command.ExecuteReader();

            dbReader.Read();
            surnameLabel.Text = dbReader["Фамилия"].ToString();
            nameLabel.Text = dbReader["Имя"].ToString();
            patronymicLabel.Text = dbReader["Отчество"].ToString();
            addressLabel.Text = dbReader["Адрес"].ToString();
            dbReader.Close();

            query = $"SELECT заказы.код_товара AS [Код товара], заказы.количество AS Количество, " +
                $"заказы.вид_доставки AS Доставка, товары.стоимость AS Стоимость, заказы.статус AS Статус " +
                $"FROM заказы INNER JOIN товары ON заказы.код_товара=товары.код_товара WHERE заказы.код_заказа={row.Cells[0].Value}";

            command = new OleDbCommand(query, myConnection);
            dbReader = command.ExecuteReader();

            dbReader.Read();
            productLabel.Text = dbReader["Код товара"].ToString();
            countLabel.Text = dbReader["Количество"].ToString();
            deliveryLabel.Text = dbReader["Доставка"].ToString();
            priceLabel.Text = (double.Parse(dbReader["Стоимость"].ToString()) * int.Parse(countLabel.Text)).ToString() + "$";
            statusLabel.Text = dbReader["Статус"].ToString();
            dbReader.Close();

            if (statusLabel.Text == "активен") setStatusButton.Visible = true;
        }

        private void ClearLabels()
        {
            surnameLabel.Text = "(нет данных)";
            nameLabel.Text = "(нет данных)";
            patronymicLabel.Text = "(нет данных)";
            addressLabel.Text = "(нет данных)";
            productLabel.Text = "(нет данных)";
            countLabel.Text = "(нет данных)";
            deliveryLabel.Text = "(нет данных)";
            priceLabel.Text = "0$";
            statusLabel.Text = "(нет данных)";
            setStatusButton.Visible = false;
        }

        private void statusLabel_TextChanged(object sender, EventArgs e)
        {
            if (statusLabel.Text == "активен")
            {
                statusLabel.ForeColor = Color.IndianRed;
                return;
            }
            if (statusLabel.Text == "выполнен")
            {
                statusLabel.ForeColor = Color.OliveDrab;
                return;
            }
            statusLabel.ForeColor = SystemColors.ControlText;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetInfo(dataGridView1.CurrentRow);
        }
    }
}
