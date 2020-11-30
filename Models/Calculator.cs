using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Models
{
    public static class Calc
    {
        private static List<double> valueList = new List<double>();
        private static List<char> operators = new List<char>();
        public static double Parse(string expression)
        {
            valueList.Clear();
            operators.Clear();
            expression = expression.Replace(" ", ""); // Удаляет лишние пробелы
            if (expression.Length == 0)
                return 0;
            if ("+-*/".IndexOf(expression[0]) != -1)
                expression = expression.Insert(0, "0");
            if ("+-*/".IndexOf(expression[expression.Length - 1]) != -1)
                expression = expression.Remove(expression.Length - 1);
            string value = "";
            double total = 0;
            char tmpOp = 'n';
            while (true) // сбор данных 
            {
                if (expression.Length == 0)
                    break;
                while (expression.Length > 0 && "1234567890".Contains(expression[0]))
                {
                    value += expression[0];
                    expression = expression.Remove(0, 1);
                }
                if (value.Length > 0)
                {
                    valueList.Add(Convert.ToDouble(value));
                    value = "";
                }
                else break;
                while (expression.Length > 0 && "+-/*".Contains(expression[0]))
                {
                    //operators.Add(expression[0]);
                    tmpOp = expression[0];
                    expression = expression.Remove(0, 1);
                }
                if (tmpOp == 'n')
                    break;
                else
                {
                    operators.Add(tmpOp);
                    tmpOp = 'n';
                }

            }

            //Высчитывание умножений и делений
            while (operators.Contains('*') || operators.Contains('/'))
            {
                int index = operators.FindIndex(x => x == '*' || x == '/');
                char op = operators[index];
                double left = valueList[index];
                double right = valueList[index + 1];
                operators.RemoveAt(index);
                valueList.RemoveRange(index, 2);
                double result = op == '*' ? left * right : left / right;
                valueList.Insert(index, result);
            }

            while (valueList.Count > 0)
            {
                if (valueList.Count == 1) return valueList[0];
                double left = valueList[0];
                double right = valueList[1];
                valueList.RemoveRange(0, 2);
                char op = operators[0];
                operators.RemoveAt(0);
                double result = 0;
                switch (op)
                {
                    case '/':
                        result = left / right;
                        break;
                    case '*':
                        result = left * right;
                        break;
                    case '-':
                        result = left - right;
                        break;
                    case '+':
                        result = left + right;
                        break;
                    default:
                        // Исключение
                        break;
                }
                total = result;
                valueList.Insert(0, result);
            }
            return total;
        }
    }
}
