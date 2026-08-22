using Dapper;
using FluentAssertions;
using Moq;
using RecordDB.DAL.Data;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using System.Data;
using Xunit;
using DbRecord = RecordDB.DAL.Models.Record;

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

        // ── SelectAsync(int) — by record id ─────────────────────────────────────

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
        public async Task SelectAsync_ById_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            _dbMock
                .Setup(x => x.GetFirstOrDefault<ArtistRecordDto, dynamic>(
                    "up_RecordSelectByIdCore",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync((ArtistRecordDto?)null);

            // Act
            var result = await _repository.SelectAsync(9999);

            // Assert
            result.Should().BeNull();
        }

        // ── SelectAsync() — all records ─────────────────────────────────────────

        [Fact]
        public async Task SelectAsync_NoParam_ShouldReturnAllRecords()
        {
            // Arrange
            var expected = new List<ArtistRecordDto>
            {
                new ArtistRecordDto { RecordId = 1, Name = "Blonde on Blonde", Recorded = 1966 },
                new ArtistRecordDto { RecordId = 2, Name = "Highway 61 Revisited", Recorded = 1965 }
            };

            _dbMock
                .Setup(x => x.GetData<ArtistRecordDto, dynamic>(
                    "up_RecordSelectAll",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.SelectAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Select(r => r.Name).Should().Contain("Blonde on Blonde");
        }

        // ── Select(string show) ──────────────────────────────────────────────────

        [Fact]
        public async Task Select_ByShow_ShouldReturnRecords_WhenShowIsValid()
        {
            // Arrange
            var expected = new List<DbRecord>
            {
                new DbRecord { RecordId = 5, Name = "Darkness on the Edge of Town", Media = "CD" }
            };

            _dbMock
                .Setup(x => x.GetData<DbRecord, dynamic>(
                    "up_RecordSelectShowCore",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.Select("CD");

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Darkness on the Edge of Town");
        }

        [Fact]
        public async Task Select_ByShow_ShouldThrowArgumentNullException_WhenShowIsNull()
        {
            // Act
            Func<Task> act = async () => await _repository.Select(null!);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        // ── SelectRecordsShowAsync ──────────────────────────────────────────────

        [Fact]
        public async Task SelectRecordsShowAsync_ShouldReturnDtos_WhenShowIsValid()
        {
            // Arrange
            var expected = new List<ArtistRecordDto>
            {
                new ArtistRecordDto { RecordId = 3, Name = "Harvest", Media = "R" }
            };

            _dbMock
                .Setup(x => x.GetData<ArtistRecordDto, dynamic>(
                    "up_RecordSelectShowCore",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.SelectRecordsShowAsync("R");

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Harvest");
        }

        [Fact]
        public async Task SelectRecordsShowAsync_ShouldThrowArgumentNullException_WhenShowIsWhitespace()
        {
            // Act
            Func<Task> act = async () => await _repository.SelectRecordsShowAsync("   ");

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        // ── Select(int recordId) ─────────────────────────────────────────────────

        [Fact]
        public async Task Select_ByRecordId_ShouldReturnRecord_WhenFound()
        {
            // Arrange
            var expected = new DbRecord { RecordId = 20, Name = "Rumors", ArtistId = 7 };

            _dbMock
                .Setup(x => x.GetFirstOrDefault<DbRecord, dynamic>(
                    "up_RecordSelectById",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.Select(20);

            // Assert
            result.Should().NotBeNull();
            result.RecordId.Should().Be(20);
            result.Name.Should().Be("Rumors");
        }

        // ── SelectArtistRecordsAsync ─────────────────────────────────────────────

        [Fact]
        public async Task SelectArtistRecordsAsync_ShouldReturnRecordsForArtist()
        {
            // Arrange
            var expected = new List<DbRecord>
            {
                new DbRecord { RecordId = 1, Name = "John Wesley Harding", ArtistId = 1 },
                new DbRecord { RecordId = 2, Name = "Nashville Skyline",   ArtistId = 1 }
            };

            _dbMock
                .Setup(x => x.GetData<DbRecord, dynamic>(
                    "up_getRecordListAndNone",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.SelectArtistRecordsAsync(1);

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(r => r.ArtistId == 1);
        }

        // ── SelectRecordReviewsAsync ─────────────────────────────────────────────

        [Fact]
        public async Task SelectRecordReviewsAsync_ShouldReturnReviewDtos()
        {
            // Arrange
            var expected = new List<RecordReviewDto>
            {
                new RecordReviewDto { Name = "Bob Dylan",  Title = "Blonde on Blonde",    Review = "Classic." },
                new RecordReviewDto { Name = "Neil Young", Title = "Harvest",              Review = "Beautiful." }
            };

            _dbMock
                .Setup(x => x.GetData<RecordReviewDto, dynamic>(
                    "up_SelectRecordReviewsCore",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.SelectRecordReviewsAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Select(r => r.Name).Should().Contain("Bob Dylan");
        }

        // ── CountDiscsAsync ──────────────────────────────────────────────────────

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

        // ── GetArtistNumberOfRecordsAsync ────────────────────────────────────────

        [Fact]
        public async Task GetArtistNumberOfRecordsAsync_ShouldReturnCountString_ForArtistId()
        {
            // Arrange
            _dbMock
                .Setup(x => x.GetScalar<int, object>(
                    "up_GetArtistNumberOfRecords",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(17);

            // Act
            var result = await _repository.GetArtistNumberOfRecordsAsync(1);

            // Assert
            result.Should().Be("17");
        }

        // ── GetRecordedYearNumberAsync ───────────────────────────────────────────

        [Fact]
        public async Task GetRecordedYearNumberAsync_ShouldReturnCountString_ForGivenYear()
        {
            // Arrange
            _dbMock
                .Setup(x => x.GetScalar<int, dynamic>(
                    "up_GetRecordedYearNumber",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(8);

            // Act
            var result = await _repository.GetRecordedYearNumberAsync(1975);

            // Assert
            result.Should().Be("8");
        }

        // ── GetRecordsByYearAsync ────────────────────────────────────────────────

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

        // ── GetRecordsByArtistNameAsync ──────────────────────────────────────────

        [Fact]
        public async Task GetRecordsByArtistNameAsync_ShouldReturnRecords_WhenArtistMatches()
        {
            // Arrange
            var expected = new List<ArtistRecordDto>
            {
                new ArtistRecordDto { RecordId = 1, Name = "Blonde on Blonde",    ArtistName = "Bob Dylan" },
                new ArtistRecordDto { RecordId = 2, Name = "Blood on the Tracks", ArtistName = "Bob Dylan" }
            };

            _dbMock
                .Setup(x => x.GetData<ArtistRecordDto, dynamic>(
                    "up_GetRecordsByArtistName",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.GetRecordsByArtistNameAsync("Bob Dylan");

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(r => r.ArtistName == "Bob Dylan");
        }

        [Fact]
        public async Task GetRecordsByArtistNameAsync_ShouldReturnEmpty_WhenNameIsWhitespace()
        {
            // Act — no db call expected; early return
            var result = await _repository.GetRecordsByArtistNameAsync("   ");

            // Assert
            result.Should().BeEmpty();
            _dbMock.Verify(x => x.GetData<ArtistRecordDto, dynamic>(
                It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CommandType>()), Times.Never);
        }

        // ── NoRecordReviewsAsync ─────────────────────────────────────────────────

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

        // ── ListRecordsWithNoTracksAsync ─────────────────────────────────────────

        [Fact]
        public async Task ListRecordsWithNoTracksAsync_ShouldReturnRecords_WithNoTracks()
        {
            // Arrange
            var expected = new List<ArtistRecordDiscDto>
            {
                new ArtistRecordDiscDto { RecordId = 8, Name = "Unplugged", ArtistName = "Eric Clapton" }
            };

            _dbMock
                .Setup(x => x.GetData<ArtistRecordDiscDto, dynamic>(
                    "up_GetRecordsWithNoTracks",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.ListRecordsWithNoTracksAsync();

            // Assert
            result.Should().HaveCount(1);
            result.First().ArtistName.Should().Be("Eric Clapton");
        }

        // ── ArtistRecordsWithNoTracksAsync ───────────────────────────────────────

        [Fact]
        public async Task ArtistRecordsWithNoTracksAsync_ShouldReturnRecords_ForArtistName()
        {
            // Arrange
            var expected = new List<ArtistRecordDiscDto>
            {
                new ArtistRecordDiscDto { RecordId = 9, Name = "461 Ocean Boulevard", ArtistName = "Eric Clapton" }
            };

            _dbMock
                .Setup(x => x.GetData<ArtistRecordDiscDto, dynamic>(
                    "up_GetArtistRecordsWithNoTracks",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.ArtistRecordsWithNoTracksAsync("Eric Clapton");

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("461 Ocean Boulevard");
        }

        // ── InsertAsync(Record) ──────────────────────────────────────────────────

        [Fact]
        public async Task InsertAsync_Record_ShouldCallSaveDataReturnId_AndReturnNewId()
        {
            // Arrange
            var newRecord = new DbRecord
            {
                ArtistId  = 1,
                Name      = "Planet Waves",
                Field     = "Folk",
                Recorded  = 1974,
                Label     = "Asylum",
                Pressing  = "US",
                Rating    = "***",
                Discs     = 1,
                Media     = "CD",
                Bought    = new DateTime(2020, 1, 1),
                Cost      = 9.99m,
                CoverName = "planet_waves.jpg",
                Review    = "A solid album."
            };

            _dbMock
                .Setup(x => x.SaveDataReturnId(
                    "adm_RecordInsert",
                    It.IsAny<DynamicParameters>(),
                    "@Result"))
                .ReturnsAsync(101);

            // Act
            var result = await _repository.InsertAsync(newRecord);

            // Assert
            result.Should().Be(101);
            _dbMock.Verify(x => x.SaveDataReturnId("adm_RecordInsert", It.IsAny<DynamicParameters>(), "@Result"), Times.Once);
        }

        // ── GetTotalCostsAsync ───────────────────────────────────────────────────

        [Fact]
        public async Task GetTotalCostsAsync_ShouldReturnTotalsList()
        {
            // Arrange
            var expected = new List<Total>
            {
                new Total { ArtistId = 1, TotalDiscs = 3, TotalCost = 29.97m }
            };

            _dbMock
                .Setup(x => x.GetData<Total, dynamic>(
                    "sp_getTotalsForEachArtist",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.GetTotalCostsAsync();

            // Assert
            result.Should().HaveCount(1);
            result.First().TotalCost.Should().Be(29.97m);
        }

        // ── DeleteAsync ──────────────────────────────────────────────────────────

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

        // ── ToShortDate (static helper) ──────────────────────────────────────────

        [Fact]
        public void ToShortDate_WithValidDateTime_ShouldReturnFormattedString()
        {
            // Arrange
            var date = new DateTime(2022, 6, 15);

            // Act
            var result = RecordRepository.ToShortDate(date);

            // Assert
            result.Should().NotBe("unk");
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void ToShortDate_WithNull_ShouldReturnUnk()
        {
            // Act
            var result = RecordRepository.ToShortDate(null!);

            // Assert
            result.Should().Be("unk");
        }
    }
}
