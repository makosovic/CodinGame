using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * CodinGame planet is being attacked by slimy insectoid aliens.
 * <---
 * Hint:To protect the planet, you can implement the pseudo-code provided in the statement, below the player.
 **/
class Enemy
{
    public string Name { get; set; }
    public int Distance { get; set; }
}

class Player
{
    static void Main(string[] args)
    {
        const int MAX_ENEMY = 2;
        // game loop
        while (true)
        {
            var enemies = new Enemy[MAX_ENEMY];

            for (int i = 0; i < MAX_ENEMY; i++)
            {
                string name = Console.ReadLine(); // name of enemy 
                int dist = int.Parse(Console.ReadLine()); // distance to enemy
                enemies[i] = (new Enemy { Name = name, Distance = dist });
            }

            Console.WriteLine(enemies.OrderBy(x => x.Distance).Select(x => x.Name).FirstOrDefault()); // replace "enemy" with a correct ship name to shoot
        }
    }
}