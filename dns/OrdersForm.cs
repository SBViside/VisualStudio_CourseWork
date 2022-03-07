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
                $"заказы.вид_доставки AS Доставка, товары.стоимость AS Стоимость, заказы.статус AS Статус, заказы.дата_оформления AS Дата " +
                $"FROM заказы INNER JOIN товары ON заказы.код_товара=товары.код_товара WHERE заказы.код_заказа={row.Cells[0].Value}";

            command = new OleDbCommand(query, myConnection);
            dbReader = command.ExecuteReader();

            dbReader.Read();
            groupBox2.Text = "Заказ от " + dbReader["Дата"].ToString().Split()[0];
            productLabel.Text = dbReader["Код товара"].ToString();
            countLabel.Text = dbReader["Количество"].ToString();
            deliveryLabel.Text = dbReader["Доставка"].ToString();
            priceLabel.Text = (double.Parse(dbReader["Стоимость"].ToString()) * int.Parse(countLabel.Text)).ToString() + "$";
            statusLabel.Text = dbReader["Статус"].ToString();
            dbReader.Close();
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
            groupBox2.Text = "Заказ";
        }

        private void statusLabel_TextChanged(object sender, EventArgs e)
        {
            if (statusLabel.Text == "активен")
            {
                statusLabel.ForeColor = Color.IndianRed;
                setStatusButton.Visible = true;
                return;
            }
            if (statusLabel.Text == "выполнен")
            {
                statusLabel.ForeColor = Color.OliveDrab;
                setStatusButton.Visible = false;
                return;
            }
            statusLabel.ForeColor = SystemColors.ControlText;
            setStatusButton.Visible = false;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetInfo(dataGridView1.CurrentRow);
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetInfo(dataGridView2.CurrentRow);
        }

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetInfo(dataGridView3.CurrentRow);
        }

        private void bindingNavigatorDelete_Click(object sender, EventArgs e)
        {
            // Окно подтверждения
            if (MessageBox.Show("Вы действительно хотите удалить строку?", "Подтверждение действия",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            DataGridView dgv = GetCurrentTab();
            string query = $"DELETE FROM заказы WHERE код_заказа={dgv.CurrentRow.Cells[0].Value}";
            QueriesClass.ApplyQuery_ReturnNone(myConnection, query);
            SetRefresh();
        }

        private void setStatusButton_Click(object sender, EventArgs e)
        {
            DataGridView dgv = GetCurrentTab();
            string query = $"UPDATE заказы SET статус='выполнен' WHERE код_заказа={dgv.CurrentRow.Cells[0].Value}";
            QueriesClass.ApplyQuery_ReturnNone(myConnection, query);
            SetRefresh();
        }

        private DataGridView GetCurrentTab()
        {
            switch (tabControl1.SelectedIndex)
            {
                case 1:
                    return dataGridView2;
                case 2:
                    return dataGridView3;
            }
            return dataGridView1;
        }

        private void посикToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridView dgv = GetCurrentTab();
            SearchForm sf = new SearchForm(dgv);
            for (int i = 0; i < dgv.ColumnCount; i++)
                sf.typeComboBox.Items.Add(dgv.Columns[i].HeaderText);
            sf.typeComboBox.SelectedIndex = 0;
            sf.ShowDialog();
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            AddOrderForm addOrderForm = new AddOrderForm();
            if(addOrderForm.ShowDialog() == DialogResult.OK) 
                SetRefresh();
        }
    }
}
