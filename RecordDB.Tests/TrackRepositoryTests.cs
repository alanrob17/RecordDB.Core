using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using RecordDB.DAL.Data;
using RecordDB.DAL.DTOs;
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

        [Fact]
        public async Task SelectTracksByPartialNameAsync_ShouldReturnTracks_WhenMatchingTracksExist()
        {
            // Arrange
            string partialTrackName = "Johanna";
            var expectedTracks = new List<ArtistRecordDiscTrackDto>
            {
                new ArtistRecordDiscTrackDto
                {
                    ArtistName = "Bob Dylan",
                    RecordId = 10,
                    Name = "Blonde on Blonde",
                    DiscId = 100,
                    DiscNo = 1,
                    TrackId = 1001,
                    TrackNo = 3,
                    TrackName = "Visions Of Johanna",
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
            string partialTrackName = "NonExistentTrack";

            _dbMock
                .Setup(x => x.GetData<ArtistRecordDiscTrackDto, dynamic>(
                    "up_SelectPartialRecordTracks",
                    It.IsAny<DynamicParameters>(),
                    CommandType.StoredProcedure))
                .ReturnsAsync(new List<ArtistRecordDiscTrackDto>());

            // Act
            var result = await _repository.SelectTracksByPartialNameAsync(partialTrackName);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}
