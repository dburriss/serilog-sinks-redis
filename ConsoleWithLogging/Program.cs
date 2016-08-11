using System;
using System.Net.Mime;
using Serilog;
using Serilog.Sinks.Redis;

namespace ConsoleWithLogging
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Redis("localhost:6379", "ConsoleWithLogging")
            .CreateLogger();

            var now = DateTime.UtcNow;
            var version = Environment.Version;
            Log.Information("App started at {now}. Version: {@version}", now, version);

            Menu();
            Input();
        }

        private static void Input()
        {
            Console.WriteLine("Log something:");
            var input = Console.ReadLine();

            if (input == "q")
                Quit();

            if (input == "v")
                Version();

            var now = DateTime.UtcNow;
            Log.Information("At {@now} : {@input}",now, input);
            Input();
        }

        private static void Quit()
        {
            Console.WriteLine("Quiting");
            Environment.Exit(0);
        }

        private static void Version()
        {
            var version = Environment.Version;
            Log.Information("Version: {@version}", version);
        }

        private static void Menu()
        {
            Console.WriteLine("'q' to quit");
            Console.WriteLine("LOG SOME INFORMATION");
        }
    }
}
