using FluentAssertions;
using RecordDB.DAL.Extensions;
using Xunit;

namespace RecordDB.Tests
{
    public class DateTimeExtensionsTests
    {
        [Fact]
        public void ToShortDate_WithValidDateTime_ReturnsFormattedDateString()
        {
            // Arrange
            var date = new DateTime(2023, 5, 15);

            // Act
            var result = DateTimeExtensions.ToShortDate(date);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().NotBe("unk");
        }

        [Fact]
        public void ToShortDate_WithNullableDateTimeNull_ReturnsUnk()
        {
            // Arrange
            DateTime? date = null;

            // Act
            var result = DateTimeExtensions.ToShortDate(date);

            // Assert
            result.Should().Be("unk");
        }

        [Fact]
        public void ToShortDate_WithObjectNull_ReturnsUnk()
        {
            // Arrange
            object? obj = null;

            // Act
            var result = DateTimeExtensions.ToShortDate(obj);

            // Assert
            result.Should().Be("unk");
        }

        [Fact]
        public void ToShortDate_WithDBNull_ReturnsUnk()
        {
            // Arrange
            object obj = DBNull.Value;

            // Act
            var result = DateTimeExtensions.ToShortDate(obj);

            // Assert
            result.Should().Be("unk");
        }

        [Fact]
        public void ToShortDate_WithValidDateString_ReturnsFormattedDateString()
        {
            // Arrange
            DateTime date = new DateTime(2022, 10, 1);

            // Act
            var result = DateTimeExtensions.ToShortDate(date);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().NotBe("unk");
        }

        [Fact]
        public void ToShortDate_WithInvalidDateString_ReturnsUnk()
        {
            // Arrange
            string dateStr = "invalid-date-string";

            // Act
            var result = DateTimeExtensions.ToShortDate(dateStr);

            // Assert
            result.Should().Be("unk");
        }
    }
}
