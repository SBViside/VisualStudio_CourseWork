using System;
using System.Windows.Forms;

namespace dns
{
    public partial class SearchForm : Form
    {
        private ItemsForm itemsForm;
        private ClientsForm clientsForm;
        //private ItemsForm form;
        //private ItemsForm form;

        private byte formType;

        public SearchForm(ItemsForm f)
        {
            InitializeComponent();
            itemsForm = f;
            formType = 1;
        }

        public SearchForm(ClientsForm f)
        {
            InitializeComponent();
            clientsForm = f;
            formType = 2;
        }

        private void executeButton_Click(object sender, EventArgs e)
        {
            switch (formType)
            {
                case 1:
                    Search(itemsForm.dataGridView1, wordTextBox.Text, typeComboBox.SelectedIndex, checkRegister.Checked);
                    break;
                case 2:
                    Search(clientsForm.dataGridView1, wordTextBox.Text, typeComboBox.SelectedIndex, checkRegister.Checked);
                    break;
            }
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void Search(DataGridView dataGridView1, string keyWord, int col, bool reg)
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
            MessageBox.Show("Совпадений не найдено!", "Результат поиска", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
