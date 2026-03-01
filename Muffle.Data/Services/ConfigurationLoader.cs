using System.Reflection;
using System.Text.Json;

namespace Muffle.Data.Services
{
    public static class ConfigurationLoader
    {
        private static string GetConfigPath()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            string assemblyLocation = Path.GetDirectoryName(assembly.Location);
            string projectDirectory = Path.GetDirectoryName(Path.GetDirectoryName(assemblyLocation));
            string csProjDir = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(projectDirectory)));
            return Path.Combine(csProjDir, "appsettings.json");
        }

        public static string GetConnectionString(string name)
        {
            var configPath = GetConfigPath();

            var jsonString = File.ReadAllText(configPath);
            var jsonDoc = JsonDocument.Parse(jsonString);
            var root = jsonDoc.RootElement;

            if (root.TryGetProperty("ConnectionStrings", out var connectionStrings) &&
                connectionStrings.TryGetProperty(name, out var connectionString))
            {
                return connectionString.GetString();
            }

            throw new KeyNotFoundException($"Connection string '{name}' not found.");
        }

        public static SmtpSettings GetSmtpSettings()
        {
            var configPath = GetConfigPath();

            var jsonString = File.ReadAllText(configPath);
            var jsonDoc = JsonDocument.Parse(jsonString);
            var root = jsonDoc.RootElement;

            if (!root.TryGetProperty("Smtp", out var smtp))
                throw new KeyNotFoundException("Smtp configuration section not found in appsettings.json.");

            return new SmtpSettings
            {
                Host = smtp.GetProperty("Host").GetString() ?? "smtp.gmail.com",
                Port = smtp.GetProperty("Port").GetInt32(),
                SenderEmail = smtp.GetProperty("SenderEmail").GetString() ?? string.Empty,
                SenderName = smtp.GetProperty("SenderName").GetString() ?? "Muffle",
                AppPassword = smtp.GetProperty("AppPassword").GetString() ?? string.Empty
            };
        }
    }

    public class SmtpSettings
    {
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = "Muffle";
        public string AppPassword { get; set; } = string.Empty;
    }
}
