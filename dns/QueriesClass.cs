using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace dns
{
    /// <summary> Класс в котором хранятся статичные методы для выполнения запросов к Базе Данных. 
    /// Благодаря методам из класса, мы имеем возможность выполнять запросы по изменению данных в БД,
    /// или возвращать конкретное значение из БД. </summary>
    class QueriesClass
    {
        /// <summary> Данный метод выполняет запрос к Базе Даннах, но не возвращает результат.
        /// Подходит для добавления элемента, удаления, обновления данных. </summary>
        static public void ApplyQuery_ReturnNone(OleDbConnection myConnection, string query)
        {
            try
            {
                OleDbCommand command = new OleDbCommand(query, myConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Действие невозможно", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary> Данный метод выполняет запрос к Базе Даннах и возвращает результат
        /// в виде строки. Подходит для поиска отдельного элемента в БД. </summary>
        static public string ApplyQuery_Return(OleDbConnection myConnection, string query)
        {
            try
            {
                OleDbCommand command = new OleDbCommand(query, myConnection);
                return (command.ExecuteScalar().ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Действие невозможно", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return ("");
        }

        /// <summary> Данный метод используется для занесения данных об имеющихся категориях товаров
        /// в определенный элемент ComboBox (Подходит для формы ItemsForm). </summary>
        static public void SetDataIntoList(OleDbConnection myConnection, ComboBox cb)
        {
            try
            {
                cb.Items.Clear();

                string query = $"SELECT название_типа FROM типы";
                OleDbCommand command = new OleDbCommand(query, myConnection);
                OleDbDataReader dbReader = command.ExecuteReader();

                while (dbReader.Read())
                    cb.Items.Add(dbReader["название_типа"]);

                dbReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Действие невозможно", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        static public bool HasLinks(OleDbConnection myConnection, string query)
        {
            OleDbCommand command = new OleDbCommand(query, myConnection);
            OleDbDataReader dbReader = command.ExecuteReader();

            if (dbReader.HasRows) return (true);
            return (false);

        }
    }
}
