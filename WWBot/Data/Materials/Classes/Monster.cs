using System;
using System.Collections.Generic;
using System.Text;

namespace WWBot.Data.Materials.Classes
{
    public class Monster : BaseCard
    {
        public int diamonds { get; set; }
        public int scrolls { get; set; }

        public Monster(int materials, int crystals, int gold, int diamonds = 0, int scrolls = 0)
        {
            this.gold = gold;
            this.materials = materials;
            this.crystals = crystals;
            this.diamonds = diamonds;
            this.scrolls = scrolls;
        }

    }
}
