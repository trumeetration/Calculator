using System.Collections.ObjectModel;
using Calculator.Interfaces;

namespace Calculator.Models.History
{
    public class HistoryRAM : IHistory
    {
        public ObservableCollection<Expression> Expressions { get; }
        public HistoryRAM()
        {
            Expressions = new ObservableCollection<Expression>();
        }
        public void Add(Expression expression)
        {
            Expressions.Add(expression);
        }

        public void Clear()
        {
            if (Expressions.Count > 0)
                Expressions.Clear();
        }
    }
}