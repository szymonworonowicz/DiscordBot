using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordBot
{
    class DiscordBotBackgroundWorker:BackgroundService
    {
        public static DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IConfiguration _config;
        private readonly CommandHandler _commandHandler;

        public DiscordBotBackgroundWorker(IConfiguration config
                        ,DiscordSocketClient client ,CommandService commands
                        ,CommandHandler handler)
        {
            _config = config;
            _client = client;
            _commands = commands;
            _commandHandler = handler;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _client.Log += Log;

            _client.Ready += ReadyAsync;
            _commands.Log += Log;

            await _client.LoginAsync(TokenType.Bot, _config["Token"]);
            await _client.StartAsync();

            await _commandHandler.InitializeAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());

            return Task.CompletedTask;
        }
        private Task ReadyAsync()
        {
            Console.WriteLine($"Connected as -> [] :)");
            return Task.CompletedTask;
        }
        
    }
}
