using System;
using System.Linq;
using System.Collections.Generic;

// Discord.NET features
using Discord;
using Discord.Commands;
using Discord.WebSocket;

// Async
using System.Threading.Tasks;

namespace WWBot.Modules.ComandsController
{
    public class RolesController : BaseController
    {
        /* Roles */
        private string correctRoleName(string role)
        {
            role = role.ToLower();
            if (!role.StartsWith("@"))
            {
                return Char.ToUpper(role[0]) + role.Substring(1); // Removes '@' from "@everyone"
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
        private async Task SetupDeckAsync(string deckColor)
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
    }
}
