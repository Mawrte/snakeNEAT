    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Phenomes;
using SharpNeat.Domains.SnakeGame.Core;

namespace SharpNeat.Domains.SnakeGame.Experiment
{
    class MultipleOutputMapper : IOutputMapper
    {
        public static string Name
        {
            get { return "MultipleOutput"; }
        }

        static MultipleOutputMapper _instance = new MultipleOutputMapper();

        static Direction[] _intToDir = new Direction[] { Direction.Left, Direction.Right, Direction.Up, Direction.Down };

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
                return 4;
            }
        }

        public void MapOutputs(ISignalArray outputSignalArray, SimpleSnakeWorld sw)
        {
            int dirIndex = 0;
            double maxVal = double.MinValue;

            //lock (_consoleLock)
            {

                //Console.WriteLine("--------------------");
                //Console.WriteLine("outputSignalArray.Length = " + outputSignalArray.Length);

                for (int i = 0; i < outputSignalArray.Length; i++)
                {
                    //Console.WriteLine("outputSignalArray[" + i + "] (Direction " + _intToDir[i] + ") - val: " + outputSignalArray[i]);
                    if (outputSignalArray[i] > maxVal)
                    {
                        maxVal = outputSignalArray[i];
                        dirIndex = i;
                    }
                }


                //Console.WriteLine("CHOSEN: outputSignalArray[" + dirIndex + "] (Direction " + _intToDir[dirIndex] + ") - val: " + outputSignalArray[dirIndex] + " - " + maxVal);
                //Console.WriteLine("--------------------");

            }

             sw.NextDirection = _intToDir[dirIndex];
        }
    }
}
