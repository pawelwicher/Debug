using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Pse.Cspr.Service.Tests
{
    public class StringService
    {
        public string ConcatWithDot(string first, string second)
        {
            return this.ConcatWithCharacter(first, second, '.');
        }

        public string ConcatWithSemicolon(string first, string second)
        {
            return this.ConcatWithCharacter(first, second, ';');
        }

        public string ConcatWithHash(string first, string second)
        {
            return this.ConcatWithCharacter(first, second, '#');
        }

        public string ConcatWithUnderscore(string first, string second)
        {
            return this.ConcatWithCharacter(first, second, '_');
        }

        private string ConcatWithCharacter(string first, string second, char concatCharacter)
        {
            if (string.IsNullOrEmpty(first) || string.IsNullOrEmpty(second))
            {
                return string.Empty;
            }

            return $"{char.ToUpper(first[0])}{first.Substring(1)}{concatCharacter}{char.ToUpper(second[0])}{second.Substring(1)}";
        }
    }

    public class TestDataGenerator : IEnumerable<object[]>
    {
        private readonly IEnumerable<object[]> data = new[]
        {
            new object[] { "aaa", "bbb", "Aaa;Bbb" },
            new object[] { string.Empty, string.Empty, string.Empty }
        };

        public IEnumerator<object[]> GetEnumerator() => this.data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public class TestData
    {
        public static IEnumerable<object[]> DataForConcatWithHashTest => new[]
        {
            new object[] { "aaa", "bbb", "Aaa#Bbb" },
            new object[] { string.Empty, string.Empty, string.Empty }
        };
    }

    public class StringServiceTests
    {
        [Theory]
        [InlineData("aaa", "bbb", "Aaa.Bbb")]
        [InlineData("", "", "")]
        public void ConcatWithDotTest(string first, string second, string expected)
        {
            var service = new StringService();

            var result = service.ConcatWithDot(first, second);

            result.Should().Be(expected);
        }

        [Theory]
        [ClassData(typeof(TestDataGenerator))]
        public void ConcatWithSemicolonTest(string first, string second, string expected)
        {
            var service = new StringService();

            var result = service.ConcatWithSemicolon(first, second);

            result.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(TestData.DataForConcatWithHashTest), MemberType = typeof(TestData))]
        public void ConcatWithHashTest(string first, string second, string expected)
        {
            var service = new StringService();

            var result = service.ConcatWithHash(first, second);

            result.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(DataForConcatWithUnderscore))]
        public void ConcatWithUnderscoreTest(string first, string second, string expected)
        {
            var service = new StringService();

            var result = service.ConcatWithUnderscore(first, second);

            result.Should().Be(expected);
        }

        public static IEnumerable<object[]> DataForConcatWithUnderscore => new[]
        {
            new object[] { "aaa", "bbb", "Aaa_Bbb" },
            new object[] { string.Empty, string.Empty, string.Empty }
        };
    }
}