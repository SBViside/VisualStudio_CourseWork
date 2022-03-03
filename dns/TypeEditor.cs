using System;
using System.Windows.Forms;
using System.Data.OleDb;

namespace dns
{
    public partial class TypeEditor : Form
    {
        private ItemsForm parentForm;

        public TypeEditor(ItemsForm f)
        {
            InitializeComponent();
            parentForm = f;
            ListRefresh();
        }

        private void ListRefresh()
        {
            try
            {
                listBox.Items.Clear();

                // Строка запроса к БД
                string query = "SELECT название_типа FROM типы";
                OleDbCommand command = new OleDbCommand(query, parentForm.myConnection); // Создаю запрос
                OleDbDataReader dbReader = command.ExecuteReader();   // Считываю данные

                // Загрузка данных в таблицу
                while (dbReader.Read())
                    listBox.Items.Add(dbReader["название_типа"]);

                dbReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ListSearch(string keyWord)
        {
            keyWord = keyWord.ToLower();
            for (int i = 0; i < listBox.Items.Count; i++)
            {
                string currentWord = listBox.Items[i].ToString().ToLower();
                if (currentWord.Contains(keyWord))
                {
                    listBox.SetSelected(i, true);
                    return;
                }
            }
            MessageBox.Show("Совпадений не найдено.", "Результат поиска", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListRefresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (typeName.Text.Length < 3) return;
            if (listBox.Items.Contains(typeName.Text))
            {
                MessageBox.Show("Похожий элемент уже существует.", "Действие невозможно.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                typeName.Clear();
                return;
            }

            string query = $"INSERT INTO типы (название_типа) VALUES ('{typeName.Text}')";
            QueriesClass.ApplyQuery_ReturnNone(parentForm.myConnection, parentForm.dataGridView1, query);

            ListRefresh();

            typeName.Clear();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            ListSearch(searchText.Text);
            searchText.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex < 0)
            {
                MessageBox.Show("Элемент не выбран.", "Действие невозможно.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Окно подтверждения
                if (MessageBox.Show("Вы действительно хотите удалить строку?", "Подтверждение действия", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string query = $"SELECT название FROM товары WHERE код_типа IN (SELECT код_типа FROM типы WHERE название_типа='{listBox.SelectedItem}')";
                    OleDbCommand command = new OleDbCommand(query, parentForm.myConnection); // Создаю запрос
                    OleDbDataReader dbReader = command.ExecuteReader();
                    if (dbReader.HasRows)
                    {
                        MessageBox.Show("Невозможно удалить категорию, так как она имеет связь с таблицей 'Товары'.", "Действие невозможно.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    query = $"DELETE FROM типы WHERE название_типа='{listBox.SelectedItem}'";
                    QueriesClass.ApplyQuery_ReturnNone(parentForm.myConnection, parentForm.dataGridView1, query);

                    // Обновление таблицы
                    ListRefresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
