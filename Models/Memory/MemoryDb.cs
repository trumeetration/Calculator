using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculator.Interfaces;
using System.Data.SQLite;

namespace Calculator.Models.Memory
{
    class MemoryDb : IMemory
    {
        public ObservableCollection<string> Memory { get; }
        private string _dbname;

        public MemoryDb(string dbname = "calculator.db")
        {
            _dbname = dbname;
            Memory = Memory ?? new ObservableCollection<string>();
            if (File.Exists(dbname) == false)
            {
                SQLiteConnection.CreateFile(dbname);
                using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + dbname))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText =
                            @"CREATE TABLE saved_values (
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
                    SQLiteCommand command = new SQLiteCommand("SELECT * FROM saved_values", connection);
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Memory.Add(dt.Rows[i][0].ToString());
                    }
                }
            }

        }
        public int Count
        {
            get => Memory.Count;
        }
        public void Add(string value)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + _dbname))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = @"INSERT INTO saved_values
                                            VALUES (@tosave)";
                command.Parameters.AddWithValue("@tosave", value);
                command.ExecuteNonQuery();
            }
            Memory.Add(value);
        }

        public void Remove(int index)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + _dbname))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = @"DELETE FROM saved_values
                                            WHERE rowid=@rowid";
                command.Parameters.AddWithValue("@rowid", index + 1);
                command.ExecuteNonQuery();
            }
            Memory.RemoveAt(index);
        }

        public void Increase(int index, string value)
        {
            double newValue = Convert.ToDouble(Memory[index]) + Convert.ToDouble(value);
            using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + _dbname))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = @"UPDATE saved_values 
                                            SET value = @value 
                                            WHERE rowid = @rowid";
                command.Parameters.AddWithValue("@rowid", index + 1);
                command.Parameters.AddWithValue("@value", newValue);
                command.ExecuteNonQuery();
            }
            Memory[index] = newValue.ToString();
        }

        public void Decrease(int index, string value)
        {
            double newValue = Convert.ToDouble(Memory[index]) - Convert.ToDouble(value);
            using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + _dbname))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = @"UPDATE saved_values 
                                            SET value = @value 
                                            WHERE rowid = @rowid";
                command.Parameters.AddWithValue("@rowid", index + 1);
                command.Parameters.AddWithValue("@value", newValue);
                command.ExecuteNonQuery();
            }
            Memory[index] = newValue.ToString();
        }

        public void Clear()
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + _dbname))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = @"DELETE FROM saved_values
                                            WHERE (SELECT * FROM saved_values)";
                command.ExecuteNonQuery();
            }
            Memory.Clear();
        }

        public bool Any()
        {
            return Memory.Any();
        }
    }
}
