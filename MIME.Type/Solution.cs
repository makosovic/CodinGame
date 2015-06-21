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
        Dictionary<string, string> mimeTable = new Dictionary<string, string>();

        int N = int.Parse(Console.ReadLine()); // Number of elements which make up the association table.
        int Q = int.Parse(Console.ReadLine()); // Number Q of file names to be analyzed.
        for (int i = 0; i < N; i++)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            string EXT = inputs[0]; // file extension
            string MT = inputs[1]; // MIME type.
            mimeTable.Add(EXT.ToLower(), MT);
        }

        StringBuilder result = new StringBuilder();

        for (int i = 0; i < Q; i++)
        {
            string FNAME = Console.ReadLine(); // One file name per line.
            string[] split = FNAME.ToLower().Split('.');
            bool found = false;

            if (split.Length > 1)
            {
                foreach (var splitPart in split)
                {
                    string fileType;
                    if (mimeTable.TryGetValue(split[split.Length - 1], out fileType))
                    {
                        result.AppendLine(fileType);
                        found = true;
                        break;
                    }
                }
            }


            if (!found)
            {
                result.AppendLine("UNKNOWN");
            }
        }

        // Write an action using Console.WriteLine()
        // To debug: Console.Error.WriteLine("Debug messages...");

        Console.WriteLine(result.ToString()); // For each of the Q filenames, display on a line the corresponding MIME type. If there is no corresponding type, then display UNKNOWN.
    }
}