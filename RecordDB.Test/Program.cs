using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecordDB.DAL.Data;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using RecordDB.Test.Services;
using System.Data;

namespace RecordDB.Test
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<IDataAccess, DataAccess>();
            services.AddTransient<ArtistRepository>();
            //services.AddTransient<RecordRepository>();
            //services.AddTransient<CreateArtistService>();
            //services.AddTransient<CreateRecordService>();
            services.AddTransient<ArtistService>();
            //services.AddTransient<RecordService>();

            var serviceProvider = services.BuildServiceProvider();

            //var artistRepository = serviceProvider.GetRequiredService<ArtistRepository>();
            // var recordRepository = serviceProvider.GetRequiredService<RecordRepository>();

            //// Create new records
            //var createArtistService = serviceProvider.GetRequiredService<CreateArtistService>();
            //var artistId = await createArtistService.RunAsync();

            //var createRecordService = serviceProvider.GetRequiredService<CreateRecordService>();
            //await createRecordService.RunAsync(artistId);

            // Search routines
            var artistService = serviceProvider.GetRequiredService<ArtistService>();
            await artistService.RunAsync();

            //var recordService = serviceProvider.GetRequiredService<RecordService>();
            //await recordService.RunAsync();
        }

        // --------------------------------------------------------------------
        // Test: retrieve all artists via a stored procedure
        // --------------------------------------------------------------------
        private static async Task TestGetAllArtists(IDataAccess dataAccess)
        {
            Console.WriteLine("=== TestGetAllArtists ===");
            try
            {
                var artists = await dataAccess.GetData<Artist, object>(
                    "up_Artist_Select",
                    parameters: new { });

                foreach (var artist in artists)
                    Console.WriteLine(artist);

                Console.WriteLine($"Total artists returned: {artists.Count()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FAIL] {ex.Message}");
            }
            Console.WriteLine();
        }

        // --------------------------------------------------------------------
        // Test: retrieve a single artist by ID
        // --------------------------------------------------------------------
        private static async Task TestGetArtistById(IDataAccess dataAccess, int artistId)
        {
            Console.WriteLine($"=== TestGetArtistById (ArtistId={artistId}) ===");
            try
            {
                var artist = await dataAccess.GetFirstOrDefault<Artist, object>(
                    "up_Artist_Select_ById",
                    parameters: new { ArtistId = artistId });

                if (artist is null)
                    Console.WriteLine("No artist found.");
                else
                    Console.WriteLine(artist);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FAIL] {ex.Message}");
            }
            Console.WriteLine();
        }

        // --------------------------------------------------------------------
        // Test: scalar — get total artist count
        // --------------------------------------------------------------------
        private static async Task TestGetArtistCount(IDataAccess dataAccess)
        {
            Console.WriteLine("=== TestGetArtistCount ===");
            try
            {
                var count = await dataAccess.GetScalar<int, object>(
                    "SELECT COUNT(*) FROM Artist",
                    parameters: new { },
                    commandType: CommandType.Text);

                Console.WriteLine($"Artist count: {count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FAIL] {ex.Message}");
            }
            Console.WriteLine();
        }
    }
}
