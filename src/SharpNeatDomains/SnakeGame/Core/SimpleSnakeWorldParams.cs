using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Domains.SnakeGame.Core
{
    class SimpleSnakeWorldParams
    {

        public SimpleSnakeWorldParams(int height, int width, int startLen, int ticksBetweenFood, int maxFood)
        {
            Height = height;
            Width = width;
            StartLen = startLen;
            TicksBetweenFood = ticksBetweenFood;
            MaxFood = maxFood;
        }

        public readonly int Height;
        public readonly int Width;
        public readonly int StartLen;
        public readonly int TicksBetweenFood;
        public readonly int MaxFood;
    }
}
