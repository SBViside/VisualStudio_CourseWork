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
            for(int i = 0; i < listBox.Items.Count; i++)
            {
                string currentWord = listBox.Items[i].ToString();
                if (currentWord.ToLower().Contains(keyWord.ToLower()))
                {
                    listBox.SetSelected(i, true);
                    return;
                }
            }
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

            try
            {
                // Строка запроса к БД
                string query = $"INSERT INTO типы (название_типа) VALUES ('{typeName.Text}')";
                OleDbCommand command = new OleDbCommand(query, parentForm.myConnection); // Создаю запрос
                command.ExecuteNonQuery();

                ListRefresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                    // Строка запроса к БД
                    string query = $"DELETE FROM типы WHERE название_типа='{listBox.SelectedItem}'";
                    OleDbCommand command = new OleDbCommand(query, parentForm.myConnection); // Создаю запрос
                    command.ExecuteNonQuery();  // Выполняю запрос

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
