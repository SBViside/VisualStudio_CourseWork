using System;
using System.Windows.Forms;

namespace dns
{
    public partial class SearchForm : Form
    {
        private DataGridView dgv;

        public SearchForm(DataGridView d)
        {
            InitializeComponent();
            dgv = d;

            for (int i = 0; i < dgv.ColumnCount; i++)
                typeComboBox.Items.Add(dgv.Columns[i].HeaderText);
            typeComboBox.SelectedIndex = 0;
        }

        private void executeButton_Click(object sender, EventArgs e)
        {
            if (checkRegister.Checked && isNumber.Checked) Search_Reg_Sub(dgv, wordTextBox.Text, typeComboBox.SelectedIndex);
            else if (!checkRegister.Checked && !isNumber.Checked) Search_noReg_noSub(dgv, wordTextBox.Text, typeComboBox.SelectedIndex);
                else if (!checkRegister.Checked && isNumber.Checked) Search_noReg_Sub(dgv, wordTextBox.Text, typeComboBox.SelectedIndex);
                    else if (checkRegister.Checked && !isNumber.Checked) Search_Reg_noSub(dgv, wordTextBox.Text, typeComboBox.SelectedIndex);

            this.Close();
        }

        // Поиск с учетом регистра, но в подстроке
        private void Search_Reg_Sub(DataGridView dataGridView1, string keyWord, int col)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                string currWord = dataGridView1.Rows[i].Cells[col].Value.ToString();

                if (currWord.Contains(keyWord))
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[i].Selected = true;
                    return;
                }
            }
            MessageBox.Show("Совпадений не найдено", "Результат поиска",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Поиск без учета регистра, но в подстроке
        private void Search_noReg_Sub(DataGridView dataGridView1, string keyWord, int col)
        {
            keyWord = keyWord.ToLower();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                string currWord = dataGridView1.Rows[i].Cells[col].Value.ToString().ToLower();

                if (currWord.Contains(keyWord))
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[i].Selected = true;
                    return;
                }
            }
            MessageBox.Show("Совпадений не найдено", "Результат поиска",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Поиск без учета регистра, но не в подстроке
        private void Search_noReg_noSub(DataGridView dataGridView1, string keyWord, int col)
        {
            keyWord = keyWord.ToLower();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                string currWord = dataGridView1.Rows[i].Cells[col].Value.ToString().ToLower();

                if (currWord == keyWord)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[i].Selected = true;
                    return;
                }
            }
            MessageBox.Show("Совпадений не найдено", "Результат поиска",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Поиск с учетом регистра, но не в подстроке
        private void Search_Reg_noSub(DataGridView dataGridView1, string keyWord, int col)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                string currWord = dataGridView1.Rows[i].Cells[col].Value.ToString();

                if (currWord == keyWord)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[i].Selected = true;
                    return;
                }
            }
            MessageBox.Show("Совпадений не найдено", "Результат поиска",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
