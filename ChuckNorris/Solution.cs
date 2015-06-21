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
        string MESSAGE = Console.ReadLine();

        // Write an action using Console.WriteLine()
        // To debug: Console.Error.WriteLine("Debug messages...");

        string one = "0";
        string zero = "00";
        StringBuilder input = new StringBuilder();
        StringBuilder result = new StringBuilder();

        byte[] ASCIIValues = Encoding.ASCII.GetBytes(MESSAGE);
        foreach (byte b in ASCIIValues)
        {
            string binary = Convert.ToString(b, 2);
            if (binary.Length < 7)
            {
                string tmp = binary;
                binary = string.Empty;
                for (int i = 0; i < 7 - tmp.Length; i++)
                {
                    binary += "0";
                }
                binary += tmp;
            }
            input.Append(binary);
        }


        string inputString = input.ToString();

        char lastBit = inputString[0];
        int groupCount = 1;

        for (int i = 1; i < inputString.Length; i++)
        {
            if (inputString[i] == lastBit)
            {
                groupCount++;
            }
            else
            {
                result.Append(lastBit == '1' ? one : zero);
                result.Append(" ");
                for (int j = 0; j < groupCount; j++)
                {
                    result.Append("0");
                }
                result.Append(" ");

                lastBit = inputString[i];
                groupCount = 1;
            }
        }

        result.Append(lastBit == '1' ? one : zero);
        result.Append(" ");
        for (int j = 0; j < groupCount; j++)
        {
            result.Append("0");
        }


        Console.WriteLine(result.ToString());
    }
}