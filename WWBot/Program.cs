using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

// Discord.NET features
using Discord;
using Discord.Commands;
using Discord.WebSocket;

// IO
using System.IO;

// Namespace usings
using WWBot.Data.Materials.Classes;

namespace WWBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();

            // Load IO
            program.loadIO();

            // Start program
            program.RunBotAsync().GetAwaiter().GetResult();
        }
        //=> new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;

        /* Static values */
        public static string CommandIndicator = "!";

        // Roles variables
        public static string RolesPath = "Data\\Roles.txt";
        public static List<string> Roles = new List<string>();

        // Deck colors variables
        public static string DeckColorIndicator = "!";
        public static int MaxColorPos = -1; // Inclusive
        public static int MinColorPos = -1; // Inclusive
        public static List<string> DeckColors = new List<string>();

        // Excel variables (Guild wars)
        public static string GuildWarsResultsFolder = "Data\\GuildWars";

        //Cards lvl variables
            // Monsters
            private static string commonMonsterPath = "Data\\Materials\\Txt\\Monsters\\Common.txt";
            private static string rareMonsterPath = "Data\\Materials\\Txt\\Monsters\\Rare.txt";
            private static string epicMonsterPath = "Data\\Materials\\Txt\\Monsters\\Epic.txt";
            private static string legendaryMonsterPath = "Data\\Materials\\Txt\\Monsters\\Legendary.txt";
            private static string eliteMonsterPath = "Data\\Materials\\Txt\\Monsters\\Elite.txt";

            //Gears
            private static string commonGearPath = "Data\\Materials\\Txt\\Gears\\Common.txt";
            private static string rareGearPath = "Data\\Materials\\Txt\\Gears\\Rare.txt";
            private static string epicGearPath = "Data\\Materials\\Txt\\Gears\\Epic.txt";
            private static string legendaryGearPath = "Data\\Materials\\Txt\\Gears\\Legendary.txt";

        // Monsters
        public static List<Monster> commonMonsters = new List<Monster>();
        private List<Monster> rareMonsters = new List<Monster>();
        private List<Monster> epicMonsters = new List<Monster>();
        private List<Monster> legendaryMonsters = new List<Monster>();
        private List<Monster> eliteMonsters = new List<Monster>();

        // Gears
        private List<Gear> commonGears = new List<Gear>();
        private List<Gear> rareGears = new List<Gear>();
        private List<Gear> epicGears = new List<Gear>();
        private List<Gear> legendaryGears = new List<Gear>();

        public async Task RunBotAsync()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();

            services = new ServiceCollection()
                .AddSingleton(client)       // Set to only have 1 client
                .AddSingleton(commands)     // Set to only have 1 commands controller
                .BuildServiceProvider();    // Build service

            string token = "NDc4NzgzMDY1MTYyNzc2NTk2.DlPuow.7l1Oi6FasKu-GJ6TJjtvsPOwwNI";

            // Event subcriptions
            client.Log += Log;
            client.UserJoined += AnnounceUserJoined;

            await RegisterCommandAsync();

            await client.LoginAsync(TokenType.Bot, token);

            await client.StartAsync();

            await Task.Delay(-1);   // Delay task so client will always run
        }

        private async Task AnnounceUserJoined(SocketGuildUser user)
        {
            var channel = user.Guild.DefaultChannel;
            await channel.SendMessageAsync($"Welcome, {user.Mention}");
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg);

            return Task.CompletedTask;
        }

        public async Task RegisterCommandAsync()
        {
            client.MessageReceived += HandleCommandAsync;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage msg)
        {
            var message = msg as SocketUserMessage;

            // Do not respond if no message or from bot
            if (message is null || message.Author.IsBot)
            {
                return;
            }

            int msgPos = 0;

            if (message.HasStringPrefix(CommandIndicator, ref msgPos) || message.HasMentionPrefix(client.CurrentUser, ref msgPos))
            {
                var context = new SocketCommandContext(client, message);

                var result = await commands.ExecuteAsync(context, msgPos, services);

                // Write error to console
                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }

        // IO for data
        public void loadIO()
        {
            // Roles
            if (File.Exists(RolesPath))
            {
                LoadRoles();
            }
            else
            {
                // Test output to create file when not exist
                /*StreamWriter writer = File.CreateText(RolesPath);
                writer.WriteLine("Created");*/
                Console.WriteLine($"{RolesPath} does not contain the Roles file");
            }

            // Monster and gears
            if (!File.Exists(commonGearPath))
            {
                Console.WriteLine($"{commonGearPath} does not exist");
            }
            else if (!File.Exists(commonMonsterPath))
            {
                Console.WriteLine($"{commonMonsterPath} does not exist");
            }
            else
            {
                LoadMaterials();
            }
        }

        private void LoadRoles()
        {
            // Open file and load data
            StreamReader reader = new StreamReader(RolesPath);
            int counter = 0;
            while (!reader.EndOfStream)
            {
                var role = reader.ReadLine();
                if (role.StartsWith(DeckColorIndicator))
                {
                    // Start of color role processing
                    if (DeckColors.Count <= 0)
                    {
                        // Set min (One-time)
                        MinColorPos = counter;
                    }

                    // Role is a color, add to color list
                    role = role.Substring(DeckColorIndicator.Length); // Start from 'length' element to remove the indicator
                    DeckColors.Add(role);
                    Roles.Add(role);

                    // Always set max so that max will be the last
                    MaxColorPos = counter;
                }
                else
                {
                    // Add into roles list
                    Roles.Add(role);
                }
                Console.WriteLine($"Added \"{role}\" into Roles");
                ++counter;
            }
        }

        private void LoadMaterials()
        {
            int counter = 0;

            // Monsters
            StreamReader commonMonster = new StreamReader(commonGearPath);
            StreamReader rareMonster = new StreamReader(commonGearPath);
            StreamReader epicMonster = new StreamReader(commonGearPath);
            StreamReader legendaryMonster = new StreamReader(commonGearPath);
            StreamReader eliteMonster = new StreamReader(commonGearPath);

            // Gears
            StreamReader commonGear = new StreamReader(commonMonsterPath);
            StreamReader rareGear = new StreamReader(commonMonsterPath);
            StreamReader epicGear = new StreamReader(commonMonsterPath);
            StreamReader legendaryGear = new StreamReader(commonMonsterPath);

            while (!commonMonster.EndOfStream)
            {
                commonMonsters.Add(new Monster(
                    Int32.Parse(commonMonster.ReadLine()),
                    Int32.Parse(commonMonster.ReadLine()),
                    Int32.Parse(commonMonster.ReadLine())
                    ));

                // Test output
                Console.WriteLine($"Common Monster :: gold: {commonMonsters[counter].gold}" +
                    $", materials : {commonMonsters[counter].materials}" +
                    $", crystals: {commonMonsters[counter].crystals} " +
                    $"and diamonds: {commonMonsters[counter].diamonds}");

                ++counter;
            }
            commonMonster.Close();
            commonGear.Close();
        }
    }
}
