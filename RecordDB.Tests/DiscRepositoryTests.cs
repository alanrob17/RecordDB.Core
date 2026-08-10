using Dapper;
using FluentAssertions;
using Moq;
using RecordDB.DAL.Data;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RecordDB.Tests
{
    public class DiscRepositoryTests
    {
        private readonly Mock<IDataAccess> _dbMock;
        private readonly DiscRepository _repository;

        public DiscRepositoryTests()
        {
            _dbMock = new Mock<IDataAccess>();
            _repository = new DiscRepository(_dbMock.Object);
        }

        [Fact]
        public async Task SelectAllDiscEntitiesAsync_ShouldReturnDiscs_WhenDataExists()
        {
            // Arrange
            var expectedDiscs = new List<Disc>
            {
                new Disc { DiscId = 1, RecordId = 10, DiscNo = 1, Length = 2400 },
                new Disc { DiscId = 2, RecordId = 10, DiscNo = 2, Length = 2500 }
            };

            _dbMock
                .Setup(x => x.GetData<Disc, dynamic>(
                    "up_SelectAllDiscEntities",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expectedDiscs);

            // Act
            var result = await _repository.SelectAllDiscEntitiesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().DiscId.Should().Be(1);
        }

        [Fact]
        public async Task GetDiscRecordsByRecordNameAsync_ShouldReturnDiscs_ForMatchingRecordName()
        {
            // Arrange
            var expectedDiscs = new List<Disc>
            {
                new Disc { DiscId = 5, RecordId = 25, DiscNo = 1 }
            };

            _dbMock
                .Setup(x => x.GetData<Disc, dynamic>(
                    "up_GetDiscRecordsByRecordName",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expectedDiscs);

            // Act
            var result = await _repository.GetDiscRecordsByRecordNameAsync("Abbey Road");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().RecordId.Should().Be(25);
        }

        [Fact]
        public async Task SelectSingleDiscAsync_ShouldReturnDisc_WhenFound()
        {
            // Arrange
            var expectedDisc = new Disc { DiscId = 12, RecordId = 50, DiscNo = 1, Length = 1800 };

            _dbMock
                .Setup(x => x.GetFirstOrDefault<Disc, dynamic>(
                    "up_SelectSingleDisc",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expectedDisc);

            // Act
            var result = await _repository.SelectSingleDiscAsync(12);

            // Assert
            result.Should().NotBeNull();
            result!.DiscId.Should().Be(12);
        }

        [Fact]
        public async Task InsertDiscAsync_ShouldCallSaveDataReturnId_WithUpInsertDisc()
        {
            // Arrange
            var newDisc = new Disc { RecordId = 100, DiscNo = 1, FreeDbId = "test1234", Length = 3000 };

            _dbMock
                .Setup(x => x.SaveDataReturnId(
                    "up_InsertDisc",
                    It.IsAny<DynamicParameters>(),
                    "@Result"))
                .ReturnsAsync(77);

            // Act
            var result = await _repository.InsertDiscAsync(newDisc);

            // Assert
            result.Should().Be(77);
            _dbMock.Verify(x => x.SaveDataReturnId("up_InsertDisc", It.IsAny<DynamicParameters>(), "@Result"), Times.Once);
        }

        [Fact]
        public async Task UpdateDiscAsync_ShouldCallSaveDataReturnId_WithUpUpdateDisc()
        {
            // Arrange
            var discToUpdate = new Disc { DiscId = 77, RecordId = 100, DiscNo = 2, Length = 3100 };

            _dbMock
                .Setup(x => x.SaveDataReturnId(
                    "up_UpdateDisc",
                    It.IsAny<DynamicParameters>(),
                    "@Result"))
                .ReturnsAsync(77);

            // Act
            var result = await _repository.UpdateDiscAsync(discToUpdate);

            // Assert
            result.Should().Be(77);
            _dbMock.Verify(x => x.SaveDataReturnId("up_UpdateDisc", It.IsAny<DynamicParameters>(), "@Result"), Times.Once);
        }

        [Fact]
        public async Task DeleteDiscAsync_ShouldCallSaveData_WithUpDiscDelete()
        {
            // Arrange
            int discIdToDelete = 77;

            _dbMock
                .Setup(x => x.SaveData(
                    "up_DiscDelete",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .Returns(Task.CompletedTask);

            // Act
            await _repository.DeleteDiscAsync(discIdToDelete);

            // Assert
            _dbMock.Verify(x => x.SaveData("up_DiscDelete", It.IsAny<DynamicParameters>(), CommandType.StoredProcedure), Times.Once);
        }
    }
}
