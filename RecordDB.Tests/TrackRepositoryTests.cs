using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using RecordDB.DAL.Data;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RecordDB.Tests
{
    public class TrackRepositoryTests
    {
        private readonly Mock<IDataAccess> _dbMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly TrackRepository _repository;

        public TrackRepositoryTests()
        {
            _dbMock = new Mock<IDataAccess>();
            _configMock = new Mock<IConfiguration>();
            _repository = new TrackRepository(_dbMock.Object, _configMock.Object);
        }

        // ── SelectAllTrackEntitiesAsync ─────────────────────────────────────────

        [Fact]
        public async Task SelectAllTrackEntitiesAsync_ShouldReturnAllTracks_UsingAdminSproc()
        {
            // Arrange
            var expected = new List<ArtistRecordDiscTrackDto>
            {
                new ArtistRecordDiscTrackDto { TrackId = 1, TrackName = "Like a Rolling Stone", ArtistName = "Bob Dylan" },
                new ArtistRecordDiscTrackDto { TrackId = 2, TrackName = "Blowin' in the Wind",  ArtistName = "Bob Dylan" }
            };

            _dbMock
                .Setup(x => x.GetData<ArtistRecordDiscTrackDto, dynamic>(
                    "adm_SelectAllTracks",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.SelectAllTrackEntitiesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().TrackName.Should().Be("Like a Rolling Stone");
        }

        [Fact]
        public async Task SelectAllTrackEntitiesAsync_ShouldReturnEmpty_WhenNoTracksExist()
        {
            // Arrange
            _dbMock
                .Setup(x => x.GetData<ArtistRecordDiscTrackDto, dynamic>(
                    "adm_SelectAllTracks",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(new List<ArtistRecordDiscTrackDto>());

            // Act
            var result = await _repository.SelectAllTrackEntitiesAsync();

            // Assert
            result.Should().BeEmpty();
        }

        // ── SelectArtistRecordTracksAsync ───────────────────────────────────────

        [Fact]
        public async Task SelectArtistRecordTracksAsync_ShouldReturnTracks_ForGivenRecordName()
        {
            // Arrange
            var expected = new List<ArtistRecordDiscTrackDto>
            {
                new ArtistRecordDiscTrackDto
                {
                    ArtistName = "Bob Dylan",
                    Name       = "Blonde on Blonde",
                    TrackId    = 10,
                    TrackName  = "Rainy Day Women #12 & 35",
                    TrackNo    = 1
                }
            };

            _dbMock
                .Setup(x => x.GetData<ArtistRecordDiscTrackDto, dynamic>(
                    "up_SelectRecordTracks",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.SelectArtistRecordTracksAsync("Blonde on Blonde");

            // Assert
            result.Should().HaveCount(1);
            result.First().TrackName.Should().Be("Rainy Day Women #12 & 35");
        }

        // ── SelectTracksByRecordAsync ────────────────────────────────────────────

        [Fact]
        public async Task SelectTracksByRecordAsync_ShouldReturnTracks_UsingCorrectSproc()
        {
            // Arrange
            var expected = new List<ArtistRecordDiscTrackDto>
            {
                new ArtistRecordDiscTrackDto { TrackId = 20, TrackName = "Hey Jude",     TrackNo = 1 },
                new ArtistRecordDiscTrackDto { TrackId = 21, TrackName = "Revolution",   TrackNo = 2 }
            };

            _dbMock
                .Setup(x => x.GetData<ArtistRecordDiscTrackDto, dynamic>(
                    "up_GetArtistRecordTracks",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.SelectTracksByRecordAsync("Hey Jude");

            // Assert
            result.Should().HaveCount(2);
            result.Select(t => t.TrackName).Should().Contain("Hey Jude");
        }

        // ── SelectTracksByPartialNameAsync ──────────────────────────────────────

        [Fact]
        public async Task SelectTracksByPartialNameAsync_ShouldReturnTracks_WhenMatchingTracksExist()
        {
            // Arrange
            string partialTrackName = "Johanna";
            var expectedTracks = new List<ArtistRecordDiscTrackDto>
            {
                new ArtistRecordDiscTrackDto
                {
                    ArtistName  = "Bob Dylan",
                    RecordId    = 10,
                    Name        = "Blonde on Blonde",
                    DiscId      = 100,
                    DiscNo      = 1,
                    TrackId     = 1001,
                    TrackNo     = 3,
                    TrackName   = "Visions Of Johanna",
                    TrackLength = 450
                }
            };

            _dbMock
                .Setup(x => x.GetData<ArtistRecordDiscTrackDto, dynamic>(
                    "up_SelectPartialRecordTracks",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(expectedTracks);

            // Act
            var result = await _repository.SelectTracksByPartialNameAsync(partialTrackName);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            var track = result.First();
            track.TrackName.Should().Be("Visions Of Johanna");
            track.ArtistName.Should().Be("Bob Dylan");
            track.Name.Should().Be("Blonde on Blonde");
            track.TrackNo.Should().Be(3);
        }

        [Fact]
        public async Task SelectTracksByPartialNameAsync_ShouldReturnEmptyList_WhenNoTracksMatch()
        {
            // Arrange
            _dbMock
                .Setup(x => x.GetData<ArtistRecordDiscTrackDto, dynamic>(
                    "up_SelectPartialRecordTracks",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(new List<ArtistRecordDiscTrackDto>());

            // Act
            var result = await _repository.SelectTracksByPartialNameAsync("NonExistentTrack");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        // ── GetTrackNumberAsync ─────────────────────────────────────────────────

        [Fact]
        public async Task GetTrackNumberAsync_ShouldReturnTrackCount_ForGivenRecordId()
        {
            // Arrange
            _dbMock
                .Setup(x => x.GetData<int, dynamic>(
                    "up_GetNumberOfTracks",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(new List<int> { 14 });

            // Act
            var result = await _repository.GetTrackNumberAsync(10);

            // Assert
            result.Should().Be(14);
        }

        [Fact]
        public async Task GetTrackNumberAsync_ShouldReturnZero_WhenNoTracksFound()
        {
            // Arrange
            _dbMock
                .Setup(x => x.GetData<int, dynamic>(
                    "up_GetNumberOfTracks",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(new List<int>());   // empty → FirstOrDefault returns 0

            // Act
            var result = await _repository.GetTrackNumberAsync(9999);

            // Assert
            result.Should().Be(0);
        }

        // ── InsertTrackAsync ────────────────────────────────────────────────────

        [Fact]
        public async Task InsertTrackAsync_ShouldCallSaveDataReturnId_AndReturnNewTrackId()
        {
            // Arrange
            var newTrack = new Track
            {
                DiscId      = 5,
                TrackNo     = 1,
                Name        = "Like a Rolling Stone",
                TrackLength = 369,
                Extended    = null
            };

            _dbMock
                .Setup(x => x.SaveDataReturnId(
                    "up_InsertTrack",
                    It.IsAny<DynamicParameters>(),
                    "@Result"))
                .ReturnsAsync(201);

            // Act
            var result = await _repository.InsertTrackAsync(newTrack);

            // Assert
            result.Should().Be(201);
            _dbMock.Verify(x => x.SaveDataReturnId("up_InsertTrack", It.IsAny<DynamicParameters>(), "@Result"), Times.Once);
        }

        // ── UpdateTrackAsync ────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateTrackAsync_ShouldCallSaveDataReturnId_AndReturnTrackId()
        {
            // Arrange
            var track = new Track
            {
                TrackId     = 201,
                TrackNo     = 1,
                Name        = "Like a Rolling Stone (Edit)",
                TrackLength = 340,
                Extended    = null
            };

            _dbMock
                .Setup(x => x.SaveDataReturnId(
                    "up_UpdateTrack",
                    It.IsAny<DynamicParameters>(),
                    "@Result"))
                .ReturnsAsync(201);

            // Act
            var result = await _repository.UpdateTrackAsync(track);

            // Assert
            result.Should().Be(201);
            _dbMock.Verify(x => x.SaveDataReturnId("up_UpdateTrack", It.IsAny<DynamicParameters>(), "@Result"), Times.Once);
        }

        // ── DeleteTrackAsync ────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteTrackAsync_ShouldCallSaveData_WithTrackId()
        {
            // Arrange
            int trackIdToDelete = 201;

            _dbMock
                .Setup(x => x.SaveData(
                    "up_DeleteTrack",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .Returns(Task.CompletedTask);

            // Act
            await _repository.DeleteTrackAsync(trackIdToDelete);

            // Assert
            _dbMock.Verify(x => x.SaveData(
                "up_DeleteTrack",
                It.IsAny<object>(),
                It.IsAny<CommandType>()), Times.Once);
        }

        // ── SelectTrackByIdAsync ────────────────────────────────────────────────

        [Fact]
        public async Task SelectTrackByIdAsync_ShouldReturnTrack_WhenFound()
        {
            // Arrange
            var expected = new ArtistRecordDiscTrackDto
            {
                TrackId   = 300,
                TrackName = "Stairway to Heaven",
                TrackNo   = 8,
                DiscId    = 15,
                DiscNo    = 1
            };

            _dbMock
                .Setup(x => x.GetData<ArtistRecordDiscTrackDto, dynamic>(
                    "up_SelectSingleTrack",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(new List<ArtistRecordDiscTrackDto> { expected });

            // Act
            var result = await _repository.SelectTrackByIdAsync(300);

            // Assert
            result.Should().NotBeNull();
            result.TrackId.Should().Be(300);
            result.TrackName.Should().Be("Stairway to Heaven");
        }

        [Fact]
        public async Task SelectTrackByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            _dbMock
                .Setup(x => x.GetData<ArtistRecordDiscTrackDto, dynamic>(
                    "up_SelectSingleTrack",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(new List<ArtistRecordDiscTrackDto>());   // empty → FirstOrDefault returns null

            // Act
            var result = await _repository.SelectTrackByIdAsync(9999);

            // Assert
            result.Should().BeNull();
        }

        // ── BulkInsertTracksAsync ───────────────────────────────────────────────

        [Fact]
        public async Task BulkInsertTracksAsync_ShouldCallSaveData_WithCorrectSproc()
        {
            // Arrange
            var tracks = new List<Track>
            {
                new Track { DiscId = 5, TrackNo = 1, Name = "Track 1", TrackLength = 200 },
                new Track { DiscId = 5, TrackNo = 2, Name = "Track 2", TrackLength = 220 }
            };

            _dbMock
                .Setup(x => x.SaveData(
                    "up_InsertTracks",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .Returns(Task.CompletedTask);

            // Act
            await _repository.BulkInsertTracksAsync(tracks);

            // Assert
            _dbMock.Verify(x => x.SaveData(
                "up_InsertTracks",
                It.IsAny<object>(),
                It.IsAny<CommandType>()), Times.Once);
        }
    }
}
