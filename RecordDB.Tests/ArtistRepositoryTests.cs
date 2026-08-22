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

        // ── GetArtistsAsync ─────────────────────────────────────────────────────

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
        public async Task GetArtistsAsync_ShouldReturnEmpty_WhenNoDataExists()
        {
            // Arrange
            _dbMock
                .Setup(x => x.GetData<Artist, dynamic>(
                    "up_ArtistSelectFull",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(new List<Artist>());

            // Act
            var result = await _repository.GetArtistsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        // ── GetArtists (sync-style wrapper) ─────────────────────────────────────

        [Fact]
        public async Task GetArtists_ShouldReturnArtistList_WhenDataExists()
        {
            // Arrange
            var expected = new List<Artist>
            {
                new Artist { ArtistId = 10, Name = "Led Zeppelin" }
            };

            _dbMock
                .Setup(x => x.GetData<Artist, dynamic>(
                    "up_ArtistSelectFull",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.GetArtists();

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Led Zeppelin");
        }

        // ── GetArtistsByPartialNameAsync ─────────────────────────────────────────

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
        public async Task GetArtistsByPartialNameAsync_ShouldReturnEmpty_WhenNoMatch()
        {
            // Arrange
            _dbMock
                .Setup(x => x.GetData<Artist, dynamic>(
                    "up_GetArtistsByPartialName",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(new List<Artist>());

            // Act
            var result = await _repository.GetArtistsByPartialNameAsync("zzznomatch");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        // ── GetArtistListAsync ──────────────────────────────────────────────────

        [Fact]
        public async Task GetArtistListAsync_ShouldReturnList_UsingCorrectSproc()
        {
            // Arrange
            var expected = new List<Artist>
            {
                new Artist { ArtistId = 0,  Name = "None" },
                new Artist { ArtistId = 1,  Name = "Bob Dylan" },
                new Artist { ArtistId = 2,  Name = "Neil Young" }
            };

            _dbMock
                .Setup(x => x.GetData<Artist, dynamic>(
                    "up_getArtistListandNone",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.GetArtistListAsync();

            // Assert
            result.Should().HaveCount(3);
            result.First().Name.Should().Be("None");
        }

        // ── GetArtistsWithNoBiographyAsync ──────────────────────────────────────

        [Fact]
        public async Task GetArtistsWithNoBiographyAsync_ShouldReturnArtists_WhenDataExists()
        {
            // Arrange
            var expected = new List<Artist>
            {
                new Artist { ArtistId = 5,  Name = "Joni Mitchell", Biography = null },
                new Artist { ArtistId = 12, Name = "Tom Waits",     Biography = null }
            };

            _dbMock
                .Setup(x => x.GetData<Artist, dynamic>(
                    "up_SelectArtistsWithNoBiography",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.GetArtistsWithNoBiographyAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().OnlyContain(a => a.Biography == null);
        }

        [Fact]
        public async Task GetArtistsWithNoBiographyAsync_ShouldReturnEmpty_WhenAllHaveBiography()
        {
            // Arrange
            _dbMock
                .Setup(x => x.GetData<Artist, dynamic>(
                    "up_SelectArtistsWithNoBiography",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(new List<Artist>());

            // Act
            var result = await _repository.GetArtistsWithNoBiographyAsync();

            // Assert
            result.Should().BeEmpty();
        }

        // ── GetArtistWithNoBiographyAsync ───────────────────────────────────────

        [Fact]
        public async Task GetArtistWithNoBiographyAsync_ShouldReturnArtist_WhenFound()
        {
            // Arrange
            var expected = new Artist { ArtistId = 7, Name = "Joni Mitchell", Biography = null };

            _dbMock
                .Setup(x => x.GetFirstOrDefault<Artist, dynamic>(
                    "up_SearchForArtistWithNoBiography",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.GetArtistWithNoBiographyAsync("Joni");

            // Assert
            result.Should().NotBeNull();
            result.ArtistId.Should().Be(7);
            result.Name.Should().Be("Joni Mitchell");
        }

        [Fact]
        public async Task GetArtistWithNoBiographyAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            _dbMock
                .Setup(x => x.GetFirstOrDefault<Artist, dynamic>(
                    "up_SearchForArtistWithNoBiography",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync((Artist?)null);

            // Act
            var result = await _repository.GetArtistWithNoBiographyAsync("zzznomatch");

            // Assert
            result.Should().BeNull();
        }

        // ── SelectAsync() — no param ────────────────────────────────────────────

        [Fact]
        public async Task SelectAsync_NoParam_ShouldReturnAllArtists()
        {
            // Arrange
            var expected = new List<Artist>
            {
                new Artist { ArtistId = 1, Name = "Bob Dylan" },
                new Artist { ArtistId = 2, Name = "Neil Young" },
                new Artist { ArtistId = 3, Name = "Bruce Springsteen" }
            };

            _dbMock
                .Setup(x => x.GetData<Artist, dynamic>(
                    "up_ArtistSelectAll",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.SelectAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Select(a => a.Name).Should().Contain("Bruce Springsteen");
        }

        // ── SelectAsync(int) — by id ────────────────────────────────────────────

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
        public async Task SelectAsync_ById_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            _dbMock
                .Setup(x => x.GetFirstOrDefault<Artist, dynamic>(
                    "up_ArtistSelectById",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync((Artist?)null);

            // Act
            var result = await _repository.SelectAsync(9999);

            // Assert
            result.Should().BeNull();
        }

        // ── SelectArtistWithNoBioAsync ──────────────────────────────────────────

        [Fact]
        public async Task SelectArtistWithNoBioAsync_ShouldReturnArtists_UsingCorrectSproc()
        {
            // Arrange
            var expected = new List<Artist>
            {
                new Artist { ArtistId = 3, Name = "Van Morrison" }
            };

            _dbMock
                .Setup(x => x.GetData<Artist, dynamic>(
                    "up_selectArtistsWithNoBio",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.SelectArtistWithNoBioAsync();

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Van Morrison");
        }

        // ── InsertAsync(Artist) ─────────────────────────────────────────────────

        [Fact]
        public async Task InsertAsync_Artist_ShouldCallSaveDataReturnId()
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

        // ── InsertAsync(string, string, string) ─────────────────────────────────

        [Fact]
        public async Task InsertAsync_ByStrings_ShouldCallSaveDataReturnId_AndReturnNewId()
        {
            // Arrange
            _dbMock
                .Setup(x => x.SaveDataReturnId(
                    "adm_ArtistInsert",
                    It.IsAny<DynamicParameters>(),
                    "@Result"))
                .ReturnsAsync(55);

            // Act
            var result = await _repository.InsertAsync("Jimi", "Hendrix", "American rock guitarist");

            // Assert
            result.Should().Be(55);
            _dbMock.Verify(x => x.SaveDataReturnId("adm_ArtistInsert", It.IsAny<DynamicParameters>(), "@Result"), Times.Once);
        }

        // ── UpdateArtistAsync ───────────────────────────────────────────────────

        [Fact]
        public async Task UpdateArtistAsync_ShouldCallSaveDataReturnId_AndReturnId()
        {
            // Arrange
            var artist = new Artist
            {
                ArtistId = 10,
                FirstName = "Bob",
                LastName = "Dylan",
                Name = "Bob Dylan",
                Biography = "American singer-songwriter"
            };

            _dbMock
                .Setup(x => x.SaveDataReturnId(
                    "up_UpdateArtist",
                    It.IsAny<DynamicParameters>(),
                    "@Result"))
                .ReturnsAsync(10);

            // Act
            var result = await _repository.UpdateArtistAsync(artist);

            // Assert
            result.Should().Be(10);
            _dbMock.Verify(x => x.SaveDataReturnId("up_UpdateArtist", It.IsAny<DynamicParameters>(), "@Result"), Times.Once);
        }

        // ── GetArtistByRecordIdAsync ────────────────────────────────────────────

        [Fact]
        public async Task GetArtistByRecordIdAsync_ShouldReturnArtist_WhenFound()
        {
            // Arrange
            var expected = new Artist { ArtistId = 3, Name = "Bruce Springsteen" };

            _dbMock
                .Setup(x => x.GetFirstOrDefault<Artist, dynamic>(
                    "up_ArtistSelectByRecordId",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.GetArtistByRecordIdAsync(999);

            // Assert
            result.Should().NotBeNull();
            result.ArtistId.Should().Be(3);
            result.Name.Should().Be("Bruce Springsteen");
        }

        [Fact]
        public async Task GetArtistByRecordIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            _dbMock
                .Setup(x => x.GetFirstOrDefault<Artist, dynamic>(
                    "up_ArtistSelectByRecordId",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync((Artist?)null);

            // Act
            var result = await _repository.GetArtistByRecordIdAsync(0);

            // Assert
            result.Should().BeNull();
        }

        // ── GetBiographyAsync ───────────────────────────────────────────────────

        [Fact]
        public async Task GetBiographyAsync_ShouldReturnBiographyString_WhenFound()
        {
            // Arrange
            const string expectedBio = "Bob Dylan is an American singer-songwriter.";

            _dbMock
                .Setup(x => x.GetScalar<string, dynamic>(
                    "up_getBiography",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expectedBio);

            // Act
            var result = await _repository.GetBiographyAsync(5);

            // Assert
            result.Should().Be(expectedBio);
        }

        [Fact]
        public async Task GetBiographyAsync_ShouldReturnEmptyString_WhenNullReturned()
        {
            // Arrange
            _dbMock
                .Setup(x => x.GetScalar<string, dynamic>(
                    "up_getBiography",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync((string?)null);

            // Act
            var result = await _repository.GetBiographyAsync(999);

            // Assert
            result.Should().BeEmpty();
        }

        // ── DeleteAsync ─────────────────────────────────────────────────────────

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
