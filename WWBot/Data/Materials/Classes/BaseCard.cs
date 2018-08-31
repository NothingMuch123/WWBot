using System;
using System.Collections.Generic;
using System.Text;

namespace WWBot.Data.Materials.Classes
{
    public abstract class BaseCard
    {
        public int gold { get; set; }
        public int materials { get; set; }
        public int crystals { get; set; }
    }
}
