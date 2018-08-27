using System;
using System.Collections.Generic;
using System.Text;

namespace WWBot.Data.Materials.Classes
{
    class Gear : ICard
    {
        public int gold { get; set; }
        public int materials { get; set; }
        public int crystals { get; set; }

        public Gear(int gold, int materials, int crystals)
        {
            this.gold = gold;
            this.materials = materials;
            this.crystals = crystals;
        }
    }
}
