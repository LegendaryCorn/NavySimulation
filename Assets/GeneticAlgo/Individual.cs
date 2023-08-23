﻿using System;
using System.Text;

public class Individual : IComparable<Individual>
{

    public int chromLength;
    public int randomSeed;
    public int[] chromosome;
    public float fitness;
    public float objectiveFunction;

    public Individual(int chromLength, int randomSeed)
    {
        chromosome = new int[chromLength];
        this.randomSeed = randomSeed;
    }

    public void Init()
    {
        for (int i = 0; i < chromosome.Length; i++)
        {
            chromosome[i] = RandomMgr.inst.Flip01(0.5f, randomSeed);
        }
    }

    public void Mutate(float pm)
    {
        for (int i = 0; i < chromosome.Length; i++)
        {
            chromosome[i] = (RandomMgr.inst.Flip(pm, randomSeed) ? 1 - chromosome[i] : chromosome[i]);
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < chromosome.Length; i++)
        {
            sb.Append(chromosome[i].ToString("0"));
        }
        sb.Append(", " + fitness);
        return sb.ToString();
    }

    public int CompareTo(Individual other)
    {
        return other.fitness.CompareTo(fitness);//From high fitness to low
    }
}
