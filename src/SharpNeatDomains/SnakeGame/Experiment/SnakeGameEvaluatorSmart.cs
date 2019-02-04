using System;
using SharpNeat.Core;
using SharpNeat.Domains.SnakeGame.Core;
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.SnakeGame.Experiment
{
    class SnakeGameEvaluatorSmart : IPhenomeEvaluator<IBlackBox>
    {
        int _wins = 0;
        private SimpleSnakeWorldParams _swparams;
        private IInputMapper _inputMapper;
        private IOutputMapper _outputMapper;
        private int _trialsPerEvaluation;
        private int _maxTicksWithoutEating;

        public SnakeGameEvaluatorSmart(SimpleSnakeWorldParams _swparams, IInputMapper _inputMapper, IOutputMapper _outputMapper, int _trialsPerEvaluation, int _maxTicksWithoutEating)
        {
            this._swparams = _swparams;
            this._inputMapper = _inputMapper;
            this._outputMapper = _outputMapper;
            this._trialsPerEvaluation = _trialsPerEvaluation;
            this._maxTicksWithoutEating = _maxTicksWithoutEating;
        }

        public ulong EvaluationCount
        {
            get;
            private set;
        }

        public bool StopConditionSatisfied
        {
            get { return _wins > 1; }
        }

        public FitnessInfo Evaluate(IBlackBox phenome)
        {

            SimpleSnakeWorld sw = new SimpleSnakeWorld(_swparams);
            sw.Init();
            SnakeRunnerSmart sr = new SnakeRunnerSmart(sw, phenome, _inputMapper, _outputMapper, 0, _maxTicksWithoutEating);

            //SimpleSnakeWorld clone = _srf.GetWorldClone();
            //clone.Init();
            //SnakeRunner sr = new SnakeRunnerWxH(clone, phenome, 0, _maxTicksWithoutScoreChange);
            

            int totalSquaredScore = 0;
            int totalScore = 0;
            int totalTicks = 0;
            double inverseOfMediumFoodDistance = 0;
            _wins = 0;
            int cutoffs = 0;
            for (int runs = 0; runs < _trialsPerEvaluation; runs++)
            {
                sr.RunTrial(runs);
                totalSquaredScore += sr.Score * sr.Score; //was totalScore += sr.Score; changed to emphasize large scores over lower scores
                totalScore += sr.Score;
                totalTicks += sr.Ticks;
                //inverseOfMediumFoodDistance += sr.Score / (double) sr.TotalFoodDistance;
                if (sr.Win)
                    _wins++;
                //if (sr.Cutoff)
                //    cutoffs++;
            }

            EvaluationCount++;
            return new FitnessInfo(totalScore, totalSquaredScore); //probably a good one
            //return new FitnessInfo(totalSquaredScore, totalScore);
            //return new FitnessInfo(totalScore, Double.MaxValue/(totalSquaredScore+1));
            //return new FitnessInfo(totalScore, Double.MaxValue / (totalSquaredScore - totalScore + 1)); //probably a good one
            //TODO: magari cambiare la fitness...
            //return new FitnessInfo((double)(totalSquaredScore + totalScore*totalScore)/1024, totalScore);
            //return new FitnessInfo(phenome.GetHashCode(), phenome.GetHashCode()); //prova
        }

        public void Reset()
        {
            EvaluationCount = 0;
            _wins = 0;
        }
    }
}