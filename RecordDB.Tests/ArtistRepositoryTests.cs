using Dapper;
using FluentAssertions;
using Moq;
using RecordDB.DAL.Data;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using System.Data;
using Xunit;

namespace RecordDB.Tests
{
    public class ArtistRepositoryTests
    {
        private readonly Mock<IDataAccess> _dbMock;
        private readonly ArtistRepository _repository;

        public ArtistRepositoryTests()
        {
            _dbMock = new Mock<IDataAccess>();
            _repository = new ArtistRepository(_dbMock.Object);
        }

        [Fact]
        public async Task GetArtistsAsync_ShouldReturnArtists_WhenDataExists()
        {
            // Arrange
            var expectedArtists = new List<Artist>
            {
                new Artist { ArtistId = 1, Name = "Bob Dylan", Biography = "Singer-songwriter" },
                new Artist { ArtistId = 2, Name = "The Beatles", Biography = "Rock band" }
            };

            _dbMock
                .Setup(x => x.GetData<Artist, dynamic>(
                    "up_ArtistSelectFull",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expectedArtists);

            // Act
            var result = await _repository.GetArtistsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Bob Dylan");
        }

        [Fact]
        public async Task GetArtistsByPartialNameAsync_ShouldPassParameter_AndReturnMatchingArtists()
        {
            // Arrange
            var expectedArtists = new List<Artist>
            {
                new Artist { ArtistId = 1, Name = "John Lennon" },
                new Artist { ArtistId = 2, Name = "John Coltrane" }
            };

            _dbMock
                .Setup(x => x.GetData<Artist, dynamic>(
                    "up_GetArtistsByPartialName",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expectedArtists);

            // Act
            var result = await _repository.GetArtistsByPartialNameAsync("John");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Select(a => a.Name).Should().Contain("John Lennon");
        }

        [Fact]
        public async Task SelectAsync_ById_ShouldReturnArtist_WhenFound()
        {
            // Arrange
            var expectedArtist = new Artist { ArtistId = 114, Name = "Neil Young", Biography = "Canadian musician" };

            _dbMock
                .Setup(x => x.GetFirstOrDefault<Artist, dynamic>(
                    "up_ArtistSelectById",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expectedArtist);

            // Act
            var result = await _repository.SelectAsync(114);

            // Assert
            result.Should().NotBeNull();
            result.ArtistId.Should().Be(114);
            result.Name.Should().Be("Neil Young");
        }

        [Fact]
        public async Task InsertAsync_ShouldCallSaveDataReturnId()
        {
            // Arrange
            var newArtist = new Artist
            {
                FirstName = "David",
                LastName = "Bowie",
                Biography = "English singer-songwriter"
            };

            _dbMock
                .Setup(x => x.SaveDataReturnId(
                    "adm_ArtistInsert",
                    It.IsAny<DynamicParameters>(),
                    "@Result"))
                .ReturnsAsync(42);

            // Act
            var result = await _repository.InsertAsync(newArtist);

            // Assert
            result.Should().Be(42);
            _dbMock.Verify(x => x.SaveDataReturnId("adm_ArtistInsert", It.IsAny<DynamicParameters>(), "@Result"), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallSaveData_WithArtistId()
        {
            // Arrange
            int artistIdToDelete = 99;

            _dbMock
                .Setup(x => x.SaveData(
                    "up_deleteArtist",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .Returns(Task.CompletedTask);

            // Act
            await _repository.DeleteAsync(artistIdToDelete);

            // Assert
            _dbMock.Verify(x => x.SaveData("up_deleteArtist", It.IsAny<DynamicParameters>(), CommandType.StoredProcedure), Times.Once);
        }
    }
}
