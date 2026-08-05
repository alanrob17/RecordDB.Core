using Dapper;
using FluentAssertions;
using Moq;
using RecordDB.DAL.Data;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using System.Data;
using Xunit;

namespace RecordDB.Tests
{
    public class RecordRepositoryTests
    {
        private readonly Mock<IDataAccess> _dbMock;
        private readonly RecordRepository _repository;

        public RecordRepositoryTests()
        {
            _dbMock = new Mock<IDataAccess>();
            _repository = new RecordRepository(_dbMock.Object);
        }

        [Fact]
        public async Task SelectAsync_ById_ShouldReturnArtistRecordDto_WhenRecordExists()
        {
            // Arrange
            var expectedDto = new ArtistRecordDto
            {
                RecordId = 10,
                ArtistId = 1,
                Name = "Blood on the Tracks",
                Field = "Folk",
                Recorded = 1975,
                Media = "CD"
            };

            _dbMock
                .Setup(x => x.GetFirstOrDefault<ArtistRecordDto, dynamic>(
                    "up_RecordSelectByIdCore",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expectedDto);

            // Act
            var result = await _repository.SelectAsync(10);

            // Assert
            result.Should().NotBeNull();
            result.RecordId.Should().Be(10);
            result.Name.Should().Be("Blood on the Tracks");
            result.Recorded.Should().Be(1975);
        }

        [Fact]
        public async Task CountDiscsAsync_ShouldReturnDiscCountString_WhenValidShowFilterPassed()
        {
            // Arrange
            _dbMock
                .Setup(x => x.GetScalar<int, object>(
                    "up_CountDiscs",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(42);

            // Act
            var result = await _repository.CountDiscsAsync("Rock");

            // Assert
            result.Should().Be("42");
        }

        [Fact]
        public async Task CountDiscsAsync_ShouldThrowArgumentNullException_WhenShowIsNull()
        {
            // Act
            Func<Task> act = async () => await _repository.CountDiscsAsync(null!);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetRecordsByYearAsync_ShouldReturnRecords_ForGivenYear()
        {
            // Arrange
            var expectedRecords = new List<ArtistRecordDto>
            {
                new ArtistRecordDto { RecordId = 1, Name = "Darkness on the Edge of Town", Recorded = 1978 }
            };

            _dbMock
                .Setup(x => x.GetData<ArtistRecordDto, dynamic>(
                    "up_GetRecordsByYear",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expectedRecords);

            // Act
            var result = await _repository.GetRecordsByYearAsync(1978);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Darkness on the Edge of Town");
        }

        [Fact]
        public async Task NoRecordReviewsAsync_ShouldReturnList_OfMissingReviewDtos()
        {
            // Arrange
            var expectedList = new List<MissingReviewDto>
            {
                new MissingReviewDto { RecordId = 5, Name = "Bob Dylan", Record = "Untitled Album" }
            };

            _dbMock
                .Setup(x => x.GetData<MissingReviewDto, dynamic>(
                    "up_NoRecordReviews",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expectedList);

            // Act
            var result = await _repository.NoRecordReviewsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().RecordId.Should().Be(5);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallSaveData_WithRecordId()
        {
            // Arrange
            int recordIdToDelete = 15;

            _dbMock
                .Setup(x => x.SaveData(
                    "up_deleteRecord",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .Returns(Task.CompletedTask);

            // Act
            await _repository.DeleteAsync(recordIdToDelete);

            // Assert
            _dbMock.Verify(x => x.SaveData("up_deleteRecord", It.IsAny<DynamicParameters>(), CommandType.StoredProcedure), Times.Once);
        }
    }
}
