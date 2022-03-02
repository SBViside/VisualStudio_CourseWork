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
    public partial class ItemsForm : Form
    {
        public string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=shopBD.accdb";
        public OleDbConnection myConnection;

        public ItemsForm(string log)
        {
            InitializeComponent();
            currUserLabel.Text = "Вход выполнен: " + log;

            // Подлючение к БД
            myConnection = new OleDbConnection(connectionString);
            myConnection.Open();

            // Заполнение таблицы
            TableRefresh();

            // Заполнение ComboBox
            SetDataIntoList(typeComboBox);
        }

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Метод обновления данных в таблице
        private void TableRefresh()
        {
            try
            {
                dataGridView1.Rows.Clear();
                // Строка запроса к БД
                string query = "SELECT товары.название, типы.название_типа, товары.количество, товары.стоимость FROM типы INNER JOIN товары ON типы.код_типа = товары.код_типа";
                OleDbCommand command = new OleDbCommand(query, myConnection); // Создаю запрос
                OleDbDataReader dbReader = command.ExecuteReader();   // Считываю данные

                // Загрузка данных в таблицу
                while (dbReader.Read())
                    dataGridView1.Rows.Add(dbReader["название"], dbReader["название_типа"], dbReader["количество"], dbReader["стоимость"]);

                dbReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            // Вызов метода обновления данных
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
                    // Строка запроса к БД
                    string query = $"DELETE FROM товары WHERE название='{dataGridView1.CurrentRow.Cells[0].Value}' and количество={dataGridView1.CurrentRow.Cells[2].Value} and стоимость={dataGridView1.CurrentRow.Cells[3].Value}";
                    OleDbCommand command = new OleDbCommand(query, myConnection); // Создаю запрос
                    command.ExecuteNonQuery();  // Выполняю запрос

                    // Обновление таблицы
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

        // Метод очистки полей ввода
        private void ClearData()
        {
            nameTextBox.Clear();
            countTextBox.Value = 1;
            priceTextBox.Value = 1;
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
            // Выделяет всю строку по клику на ячейку dataGridView
            dataGridView1.CurrentRow.Selected = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (nameTextBox.Text == "" ||
                countTextBox.Value <= 0 ||
                priceTextBox.Value <= 0)
            {
                MessageBox.Show("Проверьте введённые данные.", "Действие невозможно", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Добавление новой записи в таблицу
            try
            {
                // Строка запроса к БД
                string query = $"INSERT INTO товары (название, код_типа, количество, стоимость) VALUES ('{nameTextBox.Text}', {typeComboBox.SelectedIndex + 1}, {countTextBox.Value}, {priceTextBox.Value})";
                OleDbCommand command = new OleDbCommand(query, myConnection); // Создаю запрос
                command.ExecuteNonQuery();  // Выполняю запрос

                // Обновление таблицы
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

            // Строка запроса к БД
            string query = $"SELECT код_типа FROM товары WHERE название='{dataGridView1.CurrentRow.Cells[0].Value}'";
            OleDbCommand command = new OleDbCommand(query, myConnection); // Создаю запрос

            UpdateDataForm1 udf = new UpdateDataForm1(this, int.Parse(command.ExecuteScalar().ToString()) - 1);
            DataGridViewRow curRow = dataGridView1.CurrentRow;
            udf.Text = $"Изменение {curRow.Cells[0].Value.ToString()}";
            udf.nameTextBox.Text = curRow.Cells[0].Value.ToString();
            udf.countTextBox.Value = int.Parse(curRow.Cells[2].Value.ToString());
            udf.priceTextBox.Value = int.Parse(curRow.Cells[3].Value.ToString());
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
            // Изменение данных в таблице
            try
            {
                // Строка запроса к БД
                string query = $"UPDATE товары SET код_типа = {type}, количество = {count}, стоимость = {price} WHERE название = '{name}'";
                OleDbCommand command = new OleDbCommand(query, myConnection); // Создаю запрос
                command.ExecuteNonQuery();  // Выполняю запрос

                // Обновление таблицы
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
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
                sf.typeComboBox.Items.Add(dataGridView1.Columns[i].HeaderText);
            sf.typeComboBox.SelectedIndex = 0;
            sf.ShowDialog();
        }

        private void Items_FormClosed(object sender, FormClosedEventArgs e)
        {
            myConnection.Close();
        }

        // Метод загрузки данных в ComboBox
        public void SetDataIntoList(ComboBox cb)
        {
            try
            {
                cb.Items.Clear();
                // Строка запроса к БД
                string query = $"SELECT название_типа FROM типы";
                OleDbCommand command = new OleDbCommand(query, myConnection); // Создаю запрос
                OleDbDataReader dbReader = command.ExecuteReader();

                // Запись данных
                while (dbReader.Read())
                    cb.Items.Add(dbReader["название_типа"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Действие невозможно", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void слеваToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addPanel.Dock = DockStyle.Left;
            слеваToolStripMenuItem.Checked = true;
            справаToolStripMenuItem.Checked = false;
        }

        private void справаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addPanel.Dock = DockStyle.Right;
            справаToolStripMenuItem.Checked = true;
            слеваToolStripMenuItem.Checked = false;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            dataGridView1.ColumnHeadersHeight = 30;
            height_30.Checked = true;
            height_40.Checked = false;
            height_50.Checked = false;
        }

        private void height_40_Click(object sender, EventArgs e)
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

        private void редакторКатегорийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TypeEditor te = new TypeEditor(this);
            te.ShowDialog();
            SetDataIntoList(typeComboBox);
        }
    }
}
