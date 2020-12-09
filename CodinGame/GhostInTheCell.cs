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
    public struct factoryDist {
        public int factory1;
        public int factory2;
        public int distance;

        public factoryDist(int factory1, int factory2, int distance){
            this.factory1 = factory1;
            this.factory2 = factory2;
            this.distance = distance;
        }
    }

    public struct factory {
        public int id;
        public int units;
        public int owner;

        public factory(int id, int units, int owner) {
            this.id = id;
            this.units = units;
            this.owner = owner;
        }
    }

    static void Main(string[] args)
    {
        string[] inputs;
        int factoryCount = int.Parse(Console.ReadLine()); // the number of factories
        int linkCount = int.Parse(Console.ReadLine()); // the number of links between factories        

        var factoryDistances = new factoryDist[linkCount];
        for (int i = 0; i < linkCount; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int factory1 = int.Parse(inputs[0]);
            int factory2 = int.Parse(inputs[1]);
            int distance = int.Parse(inputs[2]);

            factoryDistances[i] = new factoryDist(factory1, factory2, distance);
        }

        var waitList = new int[factoryCount];
        for (var i = 0; i < waitList.Length; i++) 
            waitList[i] = 0;

        // game loop
        while (true)
        {
            for (var i = 0; i < waitList.Length; i++) 
                waitList[i] = waitList[i] > 0 ? waitList[i] - 1 : 0;

            var factories = new factory[factoryCount];

            int ourTopFactoryId = 0;
            int ourTopFactoryUnits = 1;
            int entityCount = int.Parse(Console.ReadLine()); // the number of entities (e.g. factories and troops)
            for (int i = 0; i < entityCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int entityId = int.Parse(inputs[0]);
                string entityType = inputs[1];
                int arg1 = int.Parse(inputs[2]);
                int arg2 = int.Parse(inputs[3]);
                int arg3 = int.Parse(inputs[4]);
                int arg4 = int.Parse(inputs[5]);
                int arg5 = int.Parse(inputs[6]);



                if (entityType.Equals("FACTORY")) {
                    factories[i] = new factory(entityId, arg2, arg1); 
                    Console.Error.WriteLine("Id=" + entityId + " Units=" + arg2 + " Owner=" + arg1);
                    if (arg1 == 1) 
                        if (arg2 > ourTopFactoryUnits) {
                            ourTopFactoryId = entityId;
                            ourTopFactoryUnits = arg2;
                        }
                }
            }

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");
            for (var i = 0; i < factoryCount; i++) {

                if (factories[i].id != ourTopFactoryId && 
                    factories[i].owner != 1 &&
                    ourTopFactoryUnits - 3 > factories[i].units &&
                    waitList[i] == 0) {
                        var unitsToSend = factories[i].units + 2;
                        Console.Error.WriteLine(
                            "From=" + ourTopFactoryId + " To=" + factories[i].id + " Units=" + unitsToSend
                        );
                        Console.WriteLine(
                            "MOVE" + " " +
                            ourTopFactoryId + " " +
                            factories[i].id + " " +
                            unitsToSend);
                        waitList[i] += 10;
                        break;
                    }
            }
            
            // Any valid action, such as "WAIT" or "MOVE source destination cyborgs"
            Console.WriteLine("WAIT");
        }
    }
}