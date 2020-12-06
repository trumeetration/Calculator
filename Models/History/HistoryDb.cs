using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculator.Interfaces;

namespace Calculator.Models.History
{
    class HistoryDb : IHistory
    {
        public ObservableCollection<Expression> Expressions { get; }
        private string _dbname;

        public HistoryDb(string dbname = "calculator.db")
        {
            _dbname = dbname;
            Expressions = Expressions ?? new ObservableCollection<Expression>();
            if (File.Exists(dbname) == false)
            {
                SQLiteConnection.CreateFile(dbname);
                using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + dbname))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText =
                            @"CREATE TABLE expressions (
                                value VARCHAR NOT NULL)";
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                        command.CommandText =
                            @"CREATE TABLE expressions (
                                expression VARCHAR NOT NULL,
                                value VARCHAR NOT NULL)";
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + dbname))
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("SELECT * FROM expressions", connection);
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Expressions.Add(new Expression(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString()));
                    }
                }
            }
        }
        public void Add(Expression expression)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + _dbname))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = @"INSERT INTO expressions
                                            VALUES (@expression, @value)";
                command.Parameters.AddWithValue("@expression", expression.Exp);
                command.Parameters.AddWithValue("@value", expression.Value);
                command.ExecuteNonQuery();
            }
            Expressions.Add(expression);
        }

        public void Clear()
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + _dbname))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = @"DELETE FROM expressions
                                            WHERE (SELECT * FROM saved_values)";
                command.ExecuteNonQuery();
            }
            Expressions.Clear();
        }
    }
}
