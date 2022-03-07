using System;
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
            QueriesClass.SetDataIntoList(myConnection, typeComboBox);
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
                string query = "SELECT товары.название, типы.название_типа, товары.количество, товары.стоимость " +
                    "FROM типы INNER JOIN товары ON типы.код_типа = товары.код_типа";
                OleDbCommand command = new OleDbCommand(query, myConnection); // Создаю запрос
                OleDbDataReader dbReader = command.ExecuteReader();   // Считываю данные

                // Загрузка данных в таблицу
                while (dbReader.Read())
                    dataGridView1.Rows.Add(dbReader["название"], dbReader["название_типа"],
                        dbReader["количество"], dbReader["стоимость"]);

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
            PanelOff();
            // Вызов метода обновления данных
            TableRefresh();
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            PanelOff();
            // Окно подтверждения
            if (MessageBox.Show("Вы действительно хотите удалить строку?", "Подтверждение действия",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            string query = $"SELECT * FROM заказы WHERE код_товара " +
                $"IN (SELECT код_товара FROM товары WHERE название='{dataGridView1.CurrentRow.Cells[0].Value}')";

            if (QueriesClass.HasLinks(myConnection, query))
            {
                MessageBox.Show("Невозможно удалить товар, так как он имеет связь с таблицей 'Заказы'",
                    "Действие невозможно", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            query = $"DELETE FROM товары WHERE название='{dataGridView1.CurrentRow.Cells[0].Value}' and " +
                $"количество={dataGridView1.CurrentRow.Cells[2].Value} and стоимость={dataGridView1.CurrentRow.Cells[3].Value}";
            QueriesClass.ApplyQuery_ReturnNone(myConnection, query);
            TableRefresh();
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
            PanelOff();
        }

        private void addPanel_VisibleChanged(object sender, EventArgs e)
        {
            ClearData();
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            if (nameTextBox.Text == "" ||
                countTextBox.Value <= 0 ||
                priceTextBox.Value <= 0)
            {
                MessageBox.Show("Проверьте введённые данные.", "Действие невозможно", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = $"SELECT код_типа FROM типы WHERE название_типа='{typeComboBox.SelectedItem}'";
            string id = QueriesClass.ApplyQuery_Return(myConnection, query);

            query = $"INSERT INTO товары (название, код_типа, количество, стоимость) " +
                $"VALUES ('{nameTextBox.Text}', {id}, {countTextBox.Value}, {priceTextBox.Value})";
            QueriesClass.ApplyQuery_ReturnNone(myConnection, query);
            PanelOff();
            TableRefresh();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            PanelOff();
            if (!(dataGridView1.CurrentRow.Index >= 0))
            {
                MessageBox.Show("Объект для изменения не выбран.", "Действие невозможно",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UpdateDataForm1 udf = new UpdateDataForm1(this, dataGridView1.CurrentRow.Cells[1].Value.ToString());
            DataGridViewRow curRow = dataGridView1.CurrentRow;
            udf.Text = $"Изменение {curRow.Cells[0].Value.ToString()}";
            udf.nameTextBox.Text = curRow.Cells[0].Value.ToString();
            udf.countTextBox.Value = int.Parse(curRow.Cells[2].Value.ToString());
            udf.priceTextBox.Value = int.Parse(curRow.Cells[3].Value.ToString());
            udf.ShowDialog();
        }

        public void UpdateData(string name, string type, int count, double price)
        {
            string query = $"SELECT код_типа FROM типы WHERE название_типа='{type}'";
            string id = QueriesClass.ApplyQuery_Return(myConnection, query);

            query = $"UPDATE товары SET код_типа = {id}, количество = {count}, " +
                $"стоимость = {price} WHERE название = '{name}'";
            QueriesClass.ApplyQuery_ReturnNone(myConnection, query);

            // Обновление таблицы
            TableRefresh();
        }

        private void посикToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchForm sf = new SearchForm(this.dataGridView1);
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
                sf.typeComboBox.Items.Add(dataGridView1.Columns[i].HeaderText);
            sf.typeComboBox.SelectedIndex = 0;
            PanelOff();
            sf.ShowDialog();
        }

        private void Items_FormClosed(object sender, FormClosedEventArgs e)
        {
            myConnection.Close();
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
            PanelOff();
            TypeEditor te = new TypeEditor(this);
            te.ShowDialog();
            QueriesClass.SetDataIntoList(myConnection, typeComboBox);
        }

        private void PanelOff()
        {
            ClearData();
            addPanel.Visible = false;
            addPanel.Enabled = false;
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            PanelOff();
        }

        private void шрифтТаблицыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.Cancel) return;
            dataGridView1.Font = fontDialog1.Font;
        }

        private void цветВыделенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.Cancel) return;
            dataGridView1.DefaultCellStyle.SelectionBackColor = colorDialog1.Color;
            dataGridView1.GridColor = colorDialog1.Color;
        }
    }
}
