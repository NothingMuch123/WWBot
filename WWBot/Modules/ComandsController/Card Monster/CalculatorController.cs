using System;
using System.Collections.Generic;
using System.Text;

// Discord.NET features
using Discord;
using Discord.Commands;
using Discord.WebSocket;

// Async
using System.Threading.Tasks;
using WWBot.Data.Materials.Classes;

namespace WWBot.Modules.ComandsController
{
    public class CalculatorController : BaseController
    {
        [Command("name")]
        private async Task GetName()
        {
            await Reply(Context.User.Username);
        }

        [Command("cm_calculate")]
        private async Task Calculate(string type, string rarity, int lvl = 2, int gold = 0, int materials = 0, int crystals = 0, int diamonds = 0, int scrolls = 0)
        {
            if(type.ToLower() == "monster")
            {
                switch (rarity.ToLower())
                {
                    case "common":
                            await ReplyUser(Program.commonMonsters[lvl - 2], lvl, gold, materials, crystals, diamonds, scrolls);
                        break;

                    case "rare":
                            await ReplyUser(Program.rareMonsters[lvl - 2], lvl, gold, materials, crystals, diamonds, scrolls);
                        break;

                    case "epic":
                            await ReplyUser(Program.epicMonsters[lvl - 2], lvl, gold, materials, crystals, diamonds, scrolls);
                        break;

                    case "legendary":
                            await ReplyUser(Program.legendaryMonsters[lvl - 2], lvl, gold, materials, crystals, diamonds, scrolls);
                        break;

                    case "elite":
                            await ReplyUser(Program.eliteMonsters[lvl - 2], lvl, gold, materials, crystals, diamonds, scrolls);
                        break;

                    default:
                            await Reply("Error! Card doesn't exist! It has to be a monster or weapon!");
                        break;
                }
            }
            else if (type.ToLower() == "gear" || type.ToLower() == "weapon")
            {
                switch (rarity.ToLower())
                {
                    case "common":
                            await ReplyWithoutDiamods(Program.commonGears[lvl - 2], lvl, gold, materials, crystals);
                        break;

                    case "rare":
                            await ReplyWithoutDiamods(Program.rareGears[lvl - 2], lvl, gold, materials, crystals);
                        break;

                    case "epic":
                            await ReplyWithoutDiamods(Program.epicGears[lvl - 2], lvl, gold, materials, crystals);
                        break;

                    case "legendary":
                            await ReplyWithoutDiamods(Program.legendaryGears[lvl - 2], lvl, gold, materials, crystals);
                        break;

                    case "elite":
                            await Reply("Error! Gear cannot has an elite rarity!");
                        break;

                    default:
                            await Reply("Error! Card doesn't exist! It has to be a monster or weapon!");
                        break;
                }
            }
            else
            {
                await Reply("Error! Card doesn't exist! It has to be a monster or weapon!");
            }          
        }

        private async Task ReplyUser(Monster card, int lvl = 2, int gold = 0, int materials = 0, int crystals = 0, int diamonds = 0, int scrolls = 0)
        {
            gold = CalculateMaterials(card.gold, gold);
            materials = CalculateMaterials(card.materials, materials);
            crystals = CalculateMaterials(card.crystals, crystals);
            diamonds = CalculateMaterials(card.diamonds, diamonds);
            scrolls = CalculateMaterials(card.scrolls, scrolls);

            if (card.diamonds != 0 && card.scrolls != 0)
             {
                await Reply($"To upgrade a monster to the lvl {lvl}, " +
                $"you will need a {gold} gold, " +
                $"{materials} materials, " +
                $"{crystals} crystals, " +
                $"{diamonds} diamonds " +
                $"and {scrolls} scrolls more.");
             }
             else
             {
                await ReplyWithoutDiamods(card, lvl, gold, materials, crystals);
             }
        }

        private async Task ReplyWithoutDiamods(BaseCard card, int lvl = 2, int gold = 0, int materials = 0, int crystals = 0)
        {
            gold = CalculateMaterials(card.gold, gold);
            materials = CalculateMaterials(card.materials, materials);
            crystals = CalculateMaterials(card.crystals, crystals);

            await Reply($"To upgrade a monster to the lvl {lvl}, " +
            $"you will need a {gold} gold, " +
            $"{materials} materials " +
            $"and {crystals} crystals more. ");
        }

        private int CalculateMaterials(int cardField, int field)
        {
            return cardField - field < 0 ? 0 : cardField - field;
        }
    }
}
