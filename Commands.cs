using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace DiscordBot
{
    public class Commands: ModuleBase
    {
        [Command("Count",RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.ViewChannel)]
        [RequireBotPermission(ChannelPermission.ReadMessageHistory)]
        public async Task CountMessageAsync([Remainder][Summary("Count slowa")] string args = null)
        {
            var messages = await Context.Channel.GetMessagesAsync(int.MaxValue).Flatten().ToArrayAsync();

            var filterMessages = messages.Where(x => x.Content.Contains(args));
            Dictionary<string,int> Counter = new Dictionary<string, int>();

            foreach(var message in filterMessages)
            {
                if (Counter.ContainsKey(message.Author.Username))
                {
                    Counter[message.Author.Username]++;
                }
                else
                {
                    Counter.Add(message.Author.Username,0);
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
