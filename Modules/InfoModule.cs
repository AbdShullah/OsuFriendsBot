﻿using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using OsuFriendBot.Services;
using System.Linq;
using System.Threading.Tasks;

namespace OsuFriendBot.Modules
{
    // Modules must be public and inherit from an IModuleBase
    [Name("Informations")]
    [Summary("Useful informations about me")]
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private readonly GuildSettingsCacheService _guildSettings;
        private readonly CommandService _commands;
        private readonly Config _config;
        private readonly ILogger _logger;

        public InfoModule(GuildSettingsCacheService guildSettings, CommandService commands, Config config, ILogger<InfoModule> logger)
        {
            _guildSettings = guildSettings;
            _commands = commands;
            _config = config;
            _logger = logger;
        }

        [Command("help")]
        [Summary("Shows all commands")]
        public async Task HelpCmd()
        {
            System.Collections.Generic.List<ModuleInfo> modules = _commands.Modules.OrderBy(x => x.Name).ToList();
            foreach (ModuleInfo module in modules)
            {
                EmbedBuilder embedBuilder = new EmbedBuilder();
                embedBuilder.WithTitle(module.Name).WithDescription(module.Summary ?? "No summary");
                System.Collections.Generic.IReadOnlyList<CommandInfo> commands = module.Commands;
                foreach (CommandInfo command in commands)
                {
                    string title = $"{string.Join(" | ", command.Aliases)}";
                    if (command.Summary != null)
                    {
                        title += $" | {command.Summary}";
                    }
                    string text = $"{(Context.Guild != null ? _guildSettings.GetOrAddGuildSettings(Context.Guild.Id).Prefix ?? _config.Prefix : _config.Prefix)}{command.Name}";
                    foreach (ParameterInfo param in command.Parameters)
                    {
                        text += $" | {param.Name}";
                        if (param.Summary != null)
                        {
                            text += $": {param.Summary}";
                        }
                    }
                    text = Format.Code('\n' + text);
                    embedBuilder.AddField(title, text);
                }
                await ReplyAsync(embed: embedBuilder.Build());
            }
        }
    }
}