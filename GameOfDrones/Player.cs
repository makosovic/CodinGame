using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Player
{

    #region skeleton
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public static class DroneExtensions
    {
        public static double DistanceFrom(this Drone drone, Zone zone)
        {
            double deltaX =
                Math.Abs(Math.Max(drone.Position.X, zone.Center.Y) -
                         Math.Min(drone.Position.X, zone.Center.Y));
            double deltaY =
                Math.Abs(Math.Max(drone.Position.X, zone.Center.Y) -
                         Math.Min(drone.Position.X, zone.Center.Y));
            return Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
        }

        public static bool In(this Drone drone, Zone zone)
        {
            return
                drone.Position.X > zone.Center.X - Zone.SquareRadius &&
                drone.Position.X < zone.Center.X + Zone.SquareRadius &&
                drone.Position.Y > zone.Center.Y - Zone.SquareRadius &&
                drone.Position.Y < zone.Center.Y + Zone.SquareRadius;
        }
    }

    public class Drone
    {
        public int Id { get; set; }
        public Point Position { get; set; }
        public int TeamId { get; set; }

        public Drone(int droneId, int teamId)
        {
            Id = droneId;
            TeamId = teamId;
        }
    }

    public class Zone
    {
        public const int NoOwnerId = -1;
        public const int Radius = 100;
        public const double SquareRadius = 70.7106781;

        public Point Center { get; set; }
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public List<Drone> Drones { get; set; }

        public Zone()
        {
            Drones = new List<Drone>();
        }
    }

    public class Team
    {
        public Team(int droneCount, int teamId)
        {
            Drones = new List<Drone>(droneCount);
            for (int droneId = 0; droneId < droneCount; droneId++)
                Drones.Add(new Drone(droneId, teamId));
        }

        public IList<Drone> Drones { get; private set; }
    }

    public abstract class Game
    {
        protected List<Zone> Zones; // all game zones
        protected List<Team> Teams; // all the team of drones. Array index = team's ID
        protected int MyTeamId; // index of my team in the array of teams

        // read initial games data (one time at the beginning of the game: P I D Z...)
        public void Init()
        {
            int[] pidz = ReadIntegers();

            MyTeamId = pidz[1];
            Zones = ReadZones(pidz[3]).ToList();
            Teams = ReadTeams(pidz[0], pidz[2]).ToList();
        }

        IEnumerable<Zone> ReadZones(int zoneCount)
        {
            for (int areaId = 0; areaId < zoneCount; areaId++)
                yield return new Zone { Id = areaId, Center = ReadPoint() };
        }

        IEnumerable<Team> ReadTeams(int teamCount, int dronesPerTeam)
        {
            for (int teamId = 0; teamId < teamCount; teamId++)
                yield return new Team(dronesPerTeam, teamId);
        }

        public void ReadContext()
        {
            foreach (Zone zone in Zones)
            {
                zone.OwnerId = ReadIntegers()[0];
                zone.Drones = new List<Drone>();
            }

            foreach (Team team in Teams)
            {
                foreach (Drone drone in team.Drones)
                {
                    drone.Position = ReadPoint();
                    foreach (Zone zone in Zones)
                    {
                        if (drone.In(zone))
                        {
                            zone.Drones.Add(drone);
                        }
                    }
                }
            }

        }

        // Compute logic here. This method is called for each game round. 
        public abstract void Play();

        static int[] ReadIntegers()
        {
            // ReSharper disable once PossibleNullReferenceException
            return Console.ReadLine().Split(' ').Select(int.Parse).ToArray();
        }

        static Point ReadPoint()
        {
            int[] xy = ReadIntegers();
            return new Point { X = xy[0], Y = xy[1] };
        }
    }

    #endregion

    #region objective
    public enum ObjectiveType
    {
        Retain,
        Aquire
    }

    public class Objective
    {
        private readonly int _value;
        private readonly Zone _zone;
        private readonly ObjectiveType _type;

        public Objective(Zone zone, int myTeamId)
        {
            _zone = zone;
            _type = zone.OwnerId != myTeamId ? ObjectiveType.Aquire : ObjectiveType.Retain;
            _value = CalculateValue(zone, myTeamId);
        }

        public int Value { get { return _value; } }
        public Zone Zone { get { return _zone; } }
        public ObjectiveType Type { get { return _type; } }

        private int CalculateValue(Zone zone, int myTeamId)
        {
            int max = zone.Drones.GroupBy(x => x.TeamId).Select(group => group.Count()).Concat(new[] { 0 }).Max();
            int mine = zone.Drones.Count(x => x.TeamId == myTeamId);

            if (_type == ObjectiveType.Aquire)
            {
                return max - mine + 1;
            }
            if (_type == ObjectiveType.Retain)
            {
                return max;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public int[] FindBestCandidates(IList<Drone> myDrones)
        {
            List<int> results = new List<int>();
            int value = _value;

            if (_type == ObjectiveType.Retain)
            {
                for (int i = myDrones.Count - 1; i >= 0; i--)
                {
                    if (value == 0) break;


                    results.Add(myDrones[i].Id);
                    myDrones.RemoveAt(i);
                    value--;
                }
            }
            if (_type == ObjectiveType.Aquire)
            {
                results.AddRange(myDrones.OrderBy(x => x.DistanceFrom(_zone)).Take(_value).Select(x => x.Id));
                foreach (int id in results)
                {
                    myDrones.Remove(myDrones.First(x => x.Id == id));
                }
            }

            return results.ToArray();
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

    internal class Hunt32Game : Game
    {
        public override void Play()
        {
            Dictionary<int, Point> moves = new Dictionary<int, Point>();
            IList<Drone> myDrones = new List<Drone>(Teams[MyTeamId].Drones);

            IList<Objective> objectives = Zones.Select(zone => new Objective(zone, MyTeamId)).OrderBy(x => x.Value).ToList();

            foreach (Objective objective in objectives)
            {
                int[] ids = objective.FindBestCandidates(myDrones);
                foreach (int id in ids)
                {
                    moves.Add(id, objective.Zone.Center);
                }
            }

            #region if any drones are not utilized send them to nearest objective
            foreach (Drone drone in myDrones)
            {
                double min = double.MaxValue;
                Point minPoint = new Point { X = 2000, Y = 900};
                foreach (var objective in objectives)
                {
                    double tmpDistance = drone.DistanceFrom(objective.Zone);
                    if (tmpDistance < min)
                    {
                        min = tmpDistance;
                        minPoint = objective.Zone.Center;
                    }
                }

                moves.Add(drone.Id, minPoint);
            }
            #endregion

            foreach (int key in moves.Keys)
            {
                Console.WriteLine("{0} {1}", moves[key].X, moves[key].Y);
            }
        }

        private Point GetRandomObjectivePoint(IList<Objective> objectives)
        {
            Random rnd = new Random();
            int index = rnd.Next(0, objectives.Count - 1);
            return objectives[index].Zone.Center;
        }
    }

}