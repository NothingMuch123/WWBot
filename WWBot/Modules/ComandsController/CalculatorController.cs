using System;
using System.Collections.Generic;
using System.Text;

// Discord.NET features
using Discord;
using Discord.Commands;
using Discord.WebSocket;

// Async
using System.Threading.Tasks;

namespace WWBot.Modules.ComandsController
{
    class CalculatorController : BaseController
    {
        [Command("calculate")]
        public async Task Calculate()
        {
            await Reply(Program.commonMonsters[0].gold.ToString());
        }
    }
}
