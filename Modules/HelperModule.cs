using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;

namespace Bittrex_Bot.Modules
{
    public class HelperModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;
        private readonly IConfigurationRoot _config;

        public HelperModule(CommandService service, IConfigurationRoot config)
        {
            _service = service;
            _config = config;
        }

        [Command("help")]
        public async Task HelperAsync()
        {
            string prefix = _config["prefix"];
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "You can use the following commands"
            };

            foreach (var module in _service.Modules)
            {
                string desc = null;
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                        desc += $"{prefix}{cmd.Aliases.First()}\n";
                }

                if (!string.IsNullOrWhiteSpace(desc))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = desc;
                        x.IsInline = false;
                    });
                }
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("help")]
        public async Task HelpAsync(string command)
        {
            var result = _service.Search(Context, command);

            if (!result.IsSuccess)
            {
                await ReplyAsync($"Sorry, I couldn't find a command like **{command}**.");
                return;
            }

            string prefix = _config["prefix"];
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = $"Here are some commands like **{command}**"
            };

            foreach (var match in result.Commands)
            {
                var cmd = match.Command;

                builder.AddField(x =>
                {
                    x.Name = string.Join(", ", cmd.Aliases);
                    x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                              $"Summary: {cmd.Summary}";
                    x.IsInline = false;
                });
            }

            await ReplyAsync("", false, builder.Build());
        }
    }
}