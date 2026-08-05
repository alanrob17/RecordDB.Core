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
            var result = date.ToShortDate();

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
            var result = date.ToShortDate();

            // Assert
            result.Should().Be("unk");
        }

        [Fact]
        public void ToShortDate_WithObjectNull_ReturnsUnk()
        {
            // Arrange
            object? obj = null;

            // Act
            var result = obj.ToShortDate();

            // Assert
            result.Should().Be("unk");
        }

        [Fact]
        public void ToShortDate_WithDBNull_ReturnsUnk()
        {
            // Arrange
            object obj = DBNull.Value;

            // Act
            var result = obj.ToShortDate();

            // Assert
            result.Should().Be("unk");
        }

        [Fact]
        public void ToShortDate_WithValidDateString_ReturnsFormattedDateString()
        {
            // Arrange
            string dateStr = "2022-10-01";

            // Act
            var result = dateStr.ToShortDate();

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
            var result = dateStr.ToShortDate();

            // Assert
            result.Should().Be("unk");
        }
    }
}
