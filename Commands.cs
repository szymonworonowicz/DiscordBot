using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DiscordBot
{
    public class Commands : ModuleBase
    {
        private readonly DiscordSocketClient _client;
        public Commands(DiscordSocketClient client)
        {
            _client = client;
        }

        //[Command("Count", RunMode = RunMode.Async)]
        //[RequireBotPermission(GuildPermission.ViewChannel)]
        //[RequireBotPermission(ChannelPermission.ReadMessageHistory)]
        //public async Task CountMessageAsync([Remainder][Summary("Count slowa")] string args = null)
        //{

        //    ConcurrentDictionary<string, int> Counter = new ConcurrentDictionary<string, int>();

        //    var channels = await Context.Guild.GetTextChannelsAsync();
        //    var time = DateTime.Now;

        //    var opt = new ParallelOptions()
        //    {
        //        MaxDegreeOfParallelism = 3
        //    };

        //    Parallel.ForEach(channels, opt, async (channel) =>
        //    {
        //        try
        //        {
        //            var channelInfo = channel as ISocketMessageChannel;

        //            await Task.Delay(5000);
        //            var messages = await channelInfo.GetMessagesAsync(int.MaxValue).FlattenAsync();
        //            ;
        //            var filterMessages = messages.Where(x => x.Content.ToLower().Contains(args.ToLower()));


        //            foreach (var message in filterMessages)
        //            {
        //                if (Counter.ContainsKey(message.Author.Username))
        //                {
        //                    Counter[message.Author.Username]++;
        //                }
        //                else
        //                {
        //                    Counter.TryAdd(message.Author.Username, 0);
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e.Message);

        //        }

        //    });




        //    StringBuilder str = new StringBuilder();

        //    foreach (var elem in Counter)
        //    {
        //        str.Append($"{elem.Key} : {elem.Value}");
        //        str.AppendLine();
        //    }

        //    Console.WriteLine(DateTime.Now - time);
        //    await Context.Channel.SendMessageAsync(str.ToString());

        //}

        //[Command("AddRemind", RunMode = RunMode.Async)]
        //[RequireBotPermission(GuildPermission.ViewChannel)]
        //public async Task RemindMessageAsync([Summary("message data")] params string[] args)
        //{
        //    var info = new RemindInfo()
        //    {
        //        Message = args[0],
        //        Date = Convert.ToDateTime(args[1]),
        //        ChannelId = Context.Channel.Id
        //    };
        //    string newJson = "";

        //    var path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName, "Reminders.json");

        //    using (StreamReader str = new StreamReader(path))
        //    {
        //        string json = await str.ReadToEndAsync();
        //        List<RemindInfo> reminders = JsonConvert.DeserializeObject<List<RemindInfo>>(json);

        //        reminders.Add(info);
        //        newJson = JsonConvert.SerializeObject(reminders);
        //    }

        //    await File.WriteAllTextAsync(path, newJson);

        //    await Context.Channel.SendMessageAsync($"Added {args[0]} in {args[1]}");
        //}

        [Command("Euro", RunMode = RunMode.Async)]
        public async Task GetEuroInfoAsync()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://api.football-data.org/v2/competitions/2018/matches"),
                Headers =
                    {
                        {"X-Auth-Token","1e880ff9ff2043c6a976afb19fa1e9a3" }
                    },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Results>(body);
                var filtered = result.Matches.Where(x => x.HomeTeam.Name != null && x.Score.FullTime.HomeTeam != null && x.UtcDate.AddHours(3) > DateTime.Now).ToList();
                foreach(var match in filtered)
                {
                    await Context.Channel.SendMessageAsync($"{match.UtcDate} - {match.HomeTeam.Name}-{match.AwayTeam.Name} => {match.Score.FullTime.HomeTeam}:{match.Score.FullTime.AwayTeam}");
                }
                if(filtered.Count ==0)
                {
                    await Context.Channel.SendMessageAsync("nie ma teraz zadnego zaczetego meczu");
                }

            }
        }
        private class Results
        {
            public List<Match> Matches { get; set; }
        }
        private class Match
        {
            public Score Score { get; set; }
            public Team HomeTeam { get; set; }
            public Team AwayTeam { get; set; }
            public DateTime UtcDate { get; set; }
        }
        private class Score
        {
            public FullTime FullTime { get; set; }
        }
        private class FullTime
        {
            public int? HomeTeam { get; set; }
            public int? AwayTeam { get; set; }
        }
        private class Team
        {
            public string Name { get; set; }
        }

    }
    
}