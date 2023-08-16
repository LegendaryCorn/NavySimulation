using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GAParameters
{
    public int populationSize;
    public int chromosomeLength;
    public int numberOfGenerations;
    public float pCross;
    public float pMut;
    public int seed;
}

public class GeneticAlgo
{
    public GAParameters gaParameters;
    public GeneticAlgo(GAParameters gap)
    {
        gaParameters = gap;
        RandomMgr r = new RandomMgr(gaParameters.seed);
    }

    public Population parents, children;
    public void Init()
    {
        MasterMgr.inst.ThreadLog("Initializing GA");

        parents = new Population(gaParameters);
        parents.Init();
        children = new Population(gaParameters);
        children.Init();

        parents.Evaluate();
        parents.Statistics();
        parents.Report(0);
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
            children.Report(i);


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
