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
        int N = int.Parse(Console.ReadLine());
        int[] P = new int[N];
        for (int i = 0; i < N; i++)
        {
            P[i] = int.Parse(Console.ReadLine());
        }

        Array.Sort(P);
        int min = P[1] - P[0];
        for (int i = 2; i != P.Length; i++)
        {
            min = Math.Min(min, P[i] - P[i - 1]);
        }

        // Write an action using Console.WriteLine()
        // To debug: Console.Error.WriteLine("Debug messages...");

        Console.WriteLine(min);
    }
}