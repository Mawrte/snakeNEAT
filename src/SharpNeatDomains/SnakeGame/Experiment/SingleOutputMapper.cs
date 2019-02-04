using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Phenomes;
using SharpNeat.Domains.SnakeGame.Core;

namespace SharpNeat.Domains.SnakeGame.Experiment
{
    class SingleOutputMapper : IOutputMapper
    {
        static public string Name
        {
            get { return "SingleOutput"; }
        }

        static SingleOutputMapper _instance = new SingleOutputMapper();

        static Direction[] _intToDir = new Direction[] { Direction.Left, Direction.Right, Direction.Up, Direction.Down, Direction.Down };

        public IOutputMapper Instance
        {
            get
            {
                return _instance;
            }
        }

        public int OutputCount
        {
            get
            {
                return 1;
            }
        }

        public void MapOutputs(ISignalArray outputSignalArray, SimpleSnakeWorld sw)
        {
            //[-1,1] -> [0,4]
            double dirNum = (outputSignalArray[0] + 1) * 2;
            int dirIndex = (int)Math.Floor(dirNum);

            sw.NextDirection = _intToDir[dirIndex];
        }
    }
}
