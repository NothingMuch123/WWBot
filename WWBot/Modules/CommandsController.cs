using System;
using System.Linq;
using System.Collections.Generic;

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

    public class CommandsController : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Helper functions
        /// </summary>
        private EmbedBuilder createEmbed(string title, string desc = "", bool inline = false)
        {
            var builder = new EmbedBuilder();
            builder.AddField(title, desc, inline);
            return builder;
        }

        private void setEmbedColor(Color color, ref EmbedBuilder builder)
        {
            if (builder != null)
            {
                builder.WithColor(color);
            }
        }

        public async Task Reply(string msg)
        {
            await ReplyAsync(msg);
        }

        public async Task Reply(Embed msg)
        {
            await ReplyAsync("", false, msg);
        }

        /* Roles */
        private string correctRoleName(string role)
        {
            role = role.ToLower();
            if (!role.StartsWith("@"))
            {
                return Char.ToUpper(role[0]) + role.Substring(1);
            }
            return role;
        }

        private SocketRole findRolesFromList(string inputRole, SocketGuild guild, List<string> listToUse = null)
        {
            // Check role exists
            inputRole = correctRoleName(inputRole);

            // Setup default listToUse
            if (listToUse == null)
            {
                listToUse = Program.Roles;
            }

            if (listToUse.Contains(inputRole))
            {
                // User data
                var data = new Data(Context);
                var role = guild.Roles.FirstOrDefault(x => x.Name == inputRole);
                return role;
            }
            return null;
        }

        public async Task SetRole(SocketRole role, SocketUser user)
        {
            await (user as IGuildUser).AddRoleAsync(role);
        }

        public async Task DeleteRole(SocketRole role, SocketUser user)
        {
            await (user as IGuildUser).RemoveRoleAsync(role);
        }

        /// <summary>
        /// Commands controller
        /// </summary>

        [Command("deck"), RequireUserPermission(GuildPermission.ManageNicknames)]
        public async Task SetupDeckAsync(string deckColor)
        {
            // User data
            var data = new Data(Context);
            var role = findRolesFromList(deckColor, data.Guild, Program.DeckColors);
            if (role != null)
            {
                // Check for existing deck color
                foreach (var userRole in (data.User as SocketGuildUser).Roles.ToList())
                {
                    if (userRole.Position >= Program.MinColorPos && userRole.Position <= Program.MaxColorPos)
                    {
                        // Deck color exist, remove
                        DeleteRole(userRole, data.User);
                    }
                }

                // Set deck color
                SetRole(role, data.User);
                Reply($"{data.User.Mention} is using {correctRoleName(deckColor)} deck!");
            }
            else
            {
                Reply($"{deckColor} is not a deck color");
            }
        }


        // Test commands
        [Command("hi")]
        public async Task HiAsync()
        {
            // User data
            var data = new Data(Context);

            // Creates a table with details
            var builder = createEmbed("Hi", "Bye");
            setEmbedColor(Color.Blue, ref builder);

            await Reply(builder.Build());
        }

        [Command("role")]
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
        }
    }
}
