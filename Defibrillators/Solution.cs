using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/

class Coordinates
{
    public double LongDeg { get; set; }
    public double LatDeg { get; set; }

    public Coordinates(double longDeg, double latDeg)
    {
        LongDeg = longDeg;
        LatDeg = latDeg;
    }
}

class Defibrillator
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string ContactPhoneNumber { get; set; }
    public Coordinates Coordinates { get; set; }
    public double Distance { get; set; }

    public Defibrillator(int id, string name, string address, string contactPhoneNumber, double longDeg, double latDeg, double distance)
    {
        Id = id;
        Name = name;
        Address = address;
        ContactPhoneNumber = contactPhoneNumber;
        Coordinates = new Coordinates(longDeg, latDeg);
        Distance = distance;
    }
}

class Solution
{
    static void Main(string[] args)
    {
        string LON = Console.ReadLine();
        string LAT = Console.ReadLine();
        int N = int.Parse(Console.ReadLine());
        List<Defibrillator> defibrillators = new List<Defibrillator>(N);

        for (int i = 0; i < N; i++)
        {
            string DEFIB = Console.ReadLine();
            string[] split = DEFIB.Split(';');

            double defibLong = Convert.ToDouble(split[4].Replace(',', '.'));
            double defibLat = Convert.ToDouble(split[5].Replace(',', '.'));
            double userLong = Convert.ToDouble(LON.Replace(',', '.'));
            double userLat = Convert.ToDouble(LAT.Replace(',', '.'));

            double defibLongRad = ConvertToRad(defibLong);
            double defibLatRad = ConvertToRad(defibLat);
            double userLongRad = ConvertToRad(userLong);
            double userLatRad = ConvertToRad(userLat);

            double x = (defibLongRad - userLongRad) * Math.Cos((userLatRad + defibLatRad) / 2);
            double y = defibLatRad - userLatRad;
            double distance = Math.Sqrt(Math.Pow(x,2) + Math.Pow(y,2)) * 6371;
            
            defibrillators.Add(new Defibrillator(Convert.ToInt32(split[0]), split[1], split[2], split[3], defibLong, defibLat, distance));
        }

        // Write an action using Console.WriteLine()
        // To debug: Console.Error.WriteLine("Debug messages...");

        double minDistance = Double.MaxValue;
        string minName = string.Empty;
        foreach (var defibrillator in defibrillators)
        {
            if (defibrillator.Distance < minDistance)
            {
                minDistance = defibrillator.Distance;
                minName = defibrillator.Name;
            }
        }

        Console.WriteLine(minName);
    }

    static double ConvertToRad(double coordinate)
    {
        return coordinate*Math.PI/180;
    }
}