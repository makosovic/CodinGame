





using System;
using System.Collections.Generic;
using System.Linq;

namespace Player
{
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

        public Drone(int droneId)
        {
            Id = droneId;
        }
    }

    public class Zone
    {
        public const int NoOwnerId = -1;
        public const int Radius = 100;
        public const double SquareRadius = 70.7106781;

        public Point Center { get; set; }
        public int OwnerId { get; set; }
        public List<Drone> Drones { get; set; }

        public Zone()
        {
            Drones = new List<Drone>();
        }
    }

    public class Team
    {
        public Team(int droneCount)
        {
            Drones = new List<Drone>(droneCount);
            for (var droneId = 0; droneId < droneCount; droneId++)
                Drones.Add(new Drone(droneId));
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
            var pidz = ReadIntegers();

            MyTeamId = pidz[1];
            Zones = ReadZones(pidz[3]).ToList();
            Teams = ReadTeams(pidz[0], pidz[2]).ToList();
        }

        IEnumerable<Zone> ReadZones(int zoneCount)
        {
            for (var areaId = 0; areaId < zoneCount; areaId++)
                yield return new Zone { Center = ReadPoint() };
        }

        IEnumerable<Team> ReadTeams(int teamCount, int dronesPerTeam)
        {
            for (var teamId = 0; teamId < teamCount; teamId++)
                yield return new Team(dronesPerTeam);
        }

        public void ReadContext()
        {
            foreach (var zone in Zones)
            {
                zone.OwnerId = ReadIntegers()[0];
                zone.Drones = new List<Drone>();
            }

            foreach (var team in Teams)
            {
                foreach (var drone in team.Drones)
                {
                    drone.Position = ReadPoint();
                    foreach (var zone in Zones)
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
            var xy = ReadIntegers();
            return new Point { X = xy[0], Y = xy[1] };
        }
    }

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

            #region conquer
            int unoccupiedZones = Zones.Count(x => x.OwnerId == Zone.NoOwnerId);
            int n = unoccupiedZones == 0 ? 0 : (myDrones.Count / unoccupiedZones) + 1;
            foreach (Zone zone in Zones.Where(x => x.OwnerId == Zone.NoOwnerId))
            {
                List<int> topClosestDrones = myDrones.Select(drone => new KeyValuePair<int, double>(drone.Id, drone.DistanceFrom(zone))).ToList().OrderByDescending(x => x.Value).Select(x => x.Key).Take(n).ToList();
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

            #region attack
            int occupiedZones = Zones.Count(x => x.OwnerId != Zone.NoOwnerId && x.OwnerId != MyTeamId);
            int m = occupiedZones == 0 ? 0 : (myDrones.Count / occupiedZones) + 1;
            foreach (Zone zone in Zones.Where(x => x.OwnerId != Zone.NoOwnerId && x.OwnerId != MyTeamId))
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

            foreach (var key in moves.Keys)
            {
                Console.WriteLine("{0} {1}", moves[key].X, moves[key].Y);
            }
        }
    }
}