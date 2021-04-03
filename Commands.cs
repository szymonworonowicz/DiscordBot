using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot
{
    public class Commands: ModuleBase
    {
        private readonly IServiceProvider _services;
        public Commands(IServiceProvider services)
        {
            _services = services;
        }

        [Command("Count",RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.ViewChannel)]
        [RequireBotPermission(ChannelPermission.ReadMessageHistory)]
        public async Task CountMessageAsync([Remainder][Summary("Count slowa")] string args = null)
        {

            var _client = _services.GetRequiredService<DiscordSocketClient>();
            Dictionary<string, int> Counter = new Dictionary<string, int>();

            var channels = await Context.Guild.GetChannelsAsync();
            foreach (var channel in channels)
            {
                var info = _client.GetChannel(channel.Id) as SocketTextChannel;
                var messages = await info.GetMessagesAsync(int.MaxValue).Flatten().ToArrayAsync();

                var filterMessages = messages.Where(x => x.Content.ToLower().Contains(args.ToLower()));

                foreach (var message in filterMessages)
                {
                    if (Counter.ContainsKey(message.Author.Username))
                    {
                        Counter[message.Author.Username]++;
                    }
                    else
                    {
                        Counter.Add(message.Author.Username, 0);
                    }
                }
            }

            

            StringBuilder str = new StringBuilder();

            foreach ( var elem in Counter)
            {
                str.Append($"{elem.Key} : {elem.Value}");
                str.AppendLine();
            }

            await Context.Channel.SendMessageAsync(str.ToString());



        }

    }
}
