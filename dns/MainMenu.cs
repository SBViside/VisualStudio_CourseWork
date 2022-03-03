﻿using System;
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
            if (loginTextBox.Text.Length > 2)
            {
                LabelWarning.Visible = false;
                return (true);
            }
            LabelWarning.Visible = true;
            MessageBox.Show("Не указан логин для входа.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            loginTextBox.Focus();
            return (false);
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
            ClientsForm clientsForm = new ClientsForm(loginTextBox.Text);
            this.Hide();
            clientsForm.ShowDialog();
            this.Show();
        }

        private void ordersButton_Click(object sender, EventArgs e)
        {
            if (!CheckLogin()) return;
            //ClientsForm clientsForm = new ClientsForm(loginTextBox.Text);
            //this.Hide();
            //clientsForm.ShowDialog();
            //this.Show();
        }

        private void employeesButton_Click(object sender, EventArgs e)
        {
            if (!CheckLogin()) return;
            //ClientsForm clientsForm = new ClientsForm(loginTextBox.Text);
            //this.Hide();
            //clientsForm.ShowDialog();
            //this.Show();
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
            string desc = "База данных 'Заказы' содержит информацию о заказах наших клиентов, товаре, кторый они заказали, количество и итоговую стоимость.";
            LabelDescription.Text = desc;
        }

        private void ordersButton_MouseLeave(object sender, EventArgs e)
        {
            RestoreDescription();
        }

        private void employeesButton_MouseEnter(object sender, EventArgs e)
        {
            string desc = "База данных 'Сотрудники' содержит информацию о сотрудниках, которые работают в нашем магазинеб, а именно их ФИО, адреса, контакты";
            LabelDescription.Text = desc;
        }

        private void employeesButton_MouseLeave(object sender, EventArgs e)
        {
            RestoreDescription();
        }
    }
}
