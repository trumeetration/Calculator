using System.Collections.ObjectModel;
using System.IO;
using Calculator.Interfaces;
using Newtonsoft.Json;

namespace Calculator.Models.History
{
    public class HistoryJson : IHistory
    {
        public ObservableCollection<Expression> Expressions { get; }
        private string path;
        public HistoryJson(string jsonPath = "history.json")
        {
            path = jsonPath;
            if (File.Exists(path) == false)
            {
                FileStream stream = File.Create(path);
                stream.Close();
                Expressions = new ObservableCollection<Expression>();
                return;
            }
            var lastTimeExpressions = File.ReadAllText(path);
            Expressions = JsonConvert.DeserializeObject<ObservableCollection<Expression>>(lastTimeExpressions);
            Expressions = Expressions ?? new ObservableCollection<Expression>();
        }
        public void Add(Expression expression)
        {
            Expressions.Add(expression);
            SaveToJson();
        }

        public void Clear()
        {
            if (Expressions.Count > 0)
            {
                Expressions.Clear();
                SaveToJson();
            }
        }

        public void SaveToJson()
        {
            if (File.Exists(path) == false)
            {
                File.Create(path);
            }
            var data = JsonConvert.SerializeObject(Expressions);
            File.WriteAllText(path, data);
        }
    }
}