using Microsoft.Extensions.Logging;
using Muffle.Data.Services;

namespace Muffle
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                //.UseMauiCommunityToolkit() // see https://github.com/CommunityToolkit/Maui/issues/501
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            //var connectionStringNameSqlServer = "SqlServerConnection";
            //var connectionStringNameSqlLite = "SqliteConnection";

            //var connectionStringSqlServer = ConfigurationLoader.GetConnectionString(connectionStringNameSqlServer);
            //var connectionStringSqlLite = ConfigurationLoader.GetConnectionString(connectionStringNameSqlLite);

            //builder.Services.AddDbContext<SqlServerDbContext>(options =>
            //{
            //    options.UseSqlite(connectionStringSqlServer);
            //});

            //builder.Services.AddDbContext<SqlLiteDbContext>(options =>
            //{
            //    options.UseSqlite(connectionStringSqlLite);
            //});


#if DEBUG
            SqlServerDbService.DisposeDatabase();
            SQLiteDbService.DisposeDatabase();
            SqlServerDbService.InitializeDatabase();
            SQLiteDbService.InitializeDatabase();
            builder.Logging.AddDebug();
#else
            SqliteDbService.InitializeDatabase();
#endif

            return builder.Build();
            
        }
    }
}
