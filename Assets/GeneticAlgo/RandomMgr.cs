using System.Collections.Generic;

public class RandomMgr
{
    public static RandomMgr inst;

    public Dictionary<int, System.Random> rand;
    public RandomMgr(int seed)
    {
        inst = this;
        rand = new Dictionary<int, System.Random>();
        rand[-1] = new System.Random(seed);
    }

    public void AddRandom(int seed) // -1 is the seed that MasterMgr uses to generate seeds for others.
    {
        rand[seed] = new System.Random(seed);
    }

    public bool Flip(float prob, int seed)
    {
        return (rand[seed].NextDouble() < prob);
    }

    public int Flip01(float prob, int seed)
    {
        return (rand[seed].NextDouble() < prob ? 0 : 1);
    }

    public int RandInt(int low, int high, int seed)
    {
        return rand[seed].Next(low, high);
    }
}
