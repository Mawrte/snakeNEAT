using SharpNeat.Domains.SnakeGame.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Domains.SnakeGame.Core
{
    class Snake
    {
        readonly Queue<TwoDPoint> _points = new Queue<TwoDPoint>();

        //readonly List<TwoDPoint> _points = new List<TwoDPoint>();

        public Snake()
        {
            TwoDPoint p = new TwoDPoint(0, 0);
            _points.Enqueue(p);
        }

        public Snake(TwoDPoint starting)
        {
            _points.Enqueue(starting);
        }

        public Snake(int x, int y)
        {
            TwoDPoint starting = new TwoDPoint(x, y);
            _points.Enqueue(starting);
        }

        public TwoDPoint this[int index]
        {
            get { return _points.ElementAt(index); }
        }

        public int Length
        {
            get { return _points.Count; }
        }


        //la testa è l'ultima perché è l'ultima messa in coda

        public TwoDPoint Head
        {
            get { return _points.Last(); }
        }

        public IEnumerable<TwoDPoint> GetHeadingPoints(int num)
        {
            return _points.TakeLast(num);
        }

        public TwoDPoint Tail
        {
            get { return _points.First(); }
        }

        public void Grow(TwoDPoint p)
        {
            _points.Enqueue(p);
        }

        //public void UnGrow()
        //{
        //    _points.RemoveAt(_points.Count - 1);
        //}

        public void Move(TwoDPoint p)
        {
            _points.Dequeue();
            _points.Enqueue(p);
        }

        public IEnumerable<TwoDPoint> Points{ get { return _points; } }
        
    }
}
