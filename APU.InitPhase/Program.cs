using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Don't let the machines win. You are humanity's last hope...
 **/
class Player
{
    static void Main(string[] args)
    {
        int width = int.Parse(Console.ReadLine()); // the number of cells on the X axis
        int height = int.Parse(Console.ReadLine()); // the number of cells on the Y axis

        string[] grid = new string[height];

        for (int i = 0; i < height; i++)
        {
            grid[i] = Console.ReadLine(); // width characters, each either 0 or .
            Console.Error.WriteLine(grid[i]);
        }

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[i][j] == '0')
                {
                    int rightX = -1;
                    int rightY = -1;
                    int downX = -1;
                    int downY = -1;

                    if (j < (width - 1))
                    {
                        for (int k = j + 1; k < width; k++)
                        {
                            Console.Error.WriteLine(k);
                            if (grid[i][k] == '0')
                            {
                                rightX = k;
                                rightY = i;
                                break;
                            }
                        }
                    }

                    if (i < (height - 1))
                    {
                        for (int k = i + 1; k < height; k++)
                        {
                            if (grid[k][j] == '0')
                            {
                                downX = j;
                                downY = k;
                                break;
                            }
                        }
                    }

                    Console.WriteLine(string.Format("{0} {1} {2} {3} {4} {5}", j, i, rightX, rightY, downX, downY));
                }
            }
        }
    }
}