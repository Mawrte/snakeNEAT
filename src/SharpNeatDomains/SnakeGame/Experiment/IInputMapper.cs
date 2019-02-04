using SharpNeat.Domains.SnakeGame.Core;
using SharpNeat.Phenomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Domains.SnakeGame.Experiment
{
    interface IInputMapper
    {
        int InputCount
        {
            get;
        }

        void MapInputs(ISignalArray inputSignalArray, SimpleSnakeWorld sw);
    }
}
