using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 * ---
 * Hint: You can use the debug stream to print initialTX and initialTY, if Thor does not follow your orders.
 **/

public enum MovementType
{
    FourOptions,
    EightOptions
}

public class Point
{
    public int x { get; set; }
    public int y { get; set; }

    public Point(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
}

public class Move
{
    Point _source;
    Point _target;
    MovementType _movementType;

    public Point Position { get; set; }

    public Move(Point source, Point target, MovementType movementType)
    {
        _source = source;
        _target = target;
        _movementType = movementType;
    }

    public void Print()
    {
        Console.WriteLine(GetNextMove(_source, _target, _movementType));
    }

    private string GetNextMove(Point source, Point target, MovementType movementType)
    {
        if (movementType == MovementType.EightOptions)
        {
            if (target.x < source.x && target.y < source.y)
            {
                Position = new Point(source.x - 1, source.y - 1);
                return "NW";
            }

            if (target.x > source.x && target.y < source.y)
            {
                Position = new Point(source.x + 1, source.y - 1);
                return "NE";
            }

            if (target.x > source.x && target.y > source.y)
            {
                Position = new Point(source.x + 1, source.y + 1);
                return "SE";
            }

            if (target.x < source.x && target.y > source.y)
            {
                Position = new Point(source.x - 1, source.y + 1);
                return "SW";
            }

            return GetNextMoveFourOptions(source, target);
        }
        else if (movementType == MovementType.FourOptions)
        {
            return GetNextMoveFourOptions(source, target);
        }

        return string.Empty;
    }

    private string GetNextMoveFourOptions(Point source, Point target)
    {
        if (target.x == source.x && target.y < source.y)
        {
            Position = new Point(source.x, source.y - 1);
            return "N";
        }

        if (target.x == source.x && target.y > source.y)
        {
            Position = new Point(source.x, source.y + 1);
            return "S";
        }

        if (target.x < source.x)
        {
            Position = new Point(source.x - 1, source.y);
            return "W";
        }

        if (target.x > source.x)
        {
            Position = new Point(source.x + 1, source.y);
            return "E";
        }

        return string.Empty;
    }
}

class Player
{
    static void Main(string[] args)
    {
        string[] inputs = Console.ReadLine().Split(' ');
        int LX = int.Parse(inputs[0]); // the X position of the light of power
        int LY = int.Parse(inputs[1]); // the Y position of the light of power
        int initialTX = int.Parse(inputs[2]); // Thor's starting X position
        int initialTY = int.Parse(inputs[3]); // Thor's starting Y position

        // game loop
        while (true)
        {
            int E = int.Parse(Console.ReadLine()); // The level of Thor's remaining energy, representing the number of moves he can still make.

            Point thor = new Point(initialTX, initialTY);
            Point power = new Point(LX, LY);
            Move nextMove = new Move(thor, power, MovementType.EightOptions);
            nextMove.Print(); // A single line providing the move to be made: N NE E SE S SW W or NW
            initialTX = nextMove.Position.x;
            initialTY = nextMove.Position.y;
        }
    }
}