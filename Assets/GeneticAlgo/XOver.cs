using System;

public class XOver
{
    public static void OnePoint(Individual parent1, Individual parent2, Individual child1, Individual child2, int chromosomeLength)
    {
        int x1 = RandomMgr.inst.RandInt(0, chromosomeLength, parent1.randomSeed);
        for (int i = x1; i < chromosomeLength; i++)
        {
            child1.chromosome[i] = parent2.chromosome[i];
            child2.chromosome[i] = parent1.chromosome[i];
        }

    }

    public static void TwoPoint(Individual parent1, Individual parent2, Individual child1, Individual child2, int chromosomeLength)
    {
        int x1 = RandomMgr.inst.RandInt(0, chromosomeLength, parent1.randomSeed);
        int x2 = RandomMgr.inst.RandInt(0, chromosomeLength, parent1.randomSeed);
        int low = Math.Min(x1, x2);
        int high = Math.Max(x1, x2);
        for (int i = low; i < high; i++)
        {
            child1.chromosome[i] = parent2.chromosome[i];
            child2.chromosome[i] = parent1.chromosome[i];
        }

    }

    public static void Uniform(Individual parent1, Individual parent2, Individual child1, Individual child2, int chromosomeLength)
    {
        for(int i = 0; i < chromosomeLength; i++)
        {
            if(RandomMgr.inst.Flip(0.5f, parent1.randomSeed))
            {
                child1.chromosome[i] = parent2.chromosome[i];
                child2.chromosome[i] = parent1.chromosome[i];
            }
        }
    }
}
