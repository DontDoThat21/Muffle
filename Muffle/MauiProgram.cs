using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Muffle.Data.Data;
using System.Reflection;
using WebRTCme;

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

            var connectionStringNameSqlServer = "SqlServerConnection";
            var connectionStringNameSqlLite = "SqliteConnection";

            var connectionStringSqlServer = ConfigurationLoader.GetConnectionString(connectionStringNameSqlServer);
            var connectionStringSqlLite = ConfigurationLoader.GetConnectionString(connectionStringNameSqlLite);

            builder.Services.AddDbContext<SqlServerDbContext>(options =>
            {
                options.UseSqlite(connectionStringSqlServer);
            });

            builder.Services.AddDbContext<SqlLiteDbContext>(options =>
            {
                options.UseSqlite(connectionStringNameSqlLite);
            });            

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
