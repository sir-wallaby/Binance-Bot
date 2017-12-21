using Discord;
using Discord.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bittrex_Bot.Modules
{
    [Name("Moderator")]
    [RequireContext(ContextType.Guild)]
    public class ClearChatModule : ModuleBase
    {
        
        [Command("clearchat")]
        [Summary("Clears the chat log for up to X messages.")]
        [RequireUserPermission(Discord.GuildPermission.ManageMessages)]
        public async Task Clear([Remainder] int numberOfMessageToDelete)
        {
            var messagesToDelete = Context.Channel.GetMessagesAsync(numberOfMessageToDelete);

            var variableToPassToDeleteMessages = await messagesToDelete.Flatten();

            await Context.Channel.DeleteMessagesAsync(variableToPassToDeleteMessages);
        }
    }
}
