using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DiscordBot
{
    public class Commands: ModuleBase
    {
        private readonly DiscordSocketClient _client;
        public Commands(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("Count",RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.ViewChannel)]
        [RequireBotPermission(ChannelPermission.ReadMessageHistory)]
        public async Task CountMessageAsync([Remainder][Summary("Count slowa")] string args = null)
        {

            Dictionary<string, int> Counter = new Dictionary<string, int>();

            var channels = await Context.Guild.GetTextChannelsAsync();
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

        [Command("AddRemind", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.ViewChannel)]
        public async Task RemindMessageAsync( [Summary("message data")] params string[] args)
        {
            var info = new RemindInfo()
            {
                Message =  args[0],
                Date = Convert.ToDateTime(args[1]),
                ChannelId = Context.Channel.Id
            };
            string newJson = "";

            var path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName ,"Reminders.json");

            using (StreamReader str = new StreamReader(path))
            {
                string json = await str.ReadToEndAsync();
                List<RemindInfo> reminders = JsonConvert.DeserializeObject<List<RemindInfo>>(json);

                reminders.Add(info);
                newJson = JsonConvert.SerializeObject(reminders);
            }

            await File.WriteAllTextAsync(path, newJson);

            await Context.Channel.SendMessageAsync($"Added {args[0]} in {args[1]}");
        }

    }
}
