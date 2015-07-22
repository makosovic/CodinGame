using System;
using System.Collections.Generic;
using System.Linq;

namespace Player
{

    #region skeleton
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public static bool operator ==(Point a, Point b)
        {
            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.X == b.X && a.Y == b.Y;
        }
        public static bool operator !=(Point a, Point b)
        {
            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(a, b))
            {
                return false;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return true;
            }

            return a.X != b.X || a.Y != b.Y;
        }
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
                (Math.Pow(drone.Position.X - zone.Center.X, 2) +
                 Math.Pow(drone.Position.Y - zone.Center.Y, 2)) < Math.Pow(Zone.Radius, 2);
        }

        public static bool InProximity(this Drone drone, Zone zone)
        {
            return
                (Math.Pow(drone.Position.X - zone.Center.X, 2) +
                 Math.Pow(drone.Position.Y - zone.Center.Y, 2)) < Math.Pow(Zone.ProximityRadius, 2);
        }
    }

    public class Drone
    {
        public int Id { get; set; }
        public Point Position { get; set; }
        public int TeamId { get; set; }
        public Objective Objective { get; set; }

        public Drone(int droneId, int teamId)
        {
            Id = droneId;
            TeamId = teamId;
        }
    }

    public class Zone
    {
        public const int NoOwnerId = -1;
        public const int Radius = 95;
        public const int ProximityRadius = 245;

        public Point Center { get; set; }
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public List<Drone> Drones { get; set; }
        public int DroneFrequency { get; set; }

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
                        if (drone.InProximity(zone))
                        {
                            zone.DroneFrequency++;
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
        private readonly int _cost;
        private readonly Zone _zone;
        private readonly ObjectiveType _type;

        public Objective(Zone zone, int myTeamId)
        {
            _zone = zone;
            _type = zone.OwnerId != myTeamId ? ObjectiveType.Aquire : ObjectiveType.Retain;
            _cost = CalculateValue(zone, myTeamId);
        }

        public int Cost { get { return _cost; } }
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
            int cost = _cost;

            if (_type == ObjectiveType.Retain)
            {
                for (int i = myDrones.Count - 1; i >= 0; i--)
                {
                    if (cost == 0) break;
                    results.Add(myDrones[i].Id);
                    myDrones.RemoveAt(i);
                    cost--;
                }
            }
            if (_type == ObjectiveType.Aquire)
            {
                results.AddRange(myDrones.OrderBy(x => x.DistanceFrom(_zone)).Take(_cost).Select(x => x.Id));
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

            #region hold
            foreach (Zone zone in Zones.Where(x => x.OwnerId == MyTeamId))
            {
                for (int i = myDrones.Count - 1; i >= 0; i--)
                {
                    if (myDrones[i].In(zone))
                    {
                        moves.Add(myDrones[i].Id, zone.Center);
                        myDrones.RemoveAt(i);
                    }
                }
            }
            #endregion

            #region attack
            int m = Teams.First().Drones.Count/Zones.Count + 1;
            foreach (Zone zone in Zones.Where(x => x.OwnerId != MyTeamId))
            {
                List<int> topClosestDrones = myDrones.Select(drone => new KeyValuePair<int, double>(drone.Id, drone.DistanceFrom(zone))).ToList().OrderByDescending(x => x.Value).Select(x => x.Key).Take(m).ToList();
                for (int i = myDrones.Count - 1; i >= 0; i--)
                {
                    if (topClosestDrones.Any(x => x == myDrones[i].Id))
                    {
                        moves.Add(myDrones[i].Id, zone.Center);
                        myDrones.RemoveAt(i);
                    }
                }
            }
            #endregion

            #region if any drones are not utilized send them to nearest objective
            foreach (Drone drone in myDrones)
            {
                double min = double.MaxValue;
                Point minPoint = new Point { X = 2000, Y = 900 };
                foreach (Zone zone in Zones)
                {
                    double tmpDistance = drone.DistanceFrom(zone);
                    if (tmpDistance < min)
                    {
                        min = tmpDistance;
                        minPoint = zone.Center;
                    }
                }

                moves.Add(drone.Id, minPoint);
            }
            #endregion

            foreach (var key in moves.Keys)
            {
                Console.WriteLine("{0} {1}", moves[key].X, moves[key].Y);
            }
        }
    }

}