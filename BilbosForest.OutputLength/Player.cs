using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    static void Main(string[] args)
    {
        string magicPhrase = Console.ReadLine();

        Game game = new Game(new GameState());

        if (!string.IsNullOrEmpty(magicPhrase))
        {
            game.CreateNewStone(magicPhrase[0]);

            for (int i = 1; i < magicPhrase.Length; i++)
            {
                MoveFactory moveFactory = new MoveFactory();
                List<BaseMove> moves = new List<BaseMove>();

                foreach (MoveType moveType in Enum.GetValues(typeof(MoveType)))
                {
                    moves.AddRange(moveFactory.Create(moveType, game.State, magicPhrase[i]));
                }

                game.MakeMove(moves.FirstOrDefault(x => x.Cost == moves.Min(y => y.Cost)));
            }
        }

        Console.WriteLine(game.State.Output);
    }
}

#region moves

public enum MoveType
{
    CreateNew,
    FindExisting,
    ModifyExisting
}

public abstract class BaseMove
{
    protected readonly StringBuilder _output;
    protected MoveType _moveType;
    protected readonly GameState _gameState;

    public char NextChar { get; private set; }
    public MoveType MoveType { get { return _moveType; } }
    public string Output { get { return _output.ToString(); } }
    public int Cost { get { return _output.Length; } }
    public GameState GameState { get { return _gameState; } }

    protected BaseMove(GameState gameState, char nextChar)
    {
        _output = new StringBuilder();
        _gameState = new GameState { CurrentIndex = gameState.CurrentIndex };
        _gameState.Output.Append(gameState.Output);
        foreach (var stone in gameState.Stones)
        {
            _gameState.Stones.Add(stone);
        }
        NextChar = nextChar;
    }

    public abstract BaseMove Calculate();
}

public class CreateNewMove : BaseMove
{
    private readonly bool _doNotMove;

    public CreateNewMove(GameState gameState, char nextChar, bool doNotMove = false)
        : base(gameState, nextChar)
    {
        _moveType = MoveType.CreateNew;
        _doNotMove = doNotMove;
    }

    public override BaseMove Calculate()
    {
        _gameState.Stones.Add(NextChar);

        if (!_doNotMove)
        {
            int shifts = 0;
            for (int i = 0; i < _gameState.Stones.Count - 1 - _gameState.CurrentIndex; i++)
            {
                _output.Append('>');
                shifts++;
            }
            _gameState.CurrentIndex += shifts;
        }

        for (int i = 0; i < Array.IndexOf(Alphabet.Letters, NextChar); i++)
        {
            _output.Append('+');
        }

        _output.Append('.');

        return this;
    }
}

public class FindExistingMove : BaseMove
{
    public FindExistingMove(GameState gameState, char nextChar)
        : base(gameState, nextChar)
    {
        _moveType = MoveType.FindExisting;
    }

    public override BaseMove Calculate()
    {
        int index = _gameState.Stones.FindIndex(x => x == NextChar);

        if (index == -1)
        {
            _output.Append(Alphabet.LongString);
            return this;
        }

        if (index < _gameState.CurrentIndex)
        {
            int shifts = 0;
            for (int i = index; i < _gameState.CurrentIndex; i++)
            {
                _output.Append('<');
                shifts++;
            }
            _gameState.CurrentIndex -= shifts;
        }
        else if (index > _gameState.CurrentIndex)
        {
            int shifts = 0;
            for (int i = _gameState.CurrentIndex; i < index; i++)
            {
                _output.Append('>');
                shifts++;
            }
            _gameState.CurrentIndex += shifts;
        }

        _output.Append('.');

        return this;
    }
}

public class ModifyExistingMove : BaseMove
{
    public int CharToModifyIndex { get; set; }

    public ModifyExistingMove(GameState gameState, char nextChar, int charToModifyIndex)
        : base(gameState, nextChar)
    {
        _moveType = MoveType.ModifyExisting;
        CharToModifyIndex = charToModifyIndex;
    }

    public override BaseMove Calculate()
    {
        int diff = GameState.CurrentIndex - CharToModifyIndex;

        if (diff > 0)
        {
            if (diff > 14)
            {
                for (int i = 0; i < 27 - diff; i++)
                {
                   _output.Append('>');
                   _gameState.CurrentIndex = ((_gameState.CurrentIndex + 1)%27);                
                }       
            }     
            else
            {
                for (int i = 0; i < diff; i++)
                {
                    _output.Append('<');
                    _gameState.CurrentIndex--;            
                }
            }
        }
        else if (diff < 0)
        {
            if (diff < -14)
            {
                for (int i = 0; i < 27 - diff; i++)
                {
                   _output.Append('>');
                   _gameState.CurrentIndex = ((_gameState.CurrentIndex + 1)%27);                
                }       
            }     
            else
            {            
                for (int i = _gameState.CurrentIndex; i < CharToModifyIndex; i++)
            {
                _output.Append('>');
                _gameState.CurrentIndex++;
            }
        }

        int charDiff = Array.IndexOf(Alphabet.Letters, NextChar) - Array.IndexOf(Alphabet.Letters, _gameState.Stones[CharToModifyIndex]);

        if (charDiff > 0)
        {
            for (int i = 0; i < charDiff; i++)
            {
                _output.Append('+');
            }
        }
        else if (charDiff < 0)
        {
            for (int i = 0; i < Math.Abs(charDiff); i++)
            {
                _output.Append('-');
            }
        }

        _output.Append('.');

        return this;
    }
}

public class MoveFactory
{
    public List<BaseMove> Create(MoveType moveType, GameState gameState, char nextChar)
    {
        switch (moveType)
        {
            case MoveType.CreateNew:
                return new List<BaseMove> { new CreateNewMove(gameState, nextChar).Calculate() };
            case MoveType.FindExisting:
                return new List<BaseMove> { new FindExistingMove(gameState, nextChar).Calculate() };
            case MoveType.ModifyExisting:
                List<BaseMove> result = new List<BaseMove>();
                for (int i = 0; i < gameState.Stones.Count; i++)
                {
                    result.Add(new ModifyExistingMove(gameState, nextChar, i).Calculate());
                }
                return result;
            default:
                throw new NotImplementedException();
        }
    }
}

#endregion

public static class Alphabet
{
    public static char[] Letters = 
    {
        ' ', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W',
        'X', 'Y', 'Z'
    };

    public static string LongString =
        "                                                                                                                                                                                                     ";
}

public class Game
{
    private GameState _state;
    public GameState State { get { return _state; } }

    public Game(GameState gameState)
    {
        _state = gameState;
    }

    public void CreateNewStone(char c)
    {
        MakeMove(new CreateNewMove(_state, c, doNotMove: true).Calculate());
    }

    public void MakeMove(BaseMove move)
    {
        _state = move.GameState;
        _state.Output.Append(move.Output);
        
        if (move.MoveType == MoveType.ModifyExisting)
        {
            ModifyExistingMove modifyMove = (ModifyExistingMove) move;
            _state.Stones[modifyMove.CharToModifyIndex] = modifyMove.NextChar;
        }
    }
}

public class GameState
{
    public StringBuilder Output { get; set; }
    public List<char> Stones { get; set; }
    public int CurrentIndex { get; set; }

    public GameState()
    {
        Stones = new List<char>();
        Output = new StringBuilder();
    }
}
