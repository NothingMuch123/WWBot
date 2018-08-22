// Discord.NET features
using Discord;
using Discord.Commands;
using Discord.WebSocket;

// Async
using System.Threading.Tasks;

/* Info to know about User class (E.g Just a Potato #6035 {Nicked: Nicked Potato})
 * - Username           = Name on discord (E.g Just a Potato)
 * - Discriminator      = Tag (E.g 6035)
 * - DiscriminatorValue = Tag (E.g 6035)
 * 
 * Info to know about IGuildUser interface (E.g Just a Potato #6035 {Nicked: Nicked Potato})
 * - Nickname   = Nicked Potato
*/

namespace WWBot.Modules
{
    // Stores all the data needed
    public struct Data
    {
        public SocketUser User;                // User that triggers the command
        public DiscordSocketClient Client;     // The client or user in which the bot is using
        public SocketGuild Guild;              // The server where the user is from
        public SocketUserMessage Message;      // The full message that triggers the command (Including command indicator)

        public Data(SocketCommandContext context)
        {
            User = context.User;
            Client = context.Client;
            Guild = context.Guild;
            Message = context.Message;
        }
    }

    /// <summary>
    /// Contains helper functions
    /// </summary>
    public class BaseController : ModuleBase<SocketCommandContext>
    {
        protected EmbedBuilder createEmbed(string title, string desc = "", bool inline = false)
        {
            var builder = new EmbedBuilder();
            builder.AddField(title, desc, inline);
            return builder;
        }

        protected void setEmbedColor(Color color, ref EmbedBuilder builder)
        {
            if (builder != null)
            {
                builder.WithColor(color);
            }
        }

        protected async Task Reply(string msg)
        {
            await ReplyAsync(msg);
        }

        protected async Task Reply(Embed msg)
        {
            await ReplyAsync("", false, msg);
        }

        /*[Command("role")]
        public async Task RoleAsync(string inputRole)
        {
            // User data
            var data = new Data(Context);
            var role = data.Guild.Roles.FirstOrDefault(x => x.Name == inputRole);
            var everyoneRole = data.Guild.Roles.FirstOrDefault(x => x.Name == "@everyone");
            var adminRole = data.Guild.Roles.FirstOrDefault(x => x.Name == "Admin");

            foreach (var r in ((SocketGuildUser)data.User).Roles)
            {
                if (r == everyoneRole || r == adminRole)
                {
                    continue;
                }
                await (data.User as IGuildUser).RemoveRoleAsync(r);
            }
            await (data.User as IGuildUser).AddRoleAsync(role);
        }

        [Command("name")]
        public async Task NameAsync()
        {
            // User data
            var data = new Data(Context);
            string reply = "";
            foreach (var role in (data.User as SocketGuildUser).Roles.ToList())
            {
                reply += role.ToString() + " - " + role.Position + " | ";
            }
            Reply(reply);
        }*/
    }
}
