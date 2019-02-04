using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Domains.SnakeGame.Core
{
    struct TwoDPoint
    {
        int _x;
        int _y;

        public TwoDPoint(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        internal int ManhattanDistance(TwoDPoint p)
        {
            return Math.Abs(_x - p.X) + Math.Abs(_y - p.Y);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            TwoDPoint p = (TwoDPoint) obj;
            return X == p.X && Y == p.Y;
        }

        public bool Equals(TwoDPoint p)
        {
            return X == p.X && Y == p.Y;
        }

        public static bool operator == (TwoDPoint p1, TwoDPoint p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(TwoDPoint p1, TwoDPoint p2)
        {
            return !(p1 == p2);
        }

        public override int GetHashCode()
        {
            return _x ^ _y;
        }
    }
}
