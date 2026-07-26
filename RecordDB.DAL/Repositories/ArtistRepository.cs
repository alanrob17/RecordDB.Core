using Dapper;
using Microsoft.Data.SqlClient;
using RecordDB.DAL.Data;
using RecordDB.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace RecordDB.DAL.Repositories
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly IDataAccess _db;

        public ArtistRepository(IDataAccess db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Artist>> GetArtistsAsync()
        {
            string sproc = "up_ArtistSelectFull";
            return await _db.GetData<Artist, dynamic>(sproc, new { });
        }

        public async Task<IEnumerable<Artist>> GetArtists()
        {
            string sproc = "up_ArtistSelectFull";
            IEnumerable<Artist> artists = await _db.GetData<Artist, dynamic>(sproc, new { });

            return artists.ToList();
        }

        public async Task<List<Artist>> GetArtistListAsync()
        {
            string sproc = "up_getArtistListandNone";
            IEnumerable<Artist> artists = await _db.GetData<Artist, dynamic>(sproc, new { });

            return artists.ToList();
        }

        public async Task<List<Artist>> SelectAsync()
        {
            string sproc = "up_ArtistSelectAll";
            IEnumerable<Artist> artists = await _db.GetData<Artist, dynamic>(sproc, new { });

            return artists.ToList();
        }

        public async Task<Artist> SelectAsync(int artistId)
        {
            string sproc = "up_ArtistSelectById";
            var parameter = new DynamicParameters();
            parameter.Add("@ArtistId", artistId);

            Artist artist = await _db.GetFirstOrDefault<Artist, dynamic>(sproc, parameter);

                return artist ?? null;
        }

        public async Task<List<Artist>> SelectArtistWithNoBioAsync()
        {
            var sproc = "up_selectArtistsWithNoBio";
            var artists = await _db.GetData<Artist, dynamic>(sproc, new { });

            return artists.ToList();
        }

        public async Task<int> InsertAsync(Artist artist)
        {
            var artistId = -1; // 0 is used for record is already in the db.
            string sproc = "adm_ArtistInsert";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FirstName", artist.FirstName);
                parameters.Add("@LastName", artist.LastName);
                parameters.Add("@Biography", artist.Biography);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                artistId = await _db.SaveDataReturnId(sproc, parameters);
                return artistId;
            }
            catch (Exception ex)
            {
                return artistId;
            }
        }

        public async Task<int> InsertAsync(string firstName, string lastName, string biography)
        {
            int artistId = 0;

            try
            {
                string sproc = "adm_ArtistInsert";
                var parameters = new DynamicParameters();
                parameters.Add("@FirstName", firstName);
                parameters.Add("@LastName", lastName);
                parameters.Add("@Biography", biography);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                artistId = await _db.SaveDataReturnId(sproc, parameters);
                return artistId;
            }
            catch (Exception ex)
            {
                return artistId;
            }
        }

        public async Task<int> UpdateArtistAsync(Artist artist)
        {
            var artistId = -1; // 0 is used for record is already in the db.
            try
            {
                string sproc = "up_UpdateArtist";
                var parameters = new DynamicParameters();
                parameters.Add("@ArtistId", artist.ArtistId);
                parameters.Add("@FirstName", artist.FirstName);
                parameters.Add("@LastName", artist.LastName);
                parameters.Add("@Name", artist.Name);
                parameters.Add("@Biography", artist.Biography);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                artistId = await _db.SaveDataReturnId(sproc, parameters);
                return artistId;
            }
            catch (Exception ex)
            {
                return artistId;
            }
        }

        public async Task<int> UpdateAsync(int artistId, string firstName, string lastName, string name, string biography)
        {
            try
            {
                string sproc = "up_UpdateArtist";
                var parameters = new DynamicParameters();
                parameters.Add("@ArtistId", artistId);
                parameters.Add("@FirstName", firstName);
                parameters.Add("@LastName", lastName);
                parameters.Add("@Name", name);
                parameters.Add("@Biography", biography);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                artistId = await _db.SaveDataReturnId(sproc, parameters);
                return artistId;
            }
            catch (Exception ex)
            {
                return artistId;
            }
        }

        public async Task<int> GetArtistIdAsync(string firstName, string lastName)
        {
            var sproc = "up_getArtistID";
            var parameters = new { FirstName = firstName, LastName = lastName };

            int? artistId = await _db.GetScalar<int, object>(sproc, parameters);
            return artistId ?? 0;
        }

        public async Task<int> GetArtistIdAsync(int recordId)
        {
            string sproc = "up_getArtistIdFromRecord";
            var parameters = new DynamicParameters();
            parameters.Add("@RecordId", recordId);
            parameters.Add("@ArtistId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            int artistId = await _db.SaveDataReturnId(sproc, parameters, "@ArtistId");

            return artistId;
        }

        public async Task DeleteAsync(int artistId)
        {
            try
            {
                string sproc = "up_deleteArtist";
                var parameter = new DynamicParameters();
                parameter.Add("@ArtistId", artistId);

                await _db.SaveData(sproc, parameter);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public async Task<Artist> GetArtistByRecordIdAsync(int recordId)
        {
            var sproc = "up_ArtistSelectByRecordId";
            var parameter = new DynamicParameters();
            parameter.Add("@RecordId", recordId);

            Artist artist = await _db.GetFirstOrDefault<Artist, dynamic>(sproc, parameter);

            return artist ?? null;
        }

        public async Task<string> GetBiographyAsync(int recordId)
        {
            var sproc = "up_getBiography";

            var parameter = new DynamicParameters();
            parameter.Add("@RecordId", recordId);

            string biography = await _db.GetScalar<string, dynamic>(sproc, parameter) ?? string.Empty;

            return biography;
        }

        public async Task<string> GetBiography(int recordId)
        {
            var sproc = "up_getBiography";

            var parameter = new DynamicParameters();
            parameter.Add("@RecordId", recordId);

            string biography = await _db.GetScalar<string, dynamic>(sproc, parameter) ?? string.Empty;

            return biography;
        }
    }
}
