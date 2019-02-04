using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Domains.SnakeGame.Core
{
    enum Direction
    {
        Left = 0,
        Right = 1,
        Up = 2,
        Down = 3,
        None = 4
    }

    static class DirectionExtensions
    {

        private static Dictionary<Direction, Direction> _opposite = new Dictionary<Direction, Direction>()
        {
            { Direction.Left, Direction.Right },
            { Direction.Right, Direction.Left },
            { Direction.Up, Direction.Down },
            { Direction.Down, Direction.Up },
            { Direction.None, Direction.None }
        };

        public static Direction GetOpposite(this Direction d)
        {
            return _opposite[d];
        }
    }
}
