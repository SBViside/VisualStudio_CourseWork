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
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private bool CheckLogin()
        {
            if (loginTextBox.Text != "")
            {
                LabelWarning.Visible = false;
                return (true);
            }
            else
            {
                MessageBox.Show("Не указан логин для входа.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                loginTextBox.Focus();
                LabelWarning.Visible = true;
                return (false);
            }
        }

        private void itemsButton_Click(object sender, EventArgs e)
        {
            if (!CheckLogin()) return;
            ItemsForm itemsForm = new ItemsForm(loginTextBox.Text);
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

        private void RestoreDescription()
        {
            string desc = "Наведите указателем мыши на необходимую базу данных.";
            LabelDescription.Text = desc;
        }

        private void itemsButton_MouseLeave(object sender, EventArgs e)
        {
            RestoreDescription();
        }

        private void itemsButton_MouseEnter(object sender, EventArgs e)
        {
            string desc = "База данных 'Товары' (электронная техника) содержит информацию о продаваемых товарах, их категориях, количестве и цене за единицу товара.";
            LabelDescription.Text = desc;
        }

        private void clientsButton_MouseEnter(object sender, EventArgs e)
        {
            string desc = "База данных 'Клиенты' содержит информацию о клиентах, которые хотя бы раз совершали покупку в нашем магазине. Мы записываем ФИО, возраст и телефон (для уведомления о скидках) клиента.";
            LabelDescription.Text = desc;
        }

        private void clientsButton_MouseLeave(object sender, EventArgs e)
        {
            RestoreDescription();
        }

        private void ordersButton_MouseEnter(object sender, EventArgs e)
        {
            string desc = "ORDERS";
            LabelDescription.Text = desc;
        }

        private void ordersButton_MouseLeave(object sender, EventArgs e)
        {
            RestoreDescription();
        }

        private void employeesButton_MouseEnter(object sender, EventArgs e)
        {
            string desc = "EMPLOYEES";
            LabelDescription.Text = desc;
        }

        private void employeesButton_MouseLeave(object sender, EventArgs e)
        {
            RestoreDescription();
        }
    }
}
