using System;
using System.Collections.Generic;
using System.Text;

namespace WWBot.Data.Materials.Classes
{
    public class Monster : ICard
    {
        public int gold { get; set; }
        public int materials { get; set; }
        public int crystals { get; set; }
        public int diamonds { get; set; }

        public Monster(int gold, int materials, int crystals, int diamonds = 0)
        {
            this.gold = gold;
            this.materials = materials;
            this.crystals = crystals;
            this.diamonds = diamonds;
        }

    }
}
