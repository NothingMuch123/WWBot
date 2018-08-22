using System;
using System.Collections.Generic;

// Excel
using OfficeOpenXml;

// Discord.NET features
using Discord;
using Discord.Commands;
using Discord.WebSocket;

// Async
using System.Threading.Tasks;

namespace WWBot.Modules.ComandsController
{
    // Guild wars weekly results excel
    [Group("gw")]
    public class GuildWarsController : BaseController
    {
        [Command("create")]
        public async Task CreateGWExcel(string DDMMYY)
        {

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
    }
}
