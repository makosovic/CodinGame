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
        int L = int.Parse(Console.ReadLine());
        int H = int.Parse(Console.ReadLine());

        string[,] results = new string[H, 27];
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ?";

        string T = Console.ReadLine();
        for (int i = 0; i < H; i++)
        {
            string ROW = Console.ReadLine();
            for (int j = 0; j < alphabet.Length; j++)
            {
                results[i, j] = ROW.Substring(j * L, L);
            }
        }

        // Write an action using Console.WriteLine()
        // To debug: Console.Error.WriteLine("Debug messages...");

        for (int i = 0; i < H; i++)
        {
            string result = string.Empty;
            foreach (var letter in T)
            {
                int index = Array.IndexOf(alphabet.ToCharArray(), Char.ToUpper(letter));
                if (index == -1)
                {
                    index = 26;
                }
                result += results[i, index];
            }
            Console.WriteLine(result);
        }
    }
}