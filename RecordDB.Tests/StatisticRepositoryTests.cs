using FluentAssertions;
using Moq;
using RecordDB.DAL.Data;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using System.Data;
using Xunit;

namespace RecordDB.Tests
{
    public class StatisticRepositoryTests
    {
        private readonly Mock<IDataAccess> _dbMock;
        private readonly StatisticRepository _repository;

        public StatisticRepositoryTests()
        {
            _dbMock = new Mock<IDataAccess>();
            _repository = new StatisticRepository(_dbMock.Object);
        }

        [Fact]
        public async Task GetStatisticsAsync_ShouldReturnPopulatedStatistics()
        {
            // Arrange: Setup scalar mocks for GetCountAsync and GetCostAsync queries
            _dbMock
                .Setup(x => x.GetScalar<int?, object>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    CommandType.Text))
                .ReturnsAsync(15);

            _dbMock
                .Setup(x => x.GetScalar<decimal?, object>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    CommandType.Text))
                .ReturnsAsync(150.50m);

            // Act
            var stats = await _repository.GetStatisticsAsync();

            // Assert
            stats.Should().NotBeNull();
            stats.TotalCDs.Should().Be(15);
            stats.TotalRecords.Should().Be(15);
            stats.RockDisks.Should().Be(15);
            stats.RecordCost.Should().Be(150.50m);
            stats.CDCost.Should().Be(150.50m);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenDataAccessIsNull()
        {
            // Act
            Action act = () => new StatisticRepository(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
