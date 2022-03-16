using System;
using System.Windows.Forms;
using System.Data.OleDb;

namespace dns
{
    public partial class TypeEditor : Form
    {
        //private ItemsForm parentForm;
        private OleDbConnection myConnection;

        public TypeEditor(OleDbConnection con)
        {
            InitializeComponent();

            myConnection = con;
            ListRefresh();
        }

        private void ListRefresh()
        {
            try
            {
                dataGridView1.Rows.Clear();

                // Строка запроса к БД
                string query = "SELECT название_типа FROM типы";
                OleDbCommand command = new OleDbCommand(query, myConnection); // Создаю запрос
                OleDbDataReader dbReader = command.ExecuteReader();   // Считываю данные

                // Загрузка данных в таблицу
                while (dbReader.Read())
                    dataGridView1.Rows.Add(dbReader["название_типа"]);

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

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                string currentWord = dataGridView1.Rows[i].Cells[0].Value.ToString().ToLower();
                if (currentWord.Contains(keyWord))
                {
                    dataGridView1.Rows[i].Selected = true;
                    return;
                }
            }
            MessageBox.Show("Совпадений не найдено", "Результат поиска",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (typeName.Text.Length < 3) return;

            foreach (DataGridViewRow row in dataGridView1.Rows)
                if (row.Cells[0].Value.ToString() == typeName.Text.Trim())
                {
                    MessageBox.Show("Похожая категория уже существует", "Действие невозможно",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    typeName.Clear();
                    return;
                }

            string query = $"INSERT INTO типы (название_типа) VALUES ('{typeName.Text}')";
            QueriesClass.ApplyQuery_ReturnNone(myConnection, query);

            ListRefresh();

            typeName.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow.Index < 0)
            {
                MessageBox.Show("Категория не выбрана", "Действие невозможно",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Окно подтверждения
            if (MessageBox.Show("Вы действительно хотите удалить категорию?", "Подтверждение действия",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;


            try
            { 
                string query = $"SELECT название FROM товары WHERE код_типа " +
                    $"IN (SELECT код_типа FROM типы WHERE название_типа='{dataGridView1.CurrentRow.Cells[0].Value}')";

                if (QueriesClass.HasLinks(myConnection, query))
                {
                    MessageBox.Show("Невозможно удалить категорию, так как она имеет связь с таблицей 'Товары'",
                        "Действие невозможно", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                query = $"DELETE FROM типы WHERE название_типа='{dataGridView1.CurrentRow.Cells[0].Value}'";
                QueriesClass.ApplyQuery_ReturnNone(myConnection, query);

                // Обновление таблицы
                ListRefresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            ListRefresh();
        }

        private void searchText_TextChanged(object sender, EventArgs e)
        {

            ListSearch(searchText.Text);
        }
    }
}
