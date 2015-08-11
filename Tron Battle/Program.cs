using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tron_Battle
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();

            while (true)
            {
                game.Initialize(Console.ReadLine().Split(' '));
                for (int i = 0; i < game.PlayerCount; i++)
                {
                    game.ReadPlayer(i, Console.ReadLine().Split(' '));
                }

                Move? move = null;
                Task<Move> task = Task.Factory.StartNew(() => game.ComputeMove());

                if (task.Wait(95))
                {
                    move = task.Result;
                }
                else
                {
                    IMoveFactory factory = new ValidMoveFactory();
                    move = factory.GetMove(game.Board, game.MyCurrentPosition);
                }
                Console.WriteLine(move);
            }
        }
    }

    #region game

    public enum GamePhase
    {
        Early,
        Middle,
        Late
    }

    public class Game
    {
        private bool _isInitialized;
        private readonly int?[,] _board;
        private readonly Position[] _currentPlayerPositions;
        private readonly List<int> _playersOutOfGame;
        private readonly GamePhase _phase;

        public int PlayerCount { get; set; }
        public int MyPlayerIx { get; set; }
        public GamePhase Phase { get; set; }
        public Position MyCurrentPosition { get { return _currentPlayerPositions[MyPlayerIx]; } }
        public int?[,] Board { get { return _board; } }

        public Game()
        {
            _board = new int?[20, 30];
            _currentPlayerPositions = new Position[4];
            _playersOutOfGame = new List<int>();
            _phase = GamePhase.Early;
        }

        public Game Initialize(string[] inputs)
        {
            if (!_isInitialized)
            {
                PlayerCount = int.Parse(inputs[0]);
                MyPlayerIx = int.Parse(inputs[1]);
                _isInitialized = true;
            }
            return this;
        }

        public Game ReadPlayer(int playerId, string[] inputs)
        {
            Position position = new Position(inputs[2], inputs[3]);

            _currentPlayerPositions[playerId] = position;

            if (position.IsReal)
            {
                _board[position.y, position.x] = playerId;
            }
            else
            {
                if (_playersOutOfGame.Any(x => x == playerId)) return this;

                ClearBoard(_board, playerId);
                _playersOutOfGame.Add(playerId);
            }

            return this;
        }

        private void ClearBoard(int?[,] board, int playerId)
        {
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    if (board[i, j] == playerId) board[i, j] = null;
                }
            }
        }

        public Move ComputeMove()
        {
            StrategyFactory factory = new StrategyFactory();
            IStrategy strategy = factory.GetStrategy(_phase);
            return strategy.GetNextMove(_board, _currentPlayerPositions, MyPlayerIx);
        }
    }

    #endregion

    #region move

    public enum Move
    {
        UP,
        RIGHT,
        DOWN,
        LEFT
    }

    public interface IMoveFactory
    {
        Move GetMove(int?[,] board, Position currentPlayerPosition);
    }

    public class ValidMoveFactory : IMoveFactory
    {
        public Move GetMove(int?[,] board, Position currentPlayerPosition)
        {
            MoveValidator validator = new MoveValidator();
            if (validator.IsValid(Move.UP, board, currentPlayerPosition))
                return Move.UP;
            if (validator.IsValid(Move.RIGHT, board, currentPlayerPosition))
                return Move.RIGHT;
            if (validator.IsValid(Move.DOWN, board, currentPlayerPosition))
                return Move.DOWN;

            return Move.LEFT;
        }
    }

    public class MoveValidator
    {
        public bool IsValid(Move move, int?[,] board, Position currentPlayerPosition)
        {
            if (move == Move.UP && currentPlayerPosition.y > 0 &&
                board[currentPlayerPosition.y - 1, currentPlayerPosition.x] == null) return true;
            if (move == Move.RIGHT && currentPlayerPosition.x < 29 &&
                board[currentPlayerPosition.y, currentPlayerPosition.x + 1] == null) return true;
            if (move == Move.DOWN && currentPlayerPosition.y < 19 &&
                board[currentPlayerPosition.y + 1, currentPlayerPosition.x] == null) return true;
            if (move == Move.LEFT && currentPlayerPosition.x > 0 &&
                board[currentPlayerPosition.y, currentPlayerPosition.x - 1] == null) return true;

            return false;
        }
    }

    #endregion

    #region strategy

    public class StrategyFactory
    {
        public IStrategy GetStrategy(GamePhase phase)
        {
            switch (phase)
            {
                case GamePhase.Early:
                    return new CaptureCenterStrategy();
                default:
                    return new TreeOfChambersStrategy();
            }
        }
    }

    public interface IStrategy
    {
        Move GetNextMove(int?[,] board, Position[] currentPlayerPositions, int myPlayerIx);
    }

    public abstract class BaseStrategy : IStrategy
    {
        public Move GetNextMove(int?[,] board, Position[] currentPlayerPositions, int myPlayerIx)
        {
            Move move = CalculateMove(board, currentPlayerPositions, myPlayerIx);

            MoveValidator validator = new MoveValidator();

            if (validator.IsValid(move, board, currentPlayerPositions[myPlayerIx]))
            {
                return move;
            }

            return GetValidMove(board, currentPlayerPositions[myPlayerIx]);
        }

        private Move GetValidMove(int?[,] board, Position currentPlayerPosition)
        {
            IMoveFactory factory = new ValidMoveFactory();
            return factory.GetMove(board, currentPlayerPosition);
        }

        public abstract Move CalculateMove(int?[,] board, Position[] currentPlayerPositions, int myPlayerIx);
    }

    public class TreeOfChambersStrategy : BaseStrategy
    {
        public override Move CalculateMove(int?[,] board, Position[] currentPlayerPositions, int myPlayerIx)
        {
            Thread.Sleep(250);
            //TODO: IMPLEMENT
            return Move.DOWN;
            throw new NotImplementedException();
        }
    }

    public class CaptureCenterStrategy : BaseStrategy
    {
        public override Move CalculateMove(int?[,] board, Position[] currentPlayerPositions, int myPlayerIx)
        {
            Thread.Sleep(250);
            //TODO: IMPLEMENT
            return Move.DOWN;
            throw new NotImplementedException();
        }
    }

    #endregion

    public class Position
    {
        public int x { get; set; }
        public int y { get; set; }
        public bool IsReal { get { return x != -1 && y != -1; } }

        public Position(string sx, string sy)
        {
            x = int.Parse(sx);
            y = int.Parse(sy);
        }
    }
}
