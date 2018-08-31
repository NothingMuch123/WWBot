using System;
using System.Collections.Generic;
using System.Text;

namespace WWBot.Data.Materials.Classes
{
    class Gear : BaseCard
    {
        public Gear(int gold, int materials, int crystals)
        {
            this.gold = gold;
            this.materials = materials;
            this.crystals = crystals;
        }
    }
}
