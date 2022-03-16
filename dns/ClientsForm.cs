using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.OleDb;
using Excel = Microsoft.Office.Interop.Excel;

namespace dns
{
    public partial class ClientsForm : Form
    {
        const string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=shopBD.mdb";
        public OleDbConnection myConnection;

        public ClientsForm(string log)
        {
            InitializeComponent();

            this.Text = "Клиенты | Вход выполнен: " + log;

            // Подлючение к БД
            myConnection = new OleDbConnection(connectionString);
            myConnection.Open();

            // Заполнение таблицы
            TableRefresh();
        }

        // Метод обновления данных в таблице
        public void TableRefresh()
        {
            try
            {
                // Очистка таблицы на форме
                dataGridView1.Rows.Clear();
                // Строка запроса к БД
                string query = "SELECT фамилия, имя, отчество, дата_рождения, адрес, телефон, эл_почта FROM клиенты";
                OleDbCommand command = new OleDbCommand(query, myConnection); // Создаю запрос
                OleDbDataReader dbReader = command.ExecuteReader();   // Считываю данные

                // Загрузка данных в таблицу
                while (dbReader.Read())
                    dataGridView1.Rows.Add(dbReader["фамилия"], dbReader["имя"], dbReader["отчество"],
                        dbReader["дата_рождения"].ToString().Split()[0], dbReader["адрес"],
                        dbReader["телефон"], dbReader["эл_почта"]);

                dbReader.Close();

                dataGridView1.ClearSelection();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                    row.Height = heightBar.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClientsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            myConnection.Close();
        }

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void height_30_Click(object sender, EventArgs e)
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

        private void посикToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();

            SearchForm sf = new SearchForm(dataGridView1);
            sf.ShowDialog();
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();

            AddClientForm addClientForm = new AddClientForm(myConnection, "adding");

            DialogResult result = addClientForm.ShowDialog();
            if (result == DialogResult.No || result == DialogResult.Abort) return;

            TableRefresh();
        }

        private void bindingNavigatorDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Клиент не выбран", "Действие невозможно",
                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Вы действительно хотите удалить клиента?", "Подтверждение действия",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            string query = $"SELECT * FROM заказы WHERE код_клиента " +
                $"IN (SELECT код_клиента FROM клиенты WHERE фамилия='{dataGridView1.CurrentRow.Cells[0].Value}' " +
                $"AND имя='{dataGridView1.CurrentRow.Cells[1].Value}' AND отчество='{dataGridView1.CurrentRow.Cells[2].Value}')";

            if (QueriesClass.HasLinks(myConnection, query))
            {
                MessageBox.Show("Невозможно удалить клиента, так как он имеет связь с таблицей 'Заказы'",
                    "Действие невозможно", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            query = $"DELETE FROM клиенты WHERE фамилия='{dataGridView1.CurrentRow.Cells[0].Value}' " +
                $"AND имя='{dataGridView1.CurrentRow.Cells[1].Value}' AND отчество='{dataGridView1.CurrentRow.Cells[2].Value}' " +
                $"AND адрес='{dataGridView1.CurrentRow.Cells[4].Value}'";

            QueriesClass.ApplyQuery_ReturnNone(myConnection, query);
            TableRefresh();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            TableRefresh();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Клиент не выбран", "Действие невозможно",
                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            AddClientForm addClientForm = new AddClientForm(myConnection, "updating");

            DataGridViewRow currentRow = dataGridView1.CurrentRow;

            addClientForm.surnameTextBox.Enabled = false;
            addClientForm.nameTextBox.Enabled = false;
            addClientForm.patronymicTextBox.Enabled = false;

            string[] date = currentRow.Cells[3].Value.ToString().Split('.');
            addClientForm.dateTimePicker.Value = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]));

            addClientForm.surnameTextBox.Text = currentRow.Cells[0].Value.ToString();
            addClientForm.nameTextBox.Text = currentRow.Cells[1].Value.ToString();
            addClientForm.patronymicTextBox.Text = currentRow.Cells[2].Value.ToString();
            addClientForm.addressTextBox.Text = currentRow.Cells[4].Value.ToString();
            addClientForm.phoneTextBox.Text = currentRow.Cells[5].Value.ToString();
            addClientForm.emailTextBox.Text = currentRow.Cells[6].Value.ToString();

            DialogResult result = addClientForm.ShowDialog();
            if (result == DialogResult.No || result == DialogResult.Abort) return;

            TableRefresh();

            MessageBox.Show("Изменено успешно", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void шрифтТаблицыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.Cancel) return;

            if (fontDialog1.Font.Size >= 15)
            {
                fontDialog1.Font = new Font(fontDialog1.Font.FontFamily, 14);
                MessageBox.Show("Вы выбрали слишком большой размер шрифта, " +
                    "поэтому размер был автоматически установлен на 14", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            dataGridView1.Font = fontDialog1.Font;
        }

        private void цветВыделенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.Cancel) return;
            dataGridView1.DefaultCellStyle.SelectionBackColor = colorDialog1.Color;
            dataGridView1.GridColor = colorDialog1.Color;
        }

        private void heightBar_Scroll(object sender, EventArgs e)
        {
            sizeLabel.Text = heightBar.Value.ToString();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Height = heightBar.Value;
            }
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
                workSheet.Range["A1", "G1"].Font.Bold = true;
                workSheet.UsedRange.Borders.Weight = 2;
                exApp.Columns.AutoFit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void экспортВExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportToExcel();
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
