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
        }

        private void executeButton_Click(object sender, EventArgs e)
        {
            if (isNumber.Checked)
                Search(dgv, wordTextBox.Text, typeComboBox.SelectedIndex);
            else
                Search(dgv, wordTextBox.Text, typeComboBox.SelectedIndex, checkRegister.Checked);


            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Search(DataGridView dataGridView1, string keyWord, int col, bool reg)
        {
            string currWord;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                currWord = dataGridView1.Rows[i].Cells[col].Value.ToString();
                if (reg)
                {
                    if (currWord.Contains(keyWord))
                    {
                        dataGridView1.ClearSelection();
                        dataGridView1.Rows[i].Selected = true;
                        return;
                    }
                }
                else
                {
                    if (currWord.ToLower().Contains(keyWord.ToLower()))
                    {
                        dataGridView1.ClearSelection();
                        dataGridView1.Rows[i].Selected = true;
                        return;
                    }
                }
            }
            MessageBox.Show("Совпадений не найдено", "Результат поиска",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Search(DataGridView dataGridView1, string keyNum, int col)
        {
            string currNum;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                currNum = dataGridView1.Rows[i].Cells[col].Value.ToString();

                if (currNum == keyNum)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[i].Selected = true;
                    return;
                }
            }
            MessageBox.Show("Совпадений не найдено", "Результат поиска",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
