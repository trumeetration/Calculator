using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Interfaces
{
    public interface ICalculator
    {
        double Parse(string expression);
        bool IsValid(string expression);
    }
}
