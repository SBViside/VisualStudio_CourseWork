using System;
using System.Windows.Forms;
using System.Data.OleDb;

namespace dns
{
    public partial class EmployeesForm : Form
    {
        public string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=shopBD.accdb";
        public OleDbConnection myConnection;

        public EmployeesForm(string log)
        {
            InitializeComponent();
            currUserLabel.Text = "Вход выполнен: " + log;

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
                string query = "SELECT фамилия, имя, отчество, дата_рождения, образование, должность, " +
                    "адрес, телефон, паспорт FROM сотрудники";
                OleDbCommand command = new OleDbCommand(query, myConnection); // Создаю запрос
                OleDbDataReader dbReader = command.ExecuteReader();   // Считываю данные

                // Загрузка данных в таблицу
                while (dbReader.Read())
                    dataGridView1.Rows.Add(dbReader["фамилия"], dbReader["имя"], dbReader["отчество"],
                        dbReader["дата_рождения"].ToString().Split()[0], dbReader["образование"], 
                        dbReader["должность"], dbReader["адрес"], dbReader["телефон"], dbReader["паспорт"]);

                dbReader.Close();

                foreach (DataGridViewRow row in dataGridView1.Rows) 
                    row.Height = 30;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            if (MessageBox.Show("Вы действительно хотите удалить сотрудника?", "Подтверждение действия",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            string query = $"DELETE FROM сотрудники WHERE фамилия='{dataGridView1.CurrentRow.Cells[0].Value}' " +
                $"and имя='{dataGridView1.CurrentRow.Cells[1].Value}' and отчество='{dataGridView1.CurrentRow.Cells[2].Value}' " +
                $"and должность='{dataGridView1.CurrentRow.Cells[5].Value}'";

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

        private void посикToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchForm sf = new SearchForm(this.dataGridView1);
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
                sf.typeComboBox.Items.Add(dataGridView1.Columns[i].HeaderText);
            sf.typeComboBox.SelectedIndex = 0;
            sf.ShowDialog();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            AddEmployeeForm addEmployeeForm = new AddEmployeeForm(this, "adding");
            addEmployeeForm.ShowDialog();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            AddEmployeeForm addEmployeeForm = new AddEmployeeForm(this, "updating");

            addEmployeeForm.surnameTextBox.Enabled = false;
            addEmployeeForm.nameTextBox.Enabled = false;
            addEmployeeForm.patronymicTextBox.Enabled = false;
            addEmployeeForm.dateTimePicker.Enabled = false;

            addEmployeeForm.surnameTextBox.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            addEmployeeForm.nameTextBox.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            addEmployeeForm.patronymicTextBox.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            addEmployeeForm.educationTextBox.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            addEmployeeForm.positionTextBox.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            addEmployeeForm.adressTextBox.Text = dataGridView1.CurrentRow.Cells[6].Value.ToString();
            addEmployeeForm.phoneTextBox.Text = dataGridView1.CurrentRow.Cells[7].Value.ToString();
            addEmployeeForm.passportTextBox.Text = dataGridView1.CurrentRow.Cells[8].Value.ToString();

            addEmployeeForm.ShowDialog();
        }
    }
}
