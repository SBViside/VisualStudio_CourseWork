using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.OleDb;
using Excel = Microsoft.Office.Interop.Excel;

namespace dns
{
    public partial class ItemsForm : Form
    {
        // Строка подлючения
        const string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=shopBD.mdb";
        public OleDbConnection myConnection;

        public ItemsForm(string log)
        {
            InitializeComponent();

            this.Text = "Товары | Вход выполнен: " + log;

            // Подлючение к Базе данных через строку подключения
            myConnection = new OleDbConnection(connectionString);
            myConnection.Open();

            // Заполнение таблицы
            TableRefresh();

            // Заполнение ComboBox на панели добавления товара
            QueriesClass.SetDataIntoList(myConnection, typeComboBox, "название_типа", "типы");
        }

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Выход из приложения
            Application.Exit();
        }

        // Метод, который заполняет таблицу данными из базы данных
        private void TableRefresh()
        {
            addPanel.Visible = false;

            try
            {
                // Очистка текущей таблицы
                dataGridView1.Rows.Clear();

                // Строка запроса к БД
                string query = "SELECT товары.название, типы.название_типа, товары.количество, товары.стоимость " +
                    "FROM типы INNER JOIN товары ON типы.код_типа=товары.код_типа";
                OleDbCommand command = new OleDbCommand(query, myConnection); // Создание запроса
                OleDbDataReader dbReader = command.ExecuteReader();   // Выполнение запроса

                // Внесение данных в таблицу
                while (dbReader.Read())
                    dataGridView1.Rows.Add(dbReader["название"], dbReader["название_типа"],
                        dbReader["количество"], dbReader["стоимость"]);

                // Закрытие DataReader
                dbReader.Close();

                // Установка высоты каждой строки в таблице
                foreach (DataGridViewRow row in dataGridView1.Rows)
                    row.Height = 30;

                dataGridView1.ClearSelection();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Height = heightBar.Value;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            // Вызов метода обновления таблицы
            TableRefresh();
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            // Скрыть панель добавления
            addPanel.Visible = false;

            // Проверка на выбранный товар
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Товар не выбран", "Действие невозможно",
                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Окно подтверждения
            if (MessageBox.Show("Вы действительно хотите удалить товар?", "Подтверждение действия",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            // Строка запроса на проверку связей
            string query = $"SELECT * FROM заказы WHERE код_товара IN " +
                $"(SELECT код_товара FROM товары WHERE название='{dataGridView1.CurrentRow.Cells[0].Value}')";

            if (QueriesClass.HasLinks(myConnection, query))
            {
                MessageBox.Show("Удаление невозможно, так как на этот товар имеются заказы",
                    "Действие невозможно", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Запичь данных в переменные
            string name = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            string count = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            string price = dataGridView1.CurrentRow.Cells[3].Value.ToString();

            // Строка запроса на удаление 
            query = $"DELETE FROM товары WHERE название='{name}' AND " +
                $"количество={count} AND стоимость={price}";
            QueriesClass.ApplyQuery_ReturnNone(myConnection, query);

            // Обновление таблицы
            TableRefresh();
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            addPanel.Visible = !addPanel.Visible;
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
            addPanel.Visible = false;
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            // Проверка на содержимое полей. Если пустые, вывести сообщение
            if (nameTextBox.Text == "" ||
                countTextBox.Value <= 0 ||
                priceTextBox.Value <= 0)
            {
                MessageBox.Show("Проверьте введённые данные", "Действие невозможно",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Проверка на совпадения
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string currName = row.Cells[0].Value.ToString();
                if (currName == nameTextBox.Text)
                {
                    MessageBox.Show("Товар с похожим названием уже существует", "Повторите попытку",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            // Определяет код категории товара по названию
            string query = $"SELECT код_типа FROM типы WHERE название_типа='{typeComboBox.SelectedItem}'";
            string id = QueriesClass.ApplyQuery_Return(myConnection, query);

            // Данные заносятся в Базу данных
            query = $"INSERT INTO товары (название, код_типа, количество, стоимость) " +
                $"VALUES ('{nameTextBox.Text}', {id}, {countTextBox.Value}, {priceTextBox.Value})";
            QueriesClass.ApplyQuery_ReturnNone(myConnection, query);

            // Таблица обновляется
            TableRefresh();
        }

        public void UpdateData(string name, string type, int count, double price)
        {
            // Определяет код категории товара по названию
            string query = $"SELECT код_типа FROM типы WHERE название_типа='{type}'";
            string id = QueriesClass.ApplyQuery_Return(myConnection, query);

            // Данные обновляются в Базе данных
            query = $"UPDATE товары SET код_типа = {id}, количество = {count}, " +
                $"стоимость = {price} WHERE название = '{name}'";
            QueriesClass.ApplyQuery_ReturnNone(myConnection, query);

            // Обновление таблицы
            TableRefresh();
        }

        private void поиcкToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Очистка выделения
            addPanel.Visible = false;
            dataGridView1.ClearSelection();

            // Создает окно поиска
            SearchForm sf = new SearchForm(dataGridView1);
            sf.ShowDialog();
        }

        private void Items_FormClosed(object sender, FormClosedEventArgs e)
        {
            myConnection.Close();
        }

        private void слеваToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addPanel.Dock = DockStyle.Left;
            слеваToolStripMenuItem.Checked = !(справаToolStripMenuItem.Checked = false);
        }

        private void справаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addPanel.Dock = DockStyle.Right;
            справаToolStripMenuItem.Checked = !(слеваToolStripMenuItem.Checked = false);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            dataGridView1.ColumnHeadersHeight = 30;
            height_30.Checked = !(height_40.Checked = height_50.Checked = false);
        }

        private void height_40_Click(object sender, EventArgs e)
        {
            dataGridView1.ColumnHeadersHeight = 40;
            height_40.Checked = !(height_30.Checked = height_50.Checked = false);
        }

        private void height_50_Click(object sender, EventArgs e)
        {
            dataGridView1.ColumnHeadersHeight = 50;
            height_50.Checked = !(height_40.Checked = height_30.Checked = false);
        }

        private void редакторКатегорийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addPanel.Visible = false;

            TypeEditor typeEditor = new TypeEditor(myConnection);
            typeEditor.ShowDialog();

            // Обновляю данные в ComboBox
            QueriesClass.SetDataIntoList(myConnection, typeComboBox, "название_типа", "типы");
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            addPanel.Visible = false;
        }

        private void шрифтТаблицыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Меняю шрифт в таблице
            if (fontDialog1.ShowDialog() == DialogResult.Cancel) return;

            if (fontDialog1.Font.Size >= 15)
            {
                fontDialog1.Font = new Font(fontDialog1.Font.FontFamily, 14);
                MessageBox.Show("Вы выбрали слишком большой размер шрифта, " +
                    "поэтому размер был автоматически установлен на 14.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            dataGridView1.Font = fontDialog1.Font;
        }

        private void цветВыделенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Меняю цвет выделения в таблице
            if (colorDialog1.ShowDialog() == DialogResult.Cancel) return;
            dataGridView1.DefaultCellStyle.SelectionBackColor = colorDialog1.Color;
            dataGridView1.GridColor = colorDialog1.Color;
        }

        private void addPanel_VisibleChanged(object sender, EventArgs e)
        {
            // Когда, статус Visible у панели меняется, происходит очистка полей ввода
            ClearData();
        }

        private void updateItemButton_Click(object sender, EventArgs e)
        {
            addPanel.Visible = false;

            // Проверка, выбран ли товар
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Товар для изменения не выбран", "Действие невозможно",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            UpdateItemForm udf = new UpdateItemForm(this, dataGridView1.CurrentRow.Cells[1].Value.ToString());
            DataGridViewRow curRow = dataGridView1.CurrentRow;

            udf.Text = $"Изменение {curRow.Cells[0].Value}";
            udf.nameTextBox.Text = curRow.Cells[0].Value.ToString();
            udf.countTextBox.Value = int.Parse(curRow.Cells[2].Value.ToString());
            udf.priceTextBox.Value = int.Parse(curRow.Cells[3].Value.ToString());

            udf.ShowDialog();
        }

        private void heightBar_Scroll(object sender, EventArgs e)
        {
            sizeLabel.Text = heightBar.Value.ToString();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Height = heightBar.Value;
            }
        }

        private void выводВExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

        private void ExportToExcel()
        {
            try
            {
                Excel.Application exApp = new Excel.Application();
                exApp.Visible = true;
                exApp.Workbooks.Add();
                Excel.Worksheet workSheet = exApp.ActiveSheet;

                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    workSheet.Cells[1, i + 1] = dataGridView1.Columns[i].HeaderText;

                    for (int j = 0; j < dataGridView1.Rows.Count; j++)
                    {
                        workSheet.Cells[j + 2, i + 1] = dataGridView1.Rows[j].Cells[i].Value.ToString();
                    }
                }
                workSheet.Range["A1", "D1"].Font.Bold = true;
                workSheet.UsedRange.Borders.Weight = 2;
                exApp.Columns.AutoFit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void searchString_TextChanged(object sender, EventArgs e)
        {
            if (searchString.Text.Length < 1)
            {
                dataGridView1.ClearSelection();
                return;
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string concat = "";
                foreach (DataGridViewCell cell in row.Cells) concat += cell.Value.ToString();
                concat = concat.ToLower();
                if (concat.Contains(searchString.Text.ToLower()))
                {
                    dataGridView1.FirstDisplayedScrollingRowIndex = row.Index;
                    row.Selected = true;
                }
            }
        }
    }
}
