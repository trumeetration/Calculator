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
using Calculator.Interfaces;
using Calculator.Models;
using Expression = Calculator.Models.Expression;

namespace Calculator
{
    class ViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public IMemory Memory { get; }
        public ViewModel()
        {
            ErrorDictionary = new Dictionary<string, string>();
            _expr = new ObservableCollection<Expression>();
            Memory = new MemoryRAM();
        }
        private static bool _hasNumberComma = false;
        private static char[] _delimiterChars = { '+', '-', '*', '/' };

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
                if (string.IsNullOrWhiteSpace(textValue))
                    ErrorDictionary[nameof(TextValue)] = "Field is empty";
                else
                    ErrorDictionary[nameof(TextValue)] = null;
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
            get => _removeFromMemory ?? new RelayCommand<ListBox>((x) =>
            {
                MessageBox.Show(x.SelectedIndex.ToString());
                    Memory.Remove(x.SelectedIndex);
                }, (x) => Memory.Any());
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
                Memory.Increase(Memory.Count - 1, TextValue);
            }, () => string.IsNullOrEmpty(TextValue) == false && Memory.Any());
        }



        private ICommand _subFromCommand;
        public ICommand SubFromLastMem
        {
            get => _subFromCommand ?? new RelayCommand(() =>
            {
                var value = Calc.Parse(TextValue);
                TextValue = Convert.ToString(value);
                Memory.Increase(Memory.Count, TextValue);
            }, () => string.IsNullOrEmpty(TextValue) == false && Memory.Any());
        }

        private ICommand _takeExp;
        public ICommand TakeExpression
        {
            get => _takeExp ?? new RelayCommand<TextBox>((x) =>
            {
                TextValue = x.Text;
            }, (x) => true);
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
                    string[] values = TextValue.Split(_delimiterChars);
                    if (values.Length > 0)
                        _hasNumberComma = values[values.Length - 1].Contains(',') ? true : false;
                    return;
                }
                if (TextValue.Length > 1 && "*/-+".IndexOf(TextValue[TextValue.Length - 1]) != -1 && "*/-+".IndexOf(x) != -1)
                {
                    TextValue = TextValue.Remove(TextValue.Length - 1) + x;
                    _hasNumberComma = false;
                    return;
                }
                TextValue += x;
                if ("-+*/".IndexOf(x) != -1)
                    _hasNumberComma = false;
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
                _hasNumberComma = true;
            }, x => _hasNumberComma == false);
        }

        private ICommand _clear;
        public ICommand Clear
        {
            get => _clear ?? new RelayCommand(() =>
            {
                TextValue = "";
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

        public Dictionary<string, string> ErrorDictionary { get; private set; }

        public string this[string columnName] => ErrorDictionary.ContainsKey(columnName) ? ErrorDictionary[columnName] : null;

        public string Error =>
            ErrorDictionary.Any(x => string.IsNullOrWhiteSpace(x.Value))
                ? string.Join(Environment.NewLine, ErrorDictionary.Where(x => string.IsNullOrWhiteSpace(x.Value) == false).GetEnumerator().Current)
                : null;
    }
}
