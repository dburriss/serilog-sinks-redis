using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using Serilog.Sinks.Redis.List;

namespace CoreConsole
{
    public class Program
    {
        private static string host;
        private static string port;

        public static void Main(string[] args)
        {
            Settings(args);

            Log.Logger = new LoggerConfiguration()
            .WriteTo.RedisList($"{host}:{port}", "ConsoleWithLogging")
            .CreateLogger();

            var now = DateTime.UtcNow;
            var version = GetVersion();
            Log.Information("App started at {now}. Version: {@version}", now, version);

            Menu();
            Input();
        }

        private static void Settings(string[] args)
        {
            if (args.Length >= 1)
                host = args[0];

            if (string.IsNullOrEmpty(host))
            {
                Console.Write("Host? (exclude port) ");
                host = Console.ReadLine();
            }

            if (args.Length >= 2)
                port = args[1];

            if (string.IsNullOrEmpty(port))
            {
                Console.Write("Port number? ");
                port = Console.ReadLine();
            }
        }

        private static void Input()
        {
            Console.WriteLine("Log something:");
            var input = Console.ReadLine();
            var processed = false;
            if (input == "-q")
            {
                Quit();
                processed = true;
            }

            if (input == "-e")
            {
                LogException();
                processed = true;
            }

            if (input == "-w")
            {
                LogWarning();
                processed = true;
            }

            if (input == "-v")
            {
                Version();
                processed = true;
            }

            if (input == "-t")
            {
                LoadTest();
                processed = true;
            }

            if (!processed)
            {
                var now = DateTime.UtcNow;
                Log.Information("At {@now} : {@input}", now, input);
                
            }
            Input();
        }

        private static void LoadTest()
        {
            Console.Write("Number of clients? ");
            int nrClients = int.Parse(Console.ReadLine());

            Console.Write("Logs per client?" );
            int nrLogs = int.Parse(Console.ReadLine());

            List<Task> tasks = new List<Task>();
            //TODO: this is shit but I have 5 minutes to write this :)
            for (int i = 0; i < nrClients; i++)
            {
                tasks.Add(SendLogs(i, nrLogs));
            }
            Console.WriteLine("Running load test...");
            Task.WhenAll(tasks);
            Console.WriteLine("Load test complete.");
        }

        private static Task SendLogs(int clientId, int nrLogs)
        {
            return Task.Run(() =>
            {
                var client = clientId;
                var log = new LoggerConfiguration()
                    .WriteTo.RedisList($"{host}:{port}", "ConsoleWithLogging")
                    .CreateLogger();

                for (int j = 0; j < nrLogs; j++)
                {
                    var now = DateTime.UtcNow;
                    log.Information("At {@now} : client {client} // log # {j}", now, client, j);
                }
                Console.WriteLine();
            });
        }

        private static void LogWarning()
        {
            var now = DateTime.UtcNow;
            Log.Warning("At {@now} : {@input}", now, "Some warning");
        }

        private static void LogException()
        {
            try
            {
                throw new InvalidOperationException("An imaginary error occured");
            }
            catch (Exception ex)
            {
                Log.ForContext<Program>();
                Log.Error(ex, "Something went wrong in {App}", "Console");
            }
        }

        private static void Quit()
        {
            Console.WriteLine("Quiting");
            Environment.Exit(0);
        }

        private static void Version()
        {
            var version = GetVersion();
            Log.Information("Version: {@version}", version);
        }

        private static string GetVersion()
        {
            var version = "1.0.0";
            return version;
        }

        private static void Menu()
        {
            Console.WriteLine("'-q' to quit");
            Console.WriteLine("'-e' to throw exception");
            Console.WriteLine("'-w' to issue warning");
            Console.WriteLine("'-v' to send version information");
            Console.WriteLine("'-t' to run load testing");
            Console.WriteLine("LOG SOME INFORMATION");
        }
    }
}
