using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Phenomes;
using SharpNeat.Domains.SnakeGame.Core;

namespace SharpNeat.Domains.SnakeGame.Experiment
{
    interface IOutputMapper
    {
        int OutputCount
        {
            get;
        }
        void MapOutputs(ISignalArray outputSignalArray, SimpleSnakeWorld _sw);
    }
}
