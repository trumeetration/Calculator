using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Interfaces
{
    interface IMemory
    {
        ObservableCollection<string> Memory { get; }
        int Count { get; }
        void Add(string value);
        void Remove(int index);
        void Increase(int index, string value);
        void Decrease(int index, string value);
        void Clear();
        bool Any();
    }
}
