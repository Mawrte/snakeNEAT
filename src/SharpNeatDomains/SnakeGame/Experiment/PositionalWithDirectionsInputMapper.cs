using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Phenomes;
using SharpNeat.Domains.SnakeGame.Core;
using SharpNeat.Domains.SnakeGame.Utils;

namespace SharpNeat.Domains.SnakeGame.Experiment
{
    class PositionalWithDirectionsInputMapper : IInputMapper
    {
        readonly int _height;
        readonly int _width;
        readonly int _maxFood;
        readonly int _startLen;
        readonly int _inputCount;
        readonly int _foodCoordinatesNumber;

        readonly Dictionary<Direction, double> _dirToDouble = new Dictionary<Direction, double>()
        {
            { Direction.Left, -1 },
            { Direction.Up, -1d/3 },
            { Direction.Right, 1d/3 },
            { Direction.Down, 1 },
            { Direction.None, 0 },
        };

        public static string Name
        {
            get { return "PositionalWithDirections"; }
        }

        public PositionalWithDirectionsInputMapper(SimpleSnakeWorldParams sp)
        {
            _height = sp.Height;
            _width = sp.Width;
            _maxFood = sp.MaxFood;
            _startLen = sp.StartLen;
            _inputCount = _maxFood * 3 + _startLen * 2 + 1; //per ogni cibo 3 inputs = 2 per la posizione e 1 per la validità
            //forse da aggiungere la direzione dello snake come input...
            _foodCoordinatesNumber = _maxFood * 2;
        }

        public int InputCount
        {
            get
            {
                return _inputCount;
            }
        }

        public void MapInputs(ISignalArray inputSignalArray, SimpleSnakeWorld SnakeWorld)
        {
            IEnumerable<TwoDPoint> points = SnakeWorld.FoodPoints;

            int i;
            int elIndex;

            // count*2 perché devo contrare entrambe le coordinate
            for (i = 0; i < points.Count()*2;)
            {
                elIndex = i/2;
                inputSignalArray[i] = ((double) points.ElementAt(elIndex).X) / _width;
                i++;
                inputSignalArray[i] = ((double) points.ElementAt(elIndex).Y) / _height;
                i++;

            }


            for (i = 0; i < points.Count(); i++)
            {
                inputSignalArray[i + _foodCoordinatesNumber] = 1.0;
            }

            for (; i < _maxFood; i++)
            {
                inputSignalArray[i + _foodCoordinatesNumber] = -1.0;
            }

            points = SnakeWorld.GetSnakeHeadingPoints(_startLen);

            for (i = 0; i < _startLen*2;)
            {
                elIndex = i/2;
                inputSignalArray[i + _foodCoordinatesNumber + _maxFood] = ((double)points.ElementAt(elIndex).X) / _width; ;
                i++;
                inputSignalArray[i + _foodCoordinatesNumber + _maxFood] = ((double)points.ElementAt(elIndex).Y) / _height;
                i++;
            }

            inputSignalArray[_inputCount - 1] = _dirToDouble[SnakeWorld.SnakeDirection];
        }
    }
}
