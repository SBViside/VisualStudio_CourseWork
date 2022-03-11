using System;
using System.Drawing;
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
            if (loginTextBox.Text.Length > 0)
            {
                LabelWarning.Visible = false;
                return (true);
            }
            LabelWarning.Visible = true;
            MessageBox.Show("Укажите логин для входа", "Вход не выполнен",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            OrdersForm ordersForm = new OrdersForm(loginTextBox.Text);
            this.Hide();
            ordersForm.ShowDialog();
            this.Show();
        }

        private void employeesButton_Click(object sender, EventArgs e)
        {
            if (!CheckLogin()) return;
            EmployeesForm employeesForm = new EmployeesForm(loginTextBox.Text);
            this.Hide();
            employeesForm.ShowDialog();
            this.Show();
        }

        private void RestoreDescription()
        {
            string desc = "Наведите указатель мыши на нужную вам кнопку.";
            LabelDescription.Text = desc;
        }

        private void itemsButton_MouseLeave(object sender, EventArgs e)
        {
            RestoreDescription();
        }

        private void itemsButton_MouseEnter(object sender, EventArgs e)
        {
            string desc = "База данных 'Товары' (электронная техника) содержит информацию о " +
                "продаваемых товарах, о категориях продаваемых товаров, количестве и цене за единицу товара.";
            LabelDescription.Text = desc;
        }

        private void clientsButton_MouseEnter(object sender, EventArgs e)
        {
            string desc = "База данных 'Клиенты' содержит информацию о клиентах, которые " +
                "хотя бы раз совершали покупку в нашем магазине. Мы записываем ФИО, дату рождения, " +
                "телефон (или E-mail, для уведомления о скидках) и адрес клиента. Адрес нужен для курьера, " +
                "который осуществляет доставку.";
            LabelDescription.Text = desc;
        }

        private void clientsButton_MouseLeave(object sender, EventArgs e)
        {
            RestoreDescription();
        }

        private void ordersButton_MouseEnter(object sender, EventArgs e)
        {
            string desc = "База данных 'Заказы' содержит информацию о заказах наших клиентов. Вы можете " +
                "просматривать активные заказы, а также заказы, которые уже выполнены.";
            LabelDescription.Text = desc;
        }

        private void ordersButton_MouseLeave(object sender, EventArgs e)
        {
            RestoreDescription();
        }

        private void employeesButton_MouseEnter(object sender, EventArgs e)
        {
            string desc = "База данных 'Сотрудники' содержит информацию о сотрудниках, " +
                "которые работают в нашем магазине, их ФИО, адреса, контакты, должности и остальную важную для " +
                "нас информация.";
            LabelDescription.Text = desc;
        }

        private void employeesButton_MouseLeave(object sender, EventArgs e)
        {
            RestoreDescription();
        }
    }
}
