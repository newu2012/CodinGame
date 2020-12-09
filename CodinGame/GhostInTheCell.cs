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
internal class Player
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

    private static void GhostInTheCell(string[] args)
    {
        string[] inputs;
        var factoryCount = int.Parse(Console.ReadLine()); // the number of factories
        var linkCount = int.Parse(Console.ReadLine()); // the number of links between factories        

        var factoryDistances = new factoryDist[linkCount];
        for (var i = 0; i < linkCount; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            var factory1 = int.Parse(inputs[0]);
            var factory2 = int.Parse(inputs[1]);
            var distance = int.Parse(inputs[2]);

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

            var ourTopFactoryId = 0;
            var ourTopFactoryUnits = 1;
            var entityCount = int.Parse(Console.ReadLine()); // the number of entities (e.g. factories and troops)
            for (var i = 0; i < entityCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                var entityId = int.Parse(inputs[0]);
                var entityType = inputs[1];
                var arg1 = int.Parse(inputs[2]);
                var arg2 = int.Parse(inputs[3]);
                var arg3 = int.Parse(inputs[4]);
                var arg4 = int.Parse(inputs[5]);
                var arg5 = int.Parse(inputs[6]);
                
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
            
            var sendUnitsString = "";
            for (var i = 0; i < factoryCount; i++)
                if (factories[i].id != ourTopFactoryId && 
                    factories[i].owner != 1 &&
                    ourTopFactoryUnits - 3 > factories[i].units &&
                    waitList[i] == 0) {
                    var unitsToSend = factories[i].units + 2;
                    Console.Error.WriteLine(
                        "From=" + ourTopFactoryId + " To=" + factories[i].id + " Units=" + unitsToSend
                    );
                    
                    sendUnitsString +=
                        "MOVE" + " " +
                        ourTopFactoryId + " " +
                        factories[i].id + " " +
                        unitsToSend + ";";
                    
                    waitList[i] += 10;
                    ourTopFactoryUnits -= unitsToSend;
                }

            if (sendUnitsString.Length > 5) {
                sendUnitsString = sendUnitsString.Remove(sendUnitsString.Length - 1);
                Console.WriteLine(sendUnitsString);
            }
            else 
                Console.WriteLine("WAIT");
        }
    }
}