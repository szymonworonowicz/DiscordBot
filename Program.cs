using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBot
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();


        private readonly IConfiguration _config;

        public Program()
        {
            var _builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName)
                .AddJsonFile("config.json")
                .AddUserSecrets<Program>();

            _config = _builder.Build();
        }


        public async Task MainAsync()
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost => { })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(_config);
                    services.AddSingleton<DiscordSocketClient>();
                    services.AddSingleton<CommandService>();
                    services.AddSingleton<CommandHandler>();
                    services.AddHostedService<DiscordBotBackgroundWorker>();
                    //services.AddHostedService<DiscordReminderBackgroudWorker>();
                })

                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();

        }

    }
}
