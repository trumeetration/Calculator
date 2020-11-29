using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Calculator
{
    class ViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public ViewModel()
        {
            _expr = new ObservableCollection<Expression>();
            _mem = new ObservableCollection<string>();
        }
        private static string lastValue = null;
        private static string op = null;
        private static bool hasNumberComma = false;
        private static char[] delimiterChars = { '+', '-', '*', '/' };

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

        private ICommand _toMemory;
        public ICommand AddToMemory
        {
            get => _toMemory ?? new RelayCommand(
                () =>
                {
                    Calc.Parse(TextValue);
                    Memory.Add(TextValue);
                }, () => TextValue.Length > 0);
        }

        private ICommand _removeFromMemory;

        public ICommand RemoveFromMemory
        {
            get => _removeFromMemory ?? new RelayCommand(
                () =>
                {
                    Memory.RemoveAt(Memory.Count() - 1);
                }, () => Memory.Any());
        }

        private ICommand _clearMem;

        public ICommand ClearMemory
        {
            get => _clearMem ?? new RelayCommand(() =>
            {
                Memory.Clear();
            }, () => Memory.Any());
        }

        private ICommand _sumMemCommand;

        public ICommand SumWithLastMem
        {
            get => _sumMemCommand ?? new RelayCommand(() =>
            {
                var value = Calc.Parse(TextValue);
                TextValue = Convert.ToString(value);
                Memory[Memory.Count - 1] =
                    Convert.ToString(Convert.ToDouble(Memory[Memory.Count - 1]) + value);
            }, () => string.IsNullOrEmpty(TextValue) == false && Memory.Any());
        }

        private ICommand _subFromCommand;
        public ICommand SubFromLastMem
        {
            get => _subFromCommand ?? new RelayCommand(() =>
            {
                var value = Calc.Parse(TextValue);
                TextValue = Convert.ToString(value);
                Memory[Memory.Count - 1] =
                    Convert.ToString(Convert.ToDouble(Memory[Memory.Count - 1]) - value);
            }, () => string.IsNullOrEmpty(TextValue) == false && Memory.Any());
        }

        private ICommand _takeExp;
        public ICommand TakeExpression
        {
            get => _takeExp ?? new RelayCommand(() =>
            {
                TextValue = Memory[Memory.Count() - 1];
            }, () => true);
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
                Expressions.Add(tmp); 
                TextValue = result;
            }, x => string.IsNullOrWhiteSpace(TextValue) == false);
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

        private ICommand _memoryUnit;

        public ICommand MemoryUnit
        {
            get => _memoryUnit ?? new RelayCommand<DockPanel>((x) =>
            {
                if (x.Visibility == Visibility.Collapsed)
                {
                    x.Visibility = Visibility.Visible;
                    if (Application.Current.MainWindow != null) Application.Current.MainWindow.Width += x.Width;
                }
                else
                {
                    x.Visibility = Visibility.Collapsed;
                    if (Application.Current.MainWindow != null) Application.Current.MainWindow.Width -= x.Width;
                }
            }, (x) => true);
        }

        private ObservableCollection<Expression> _expr;
        public ObservableCollection<Expression> Expressions
        {
            get => _expr;
        }

        private ObservableCollection<string> _mem;

        public ObservableCollection<string> Memory
        {
            get => _mem;
        }

        public Dictionary<string, string> ErrorCollection { get; private set; } = new Dictionary<string, string>();

        public string Error => null;

        public string this[string name]
        {
            get
            {
                string errorMessage = null;
                switch (name)
                {
                    case "TextValue":
                        if (string.IsNullOrWhiteSpace(TextValue))
                            errorMessage = "Empty field";
                        break;
                }

                if (ErrorCollection.ContainsKey(name))
                    ErrorCollection[name] = errorMessage;
                else if (errorMessage != null)
                    ErrorCollection.Add(name, errorMessage);
                OnPropertyChanged(nameof(ErrorCollection));
                return errorMessage;
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
        public Expression(string exp, string answ)
        {
            Exp = exp;
            Value = answ;
        }

        public string Value { get; }

        public string Exp { get; }
    }
}
