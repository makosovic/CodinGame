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
class Solution
{
    static void Main(string[] args)
    {
        int N = int.Parse(Console.ReadLine()); // the number of temperatures to analyse
        string TEMPS = Console.ReadLine(); // the N temperatures expressed as integers ranging from -273 to 5526

        if (N != 0)
        {
            int[] tempsInt = TEMPS.Split(' ').Select(x => Convert.ToInt32(x)).ToArray();
            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            int amount = tempsInt.Select(x => Math.Abs(x)).Min();
            int result = tempsInt.Any(x => x == amount) ? amount : -amount;

            Console.WriteLine(result);
        }
        else
        {
            Console.WriteLine(0);
        }

    }
}