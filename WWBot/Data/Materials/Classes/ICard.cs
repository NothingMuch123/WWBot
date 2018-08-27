using System;
using System.Collections.Generic;
using System.Text;

namespace WWBot.Data.Materials.Classes
{
    public interface ICard
    {
        int gold { get; set; }
        int materials { get; set; }
        int crystals { get; set; }
    }
}
