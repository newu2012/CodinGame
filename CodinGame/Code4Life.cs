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
            Console.Error.WriteLine(samples[i].sampleId);
            if (samples[i].carriedBy != 1)
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
        int projectCount = int.Parse(Console.ReadLine());
        for (int i = 0; i < projectCount; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int a = int.Parse(inputs[0]);
            int b = int.Parse(inputs[1]);
            int c = int.Parse(inputs[2]);
            int d = int.Parse(inputs[3]);
            int e = int.Parse(inputs[4]);
        }

        var typesOfWork = new string[]{
            "SAMPLES",
            "CollectSample",
            "DIAGNOSIS", 
            "AnalyzeSample",
            "MOLECULES", 
            "CollectMolecules",
            "LABORATORY", 
            "ProduceMedicine"};
        var workToDo = typesOfWork[0];

        string[] moleculeTypes = {"A", "B", "C", "D", "E"};

        Sample selectedSample = null;
        int selectedMoleculeIndex;
        var playerStorage = new int[5];

        // game loop
        while (true)
        {
            for (int i = 0; i < 2; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                string target = inputs[0];
                int eta = int.Parse(inputs[1]);
                int score = int.Parse(inputs[2]);
                int storageA = int.Parse(inputs[3]);
                int storageB = int.Parse(inputs[4]);
                int storageC = int.Parse(inputs[5]);
                int storageD = int.Parse(inputs[6]);
                int storageE = int.Parse(inputs[7]);
                int expertiseA = int.Parse(inputs[8]);
                int expertiseB = int.Parse(inputs[9]);
                int expertiseC = int.Parse(inputs[10]);
                int expertiseD = int.Parse(inputs[11]);
                int expertiseE = int.Parse(inputs[12]);
            }


            inputs = Console.ReadLine().Split(' ');
            int availableA = int.Parse(inputs[0]);
            int availableB = int.Parse(inputs[1]);
            int availableC = int.Parse(inputs[2]);
            int availableD = int.Parse(inputs[3]);
            int availableE = int.Parse(inputs[4]);
            int sampleCount = int.Parse(Console.ReadLine());

            var availableSamples = new Sample[sampleCount];
            if (sampleCount == 0 && workToDo.Equals("SAMPLES")) {
                Console.WriteLine("GOTO " + workToDo);
                workToDo = "CollectSample";
                continue;
            }
            if (sampleCount > 0) {
                for (int i = 0; i < sampleCount; i++)
                {
                    inputs = Console.ReadLine().Split(' ');
                    int sampleId = int.Parse(inputs[0]);
                    int carriedBy = int.Parse(inputs[1]);
                    int rank = int.Parse(inputs[2]);
                    string expertiseGain = inputs[3];
                    int health = int.Parse(inputs[4]);
                    int costA = int.Parse(inputs[5]);
                    int costB = int.Parse(inputs[6]);
                    int costC = int.Parse(inputs[7]);
                    int costD = int.Parse(inputs[8]);
                    int costE = int.Parse(inputs[9]);

                    availableSamples[i] = (new Sample(
                        sampleId, 
                        carriedBy, 
                        rank, 
                        Int32.Parse(expertiseGain), 
                        health, 
                        new int[]{costA, costB, costC, costD, costE}));
                }
                if (selectedSample == null)
                    selectedSample = availableSamples[0];
            }

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            switch(workToDo) {
                case "SAMPLES":
                    Console.WriteLine("GOTO " + workToDo);
                    workToDo = "CollectSample";
                    break;
                case "CollectSample":
                    var temp_rank = 2; // TODO CollectSample Call
                    Console.WriteLine("CONNECT " + temp_rank);
                    workToDo = "DIAGNOSIS";
                    break;
                case "DIAGNOSIS":
                    Console.WriteLine("GOTO " + workToDo);
                    workToDo = "AnalyzeSample";
                    break;
                case "AnalyzeSample":
                    selectedSample = SelectSample(availableSamples);
                    workToDo = "MOLECULES";
                    Console.WriteLine("CONNECT " + selectedSample.sampleId);
                    break;
                case "MOLECULES":
                    selectedSample = availableSamples.Select(x => x).First(x => x.sampleId == selectedSample.sampleId);
                    Console.WriteLine("GOTO " + workToDo);
                    workToDo = "CollectMolecules";
                    break;
                case "CollectMolecules": 
                    selectedMoleculeIndex = CollectMolecules(selectedSample, playerStorage);
                    if (selectedMoleculeIndex != -1) {
                        Console.WriteLine("CONNECT " + moleculeTypes[selectedMoleculeIndex]);
                        playerStorage[selectedMoleculeIndex]++;
                        break;
                    }
                    workToDo = "LABORATORY";
                    Console.WriteLine("GOTO " + workToDo);
                    break;
                case "LABORATORY":
                    Console.WriteLine("GOTO " + workToDo);
                    workToDo = "ProduceMedicine";
                    break;
                case "ProduceMedicine":
                    Console.WriteLine("CONNECT " + selectedSample.sampleId);
                    workToDo = "SAMPLES";
                    playerStorage = new int[5];
                    break;
            }
        }
    }
}