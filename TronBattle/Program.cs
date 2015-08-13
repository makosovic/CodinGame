using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace TronBattle
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

                if (task.Wait(90))
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
        private readonly List<Position> _currentPlayerPositions;
        private readonly List<int> _playersOutOfGame;
        private GamePhase _phase;

        public int PlayerCount { get; set; }
        public int MyPlayerIx { get; set; }
        public GamePhase Phase { get; set; }
        public Position MyCurrentPosition { get { return _currentPlayerPositions[MyPlayerIx]; } }
        public int?[,] Board { get { return _board; } }

        public Game()
        {
            _board = new int?[20, 30];
            _currentPlayerPositions = new List<Position>();
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
            Position startingPosition = new Position(inputs[0], inputs[1]);
            Position position = new Position(inputs[2], inputs[3]);

            if (_currentPlayerPositions.Count == PlayerCount)
            {
                _currentPlayerPositions[playerId] = position;
            }
            else
            {
                _currentPlayerPositions.Add(position);
            }

            if (position.IsReal)
            {
                _board[position.y, position.x] = playerId;
                _board[startingPosition.y, startingPosition.x] = playerId;
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

    #region board

    public class BoardManager
    {
        public BoardManager(ref int?[,] board)
        {
            Board = board;
        }

        public BoardManager(ref int?[,] board, ref List<Position> currentPlayerPositions)
        {
            Board = board;
            CurrentPlayerPositions = currentPlayerPositions;
        }

        public List<Position> CurrentPlayerPositions { get; set; }
        public int?[,] Board { get; set; }

        public bool ExecuteMove(Move move, int playerIx)
        {
            Position currentPosition = CurrentPlayerPositions[playerIx];
            return ExecuteMove(move, currentPosition);
        }

        public bool ExecuteMove(Move move, Position position)
        {
            MoveValidator validator = new MoveValidator();
            switch (move)
            {
                case Move.UP:
                    if (!validator.IsValid(Move.UP, Board, position)) return false;
                    position.y--;
                    Board[position.y, position.x] = Board[position.y + 1, position.x];
                    return true;
                case Move.RIGHT:
                    if (!validator.IsValid(Move.RIGHT, Board, position)) return false;
                    position.x++;
                    Board[position.y, position.x] = Board[position.y, position.x - 1];
                    return true;
                case Move.DOWN:
                    if (!validator.IsValid(Move.DOWN, Board, position)) return false;
                    position.y++;
                    Board[position.y, position.x] = Board[position.y - 1, position.x];
                    return true;
                case Move.LEFT:
                    if (!validator.IsValid(Move.LEFT, Board, position)) return false;
                    position.x--;
                    Board[position.y, position.x] = Board[position.y, position.x + 1];
                    return true;
                default:
                    throw new NotImplementedException();
            }
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

    public class RandomMoveFactory : IMoveFactory
    {
        public Move GetMove(int?[,] board, Position currentPlayerPosition)
        {
            Random rnd = new Random();
            return (Move)rnd.Next(0, 3);
        }
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
            if (move == Move.UP
                && currentPlayerPosition.x >= 0
                && currentPlayerPosition.y > 0
                && board[currentPlayerPosition.y - 1, currentPlayerPosition.x] == null)
                return true;
            if (move == Move.RIGHT
                && currentPlayerPosition.x >= 0
                && currentPlayerPosition.y >= 0
                && currentPlayerPosition.x < 29
                && board[currentPlayerPosition.y, currentPlayerPosition.x + 1] == null)
                return true;
            if (move == Move.DOWN
                && currentPlayerPosition.x >= 0
                && currentPlayerPosition.y >= 0
                && currentPlayerPosition.y < 19
                && board[currentPlayerPosition.y + 1, currentPlayerPosition.x] == null)
                return true;
            if (move == Move.LEFT
                && currentPlayerPosition.x > 0
                && currentPlayerPosition.y >= 0
                && board[currentPlayerPosition.y, currentPlayerPosition.x - 1] == null) return true;

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
                default:
                    return new VornoiStrategy();
            }
        }
    }

    public interface IStrategy
    {
        Move GetNextMove(int?[,] board, List<Position> currentPlayerPositions, int myPlayerIx);
    }

    public abstract class BaseStrategy : IStrategy
    {
        public Move GetNextMove(int?[,] board, List<Position> currentPlayerPositions, int myPlayerIx)
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

        public abstract Move CalculateMove(int?[,] board, List<Position> currentPlayerPositions, int myPlayerIx);
    }

    public class VornoiStrategy : BaseStrategy
    {
        public override Move CalculateMove(int?[,] board, List<Position> currentPlayerPositions, int myPlayerIx)
        {
            int maxVornoi = 0;
            Move? bestMove = null;
            IMoveFactory factory = new RandomMoveFactory();

            foreach (Move move in Enum.GetValues(typeof(Move)))
            {
                int?[,] nextBoard = board.DeepClone();
                List<Position> nextCurrentPlayerPositions = currentPlayerPositions.DeepClone();
                BoardManager manager = new BoardManager(ref nextBoard, ref nextCurrentPlayerPositions);
                MoveValidator validator = new MoveValidator();

                // execute random moves on behalf of opponents
                for (int i = 0; i < nextCurrentPlayerPositions.Count; i++)
                {
                    if (i != myPlayerIx && nextCurrentPlayerPositions[i].x != -1)
                    {
                        while (!manager.ExecuteMove(factory.GetMove(nextBoard, nextCurrentPlayerPositions[i]), i)) { }
                    }
                }

                // execute my move
                if (manager.ExecuteMove(move, myPlayerIx))
                {
                    bool isDeadEnd = IsDeadEnd(nextBoard.DeepClone(), nextCurrentPlayerPositions[myPlayerIx].DeepClone());
                    int vornoiCount = CalculateVornoi(nextBoard, new Queue<Position>(nextCurrentPlayerPositions), myPlayerIx);

                    if (vornoiCount > maxVornoi && !isDeadEnd)
                    {
                        maxVornoi = vornoiCount;
                        bestMove = move;
                    }
                }
            }

            return bestMove ?? Move.UP;
        }

        private bool IsDeadEnd(int?[,] board, Position position)
        {
            int validPositions = 0;
            Position madeMove = null;
            BoardManager manager = new BoardManager(ref board);

            foreach (Move move in Enum.GetValues(typeof(Move)))
            {
                Position newposition = position.DeepClone();
                if (manager.ExecuteMove(move, newposition))
                {
                    validPositions++;
                    madeMove = newposition;
                }
            }

            switch (validPositions)
            {
                case 0:
                    return true;
                case 1:
                    return IsDeadEnd(board.DeepClone(), madeMove);
                default:
                    return false;
            }
        }

        private int CalculateVornoi(int?[,] board, Queue<Position> positionsToVisit, int myPlayerIx)
        {
            BoardManager manager = new BoardManager(ref board);
            while (positionsToVisit.Count > 0)
            {
                Position position = positionsToVisit.Dequeue();
                foreach (Move move in Enum.GetValues(typeof(Move)))
                {
                    if (manager.ExecuteMove(move, position))
                    {
                        positionsToVisit.Enqueue(position);
                    }
                }
            }

            return CountPositions(board, myPlayerIx);
        }

        private int CountPositions(int?[,] board, int myPlayerIx)
        {
            int sum = 0;
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    if (board[i, j] == myPlayerIx) sum++;
                }
            }
            return sum;
        }
    }

    public class TreeOfChambersStrategy : BaseStrategy
    {
        public override Move CalculateMove(int?[,] board, List<Position> currentPlayerPositions, int myPlayerIx)
        {
            Thread.Sleep(250);
            throw new NotImplementedException();
        }
    }

    #endregion

    [Serializable]
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

        public Position(int ix, int iy)
        {
            x = ix;
            y = iy;
        }
    }

    public static class GenericHelpers
    {
        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
