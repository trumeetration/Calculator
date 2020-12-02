using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Models
{
    public class Expression
    {
        public Expression() { }
        public Expression(string exp, string answ)
        {
            Exp = exp;
            Value = answ;
        }

        public string Value { get; set; }

        public string Exp { get; set; }
    }
}
