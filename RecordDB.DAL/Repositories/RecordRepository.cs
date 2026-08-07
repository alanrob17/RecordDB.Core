using Dapper;
using RecordDB.DAL.Data;
using RecordDB.DAL.DTOs;
using RecordDB.DAL.Extensions;
using RecordDB.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecordDB.DAL.Repositories
{
    public class RecordRepository : IRecordRepository
    {
        private readonly IDataAccess _db;

        public RecordRepository(IDataAccess db)
        {
            _db = db;
        }

        public async Task<ArtistRecordDto> SelectAsync(int recordId)
        {
            var sproc = "up_RecordSelectByIdCore";
            var parameter = new DynamicParameters();
            parameter.Add("@RecordId", recordId);

            ArtistRecordDto record = await _db.GetFirstOrDefault<ArtistRecordDto, dynamic>(sproc, parameter);

            return record ?? null;
        }

        public async Task<string> CountDiscsAsync(string show)
        {
            int discs = 0;

            if (show == null)
            {
                throw new ArgumentNullException("show");
            }
            else
            {

                var sproc = "up_CountDiscs";

                var parameter = new DynamicParameters();
                parameter.Add("@Show", show);

                discs = await _db.GetScalar<int, object>(sproc, parameter);

                return discs.ToString(CultureInfo.InvariantCulture);
            }
        }

        public async Task<string> GetArtistNumberOfRecordsAsync(int artistId)
        {
            var discs = 0;
            var sproc = "up_GetArtistNumberOfRecords";
            var parameter = new DynamicParameters();
            parameter.Add("@ArtistId", artistId);

            discs = await _db.GetScalar<int, object>(sproc, parameter);

            return discs.ToString(CultureInfo.InvariantCulture);
        }

        public async Task<List<ArtistRecordDto>> SelectAsync()
        {
            var sproc = "up_RecordSelectAll";
            var records = await _db.GetData<ArtistRecordDto, dynamic>(sproc, new { });

            return records.ToList();
        }

        public async Task<List<Record>> Select(string show)
        {
            if (show == null)
            {
                throw new ArgumentNullException("show");
            }

            var sproc = "up_RecordSelectShowCore";

            var parameter = new DynamicParameters();
            parameter.Add("@Show", show);

            IEnumerable<Record> records = await _db.GetData<Record, dynamic>(sproc, parameter);

            return records.ToList();
        }

        public async Task<List<ArtistRecordDto>> SelectRecordsShowAsync(string show)
        {
            if (string.IsNullOrWhiteSpace(show))
            {
                throw new ArgumentNullException(nameof(show));
            }

            var sproc = "up_RecordSelectShowCore";

            var parameter = new DynamicParameters();
            parameter.Add("@Show", show);

            var records = await _db.GetData<ArtistRecordDto, dynamic>(sproc, parameter);

            return records.ToList();
        }

        public async Task<Record> Select(int recordId)
        {
            var sproc = "up_RecordSelectById";

            var parameter = new DynamicParameters();
            parameter.Add("@RecordId", recordId);

            Record record = await _db.GetFirstOrDefault<Record, dynamic>(sproc, parameter);

            return record;
        }

        public async Task<List<Record>> SelectArtistRecordsAsync(int artistId)
        {
            var sproc = "up_getRecordListAndNone";

            var parameter = new DynamicParameters();
            parameter.Add("@ArtistId", artistId);

            var records = await _db.GetData<Record, dynamic>(sproc, parameter);

            return records.ToList();
        }

        public async Task<List<RecordReviewDto>> SelectRecordReviewsAsync()
        {
            var sproc = "up_SelectRecordReviewsCore";

            var records = await _db.GetData<RecordReviewDto, dynamic>(sproc, new { });

            return records.ToList();
        }

        public async Task<List<Record>> SelectRecordReviews()
        {
            var sproc = "up_SelectRecordReviews2";

            var records = await _db.GetData<Record, dynamic>(sproc, new { });

            return records.ToList();
        }

        public async Task<string> GetRecordedYearNumberAsync(int year)
        {
            var discs = 0;
            var sproc = "up_GetRecordedYearNumber";

            var parameter = new DynamicParameters();
            parameter.Add("@Year", year);

            discs = await _db.GetScalar<int, dynamic>(sproc, parameter);

            return discs.ToString(CultureInfo.InvariantCulture);
        }

        public async Task<List<MissingReviewDto>> NoRecordReviewsAsync()
        {
            var sproc = "up_NoRecordReviews";

            var records = await _db.GetData<MissingReviewDto, dynamic>(sproc, new { });

            return records.ToList();
        }

        public static string ToShortDate(object bought)
        {
            var shortDate = "unk";

            if (bought != null)
            {
                DateTime dt = Convert.ToDateTime(bought);

                shortDate = DateTimeExtensions.ToShortDate(dt);
            }

            return shortDate;
        }

        public async Task<List<Total>> GetTotalCostsAsync()
        {
            var sproc = "sp_getTotalsForEachArtist";

            var totals = await _db.GetData<Total, dynamic>(sproc, new { });

            return totals.ToList();
        }

        public async Task<int> InsertAsync(Record record)
        {
            var recordId = -1; // 0 is used for record is already in the db.
            var sproc = "adm_RecordInsert";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ArtistId", record.ArtistId);
                parameters.Add("@Name", record.Name);
                parameters.Add("@Field", record.Field);
                parameters.Add("@Recorded", record.Recorded);
                parameters.Add("@Label", record.Label);
                parameters.Add("@Pressing", record.Pressing);
                parameters.Add("@Rating", record.Rating);
                parameters.Add("@Discs", record.Discs);
                parameters.Add("@Media", record.Media);
                parameters.Add("@Bought", record.Bought);
                parameters.Add("@Cost", record.Cost);
                parameters.Add("@CoverName", record.CoverName);
                parameters.Add("@Review", record.Review);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                recordId =await _db.SaveDataReturnId(sproc, parameters);

                return recordId;
            }
            catch (Exception ex)
            {
                return recordId;
            }
        }

        public async Task<int> InsertAsync(int artistId, string name, string field, int recorded, string label, string pressing, string rating, int discs, string media, DateTime bought, decimal cost, string coverName, string review)
        {
            var recordId = -1; // 0 is used for record is already in the db.
            var sproc = "adm_RecordInsert";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ArtistId", artistId);
                parameters.Add("@Name", name);
                parameters.Add("@Field", field);
                parameters.Add("@Recorded", recorded);
                parameters.Add("@Label", label);
                parameters.Add("@Pressing", pressing);
                parameters.Add("@Rating", rating);
                parameters.Add("@Discs", discs);
                parameters.Add("@Media", media);
                parameters.Add("@Bought", bought);
                parameters.Add("@Cost", cost);
                parameters.Add("@Review", review);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                recordId = await _db.SaveDataReturnId(sproc, parameters);
                
                return recordId;
            }
            catch (Exception ex)
            {
                return recordId;
            }
        }

        public async Task<int> UpdateAsync(int recordId, int artistId, string name, string field, int recorded, string label, string pressing, string rating, int discs, string media, DateTime bought, decimal cost, string coverName, string review)
        {
            var recId = 0;

            try
            {
                string sproc = "adm_UpdateRecord";
                var parameters = new DynamicParameters();
                parameters.Add("@RecordId", recordId);
                parameters.Add("@ArtistId", artistId);
                parameters.Add("@Name", name);
                parameters.Add("@Field", field);
                parameters.Add("@Recorded", recorded);
                parameters.Add("@Label", label);
                parameters.Add("@Pressing", pressing);
                parameters.Add("@Rating", rating);
                parameters.Add("@Discs", discs);
                parameters.Add("@Media", media);
                parameters.Add("@Bought", bought);
                parameters.Add("@Cost", cost);
                parameters.Add("@Review", review);
                // parameters.Add("@Result", result, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                await _db.SaveData(sproc, parameters);
                recId = parameters.Get<int>("@RecordId");

                return recId;
            }
            catch (Exception ex)
            {
                return recId;
            }
        }

        public async Task<int> UpdateAsync(ArtistRecordDto record)
        {
            var recordId = 0;

            try
            {
                string sproc = "adm_UpdateRecord";
                var parameters = new DynamicParameters();
                parameters.Add("@RecordId", record.RecordId);
                parameters.Add("@ArtistId", record.ArtistId);
                parameters.Add("@Name", record.Name);
                parameters.Add("@Field", record.Field);
                parameters.Add("@Recorded", record.Recorded);
                parameters.Add("@Label", record.Label);
                parameters.Add("@Pressing", record.Pressing);
                parameters.Add("@Rating", record.Rating);
                parameters.Add("@Discs", record.Discs);
                parameters.Add("@Media", record.Media);
                parameters.Add("@Bought", record.Bought);
                parameters.Add("@Cost", record.Cost);
                parameters.Add("@Review", record.Review);

                await _db.SaveData(sproc, parameters);

                recordId = parameters.Get<int>("@RecordId");

                return recordId;

            }
            catch (Exception ex)
            {
                return recordId;
            }
        }

        public async Task DeleteAsync(int recordId)
        {
            var sproc = "up_deleteRecord";

            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@RecordId", recordId);

                await _db.SaveData(sproc, parameter);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public async Task<List<Record>> GetArtistRecordsAsync(int artistId)
        {
            var sproc = "up_getArtistRecords";

            var records = await _db.GetData<Record, Artist, Record>(
                sproc,
                (record, artist) =>
                {
                    record.Artist = artist;
                    record.ArtistId = artist.ArtistId;
                    return record;
                },
                new { ArtistId = artistId },
                splitOn: "ArtistId");

            return records.ToList();
        }

        public async Task<List<ArtistRecordDto>> GetRecordsByArtistNameAsync(string artistName)
        {
            if (string.IsNullOrWhiteSpace(artistName))
            {
                return [];
            }

            var sproc = "up_GetRecordsByArtistName";
            var parameter = new DynamicParameters();
            parameter.Add("@ArtistName", artistName);

            var records = await _db.GetData<ArtistRecordDto, dynamic>(sproc, parameter);

            return records.ToList();
        }

        public async Task<List<ArtistRecordDto>> GetRecordsByYearAsync(int recorded)
        {
            var sproc = "up_GetRecordsByYear";
            var parameter = new DynamicParameters();
            parameter.Add("@Recorded", recorded);

            var records = await _db.GetData<ArtistRecordDto, dynamic>(sproc, parameter);

            return records.ToList();
        }
    }
}
