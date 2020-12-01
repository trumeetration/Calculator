using NUnit.Framework;
using FluentAssertions;
using System.Collections.Generic;
using System;
using Calculator.Models;

namespace NUnitTests
{
    public class Tests
    {
        [Test]
        public void FourFiveFiveSix_Plus_FourNineSevenFourFive_ShouldReturn_54301()
        {
            
            var result = Calc.Parse("           4556           +    49745              ").Should().Be(54301).And.BePositive("Problemo");
        }

        [Test]
        public void FourFiveFiveSix_Plus_FourNineSevenFourFive_Minus_Hundred_ShouldReturn_54201()
        {
            
            var result = Calc.Parse("           4556           +    49745    -100          ").Should().Be(54201).And.BePositive("Problemo");
        }

        [Test]
        public void OneTwoThree_ShouldReturn_123()
        {
            
            var result = Calc.Parse("     123       ").Should().Be(123).And.BePositive("Problemo");
        }

        [Test]
        public void Five_Plus_Four_Multiply_Ten_ShouldReturn_45()
        {
            
            var result = Calc.Parse("           5           +    4    *10          ").Should().Be(45).And.BePositive("Problemo");
        }

        [Test]
        public void Five_Plus_Four_Multiply_Ten_Multiply_Ten_ShouldReturn_405()
        {
            
            var result = Calc.Parse("           5           +    4    *10*10          ").Should().Be(405).And.BePositive("Problemo");
        }

        [Test]
        public void Five_Plus_Four_Multiply_Ten_Split_Ten_ShouldReturn_9()
        {
            
            var result = Calc.Parse("           5           +    4    *10/10          ").Should().Be(9).And.BePositive("Problemo");
        }

        [Test]
        public void Five_Plus_Minus_Four_Multiply_Ten_Split_Ten_ShouldReturn_1()
        {
            
            var result = Calc.Parse("           5           +-    4    *10/10          ").Should().Be(1).And.BePositive("Problemo");
        }

        [Test]
        public void Five_Plus_Minus_Multiply_Four_Multiply_Ten_Split_Ten_ShouldReturn_20()
        {
            
            var result = Calc.Parse("           5           +-*    4    *10/10          ").Should().Be(20).And.BePositive("Problemo");
        }

        [Test]
        public void Five_Plus_Minus_Multiply_Four_Multiply_Ten_Split_Ten_ShouldReturn_y()
        {
            
            var result = Calc.Parse("-           5           +-*    4    *10/10*          ").Should().Be(-20).And.BeNegative("Problemo");
        }
    }

    
}