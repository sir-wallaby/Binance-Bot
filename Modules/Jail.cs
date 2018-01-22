using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;


namespace Binance_Bot.Modules
{
    public class Jail : ModuleBase<SocketCommandContext> 
    {
        [Name("Send To Jail")]
        [Command("mute")]
            public async Task sucker (IGuildUser user)
        {
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToString() == "SuckerRole");

            await (user as IGuildUser).AddRoleAsync(role);
        }

        [Name("Remove From Jail")]
        [Command("unmute")]
        public async Task unSucker(IGuildUser user)
        {
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToString() == "SuckerRole");

            await (user as IGuildUser).RemoveRoleAsync(role);
        }
    }
}
