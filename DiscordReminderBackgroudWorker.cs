using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace DiscordBot
{
    class DiscordReminderBackgroudWorker : BackgroundService
    {
        private readonly DiscordSocketClient _client;

        public DiscordReminderBackgroudWorker(DiscordSocketClient client)
        {
            _client = client;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (DateTime.Now.TimeOfDay > TimeSpan.FromHours(11))
            {
                await Task.Delay(TimeSpan.FromHours(35) - DateTime.Now.TimeOfDay);
            }
            else
            {
                await Task.Delay(TimeSpan.FromHours(11) - DateTime.Now.TimeOfDay);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                var path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName, "Reminders.json");

                List<RemindInfo> reminds = null;
                using (StreamReader str = new StreamReader(path))
                {
                    var json = await str.ReadToEndAsync();
                    reminds = JsonConvert.DeserializeObject<List<RemindInfo>>(json);

                }

                List<RemindInfo> toDelete = new List<RemindInfo>();
                foreach (var remind in reminds)
                {
                    if (remind.Date.Date == DateTime.Now.Date)
                    {
                        toDelete.Add(remind);
                        if (_client.GetChannel(remind.ChannelId) is SocketTextChannel channel)
                        {
                            await channel.SendMessageAsync(remind.Message);
                        }
                    }
                }

                foreach (var delete in toDelete)
                {
                    reminds.Remove(delete);
                }

                var newJson = JsonConvert.SerializeObject(reminds);
                await File.WriteAllTextAsync(path, newJson);

                await Task.Delay(TimeSpan.FromDays(1));

            }
        }
    }
}
