using System;
using System.Collections.Generic;
using System.Linq;
using AutoAdmin.Core.Collections.Extensions;
using AutoAdmin.Core.Infrastructure.Security;
using Xunit;
using Xunit.Extensions.Ordering;
using Xunit.Abstractions;
using FluentAssertions;

namespace AutoAdmin.IntegrationTest.Infrastructure.Security
{
    [Collection("AutoAdmin Collection"), Order(3)]
    public class SqlInjectionScannerTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public SqlInjectionScannerTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public static IEnumerable<object[]> ValidConditions = new List<object[]>
        {
            new object[]
            {
                @"(numberOfWindows < @p1) AND (numberOfDoors >= @p2) OR (height < @p3)",
                new List<string>() {"numberOfWindows", "numberOfDoors", "height"}
            },
            new object[]
            {
                @"(name like @p1) OR (lastName not like @p2) OR (age = @p3)",
                new List<string>() {"name", "lastName", "age"}
            }
        };

        [Theory(DisplayName = "Should pass")]
        [MemberData(nameof(ValidConditions))]
        public void SuccessfulColumnsExtractionTest(string whereCondition, List<string> expectedColumns)
        {
            _testOutputHelper.WriteLine($"WhereCondition = {whereCondition}");
            _testOutputHelper.WriteLine($"Expected columns = {expectedColumns.ToString<string>()}");
            var columnsNames = SqlInjectionScanner.GetColumnsNames(whereCondition);
            columnsNames.Should().BeEquivalentTo(expectedColumns);
        }
        
        public static IEnumerable<object[]> UnsecureConditions = new List<object[]>
        {
            new object[]
            {
                @"(select * from customer < @p1)"
            },
            new object[]
            {
                @"(name like delete * from account)"
            }
        };

        [Theory()]
        [MemberData(nameof(UnsecureConditions))]
        public void InjectionTest(string condition)
        {
            var columnNames = SqlInjectionScanner.GetColumnsNames(condition);
            columnNames.Count().Should().Be(0);
        }
        
        public static IEnumerable<object[]> KnownColumns = new List<object[]>
        {
            new object[]
            {
                @"(numberOfWindows < @p1) AND (numberOfDoors >= @p2) OR (height < @p3)",
                new [] { "id", "area", "numberOfWindows", "numberOfDoors", "width", "height", "length" }
            },
            new object[]
            {
                @"(name like @p1) AND (area >= @p2)",
                new [] { "id", "name", "numberOfWindows", "numberOfDoors", "area", "height", "length" }
            }
        };

        [Theory()]
        [MemberData(nameof(KnownColumns))]
        public void KnownColumnsTest(string condition, string[] entityColumns)
        {
            Exception exception = null;
            try
            {
                var columnNames = SqlInjectionScanner.ScanCondition(condition, entityColumns);
                entityColumns.Should().Contain(columnNames);

            }
            catch (Exception e)
            {
                exception = e;
            }
            exception.Should().BeNull();
        }
        
        public static IEnumerable<object[]> UnKnownColumns = new List<object[]>
        {
            new object[]
            {
                @"(middleName < @p1) OR (volume < @p3)",
                new [] { "id", "area", "numberOfWindows", "numberOfDoors", "width", "height", "length" }
            }
        };
        
        [Theory()]
        [MemberData(nameof(UnKnownColumns))]
        public void UKnownColumnsTest(string condition, string[] entityColumns)
        {
            Exception exception = null;
            try
            {
                var columnNames = SqlInjectionScanner.ScanCondition(condition, entityColumns);
                entityColumns.Should().Contain(columnNames);

            }
            catch (Exception e)
            {
                exception = e;
            }
            exception.Should().NotBeNull();
        }
    }
}