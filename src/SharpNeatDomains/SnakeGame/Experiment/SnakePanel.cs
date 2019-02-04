using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using SharpNeat.Genomes.Neat;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using SharpNeat.Domains.SnakeGame.Core;

namespace SharpNeat.Domains.SnakeGame.Experiment
{
    partial class SnakePanel : AbstractDomainView
    {
        static Dictionary<Tile, Brush> _tileToColor = new Dictionary<Tile, Brush>()
        {
            { Tile.empty, Brushes.White },
            { Tile.snake, Brushes.Black },
            { Tile.wall, Brushes.Blue },
            { Tile.food, Brushes.Red }
        };

        //SnakeGame paramteres
        //int _height = 30;
        //int _width = 30;
        int _scaleFactor = 15;

        SimpleSnakeWorld _sw;
        IGenomeDecoder<NeatGenome, IBlackBox> _genomeDecoder;
        IBlackBox _box;
        Thread _simThread;
        int _simRunningFlag = 0;
        AutoResetEvent _simStartEvent = new AutoResetEvent(false);
        SnakeRunnerSmart _snakeRunner;
        private bool _stopThread = false;

        public SnakePanel()
        {
            InitializeComponent();
            _simThread = new Thread(new ThreadStart(SimulationThread));
            _simThread.IsBackground = true;
            _simThread.Start();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.ParentForm.FormClosing += this.OnClosing;
        }
        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            //_stopThread = true;
            _snakeRunner.Stop();
            _simThread.Abort();//questo blocca anche se il thread SEMBRA uscire (ma in realtà non esce non se ne capisce il perché)
            //_stopThread = false;
        }

        public void Init(SnakeRunnerSmart snakeRunner, IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder)
        {
            _snakeRunner = snakeRunner;
            _sw = _snakeRunner.SnakeWorld;
            _genomeDecoder = genomeDecoder;
            this.Size = new Size((_sw.Width + 2) * _scaleFactor, (_sw.Height + 6) * _scaleFactor);
        }

        void ReDrawThreadSafe(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate {
                this.ReDraw(sender, e);
            });
        }

        private void ReDraw(object sender, EventArgs e)
        {
            if (this.IsDisposed)
                return;

            scoreLabel.Text = _sw.Score.ToString();

            Bitmap worldPicture = (Bitmap)pictureBox1.Image;
            DrawOnBitmap(worldPicture);
            pictureBox1.Image = worldPicture;

            this.Refresh();
        }

        void InitGraphics()
        {

            scoreLabel.Text = _sw.Score.ToString();

            pictureBox1.Size = new Size(_sw.Height, _sw.Width);
            //this.Controls.Add(pictureBox1);

            Bitmap worldPicture = new Bitmap(_sw.Width * _scaleFactor, _sw.Height * _scaleFactor);
            DrawOnBitmap(worldPicture);
            pictureBox1.Image = worldPicture;

            this.Refresh();

        }

        void DrawOnBitmap(Bitmap worldPicture)
        {
            Graphics flagGraphics = Graphics.FromImage(worldPicture);
            for (int wi = 0; wi < _sw.Width; wi++)
            {
                for (int hi = 0; hi < _sw.Height; hi++)
                {
                    flagGraphics.FillRectangle(_tileToColor[_sw[wi,hi]], wi * _scaleFactor, hi * _scaleFactor, _scaleFactor, _scaleFactor);
                }
            }
        }

        private void SnakePanel_Load(object sender, EventArgs e)
        {
            InitGraphics();
            _sw.Ticked += ReDrawThreadSafe;
        }
        
        private void SimulationThread()
        {
            try
            {
                // Wait for first agent to be passed in.
                _simStartEvent.WaitOne();
                while(!_stopThread)
                {

                    try
                    {
                        _snakeRunner.Phenome = _box;
                        _snakeRunner.RunTrial();
                    }
                    finally
                    {   // Simulation completed. Reset _simRunningFlag to allow another simulation to be started.
                        Interlocked.Exchange(ref _simRunningFlag, 0);
                    }
                }
            }
            catch (ThreadAbortException)
            {   // Thread abort exceptions are expected.
            }
        }

        public override void RefreshView(object genome)
        {

            // Zero indicates that the simulation is not currently running.
            if (0 == Interlocked.Exchange(ref _simRunningFlag, 1))
            {
                // We got the lock. Decode the genome and store resuly in an instance field.
                NeatGenome neatGenome = genome as NeatGenome;
                _box = _genomeDecoder.Decode(neatGenome);
                // Signal simulation thread to start running a simulation.
                _simStartEvent.Set();
            }

        }

        public override Size WindowSize
        {
            get
            {
                return this.Size;
            }
        }
    }
}
