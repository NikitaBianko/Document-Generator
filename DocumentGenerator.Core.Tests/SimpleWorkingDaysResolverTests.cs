using System;
using System.Collections.Generic;
using Xunit;

namespace DocumentGenerator.Core.Tests
{
    public class SimpleWorkingDaysResolverTests
    {
        [Fact]
        public void FridayShouldBeWorkingDay()
        {
            var sut = new SimpleWorkingDaysResolver(
                DateTime.MinValue,
                DateTime.MaxValue,
                new List<DateTime>());

            var result = sut.IsWorkingDay(new DateTime(2021, 7, 9)); // Friday

            Assert.True(result);
        }
        
        [Fact]
        public void SundayShouldBePublicHoliday()
        {
            var sut = new SimpleWorkingDaysResolver(
                DateTime.MinValue,
                DateTime.MaxValue,
                new List<DateTime>());

            var result = sut.IsWorkingDay(new DateTime(2021, 7, 11)); // Sunday

            Assert.False(result);
        }
        
        [Fact]
        public void ExplicitHolidayShouldBeChecked()
        {
            var sut = new SimpleWorkingDaysResolver(
                DateTime.MinValue,
                DateTime.MaxValue,
                new List<DateTime>{new DateTime(2021, 7, 11)});

            var result = sut.IsWorkingDay(new DateTime(2021, 7, 11)); // Friday, but explicitly set to be holiday

            Assert.False(result);
        }
    }
}