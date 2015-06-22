using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Save the Planet.
 * Use less Fossil Fuel.
 **/

class Point
{
    public int x { get; set; }
    public int y { get; set; }

    public Point(int xx, int yy)
    {
        x = xx;
        y = yy;
    }
}

class Line
{
    public Point Start { get; set; }
    public Point End { get; set; }

    public Line(Point start, Point end)
    {
        Start = start;
        End = end;
    }

    public Line(int x1, int y1, int x2, int y2)
    {
        Start = new Point(x1, y1);
        End = new Point(x2, y2);
    }
}

struct Constants
{
    public static int AllowedHorizontalSpeed = 20;
    public static int AllowedVerticalSpeed = 40;
    public static int DistanceFromLandingStartCorrecting = 750;
    public static double AngleCorrectionFactor = 0.75;
    public static int DistanceFromFlatLandStart = 50;
}

class RoverPositioningSystem
{
    private readonly RoverSensorData _rsd;
    private readonly Line _landingSite;
    
    public RoverPositioningSystem(RoverSensorData rsd, Line landingSite)
    {
        _rsd = rsd;
        _landingSite = landingSite;
    }

    public string GetMove()
    {
        MoveFactory mf = new MoveFactory(_rsd, _landingSite);
        BaseMove move = mf.NextMove();
        move.Calculate();
        return move.ToString();
    }
}

class MoveFactory
{
    private readonly RoverSensorData _rsd;
    private readonly Line _flatLand;

    private readonly bool _isPositionedForLanding;
    private readonly bool _isHorizontalSpeedAcceptable;
    private readonly bool _isVerticalSpeedAcceptable;

    public MoveFactory(RoverSensorData rsd, Line flatLand)
    {
        _rsd = rsd;
        _flatLand = flatLand;

        _isPositionedForLanding = rsd.Position.x > _flatLand.Start.x && rsd.Position.x < _flatLand.End.x;
        _isHorizontalSpeedAcceptable = rsd.HorizonalSpeed <= Constants.AllowedHorizontalSpeed && rsd.RotationAngle == 0;
        _isVerticalSpeedAcceptable = rsd.VerticalSpeed <= Constants.AllowedVerticalSpeed;
    }

    public BaseMove NextMove()
    {
        if (!_isPositionedForLanding)
        {
            return new PositionForLanding(_rsd) { FlatLand = _flatLand };
        }
        if (!_isHorizontalSpeedAcceptable)
        {
            return new FixHorizontalSpeed(_rsd);
        }
        if (!_isVerticalSpeedAcceptable)
        {
            return new FixVerticalSpeed(_rsd);
        }

        return new FreeFall(_rsd);
    }
}

class PositionForLanding : BaseMove
{
    public override void Calculate()
    {
        if (Math.Abs(Math.Min(_rsd.Position.x - FlatLand.Start.x, _rsd.Position.x - FlatLand.End.x)) < Constants.DistanceFromLandingStartCorrecting)
        {
            _rotationAngle = Convert.ToInt32(_rsd.RotationAngle * Constants.AngleCorrectionFactor);
            _throtlePower = 4;
        }
        else if (_rsd.Position.x < FlatLand.Start.x)
        {
            _rotationAngle = -15;
            _throtlePower = 4;
        }
        else
        {
            _rotationAngle = 15;
            _throtlePower = 4;
        }
        Console.Error.WriteLine("PositionForLanding");
    }

    public PositionForLanding(RoverSensorData rsd)
        : base(rsd)
    {
    }
}

class FixHorizontalSpeed : BaseMove
{
    public override void Calculate()
    {
        if (_rsd.HorizonalSpeed > 0)
        {
            _rotationAngle = -Math.Abs(Convert.ToInt32(_rsd.RotationAngle*Constants.AngleCorrectionFactor));
        }
        else
        {
            _rotationAngle = Math.Abs(Convert.ToInt32(_rsd.RotationAngle * Constants.AngleCorrectionFactor));
        }
        _throtlePower = 3;
        
        Console.Error.WriteLine("FixHorizontalSpeed");
    }

    public FixHorizontalSpeed(RoverSensorData rsd)
        : base(rsd)
    {
    }
}

class FixVerticalSpeed : BaseMove
{
    public override void Calculate()
    {
        _rotationAngle = 0;
        _throtlePower = 4;
        Console.Error.WriteLine("FixVerticalSpeed");

    }

    public FixVerticalSpeed(RoverSensorData rsd)
        : base(rsd)
    {
    }
}

class FreeFall : BaseMove
{
    public override void Calculate()
    {
        _rotationAngle = 0;
        _throtlePower = 0;
        Console.Error.WriteLine("FreeFall");

    }

    public FreeFall(RoverSensorData rsd)
        : base(rsd)
    {
    }
}

abstract class BaseMove
{
    protected int _rotationAngle;
    protected int _throtlePower;

    protected RoverSensorData _rsd;

    public Line FlatLand { get; set; }

    protected BaseMove(RoverSensorData rsd)
    {
        _rsd = rsd;
    }

    public abstract void Calculate();
    public override string ToString()
    {
        return string.Format("{0} {1}", _rotationAngle, _throtlePower);
    }
}

class RoverSensorData
{
    public Point Position { get; set; }
    public int HorizonalSpeed { get; set; }
    public int VerticalSpeed { get; set; }
    public int RemainingFuel { get; set; }
    public int RotationAngle { get; set; }
    public int ThrustPower { get; set; }

    public RoverSensorData(Point position, int horizonalSpeed, int verticalSpeed, int remainingFuel, int rotationAngle, int thrustPower)
    {
        Position = position;
        HorizonalSpeed = horizonalSpeed;
        VerticalSpeed = verticalSpeed;
        RemainingFuel = remainingFuel;
        RotationAngle = rotationAngle;
        ThrustPower = thrustPower;
    }
}

class Player
{
    static void Main(string[] args)
    {
        string[] inputs;
        int N = int.Parse(Console.ReadLine()); // the number of points used to draw the surface of Mars.

        inputs = Console.ReadLine().Split(' ');
        Point lastLandPoint = new Point(int.Parse(inputs[0]), int.Parse(inputs[1]));
        Line flatLand = null;

        for (int i = 1; i < N; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            Point landPoint = new Point(int.Parse(inputs[0]), int.Parse(inputs[1]));

            if (lastLandPoint.y == landPoint.y)
            {
                flatLand = new Line(lastLandPoint, landPoint);
            }

            lastLandPoint = landPoint;
        }

        if (flatLand == null)
        {
            throw new ArgumentException("No flat land!");
        }

        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            RoverSensorData rsd = new RoverSensorData(
                new Point(int.Parse(inputs[0]), int.Parse(inputs[1])),
                int.Parse(inputs[2]), // the horizontal speed (in m/s), can be negative.
                int.Parse(inputs[3]), // the vertical speed (in m/s), can be negative.
                int.Parse(inputs[4]), // the quantity of remaining fuel in liters.
                int.Parse(inputs[5]), // the rotation angle in degrees (-90 to 90).
                int.Parse(inputs[6])
                );

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            // R P. R is the desired rotation angle. P is the desired thrust power.
            RoverPositioningSystem rps = new RoverPositioningSystem(rsd, flatLand);
            Console.WriteLine(rps.GetMove());
        }
    }
}

