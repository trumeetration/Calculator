﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using Calculator.Interfaces;

namespace Calculator.Models.Memory
{
    public class MemoryRAM : IMemory
    {
        public MemoryRAM()
        {
            Memory = Memory ?? new ObservableCollection<string>();
        }
        public ObservableCollection<string> Memory { get; }

        public int Count
        {
            get => Memory.Count;
        }

        public void Add(string value)
        {
            Memory.Add(value);
        }

        public void Remove(int index)
        {
            Memory.RemoveAt(index);
        }

        public void Increase(int index, string value)
        {
            Memory[index] = Convert.ToString(Convert.ToDouble(value) + Convert.ToDouble(Memory[index]));
        }

        public void Decrease(int index, string value)
        {
            Memory[index] = Convert.ToString(Convert.ToDouble(value) - Convert.ToDouble(Memory[index]));
        }

        public void Clear()
        {
            Memory.Clear();
        }

        public bool Any()
        {
            return Memory.Any();
        }
    }
}
