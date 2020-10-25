using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Calculator
{
    class ViewModel : INotifyPropertyChanged
    {
        private static string lastValue = null;
        private static string op = null;
        private static bool hasNumberComma = false;
        private static char[] delimiterChars = { '+', '-', '*', '/' };
        private ObservableCollection<Expression> _observableprsn = new ObservableCollection<Expression>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string textValue = "";

        public string TextValue
        {
            get => textValue;
            set
            {
                textValue = value;
                OnPropertyChanged(nameof(TextValue));
            }
        }

        private ICommand _addDigit;
        public ICommand AddDigit
        {
            get => _addDigit ?? new RelayCommand<string>(x =>
            {
                if (x == "Back")
                {
                    if (TextValue.Length == 0)
                        return;
                    TextValue = TextValue.Remove(TextValue.Length - 1);
                    string[] values = TextValue.Split(delimiterChars);
                    if (values.Length > 0)
                        hasNumberComma = values[values.Length - 1].Contains(',') ? true : false;
                    return;
                }
                if (TextValue.Length > 1 && "*/-+".IndexOf(TextValue[TextValue.Length - 1]) != -1 && "*/-+".IndexOf(x) != -1)
                {
                    TextValue = TextValue.Remove(TextValue.Length - 1) + x;
                    hasNumberComma = false;
                    return;
                }
                TextValue += x;
                if ("-+*/".IndexOf(x) != -1)
                    hasNumberComma = false;
            }, x => true);
        }

        private ICommand _renderExpression;
        public ICommand RenderExpression
        {
            get => _renderExpression ?? new RelayCommand<string>(x =>
            {
                var result = Convert.ToString(Calc.Parse(TextValue));
                Expression tmp = new Expression(TextValue, result);
                Expressions.Add(tmp); //todo : FIX
                TextValue = result;
            }, x => true);
        }
        private ICommand _comma;
        public ICommand Comma
        {
            get => _comma ?? new RelayCommand<string>(x =>
            {
                TextValue += x;
                hasNumberComma = true;
            }, x => hasNumberComma == false);
        }

        private ICommand _clear;
        public ICommand Clear
        {
            get => _clear ?? new RelayCommand(() =>
            {
                TextValue = "";
                lastValue = null;
                op = null;
            }, () => true);
        }

        private ObservableCollection<Expression> _expr { get; set; }
        public ObservableCollection<Expression> Expressions
        {
            get => _expr;
            set
            {
                if (_expr != value)
                {
                    _expr = value;
                    OnPropertyChanged(nameof(Expressions));
                }
            }
        }

    }

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

    public class Expression
    {
        private string expression;
        private string value;
        public Expression(string exp, string answ)
        {
            expression = exp;
            value = answ;
        }


    }
}
