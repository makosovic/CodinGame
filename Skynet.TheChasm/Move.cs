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
        int R = int.Parse(Console.ReadLine()); // the length of the road before the gap.
        int G = int.Parse(Console.ReadLine()); // the length of the gap.
        int L = int.Parse(Console.ReadLine()); // the length of the landing platform.
        bool jumped = false;

        // game loop
        while (true)
        {
            int S = int.Parse(Console.ReadLine()); // the motorbike's speed.
            int X = int.Parse(Console.ReadLine()); // the position on the road of the motorbike.
            int RNW = R - X;
            int targetS = G + 1;

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            if (RNW - targetS < 1 && !jumped)
            {
                Console.Error.WriteLine(RNW);
                Console.WriteLine("JUMP");
                jumped = true;
            }
            else if (jumped)
            {
                Console.WriteLine("SLOW");
            }
            else if (S > targetS)
            {
                Console.WriteLine("SLOW");
            }
            else if (S < targetS)
            {
                Console.WriteLine("SPEED");
            }
            else
            {
                Console.WriteLine("WAIT");
            }
            // A single line containing one of 4 keywords: SPEED, SLOW, JUMP, WAIT.
        }
    }
}