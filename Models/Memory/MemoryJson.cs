using System;
using System.Collections.ObjectModel;
using System.IO;
using Calculator.Interfaces;
using Newtonsoft.Json;

namespace Calculator.Models.Memory
{
    public class MemoryJson : IMemory
    {
        private string path;
        public ObservableCollection<string> Memory { get; }
        public MemoryJson(string jsonPath = "memory.json")
        {
            path = jsonPath;
            if (File.Exists(path) == false)
            {
                FileStream stream =  File.Create(path);
                stream.Close();
                Memory = new ObservableCollection<string>();
                return;
            }
            var lastTimeMemory = File.ReadAllText(path);
            Memory = JsonConvert.DeserializeObject<ObservableCollection<string>>(lastTimeMemory);
            Memory = Memory ?? new ObservableCollection<string>();
        }

        public int Count
        {
            get => Memory.Count;
        }

        public void Add(string value)
        {
            Memory.Add(value);
            SaveToJson();
        }

        public void Remove(int index)
        {
            Memory.RemoveAt(index);
            SaveToJson();
        }

        public void SaveToJson()
        {
            if (File.Exists(path) == false)
            {
                File.Create(path);
            }
            var data = JsonConvert.SerializeObject(Memory);
            File.WriteAllText(path, data);
        }

        public void Increase(int index, string value)
        {
            Memory[index] = Convert.ToString(Convert.ToDouble(value) + Convert.ToDouble(Memory[index]));
            SaveToJson();
        }

        public void Decrease(int index, string value)
        {
            Memory[index] = Convert.ToString(Convert.ToDouble(Memory[index]) - Convert.ToDouble(value));
            SaveToJson();
        }

        public void Clear()
        {
            Memory.Clear();
            SaveToJson();
        }

        public bool Any()
        {
            return Memory.Count > 0;
        }
    }
}