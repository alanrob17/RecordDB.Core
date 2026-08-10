using RecordDB.DAL.Data;
using RecordDB.DAL.Repositories;
using Serilog;

namespace RecordDB.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Bootstrap logger for startup errors — replaced by full config after builder is created
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                Log.Information("Starting RecordDB.Core web application");

                var builder = WebApplication.CreateBuilder(args);

                // Configure Serilog from appsettings.json
                builder.Host.UseSerilog((context, services, configuration) =>
                    configuration.ReadFrom.Configuration(context.Configuration));

                // Add services to the container.
                builder.Services.AddRazorPages();

                // Register data access and repositories
                builder.Services.AddScoped<IDataAccess, DataAccess>();
                builder.Services.AddScoped<IArtistRepository, ArtistRepository>();
                builder.Services.AddScoped<IRecordRepository, RecordRepository>();
                builder.Services.AddScoped<IDiscRepository, DiscRepository>();
                builder.Services.AddScoped<IStatisticRepository, StatisticRepository>();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();

                // Log every HTTP request with duration
                // app.UseSerilogRequestLogging();

                app.UseRouting();

                app.UseAuthorization();

                app.MapStaticAssets();
                app.MapRazorPages()
                   .WithStaticAssets();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "RecordDB.Core terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
