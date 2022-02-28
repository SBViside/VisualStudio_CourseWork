using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dns
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private bool CheckLogin()
        {
            if (loginTextBox.Text != "")
            {
                return (true);
            }
            else
            {
                MessageBox.Show("Не указан логин для входа.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false);
            }
        }

        private void itemsButton_Click(object sender, EventArgs e)
        {
            if (!CheckLogin()) return;
            Items itemsForm = new Items(loginTextBox.Text);
            this.Hide();
            itemsForm.ShowDialog();
            this.Show();
        }

        private void clientsButton_Click(object sender, EventArgs e)
        {
            if (!CheckLogin()) return;
        }

        private void ordersButton_Click(object sender, EventArgs e)
        {
            if (!CheckLogin()) return;
        }

        private void employeesButton_Click(object sender, EventArgs e)
        {
            if (!CheckLogin()) return;
        }
    }
}
