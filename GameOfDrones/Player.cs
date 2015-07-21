using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Player
{

    #region abstract class game

    public class Point
    {
        public double x { get; set; }
        public double y { get; set; }

        public Point(double _x, double _y)
        {
            x = _x;
            y = _y;
        }
    }

    public class DinstanceFrom<TFrom, TTo>
        where TFrom : IPosition
        where TTo : IPosition
    {
        private readonly TFrom _fromObject;
        private readonly TTo _toObject;
        private readonly double _distance;

        public TFrom FromObject { get { return _fromObject; } }
        public TTo ToObject { get { return _toObject; } }
        public double Distance { get { return _distance; } }

        public DinstanceFrom(TFrom fromObject, TTo toObject)
        {
            _fromObject = fromObject;
            _toObject = toObject;
            _distance = CalculateDistance();
        }

        private double CalculateDistance()
        {
            double deltaX =
                Math.Abs(Math.Max(_fromObject.Position.x, _toObject.Position.x) -
                         Math.Min(_fromObject.Position.x, _toObject.Position.x));
            double deltaY =
                Math.Abs(Math.Max(_fromObject.Position.x, _toObject.Position.x) -
                         Math.Min(_fromObject.Position.x, _toObject.Position.x));
            return Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
        }
    }

    public interface IPosition
    {
        Point Position { get; }
    }

    public class Drone : IPosition
    {
        private int _id;

        public int Id { get { return _id; } }

        public Drone(int droneId)
        {
            _id = droneId;
        }

        public Point Position { get; set; }
    }

    public static class DroneExtensions
    {
        public static DinstanceFrom<TFrom, TTo> DistanceFrom<TFrom, TTo>(this TFrom drone, TTo zone)
            where TFrom : IPosition
            where TTo : IPosition
        {
            return new DinstanceFrom<TFrom, TTo>(drone, zone);
        }

        public static bool In(this Drone drone, Zone zone)
        {
            return
                drone.Position.x > zone.Center.x - Zone.SquareRadius &&
                drone.Position.x < zone.Center.x + Zone.SquareRadius &&
                drone.Position.y > zone.Center.y - Zone.SquareRadius &&
                drone.Position.y < zone.Center.y + Zone.SquareRadius;
        }
    }

    public class Zone : IPosition
    {
        public const int Radius = 100;
        public const double SquareRadius = 70.7106781;
        public const int NoOwnerId = -1;

        public Point Center { get; set; }
        public int OwnerId { get; set; }

        public Point Position
        {
            get { return Center; }
        }
    }

    public class Team
    {
        public Team(int droneCount, bool myTeam)
        {
            MyTeam = myTeam;
            Drones = new List<Drone>(droneCount);
            for (int droneId = 0; droneId < droneCount; droneId++)
            {
                Drones.Add(new Drone(droneId));
            }
        }

        public IList<Drone> Drones { get; private set; }
        public bool MyTeam { get; private set; }
    }

    public abstract class Game
    {
        private List<Zone> _zones;
        private List<Team> _teams;

        protected GameContext Context { get { return new GameContext(_zones, _teams); } }

        public void Init()
        {
            int[] pidz = ReadIntegers(Console.ReadLine());

            _zones = ReadZones(pidz[3]).ToList();
            _teams = ReadTeams(pidz[0], pidz[1], pidz[2]).ToList();
        }

        public void ReadContext()
        {
            foreach (Zone zone in _zones)
                zone.OwnerId = ReadIntegers(Console.ReadLine())[0];

            foreach (Team team in _teams) //TODO: optimize with .Where((team, id) => id != MyTeamId) but have to update your own position not read from server
                foreach (Drone drone in team.Drones)
                    drone.Position = ReadPoint(Console.ReadLine());
        }

        public abstract void Play();

        #region private methods

        private IEnumerable<Zone> ReadZones(int zoneCount)
        {
            for (int areaId = 0; areaId < zoneCount; areaId++)
                yield return new Zone { Center = ReadPoint(Console.ReadLine()) };
        }

        private IEnumerable<Team> ReadTeams(int teamCount, int myTeamIndex, int dronesPerTeam)
        {
            for (int teamId = 0; teamId < teamCount; teamId++)
                yield return new Team(dronesPerTeam, teamId == myTeamIndex);
        }

        private static int[] ReadIntegers(string consoleLine)
        {
            return Array.ConvertAll(consoleLine.Split(' '), int.Parse);
        }

        private static Point ReadPoint(string consoleLine)
        {
            int[] xy = ReadIntegers(consoleLine);
            return new Point(xy[0], xy[1]);
        }


        #endregion

    }

    public class GameContext
    {
        private readonly IList<Zone> _zones;
        private readonly IList<Team> _teams;
        private readonly int _myTeamId;

        public IList<Zone> Zones
        {
            get { return _zones; }
        }

        public IList<Team> Teams
        {
            get { return _teams; }
        }

        public int MyTeamId
        {
            get { return _myTeamId; }
        }

        public GameContext(IEnumerable<Zone> zones, IEnumerable<Team> teams)
        {
            _zones = new List<Zone>(zones);
            _teams = new List<Team>(teams);
            _myTeamId = _teams.Where(x => x.MyTeam).Select((team, index) => index).First();
        }
    }


    #endregion

    //#region objectives

    //public enum ObjectiveType
    //{
    //    Prevent,
    //    Attack,
    //    Defend,
    //    Conquer,
    //    Hold
    //}

    //public interface IObjective
    //{
    //    ObjectiveType Type { get; }
    //    Zone Zone { get; }
    //    IEnumerable<Drone> Resources { get; } 
    //}

    //public interface IObjectiveValue
    //{
    //    double Value { get; }
    //}

    //public class Objective : IObjective, IObjectiveValue
    //{
    //    private readonly Zone _zone;
    //    private readonly ObjectiveType _type;
    //    private readonly IEnumerable<Drone> _resources;

    //    public ObjectiveType Type
    //    {
    //        get { return _type; }
    //    }

    //    public Zone Zone
    //    {
    //        get { return _zone; }
    //    }

    //    public IEnumerable<Drone> Resources
    //    {
    //        get { return _resources; }
    //    }

    //    public double Value { get; set; }

    //    public Objective(Zone zone, ObjectiveType type, IEnumerable<Drone> resources)
    //    {
    //        _zone = zone;
    //        _type = type;
    //        _resources = resources;
    //    }
    //}

    //public class ObjectiveFactory
    //{
    //    public IList<Objective> Create(GameContext context)
    //    {
    //        IList<Objective> objectives = new List<Objective>();
    //        IList<Drone> myDrones = context.Teams.First(x => x.MyTeam).Drones;

    //        // hold
    //        foreach (Zone zone in context.Zones.Where(x => x.OwnerId == context.MyTeamId))
    //        {
    //            ICollection<Drone> drones = new List<Drone>();
    //            for (int i = myDrones.Count; i > 0; i--)
    //            {
    //                if (myDrones[i].In(zone))
    //                {
    //                    drones.Add(myDrones[i]);
    //                    myDrones.RemoveAt(i);
    //                }
    //            }
    //            objectives.Add(new Objective(zone, ObjectiveType.Hold, drones));
    //        }

    //        // conquer
    //        foreach (Zone zone in context.Zones.Where(x => x.OwnerId == Zone.NoOwnerId))
    //        {
    //            ICollection<Drone> top2ClosestDrones = myDrones.Select(drone => drone.DistanceFrom(zone)).OrderByDescending(x => x.Distance).Select(x => (x.FromObject)).Take(2).ToList();
    //            ICollection<Drone> drones = new List<Drone>();
    //            for (int i = myDrones.Count - 1; i >= 0; i--)
    //            {
    //                if (top2ClosestDrones.Any(x => x.Id == myDrones[i].Id))
    //                {
    //                    drones.Add(myDrones[i]);
    //                    myDrones.RemoveAt(i);
    //                }
    //            }
    //            objectives.Add(new Objective(zone, ObjectiveType.Conquer, drones));
    //        }

    //        return objectives;
    //    }
    //}

    //#endregion

    #region moves

    public class Move
    {
        private readonly int _index;
        private readonly Point _position;

        public int Index
        {
            get { return _index; }
        }

        public Point Position
        {
            get { return _position; }
        }

        public Move(int index, Point position)
        {
            _index = index;
            _position = position;
        }
    }

    public class MoveFactory
    {
        public List<Move> Create(GameContext context)
        {
            List<Move> moves = new List<Move>();
            IList<Drone> myDrones = context.Teams.First(x => x.MyTeam).Drones;

            // hold
            foreach (Zone zone in context.Zones.Where(x => x.OwnerId == context.MyTeamId))
            {
                for (int i = myDrones.Count; i > 0; i--)
                {
                    if (myDrones[i].In(zone))
                    {
                        moves.Add(new Move(myDrones[i].Id, zone.Position));
                        myDrones.RemoveAt(i);
                    }
                }
            }

            // conquer
            foreach (Zone zone in context.Zones.Where(x => x.OwnerId == Zone.NoOwnerId))
            {
                ICollection<Drone> top2ClosestDrones = myDrones.Select(drone => drone.DistanceFrom(zone)).OrderByDescending(x => x.Distance).Select(x => (x.FromObject)).Take(2).ToList();
                for (int i = myDrones.Count - 1; i >= 0; i--)
                {
                    if (top2ClosestDrones.Any(x => x.Id == myDrones[i].Id))
                    {
                        moves.Add(new Move(myDrones[i].Id, zone.Position));
                        myDrones.RemoveAt(i);
                    }
                }
            }

            return moves.OrderBy(x => x.Index).ToList();
        }
    }

    #endregion


    static class Program
    {
        static void Main()
        {
            Game game = new Hunt32Game();

            game.Init();

            while (true)
            {
                game.ReadContext();
                game.Play();
            }
        }
    }

    public class Hunt32Game : Game
    {
        public override void Play()
        {
            MoveFactory moveFactory = new MoveFactory();
            List<Move> moves = moveFactory.Create(this.Context);
            moves.ForEach(move => Console.WriteLine("{0} {1}", move.Position.x, move.Position.y));
        }
    }
}