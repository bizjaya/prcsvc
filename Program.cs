using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PRCSVC
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    string mysql = "server=localhost;port=3306;user=root;database=ethscan;Allow User Variables=true;charset=utf8mb4;";


                    services.AddDbContext<TokenDbCtx>(options => options.UseMySql(mysql, ServerVersion.AutoDetect(mysql)));

                    services.AddHostedService<TokenPriceUpdater>();
                    services.AddHttpClient();
                });
    }

    public class TokenPriceUpdater : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public TokenPriceUpdater(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(UpdateTokenPrices, null, TimeSpan.Zero, TimeSpan.FromHours(1));
            return Task.CompletedTask;
        }

        private async void UpdateTokenPrices(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TokenDbCtx>();
                var tokens = context.Tokens.ToList();
                var semaphore = new SemaphoreSlim(3);
                var tasks = new List<Task>();

                foreach (var token in tokens)
                {
                    await semaphore.WaitAsync();

                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            var price = await GetTokenPriceAsync(token.Symbol);
                            token.Price = price;
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });

                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);

                var updateCommands = tokens.Select(token =>
                    $"UPDATE tokens SET Price={token.Price} WHERE Symbol='{token.Symbol}';");

                var concatenatedCommand = string.Join(Environment.NewLine, updateCommands);
                await context.Database.ExecuteSqlRawAsync(concatenatedCommand);
            }
        }

        private static async Task<decimal> GetTokenPriceAsync(string symbol)
        {
            var apiKey = "your_api_key"; // Replace with your CryptoCompare API key
            var url = $"https://min-api.cryptocompare.com/data/price?fsym={symbol}&tsyms=USD&api_key={apiKey}";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync(url);
                var priceData = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(response);
                return priceData["USD"];
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
