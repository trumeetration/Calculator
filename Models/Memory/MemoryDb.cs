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

        public MemoryDb(string dbname = "calculator.db")
        {
            Memory = Memory ?? new ObservableCollection<string>();
            if (File.Exists(dbname) == false)
            {
                SQLiteConnection.CreateFile(dbname);
                using (SQLiteConnection connection = new SQLiteConnection())
                {
                    connection.ConnectionString = "Data Source = " + dbname;
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
                                Exp VARCHAR NOT NULL,
                                Value VARCHAR NOT NULL)";
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                using (SQLiteConnection connection = new SQLiteConnection())
                {
                    connection.ConnectionString = "Data Source = " + dbname;
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
        public int Count { get; }
        public void Add(string value)
        {
            throw new NotImplementedException();
        }

        public void Remove(int index)
        {
            throw new NotImplementedException();
        }

        public void Increase(int index, string value)
        {
            throw new NotImplementedException();
        }

        public void Decrease(int index, string value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Any()
        {
            return Memory.Any();
        }
    }
}
