using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpNeat.Domains.SnakeGame.Core
{
    enum Tile
    {
        empty,
        snake,
        food,
        wall
    }

    enum GameState
    {
        running,
        loss,
        win
    }

    class SimpleSnakeWorld
    {
        public readonly int MinHeight = 5;
        public readonly int MinWidth = 5;
        public readonly int MaxStartLength = 3;

        public EventHandler Ticked;


        Dictionary<Direction, Action> _getNextPosDict = new Dictionary<Direction, Action>();

        Direction _snakeDirection;

        Snake _snake;
        HashSet<TwoDPoint> _food = new HashSet<TwoDPoint>();
        HashSet<TwoDPoint> _emptyTiles = new HashSet<TwoDPoint>();
        TwoDPoint _nextPos = new TwoDPoint();

        int _ticksWithoutFood = 0;
        int _score = 0;

        Tile[,] _field;

        System.Random _rand = new System.Random(Guid.NewGuid().GetHashCode());

        private GameState _gameState;

        public GameState CurrGameState
        {
            get { return _gameState; }
        }

        public Tile this[int x, int y]
        {
            get { return _field[x, y];  }
        }

        public readonly int Height;

        public readonly int Width;

        public readonly int SnakeStartingLength;

        public readonly int TicksBetweenFood;

        public readonly int MaxFood;

        public readonly bool Infinite = false;

        public SimpleSnakeWorld(int height, int width, int startLen, int ticksBetweenFood, int maxFood)
        {
            if (width < MinWidth || height < MinHeight || startLen <= 0 || startLen > MaxStartLength || ticksBetweenFood < 0 || maxFood < 1)
            {
                throw new ArgumentException("SimpleSnakeWorld - constructor: some params are wrong");
            }

            Height = height;
            Width = width;
            SnakeStartingLength = startLen;
            TicksBetweenFood = ticksBetweenFood;
            MaxFood = maxFood;
        }

        public SimpleSnakeWorld(SimpleSnakeWorldParams p) : this(height: p.Height, width: p.Width, startLen: p.StartLen, ticksBetweenFood: p.TicksBetweenFood, maxFood: p.MaxFood)
        {
        }

        public void Seed(int seed)
        {
            _rand = new System.Random(seed);
        }

        public void Init(int seed)
        {
            _rand = new System.Random(seed);
            Init();
        }

        public void Init()
        {
            _field = new Tile[Width, Height];
            _getNextPosDict.Add(Direction.Left, () => _nextPos.X = _nextPos.X - 1);
            _getNextPosDict.Add(Direction.Right, () => _nextPos.X = _nextPos.X + 1);
            _getNextPosDict.Add(Direction.Up, () => _nextPos.Y = _nextPos.Y - 1);
            _getNextPosDict.Add(Direction.Down, () => _nextPos.Y = _nextPos.Y + 1);
            Reset();
        }

        private int NegativeModulus(int v, int m)
        {
            while (v < 0)
                v = v + m;
            return v % m;
        }

        public void Reset(int seed)
        {
            _rand = new System.Random(seed);
            Reset();
        }

        public void Reset()
        {
            _gameState = GameState.running;
            ResetScore();
            ResetTiles();
            PlaceSnake();
            TryPlaceFood();
            _snakeDirection = Direction.None;
            NextDirection = Direction.None;
        }

        private void ResetTiles()
        {
            _emptyTiles.Clear();
            _food.Clear();
            for (int i = 0; i < _field.GetLength(0); i++)
            {
                for (int j = 0; j < _field.GetLength(1); j++)
                {
                    _field[i, j] = Tile.empty;
                    _emptyTiles.Add(new TwoDPoint(i,j));
                }
            }
        }

        public IEnumerable<TwoDPoint> GetSnakeHeadingPoints(int num)
        {
            //debug
            //var pointsDebug = _snake.GetHeadingPoints(num);
            //var headDebug = _snake.Head;
            return _snake.GetHeadingPoints(num);
        }

        private void PlaceSnake()
        {
            _nextPos = RandomPoint(_emptyTiles);

            _snake = new Snake(_nextPos);

            DrawSnakeHeadOnTiles();

            for (int i = 0; i < SnakeStartingLength - 1; i++)
            {
                _nextPos = RandomPoint(GetAdjacent(_nextPos));
                _snake.Grow(_nextPos);
                DrawSnakeHeadOnTiles();
            }
        }

        private IEnumerable<TwoDPoint> GetAdjacent(TwoDPoint point)
        {
            return _emptyTiles.Where(p => p.ManhattanDistance(point) == 1);
        }

        private void DrawSnakeHeadOnTiles()
        {
            _field[_snake.Head.X, _snake.Head.Y] = Tile.snake;
            _emptyTiles.Remove(_snake.Head);
        }

        private void PlaceFood()
        {
            TwoDPoint fp = RandomPoint(_emptyTiles);

            _field[fp.X, fp.Y] = Tile.food;
            _emptyTiles.Remove(fp);

            _food.Add(fp);
        }

        public TwoDPoint RandomPoint(IEnumerable<TwoDPoint> feasiblePoints)
        {
            int index = _rand.Next(feasiblePoints.Count());
            return feasiblePoints.ElementAt(index);
        }

        private void ResetScore()
        {
            _score = 0;
        }

        public int Score
        {
            get { return _score; }
        }

        public Direction NextDirection
        {
            get;
            set;
        }

        public Direction SnakeDirection
        {
            get
            {
                return _snakeDirection;
            }
            private set
            {
                if (value != Direction.None && _snakeDirection.GetOpposite() != value)
                    _snakeDirection = value;
            }
        }

        public bool FullTiles
        {
            get
            {
                for (int i = 0; i < _field.GetLength(0); i++)
                {
                    for (int j = 0; j < _field.GetLength(1); j++)
                    {
                        if (_field[i, j] == Tile.empty || _field[i, j] == Tile.food)
                            return false;
                    }
                }
                return true;
            }
        }

        public void Tick()
        {
            if (_gameState != GameState.running)
            {
                Reset();
            }

            if(NextDirection != Direction.None)
                SnakeDirection = NextDirection;

            if (SnakeDirection == Direction.None)
            {
                OnTick();
                return;
            }

            _getNextPosDict[SnakeDirection]();

            if (Infinite)
            {
                _nextPos.X = NegativeModulus(_nextPos.X, Width);
                _nextPos.Y = NegativeModulus(_nextPos.Y, Height);
            }
            else if(isInvalid(_nextPos))
            {
                _gameState = GameState.loss;
                return;
            }

            if (_field[_nextPos.X, _nextPos.Y] == Tile.snake || _field[_nextPos.X, _nextPos.Y] == Tile.wall)
            {
                _gameState = GameState.loss;
                return;
            }

            MoveTo(_nextPos);

            if (FullTiles)
            {
                _gameState = GameState.win;
                return;
            }

            TryPlaceFood();

            OnTick();
        }

        private void TryPlaceFood()
        {
            if (_food.Count != MaxFood)
            {
                if (TicksBetweenFood == _ticksWithoutFood)
                {
                    _ticksWithoutFood = 0;
                    PlaceFood();
                }
                else
                {
                    _ticksWithoutFood++;
                }
            }
        }

        private bool isInvalid(TwoDPoint p)
        {
            return p.X < 0 || p.Y < 0 || p.X >= Width || p.Y >= Height;
        }

        private void OnTick()
        {
            if (Ticked != null)
                Ticked(this, EventArgs.Empty);
        }

        void MoveTo(TwoDPoint p)
        {
            if (_food.Contains(p))
            {
                _snake.Grow(p);
                _score++;
                _food.Remove(p);
            }
            else
            {
                _emptyTiles.Remove(p);
                _field[_snake.Tail.X, _snake.Tail.Y] = Tile.empty;
                _emptyTiles.Add(_snake.Tail);
                _snake.Move(p);
            }

            _field[p.X, p.Y] = Tile.snake;
        }

        public IEnumerable<TwoDPoint> SnakePoints
        {
            get
            {
                return _snake.Points;
            }
        }

        public IEnumerable<TwoDPoint> FoodPoints
        {
            get
            {
                return _food;
            }
        }

        public IEnumerable<TwoDPoint> OccupiedPoints
        {
            get
            {
                return SnakePoints.Concat(FoodPoints);
            }
        }

        public int CurrentFoodDistance
        {
            get
            {

                if (_food.Count == 0)
                {
                    return Height + Width;
                }

                int dist = 0;
                foreach (var p in _food)
                {
                    dist += _snake.Head.ManhattanDistance(p);
                }

                return dist;
            }
        }

        public SimpleSnakeWorld CloneInitialState()
        {
            return new SimpleSnakeWorld(height: Height, width: Width, startLen: SnakeStartingLength, ticksBetweenFood: TicksBetweenFood, maxFood: MaxFood);
        }
    }
}
