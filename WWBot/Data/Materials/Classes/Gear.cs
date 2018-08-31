using System;
using System.Collections.Generic;
using System.Text;

namespace WWBot.Data.Materials.Classes
{
    class Gear : BaseCard
    {
        public override int gold { get; set; }
        public override int materials { get; set; }
        public override int crystals { get; set; }

        public Gear(int gold, int materials, int crystals)
        {
            this.gold = gold;
            this.materials = materials;
            this.crystals = crystals;
        }
    }
}
