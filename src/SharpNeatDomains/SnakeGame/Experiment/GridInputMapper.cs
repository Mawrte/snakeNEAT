using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Phenomes;
using SharpNeat.Domains.SnakeGame.Core;

namespace SharpNeat.Domains.SnakeGame.Experiment
{
    class GridInputMapper : IInputMapper
    {
        readonly int _height;
        readonly int _width;
        readonly int _inputCount;

        static Dictionary<Tile, double> _tileToInput = new Dictionary<Tile, double>()
        {
            { Tile.empty , 0},
            { Tile.snake , 1},
            { Tile.food , -1},
            { Tile.wall, 0.5}
        };

        public static string Name
        {
            get { return "Grid"; }
        }

        public GridInputMapper(SimpleSnakeWorldParams sp)
        {
            _height = sp.Height;
            _width = sp.Width;
            _inputCount = _height * _width;
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
            int wi, hi;
            //_phenome.InputSignalArray.Reset();
            for (wi = 0; wi < SnakeWorld.Width; wi++)
            {
                for (hi = 0; hi < SnakeWorld.Height; hi++)
                {
                    Tile currTile = SnakeWorld[wi, hi];
                    //maybe enclose in a function to linearize...?
                    inputSignalArray[wi * SnakeWorld.Height + hi] = _tileToInput[currTile]; //HERES THE FOKKIN BUG 

                    //Console.WriteLine("_phenome.InputSignalArray[" + (wi * _sw.CurrHeight + hi) + "] = " + _tileToInput[currTile] + " - " + currTile);
                }
            }
        }
    }
}
