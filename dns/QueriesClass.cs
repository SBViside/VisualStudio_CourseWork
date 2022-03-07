using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace dns
{
    /// <summary> Класс в котором хранятся статичные методы для выполнения запросов к Базе данных. 
    /// Благодаря методам из класса, мы имеем возможность выполнять запросы по изменению данных в БД,
    /// или возвращать конкретное значение из БД. </summary>
    class QueriesClass
    {
        /// <summary> Данный метод выполняет запрос к Базе данных, но не возвращает результат.
        /// Подходит для добавления элемента, удаления, перезаписи данных. </summary>
        static public void ApplyQuery_ReturnNone(OleDbConnection myConnection, string query)
        {
            try
            {
                // Выполнение запроса исходя из переменной query
                OleDbCommand command = new OleDbCommand(query, myConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Сообщение об ошибке
                MessageBox.Show(ex.Message, "Действие невозможно",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary> Данный метод выполняет запрос к Базе данных и возвращает результат
        /// в виде строки. Подходит для поиска отдельной записи в таблице. </summary>
        static public string ApplyQuery_Return(OleDbConnection myConnection, string query)
        {
            try
            {
                // Выполнение запроса исходя из переменной query.
                // Результат возвращается в виде строки.
                OleDbCommand command = new OleDbCommand(query, myConnection);
                return (command.ExecuteScalar().ToString());
            }
            catch (Exception ex)
            {
                // Сообщение об ошибке
                MessageBox.Show(ex.Message, "Действие невозможно",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return ("");
        }

        /// <summary> Данный метод используется для занесения данных 
        /// в определенный элемент ComboBox.</summary>
        static public void SetDataIntoList(OleDbConnection myConnection, ComboBox cb, string what, string from)
        {
            try
            {
                // Очистка выбранного ComboBox
                cb.Items.Clear();

                // Выполнение запроса, согласно переданным данных
                string query = $"SELECT {what} FROM {from}";
                OleDbCommand command = new OleDbCommand(query, myConnection);
                OleDbDataReader dbReader = command.ExecuteReader();

                // Внесение данных в выбранный ComboBox
                while (dbReader.Read())
                    cb.Items.Add(dbReader[what]);

                dbReader.Close();
            }
            catch (Exception ex)
            {
                // Сообщение об ошибке
                MessageBox.Show(ex.Message, "Действие невозможно",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary> Метод помогает проверить есть ли связи у строки с другой таблицей. Если связи 
        /// присутствуют, метод возвращает True, если связей нет, возвращает False. </summary>
        static public bool HasLinks(OleDbConnection myConnection, string query)
        {
            try
            {
                // Выполнение запроса исходя из переменной query.
                OleDbCommand command = new OleDbCommand(query, myConnection);
                OleDbDataReader dbReader = command.ExecuteReader();

                // Если имеется связь, закрыть DataReader и вернуть True
                if (dbReader.HasRows)
                {
                    dbReader.Close();
                    return (true);
                }
            }
            catch (Exception ex)
            {
                // Сообщение об ошибке
                MessageBox.Show(ex.Message, "Действие невозможно",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            // Вернуть False
            return (false);
        }
    }
}
