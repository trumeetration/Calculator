using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculator.Models;

namespace Calculator.Interfaces
{
    interface IHistory
    {
        ObservableCollection<Expression> Expressions { get; }
        void Add(Expression expression);
        void Clear();
    }
}
