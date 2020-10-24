using NUnit.Framework;
using FluentAssertions;
using System.Collections.Generic;
using System;

namespace NUnitTests
{
    public class Tests
    {
        [Test]
        public void FourFiveFiveSix_Plus_FourNineSevenFourFive_ShouldReturn_54301()
        {
            var calc = new Calc();
            var result = calc.Parse("           4556           +    49745              ").Should().Be(54301).And.BePositive("Problemo");
        }

        [Test]
        public void FourFiveFiveSix_Plus_FourNineSevenFourFive_Minus_Hundred_ShouldReturn_54201()
        {
            var calc = new Calc();
            var result = calc.Parse("           4556           +    49745    -100          ").Should().Be(54201).And.BePositive("Problemo");
        }

        [Test]
        public void OneTwoThree_ShouldReturn_123()
        {
            var Calc = new Calc();
            var result = Calc.Parse("     123       ").Should().Be(123).And.BePositive("Problemo");
        }

        [Test]
        public void Five_Plus_Four_Multiply_Ten_ShouldReturn_45()
        {
            var calc = new Calc();
            var result = calc.Parse("           5           +    4    *10          ").Should().Be(45).And.BePositive("Problemo");
        }

        [Test]
        public void Five_Plus_Four_Multiply_Ten_Multiply_Ten_ShouldReturn_405()
        {
            var calc = new Calc();
            var result = calc.Parse("           5           +    4    *10*10          ").Should().Be(405).And.BePositive("Problemo");
        }

        [Test]
        public void Five_Plus_Four_Multiply_Ten_Split_Ten_ShouldReturn_9()
        {
            var calc = new Calc();
            var result = calc.Parse("           5           +    4    *10/10          ").Should().Be(9).And.BePositive("Problemo");
        }

        [Test]
        public void Five_Plus_Minus_Four_Multiply_Ten_Split_Ten_ShouldReturn_1()
        {
            var calc = new Calc();
            var result = calc.Parse("           5           +-    4    *10/10          ").Should().Be(1).And.BePositive("Problemo");
        }

        [Test]
        public void Five_Plus_Minus_Multiply_Four_Multiply_Ten_Split_Ten_ShouldReturn_20()
        {
            var calc = new Calc();
            var result = calc.Parse("           5           +-*    4    *10/10          ").Should().Be(20).And.BePositive("Problemo");
        }

        [Test]
        public void Five_Plus_Minus_Multiply_Four_Multiply_Ten_Split_Ten_ShouldReturn_y()
        {
            var calc = new Calc();
            var result = calc.Parse("-           5           +-*    4    *10/10*          ").Should().Be(20).And.BePositive("Problemo");
        }
    }

    internal class Calc
    {
        private List<double> valueList = new List<double>();
        private List<char> operators = new List<char>();
        public Calc()
        {
        }


        public double Parse(string expression)
        {
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