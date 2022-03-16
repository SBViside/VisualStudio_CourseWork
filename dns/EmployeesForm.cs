using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.OleDb;
using Excel = Microsoft.Office.Interop.Excel;

namespace dns
{
    public partial class EmployeesForm : Form
    {
        const string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=shopBD.mdb";
        public OleDbConnection myConnection;

        public EmployeesForm(string log)
        {
            InitializeComponent();

            this.Text = "Сотрудники | Вход выполнен: " + log;

            // Подлючение к БД
            myConnection = new OleDbConnection(connectionString);
            myConnection.Open();

            // Заполнение таблицы
            TableRefresh();
        }

        public void TableRefresh()
        {
            try
            {
                // Очистка таблицы на форме
                dataGridView1.Rows.Clear();

                // Строка запроса к БД
                string query = "SELECT фамилия, имя, отчество, должность FROM сотрудники";
                OleDbCommand command = new OleDbCommand(query, myConnection); // Создаю запрос
                OleDbDataReader dbReader = command.ExecuteReader();   // Считываю данные

                // Загрузка данных в таблицу
                while (dbReader.Read())
                    dataGridView1.Rows.Add(dbReader["фамилия"], dbReader["имя"],
                        dbReader["отчество"], dbReader["должность"]);

                dbReader.Close();

                dataGridView1.ClearSelection();
                ClearLabels();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                    row.Height = heightBar.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearLabels()
        {
            surnameLabel.Text = "(нет данных)";
            nameLabel.Text = "(нет данных)";
            patronymicLabel.Text = "(нет данных)";

            birthLabel.Text = "(нет данных)";
            positionLabel.Text = "(нет данных)";

            educationLabel.Text = "(нет дынных)";
            addressLabel.Text = "(нет дынных)";
            phoneLabel.Text = "(нет данных)";
            passportLabel.Text = "(нет данных)";
        }

        private void EmployeesForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            myConnection.Close();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            TableRefresh();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Сотрудник не выбран", "Действие невозможно",
                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Вы действительно хотите удалить сотрудника?", "Подтверждение действия",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            DataGridViewRow currentRow = dataGridView1.CurrentRow;

            string query = $"DELETE FROM сотрудники WHERE фамилия='{currentRow.Cells[0].Value}' " +
                $"and имя='{currentRow.Cells[1].Value}' and отчество='{currentRow.Cells[2].Value}' " +
                $"and должность='{currentRow.Cells[3].Value}'";

            QueriesClass.ApplyQuery_ReturnNone(myConnection, query);
            TableRefresh();
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

        private void посикToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();

            SearchForm sf = new SearchForm(dataGridView1);
            sf.ShowDialog();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            ClearLabels();

            AddEmployeeForm addEmployeeForm = new AddEmployeeForm(this, "adding");
            addEmployeeForm.ShowDialog();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Сотрудник не выбран", "Действие невозможно",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            AddEmployeeForm addEmployeeForm = new AddEmployeeForm(this, "updating");

            addEmployeeForm.surnameTextBox.Enabled = false;
            addEmployeeForm.nameTextBox.Enabled = false;
            addEmployeeForm.patronymicTextBox.Enabled = false;

            DataGridViewRow currentRow = dataGridView1.CurrentRow;

            // Получаю дату рождения из базы данных и заношу в DateTimePicker
            string query = $"SELECT дата_рождения FROM сотрудники WHERE фамилия='{currentRow.Cells[0].Value}' " +
                $"AND имя='{currentRow.Cells[1].Value}' AND отчество='{currentRow.Cells[2].Value}'";
            string[] date = QueriesClass.ApplyQuery_Return(myConnection, query).Split()[0].Split('.');

            addEmployeeForm.dateTimePicker.Value = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]));

            // Заношу остальные данные в поля ввода
            addEmployeeForm.surnameTextBox.Text = currentRow.Cells[0].Value.ToString();
            addEmployeeForm.nameTextBox.Text = currentRow.Cells[1].Value.ToString();
            addEmployeeForm.patronymicTextBox.Text = currentRow.Cells[2].Value.ToString();

            addEmployeeForm.educationTextBox.Text = educationLabel.Text;
            addEmployeeForm.positionTextBox.Text = currentRow.Cells[3].Value.ToString();
            addEmployeeForm.adressTextBox.Text = addressLabel.Text;
            addEmployeeForm.phoneTextBox.Text = phoneLabel.Text;
            addEmployeeForm.passportTextBox.Text = passportLabel.Text;

            addEmployeeForm.ShowDialog();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                ClearLabels();
                return;
            }
            GetInfo(dataGridView1.CurrentRow);
        }

        private void GetInfo(DataGridViewRow row)
        {
            // Заношу данные из таблицы в метки
            surnameLabel.Text = row.Cells[0].Value.ToString();
            nameLabel.Text = row.Cells[1].Value.ToString();
            patronymicLabel.Text = row.Cells[2].Value.ToString();
            positionLabel.Text = row.Cells[3].Value.ToString();

            // Генерирую запрос на получения данных
            string query = $"SELECT дата_рождения, образование, дата_приема, адрес, телефон, " +
                $"паспорт FROM сотрудники WHERE фамилия='{row.Cells[0].Value}' " +
                $"AND имя='{row.Cells[1].Value}' AND отчество='{row.Cells[2].Value}'";

            OleDbCommand command = new OleDbCommand(query, myConnection);
            OleDbDataReader dbReader = command.ExecuteReader();

            // Заношу данные из Базы данных в метки
            dbReader.Read();
            positionLabel.Text += $" от {dbReader["дата_приема"].ToString().Split()[0]}";
            birthLabel.Text = dbReader["дата_рождения"].ToString().Split()[0];
            educationLabel.Text = dbReader["образование"].ToString();
            addressLabel.Text = dbReader["адрес"].ToString();
            phoneLabel.Text = dbReader["телефон"].ToString();
            passportLabel.Text = dbReader["паспорт"].ToString();
            dbReader.Close();
        }

        private void heightBar_Scroll(object sender, EventArgs e)
        {
            sizeLabel.Text = heightBar.Value.ToString();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Height = heightBar.Value;
            }
        }

        private void экспортВExcelToolStripMenuItem_Click(object sender, EventArgs e)
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
