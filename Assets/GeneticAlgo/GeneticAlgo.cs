using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GAParameters
{
    public int populationSize;
    public int numberOfGenerations;
    public float pCross;
    public float pMut;
    public int seed;

    public List<int> chromosomeLength;
    public List<float> chromoMin;
    public List<float> chromoMax;
    public int chromosomeTotal;
}

public class GeneticAlgo
{
    public GAParameters gaParameters;
    long startTime;

    public GeneticAlgo(GAParameters gap)
    {
        gaParameters = gap;
        RandomMgr r = new RandomMgr(gaParameters.seed);
        startTime = System.DateTime.Now.ToFileTimeUtc();
    }

    public Population parents, children;
    public void Init()
    {
        MasterMgr.inst.ThreadLog("Initializing GA");
        RunUIMgr.inst.genCount = gaParameters.numberOfGenerations;

        parents = new Population(gaParameters);
        parents.Init();
        children = new Population(gaParameters);
        children.Init();

        parents.Evaluate();
        parents.Statistics();
        parents.Report(0, startTime);
        MasterMgr.inst.ThreadLog("Initialed GA");

    }


    public void Run()
    {
        Init();
        Evolve();
        Cleanup();

    }

    public void Evolve()
    {
        for (int i = 1; i < gaParameters.numberOfGenerations; i++)
        {
            //Thread.Sleep(1);
            //InputHandler.inst.ThreadLog("Generation: " + i); 
            parents.CHCGeneration(children);
            children.Statistics();
            children.Report(i, startTime);


            Population tmp = parents;
            parents = children;
            children = tmp;

        }
    }

    public void Cleanup()
    {
        MasterMgr.inst.ThreadLog("Cleaning up");
    }
}
