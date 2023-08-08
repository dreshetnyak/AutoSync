using System;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace AutoSyncTool.Models
{
    internal static class Configuration
    {
        public enum PeriodicityType { SpecificTime, Interval }

        private static IConfiguration Config { get; }

        internal static class App
        {
            public static bool StartMinimizedToTray
            {
                get => bool.TryParse(Config["App:StartMinimizedToTray"], out var startMinimizedToTray) && startMinimizedToTray;
                set => Config["App:StartMinimizedToTray"] = value.ToString();
            }

            public static bool StartWithWindows
            {
                get => bool.TryParse(Config["App:StartWithWindows"], out var startWithWindows) && startWithWindows;
                set => Config["App:StartWithWindows"] = value.ToString();
            }
        }

        internal static class Scan
        {
            public static PeriodicityType Type
            {
                get => Enum.TryParse<PeriodicityType>(Config["Scan:Type"], false, out var type) ? type : default;
                set => Config["Scan:Type"] = value.ToString();
            }

            public static TimeOnly Time
            {
                get => TimeOnly.TryParseExact(Config["Scan:Time"], "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var time) ? time : default;
                set => Config["Scan:Time"] = value.ToString("HH:mm:ss");
            }

            public static TimeSpan Frequency
            {
                get => TimeSpan.TryParseExact(Config["Scan:Frequency"], "g", CultureInfo.InvariantCulture, TimeSpanStyles.None, out var frequency) ? frequency : default;
                set => Config["Scan:Frequency"] = value.ToString("g");
            }
        }

        static Configuration()
        {
            Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }
}
