using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Save humans, destroy zombies!
 **/

public class Human {
    public int humanId;
    public int humanX;
    public int humanY;
    public double distance;

    public Human (int humanId, int humanX, int humanY) {
    this.humanId = humanId;
    this.humanX = humanX;
    this.humanY = humanY;
    }

    public double CalculateDistance (int fromX, int fromY) {
        this.distance = Math.Sqrt(Math.Pow(fromX * fromX - humanX * humanX, 2) + 
                             Math.Pow(fromY * fromY - humanY * humanY, 2));
        return distance;
    }
}

public class Zombie {
    public int zombieId;
    public int zombieX;
    public int zombieY;
    public int zombieXNext;
    public int zombieYNext;

    public Zombie (int zombieId, int zombieX, int zombieY, int zombieXNext, int zombieYNext) {
    this.zombieId = zombieId;
    this.zombieX = zombieX;
    this.zombieY = zombieY;
    this.zombieXNext = zombieXNext;
    this.zombieYNext = zombieYNext;
    }
}

class CodeVsZombiesPlayer
{
    static void CodeVsZombies(string[] args)
    {
        string[] inputs;

        // game loop
        while (true)
        {

            inputs = Console.ReadLine().Split(' ');
            var x = int.Parse(inputs[0]);
            var y = int.Parse(inputs[1]);
            var humanCount = int.Parse(Console.ReadLine());

            var humans = new Human[humanCount];

            for (var i = 0; i < humanCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                var humanId = int.Parse(inputs[0]);
                var humanX = int.Parse(inputs[1]);
                var humanY = int.Parse(inputs[2]);
                humans[i] = new Human(humanId, humanX, humanY);
                humans[i].CalculateDistance(x, y);
            }
            var zombieCount = int.Parse(Console.ReadLine());

            var zombies = new Zombie[zombieCount];
            
            for (var i = 0; i < zombieCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                var zombieId = int.Parse(inputs[0]);
                var zombieX = int.Parse(inputs[1]);
                var zombieY = int.Parse(inputs[2]);
                var zombieXNext = int.Parse(inputs[3]);
                var zombieYNext = int.Parse(inputs[4]);
                zombies[i] = new Zombie(zombieId, zombieX, zombieY, zombieXNext, zombieYNext);
            }

            if (humans.Length < 5 && zombies.Length > 1) {
                Array.Sort<Human>(humans, (hum1, hum2) => hum1.distance.CompareTo(hum2.distance));
                foreach(var human in humans)
                    Console.Error.WriteLine(human.distance);
                Console.WriteLine(humans[0].humanX + " " + humans[0].humanY);
                continue;
            }

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            Console.WriteLine(zombies[0].zombieXNext + " " + zombies[0].zombieYNext); // Your destination coordinates

        }
    }
}