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

        //Cards lvl variables

            // Monsters
            private static string monstersFolderPath = "Data\\Materials\\Txt\\Monsters";

            private static string commonMonsterPath = Path.Combine(monstersFolderPath, "Common.txt");
            private static string rareMonsterPath = Path.Combine(monstersFolderPath, "Rare.txt");
            private static string epicMonsterPath = Path.Combine(monstersFolderPath, "Epic.txt");
            private static string legendaryMonsterPath = Path.Combine(monstersFolderPath, "Legendary.txt");
            private static string eliteMonsterPath = Path.Combine(monstersFolderPath, "Elite.txt");

            //Gears
            private static string gearsFolderPath = "Data\\Materials\\Txt\\Gears";

            private static string commonGearPath = Path.Combine(gearsFolderPath, "Common.txt");
            private static string rareGearPath = Path.Combine(gearsFolderPath, "Rare.txt");
            private static string epicGearPath = Path.Combine(gearsFolderPath, "Epic.txt");
            private static string legendaryGearPath = Path.Combine(gearsFolderPath, "Legendary.txt");

        // Monsters
        public static List<Monster> commonMonsters { get; set; }
        public static List<Monster> rareMonsters { get; set; }
        public static List<Monster> epicMonsters { get; set; }
        public static List<Monster> legendaryMonsters { get; set; }
        public static List<Monster> eliteMonsters { get; set; }

        // Gears
        public static List<Gear> commonGears { get; set; }
        public static List<Gear> rareGears { get; set; }
        public static List<Gear> epicGears { get; set; }
        public static List<Gear> legendaryGears { get; set; }

        private Program ()
        {
            commonMonsters = new List<Monster>();
            rareMonsters = new List<Monster>();
            epicMonsters = new List<Monster>();
            legendaryMonsters = new List<Monster>();
            eliteMonsters = new List<Monster>();

            commonGears = new List<Gear>();
            rareGears = new List<Gear>();
            epicGears = new List<Gear>();
            legendaryGears = new List<Gear>();
        }

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
            //int counter = 0;

                #region Fullfilling lists of Monsters and Gears

                // Fullfilling list of Gears the gears txt files

                // Monsters
                MonsterStreamer(commonMonsterPath, commonMonsters);
                MonsterStreamer(rareMonsterPath, rareMonsters);
                MonsterStreamer(epicMonsterPath, epicMonsters);
                MonsterStreamer(legendaryMonsterPath, legendaryMonsters);
                MonsterStreamer(eliteMonsterPath, eliteMonsters);

                // Gears
                GearStreamer(commonGearPath, commonGears);
                GearStreamer(rareGearPath, rareGears);
                GearStreamer(epicGearPath, epicGears);
                GearStreamer(legendaryGearPath, legendaryGears);

                #endregion
            
            }

        private void MonsterStreamer(string path, List<Monster> monsterList)
        {
            StreamReader reader = new StreamReader(path);

            CreateMonsterList(reader, monsterList);

            CloseStreamer(reader);
        }

        private void GearStreamer(string path, List<Gear> gearList)
        {
            StreamReader reader = new StreamReader(path);

            CreateGearList(reader, gearList);

            CloseStreamer(reader);
        }

        private void CloseStreamer(StreamReader stream)
        {
            stream.Close();
        }

        private void CreateMonsterList(StreamReader streamReader, List<Monster> cardList)
        {
            while (!streamReader.EndOfStream)
            {
                try
                {
                    cardList.Add(new Monster(
                        Int32.Parse(streamReader.ReadLine()),
                        Int32.Parse(streamReader.ReadLine()),
                        Int32.Parse(streamReader.ReadLine()),
                        Int32.Parse(streamReader.ReadLine()),
                        Int32.Parse(streamReader.ReadLine())
                        ));
                }
                catch (ArgumentNullException e)
                {
                    throw new ArgumentNullException(string.Format("Couldn't fullfill list. TIP: compare number of lines with elemnts count."));
                }
            }
        }

        private void CreateGearList(StreamReader streamReader, List<Gear> cardList)
        {
            while (!streamReader.EndOfStream)
            {
                try
                {
                    cardList.Add(new Gear(
                        Int32.Parse(streamReader.ReadLine()),
                        Int32.Parse(streamReader.ReadLine()),
                        Int32.Parse(streamReader.ReadLine())
                        ));
                }
                catch (ArgumentNullException e)
                {
                    throw new ArgumentNullException(string.Format("Couldn't fullfill list. TIP: compare number of lines with elemnts count."));
                }

            }
        }
    }
}
