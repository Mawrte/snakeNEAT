using SharpNeat.Phenomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Domains.SnakeGame.Core;

namespace SharpNeat.Domains.SnakeGame.Experiment
{
    class SnakeRunnerSmart
    {
        SimpleSnakeWorld _sw;
        int _msBetweenTicks;
        int _maxTicksWithoutScoreChange;
        bool _stop = false;

        IInputMapper _inputMapper;
        IOutputMapper _outputMapper;


        public SnakeRunnerSmart(SimpleSnakeWorld sw, IBlackBox phenome, IInputMapper inputMapper, IOutputMapper outputMapper, int _msBetweenTicks, int _maxTicksWithoutScoreChange)
        {
            this._sw = sw;
            this.Phenome = phenome;
            this._msBetweenTicks = _msBetweenTicks;
            this._maxTicksWithoutScoreChange = _maxTicksWithoutScoreChange;

            _inputMapper = inputMapper;
            _outputMapper = outputMapper;
        }

        public SimpleSnakeWorld SnakeWorld
        {
            get { return _sw; }
        }

        public int Ticks
        {
            get;
            private set;
        }

        public int Score
        {
            get;
            private set;
        }

        public bool Cutoff
        {
            get;
            private set;
        }

        public bool Win
        {
            get;
            private set;
        }

        public int TotalFoodDistance
        {
            get;
            private set;
        }

        public IBlackBox Phenome
        {
            get;
            set;
        }

        public void RunTrial()
        {
            _sw.Reset();
            PerformRun();
        }

        public void RunTrial(int seed)
        {
            _sw.Reset(seed);
            PerformRun();
        }

        void PerformRun()
        {
            Ticks = 0;
            Win = false;
            Cutoff = false;
            Phenome.ResetState();

            int lastTickScore = _sw.Score;
            int ticksWithoutScoreChange = 0;
            TotalFoodDistance = 0;

            while (_sw.CurrGameState == GameState.running && ticksWithoutScoreChange <= _maxTicksWithoutScoreChange && !_stop)
            {
                if (_msBetweenTicks > 0)
                {
                    System.Threading.Thread.Sleep(_msBetweenTicks);
                }

                _inputMapper.MapInputs(Phenome.InputSignalArray, _sw);

                Phenome.Activate();

                _outputMapper.MapOutputs(Phenome.OutputSignalArray, _sw);

                _sw.Tick();
                Ticks++;

                TotalFoodDistance = TotalFoodDistance + _sw.CurrentFoodDistance;

                if (_sw.Score == lastTickScore)
                {
                    ticksWithoutScoreChange++;
                }
                else
                {
                    ticksWithoutScoreChange = 0;
                }

                lastTickScore = _sw.Score;
            }

            Cutoff = ticksWithoutScoreChange >= _maxTicksWithoutScoreChange;
            _stop = false;
            Score = _sw.Score;
            Win = _sw.CurrGameState == GameState.win;
        }

        public void Stop()
        {
            _stop = true;
        }
    }
}
