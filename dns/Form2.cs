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
using System.Configuration;

namespace dns
{
    public partial class Items : Form
    {
        public string connectionString = ConfigurationManager.ConnectionStrings["dns.Properties.Settings.shopBDConnectionString"].ConnectionString;

        public Items(string log)
        {
            InitializeComponent();
            currUserLabel.Text = log;
        }

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Items_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'shopBDDataSet.типы' table. You can move, or remove it, as needed.
            this.типыTableAdapter.Fill(this.shopBDDataSet.типы);
            TableRefresh();
        }

        private void TableRefresh()
        {
            try
            {
                string query = "SELECT товары.код_товара as Код, товары.название as Название, типы.название_типа as Тип, товары.количество as Количество, товары.стоимость as Стоимость FROM типы INNER JOIN товары ON типы.код_типа = товары.код_типа";
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, connectionString))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;
                }
                dataGridView1.Columns[0].Width = 40;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            TableRefresh();
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            // По индексу строки удаляем всю строку
            try
            {
                // Окно подтверждения
                if (MessageBox.Show("Вы действительно хотите удалить строку?", "Подтверждение действия", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string query = $"DELETE FROM товары WHERE код_товара={dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value}";
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, connectionString))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                    }
                    TableRefresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Действие невозможно", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            // открытие панели ввода данных
            addPanel.Visible = !addPanel.Visible;
            addPanel.Enabled = !addPanel.Enabled;
        }

        private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Обновление данных
            TableRefresh();
        }

        private void ClearData()
        {
            button1.Text = "Добавить";
            // Метод очистки полей ввода
            nameTextBox.Clear();
            countTextBox.Value = 0;
            priceTextBox.Value = 0;
            typeComboBox.SelectedIndex = 0;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            addPanel.Enabled = false;
            addPanel.Visible = false;
        }

        private void addPanel_VisibleChanged(object sender, EventArgs e)
        {
            ClearData();
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            dataGridView1.CurrentRow.Selected = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (nameTextBox.Text == "" || countTextBox.Value <= 0 || priceTextBox.Value <= 0)
            {
                MessageBox.Show("Проверьте введённые данные.", "Действие невозможно", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Добавление новой записи в таблицу
            try
            {
                string query = $"INSERT INTO товары (название, код_типа, количество, стоимость) VALUES ('{nameTextBox.Text}', {typeComboBox.SelectedIndex + 1}, {countTextBox.Value}, {priceTextBox.Value})";
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, connectionString))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                }
                TableRefresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Действие невозможно", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                addPanel.Visible = false;
                addPanel.Enabled = false;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (!(dataGridView1.CurrentRow.Index >= 0))
            {
                MessageBox.Show("Объект для изменения не выбран.", "Действие невозможно", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UpdateDataForm1 udf = new UpdateDataForm1(this);
            DataGridViewRow curRow = dataGridView1.CurrentRow;
            udf.Text = $"Изменение {curRow.Cells[1].Value.ToString()}";
            udf.nameTextBox.Text = curRow.Cells[1].Value.ToString();
            udf.countTextBox.Value = int.Parse(curRow.Cells[3].Value.ToString());
            udf.priceTextBox.Value = int.Parse(curRow.Cells[4].Value.ToString());
            udf.ShowDialog();
        }

        public void Search(string keyWord, int col, bool reg)
        {
            string currWord;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                currWord = dataGridView1.Rows[i].Cells[col].Value.ToString();
                if (reg)
                {
                    if (currWord.Contains(keyWord))
                    {
                        dataGridView1.ClearSelection();
                        dataGridView1.Rows[i].Selected = true;
                        return;
                    }
                }
                else
                {
                    if (currWord.ToLower().Contains(keyWord.ToLower()))
                    {
                        dataGridView1.ClearSelection();
                        dataGridView1.Rows[i].Selected = true;
                        return;
                    }
                }
            }
            MessageBox.Show("Совпадений не найдено!", "Результат поиска", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void UpdateData(string name, int type, double count, double price)
        {
            try
            {
                string query = $"UPDATE товары SET код_типа = {type}, количество = {count}, стоимость = {price} WHERE название = '{name}'";
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, connectionString))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                }
                TableRefresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Действие невозможно", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void посикToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchForm sf = new SearchForm(this);
            for (int i = 1; i < dataGridView1.ColumnCount; i++) 
                sf.typeComboBox.Items.Add(dataGridView1.Columns[i].HeaderText);
            sf.typeComboBox.SelectedIndex = 0;
            sf.ShowDialog();
        }
    }
}
