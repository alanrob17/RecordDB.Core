using System;
using System.Collections.Generic;
using System.Text;

namespace RecordDB.Test.Services
{
    public interface IDiscService
    {
        Task SelectDiscsAsync();
        Task SelectDiscsWithoutLengthsAsync();
        Task GetDiscRecordsByRecordNameAsync(string recordName);
        Task SelectSingleDiscAsync(int discId);
        Task UpdateDiscAsync();
        Task DeleteDiscAsync();
    }
}
