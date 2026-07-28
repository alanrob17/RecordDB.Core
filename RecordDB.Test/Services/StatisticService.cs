using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RecordDB.Test.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IStatisticRepository _statisticRepository;

        public StatisticService(IStatisticRepository statisticRepository)
        {
            _statisticRepository = statisticRepository;
        }

        public async Task GetStatisticsAsync()
        {
            var statistics = await _statisticRepository.GetStatisticsAsync();

            var disks2022 = statistics.Disks2022.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine($"Disks 2022: {disks2022}");
            var cost2022 = statistics.Cost2022.ToString("C");
            Console.WriteLine($"Cost 2022: {cost2022}");
            var av2022 = statistics.Av2022.ToString("C");
            Console.WriteLine($"Average 2022: {av2022}");
            var disks2021 = statistics.Disks2021.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine($"Disks 2021: {disks2021}");
            var cost2021 = statistics.Cost2021.ToString("C");
            Console.WriteLine($"Cost 2021: {cost2021}");
            var av2021 = statistics.Av2021.ToString("C");
            Console.WriteLine($"Average 2021: {av2021}");
            var cost2020 = statistics.Cost2020.ToString("C");
            Console.WriteLine($"Cost 2020: {cost2020}");
            var av2020 = statistics.Av2020.ToString("C");
            Console.WriteLine($"Average 2020: {av2020}");
            var disks2019 = statistics.Disks2019.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine($"Disks 2019: {disks2019}");
            var cost2019 = statistics.Cost2019.ToString("C");
            Console.WriteLine($"Cost 2019: {cost2019}");
            var av2019 = statistics.Av2019.ToString("C");
            Console.WriteLine($"Average 2019: {av2019}");
            var disks2018 = statistics.Disks2018.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine($"Disks 2018: {disks2018}");
            var cost2018 = statistics.Cost2018.ToString("C");
            Console.WriteLine($"Cost 2018: {cost2018}");
            var av2018 = statistics.Av2018.ToString("C");
            Console.WriteLine($"Average 2018: {av2018}");
            var disks2017 = statistics.Disks2017.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine($"Disks 2017: {disks2017}");
            var cost2017 = statistics.Cost2017.ToString("C");
            Console.WriteLine($"Cost 2017: {cost2017}");
            var av2017 = statistics.Av2017.ToString("C");
            Console.WriteLine($"Average 2017: {av2017}");
            var totalCDs = statistics.TotalCDs.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine($"Total CDs: {totalCDs}");
            var cDCost = statistics.CDCost.ToString("C");
            Console.WriteLine($"CD Cost: {cDCost}");
            var avCDCost = statistics.AvCDCost.ToString("C");
            Console.WriteLine($"Average CD Cost: {avCDCost}");
            var totalRecords = statistics.TotalRecords.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine($"Total Records: {totalRecords}");
            var recordCost = statistics.RecordCost.ToString("C");
            Console.WriteLine($"Record Cost: {recordCost}");
            var totalCost = statistics.TotalCost.ToString("C");
            Console.WriteLine($"Total Cost: {totalCost}");
            var rockDisks = statistics.RockDisks.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine($"Rock Disks: {rockDisks}");
            var folkDisks = statistics.FolkDisks.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine($"Folk Disks: {folkDisks}");
            var acousticDisks = statistics.AcousticDisks.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine($"Acoustic Disks: {acousticDisks}");
            var jazzDisks = statistics.JazzDisks.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine($"Jazz Disks: {jazzDisks}");
            var bluesDisks = statistics.BluesDisks.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine($"Blues Disks: {bluesDisks}");
            var countryDisks = statistics.CountryDisks.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine($"Country Disks: {countryDisks}");
            var classicalDisks = statistics.ClassicalDisks.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine($"Classical Disks: {classicalDisks}");
            var soundtrackDisks = statistics.SoundtrackDisks.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine($"Soundtrack Disks: {soundtrackDisks}");
            var fourStarDisks = statistics.FourStarDisks.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine($"Four-Star Disks: {fourStarDisks}");
            var threeStarDisks = statistics.ThreeStarDisks.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine($"Three-Star Disks: {threeStarDisks}");
            var twoStarDisks = statistics.TwoStarDisks.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine($"Two-Star Disks: {twoStarDisks}");
            var oneStarDisks = statistics.OneStarDisks.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine($"One-Star Disks: {oneStarDisks}");
        }
    }
}
