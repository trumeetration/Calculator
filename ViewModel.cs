using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Calculator
{
    class ViewModel : INotifyPropertyChanged
    {
        private static string lastValue = null;
        private static string op = null;
        private static bool IsFinished = false;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string textValue = "0";

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
                if (IsFinished)
                {
                    TextValue = "0";
                    IsFinished = false;
                }
                if (Regex.IsMatch(x, "\\d+"))
                {
                    if (TextValue == "0" || TextValue == null)
                    {
                        TextValue = x;
                        return;
                    }
                    TextValue += x;
                }
                if (x == "Back")
                {
                    if (TextValue.Count() == 1)
                    {
                        TextValue = "0";
                        return;
                    }
                    TextValue = TextValue.Remove(TextValue.Count() - 1);
                }
            }, x => true);
        }

        private ICommand _renderExpression;
        public ICommand RenderExpression
        {
            get => _renderExpression ?? new RelayCommand<string>(x =>
            {
                IsFinished = true;
                if (lastValue == null)
                {
                    lastValue = TextValue;
                    op = x == "=" ? null : x;
                    TextValue = "0";
                    return;
                }
                if (op == null)
                {
                    if (x == "=") return;
                    op = x;
                    lastValue = TextValue;
                    return;
                }
                //
                switch (op)
                {
                    case "+": lastValue = TextValue = Convert.ToString(Double.Parse(lastValue) + Double.Parse(TextValue)); break;
                    case "-": lastValue = TextValue = Convert.ToString(Double.Parse(lastValue) - Double.Parse(TextValue)); break;
                    case "*": lastValue = TextValue = Convert.ToString(Double.Parse(lastValue) * Double.Parse(TextValue)); break;
                    case "/": lastValue = TextValue = Convert.ToString(Double.Parse(lastValue) / Double.Parse(TextValue)); break;
                    default: break;
                }
                if (x == "=")
                {
                    op = null;
                    lastValue = null;
                }
            }, x => true);
        }
        private ICommand _comma;
        public ICommand Comma
        {
            get => _comma ?? new RelayCommand<string>(x =>
            {
                TextValue += x;
            }, x => !TextValue.Contains(x));
        }

        private ICommand _clear;
        public ICommand Clear
        {
            get => _clear ?? new RelayCommand(() =>
            {
                TextValue = "0";
                lastValue = null;
                op = null;
                IsFinished = false;
            }, () => true);
        }
    }
}
