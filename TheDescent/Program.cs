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
class Player
{
    static void Main(string[] args)
    {
        Dictionary<int, int> heights = new Dictionary<int, int>
        {
            { 0, 0 },
            { 1, 0 },
            { 2, 0 },
            { 3, 0 },
            { 4, 0 },
            { 5, 0 },
            { 6, 0 },
            { 7, 0 }
        };
        // game loop
        while (true)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            int SX = int.Parse(inputs[0]);
            int SY = int.Parse(inputs[1]);
            bool shot = false;
            int? index = null;

            for (int i = 0; i < 8; i++)
            {
                int max = heights.Max(x => x.Value);
                if (max != 0)
                {
                    if (SY % 2 == 0)
                    {
                        index = heights.OrderBy(x => x.Key).Where(x => x.Value == max).First().Key;
                    }
                    else
                    {
                        index = heights.OrderByDescending(x => x.Key).Where(x => x.Value == max).First().Key;
                    }
                }

                int MH = int.Parse(Console.ReadLine()); // represents the height of one mountain, from 9 to 0. Mountain heights are provided from left to right.
                heights[i] = MH;
                if (MH == SY - 1 && SX == i)
                {
                    Console.WriteLine("FIRE");
                    shot = true;
                }


                if (index != null && !shot && SX == index && i == index)
                {
                    Console.WriteLine("FIRE");
                    shot = true;
                }
            }

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            if (!shot)
            {
                Console.WriteLine("HOLD"); // either:  FIRE (ship is firing its phase cannons) or HOLD (ship is not firing).
            }

        }
    }
}