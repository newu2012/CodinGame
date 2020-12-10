using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class Sample {
    public int sampleId;
    public int carriedBy;
    public int rank;
    public int gain;
    public int health;
    public int[] costs = new int[5];

    public Sample (int sampleId, int carriedBy, int rank, int gain, int health, int[] costs) {
        this.sampleId = sampleId;
        this.carriedBy = carriedBy;
        this.rank = rank;
        this.gain = gain;
        this.health = health;
        this.costs = costs;
    }
}

class Code4LifePlayer
{
    public static Sample SelectSample(Sample[] samples) {
        Array.Sort<Sample>(samples, (x,y) => y.health.CompareTo(x.health));

        for (var i = 0; i < samples.Length; i++) {
            if (samples[i].carriedBy == -1)
                return samples[i];
        }
        return samples[0];
    }

    public static int CollectMolecules(Sample currentSample, int[] playerStorage) {
        Console.Error.Write("Molecules: ");
        for (var i = 0; i < currentSample.costs.Length; i++) 
            Console.Error.Write(playerStorage[i] + "/" + currentSample.costs[i] + " ");
        for (var i = 0; i < currentSample.costs.Length; i++) {
            if (currentSample.costs[i] > playerStorage[i])
                return i;
        }
        return -1;
    }

    static void Code4Life(string[] args)
    {
        string[] inputs;
        var projectCount = int.Parse(Console.ReadLine());
        for (var i = 0; i < projectCount; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            var a = int.Parse(inputs[0]);
            var b = int.Parse(inputs[1]);
            var c = int.Parse(inputs[2]);
            var d = int.Parse(inputs[3]);
            var e = int.Parse(inputs[4]);
        }

        var typesOfWork = new string[]{
            "DIAGNOSIS", 
            "SelectingSample",
            "MOLECULES", 
            "CollectingMolecules",
            "LABORATORY", 
            "ProducingMedicine"};
        var workToDo = typesOfWork[0];

        string[] moleculeTypes = {"A", "B", "C", "D", "E"};

        Sample selectedSample = null;
        int selectedMoleculeIndex;
        var playerStorage = new int[5];

        // game loop
        while (true)
        {
            for (var i = 0; i < 2; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                var target = inputs[0];
                var eta = int.Parse(inputs[1]);
                var score = int.Parse(inputs[2]);
                var storageA = int.Parse(inputs[3]);
                var storageB = int.Parse(inputs[4]);
                var storageC = int.Parse(inputs[5]);
                var storageD = int.Parse(inputs[6]);
                var storageE = int.Parse(inputs[7]);
                var expertiseA = int.Parse(inputs[8]);
                var expertiseB = int.Parse(inputs[9]);
                var expertiseC = int.Parse(inputs[10]);
                var expertiseD = int.Parse(inputs[11]);
                var expertiseE = int.Parse(inputs[12]);
            }

            inputs = Console.ReadLine().Split(' ');
            var availableA = int.Parse(inputs[0]);
            var availableB = int.Parse(inputs[1]);
            var availableC = int.Parse(inputs[2]);
            var availableD = int.Parse(inputs[3]);
            var availableE = int.Parse(inputs[4]);
            var sampleCount = int.Parse(Console.ReadLine());

            var availableSamples = new Sample[sampleCount];

            for (var i = 0; i < sampleCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                var sampleId = int.Parse(inputs[0]);
                var carriedBy = int.Parse(inputs[1]);
                var rank = int.Parse(inputs[2]);
                var expertiseGain = inputs[3];
                var health = int.Parse(inputs[4]);
                var costA = int.Parse(inputs[5]);
                var costB = int.Parse(inputs[6]);
                var costC = int.Parse(inputs[7]);
                var costD = int.Parse(inputs[8]);
                var costE = int.Parse(inputs[9]);
                availableSamples[i] = (new Sample(
                    sampleId, 
                    carriedBy, 
                    rank, 
                    int.Parse(expertiseGain), 
                    health, 
                    new[]{costA, costB, costC, costD, costE}));
            }
            if (selectedSample == null)
                selectedSample = availableSamples[0];

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            switch(workToDo) {
                case "DIAGNOSIS":
                    Console.WriteLine("GOTO " + workToDo);
                    workToDo = "SelectingSample";
                    break;
                case "SelectingSample":
                    selectedSample = SelectSample(availableSamples);
                    workToDo = "MOLECULES";
                    Console.WriteLine("CONNECT " + selectedSample.sampleId);
                    break;
                case "MOLECULES":
                    Console.WriteLine("GOTO " + workToDo);
                    workToDo = "CollectingMolecules";
                    break;
                case "CollectingMolecules": 
                    selectedMoleculeIndex = CollectMolecules(selectedSample, playerStorage);
                    if (selectedMoleculeIndex != -1) {
                        Console.WriteLine("CONNECT " + moleculeTypes[selectedMoleculeIndex]);
                        playerStorage[selectedMoleculeIndex]++;
                        break;
                    }
                    workToDo = "ProducingMedicine";
                    Console.WriteLine("GOTO " + "LABORATORY");
                    break;
                case "LABORATORY":
                    Console.WriteLine("GOTO " + workToDo);
                    workToDo = "ProducingMedicine";
                    break;
                case "ProducingMedicine":
                    Console.WriteLine("CONNECT " + selectedSample.sampleId);
                    workToDo = "DIAGNOSIS";
                    playerStorage = new int[5];
                    break;
            }
        }
    }
}