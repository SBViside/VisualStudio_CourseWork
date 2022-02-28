﻿using System;
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
    public partial class UpdateDataForm1 : Form
    {
        private Items form;

        public UpdateDataForm1(Items f)
        {
            InitializeComponent();
            form = f;

            //for (int i = 0; i < this.typeComboBox.Items.Count; i++)
            //    if (this.typeComboBox.Items[i] == form.dataGridView1.CurrentRow.Cells[2].Value.ToString())
            //    {
            //        this.typeComboBox.SelectedIndex = i;
            //        break;
            //    }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            form.UpdateData(nameTextBox.Text, typeComboBox.SelectedIndex + 1, (int)countTextBox.Value, (int)priceTextBox.Value);
            this.Close();
        }

        private void UpdateDataForm1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'shopBDDataSet.типы' table. You can move, or remove it, as needed.
            this.типыTableAdapter.Fill(this.shopBDDataSet.типы);

        }
    }
}