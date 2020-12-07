using NUnit.Framework;
using FluentAssertions;
using System.Collections.Generic;
using System;
using Calculator.Models;
using Calculator.Interfaces;

namespace NUnitTests
{
    public class Tests
    {
        [Test]
        public void FourFiveFiveSix_Plus_FourNineSevenFourFive_ShouldReturn_54301()
        {
            ICalculator Calc = new Calc();
            var result = Calc.Parse("           4556           +    49745              ").Should().Be(54301).And.BePositive("Problemo");
        }

        [Test]
        public void FourFiveFiveSix_Plus_FourNineSevenFourFive_Minus_Hundred_ShouldReturn_54201()
        {
            ICalculator Calc = new Calc();
            var result = Calc.Parse("           4556           +    49745    -100          ").Should().Be(54201).And.BePositive("Problemo");
        }

        [Test]
        public void OneTwoThree_ShouldReturn_123()
        {
            ICalculator Calc = new Calc();
            var result = Calc.Parse("     123       ").Should().Be(123).And.BePositive("Problemo");
        }

        [Test]
        public void Five_Plus_Four_Multiply_Ten_ShouldReturn_45()
        {
            ICalculator Calc = new Calc();
            var result = Calc.Parse("           5           +    4    *10          ").Should().Be(45).And.BePositive("Problemo");
        }

        [Test]
        public void Five_Plus_Four_Multiply_Ten_Multiply_Ten_ShouldReturn_405()
        {
            ICalculator Calc = new Calc();
            var result = Calc.Parse("           5           +    4    *10*10          ").Should().Be(405).And.BePositive("Problemo");
        }

        [Test]
        public void Five_Plus_Four_Multiply_Ten_Split_Ten_ShouldReturn_9()
        {
            ICalculator Calc = new Calc();
            var result = Calc.Parse("           5           +    4    *10/10          ").Should().Be(9).And.BePositive("Problemo");
        }

        [Test]
        public void Five_Plus_Minus_Four_Multiply_Ten_Split_Ten_ShouldReturn_1()
        {
            ICalculator Calc = new Calc();
            var result = Calc.Parse("           5           +-    4    *10/10          ").Should().Be(1).And.BePositive("Problemo");
        }

        [Test]
        public void Five_Plus_Minus_Multiply_Four_Multiply_Ten_Split_Ten_ShouldReturn_20()
        {
            ICalculator Calc = new Calc();
            var result = Calc.Parse("           5           +-*    4    *10/10          ").Should().Be(20).And.BePositive("Problemo");
        }

        [Test]
        public void Five_Plus_Minus_Multiply_Four_Multiply_Ten_Split_Ten_ShouldReturn_y()
        {
            ICalculator Calc = new Calc();
            var result = Calc.Parse("-           5           +-*    4    *10/10*          ").Should().Be(-20).And.BeNegative("Problemo");
        }

        [Test]
        public void SingleBrackets()
        {
            ICalculator Calc = new Calc();
            var result = Calc.Parse("5+4-(10+4*2)").Should().Be(-9).And.BeNegative("Problemo");
        }

        [Test]
        public void DoubleBrackets()
        {
            ICalculator Calc = new Calc();
            var result = Calc.Parse("5+4-(10+(4*2))+(1+1)").Should().Be(-7).And.BeNegative("Problemo");
        }

    }

    
}