using System;
using System.Collections.Generic;
using System.Text;

namespace WWBot.Data.Materials.Classes
{
    public abstract class BaseCard
    {
        public abstract int gold { get; set; }
        public abstract int materials { get; set; }
        public abstract int crystals { get; set; }
    }
}
