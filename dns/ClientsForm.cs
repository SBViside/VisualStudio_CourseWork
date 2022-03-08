using System;
using System.Windows.Forms;
using System.Data.OleDb;

namespace dns
{
    public partial class ClientsForm : Form
    {
        public string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=shopBD.accdb";
        public OleDbConnection myConnection;

        public ClientsForm(string log)
        {
            InitializeComponent();
            currUserLabel.Text = "Вход выполнен: " + log;

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

                foreach (DataGridViewRow row in dataGridView1.Rows)
                    row.Height = 30;
                
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void посикToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchForm sf = new SearchForm(this.dataGridView1);

            for (int i = 0; i < dataGridView1.ColumnCount; i++)
                sf.typeComboBox.Items.Add(dataGridView1.Columns[i].HeaderText);

            sf.typeComboBox.SelectedIndex = 0;
            sf.ShowDialog();
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();

            AddClientForm addClientForm = new AddClientForm(myConnection, "adding");
            addClientForm.ShowDialog();

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
                $"and имя='{dataGridView1.CurrentRow.Cells[1].Value}' and отчество='{dataGridView1.CurrentRow.Cells[2].Value}' " +
                $"and адрес='{dataGridView1.CurrentRow.Cells[4].Value}'";

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

            addClientForm.surnameTextBox.Enabled = false;
            addClientForm.nameTextBox.Enabled = false;
            addClientForm.patronymicTextBox.Enabled = false;
            addClientForm.dateTimePicker.Enabled = false;

            addClientForm.surnameTextBox.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            addClientForm.nameTextBox.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            addClientForm.patronymicTextBox.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            addClientForm.adressTextBox.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            addClientForm.phoneTextBox.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            addClientForm.emailTextBox.Text = dataGridView1.CurrentRow.Cells[6].Value.ToString();

            addClientForm.ShowDialog();

            TableRefresh();
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
