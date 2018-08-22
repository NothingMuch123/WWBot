using System;
using System.Collections.Generic;
using System.Text;

namespace WWBot.Data.Materials.Classes
{
    public class Card
    {
        public List<int> Gold { get; set; }
        public List<int> Materials { get; set; }
        public List<int> Crystals { get; set; }

        public Card ()
        {
            this.Gold = new List<int>();
            this.Materials = new List<int>();
            this.Crystals = new List<int>();
        }
    }
}
