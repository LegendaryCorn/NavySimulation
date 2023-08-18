using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Evaluator
{
    public static float Evaluate(Individual individual)
    {
        /*
        PotentialParameters parameters = ParseChromosome(individual.chromosome);
        float sum = 0;

        for(int i = 0; i < ScenarioMgr.inst.scenarios.Count; i++)
        {
            Scenario s = ScenarioMgr.inst.scenarios[i];

            GameMgr game = new GameMgr(parameters);
            game.ExecuteGame(i);
            sum += game.fitnessMgr.totalFitness / (9f * 300f * s.scenarioEntities.Count);
        }

        return sum;
        */

        float total = 0;
        //First half of chromosome should be 1's, second half should be 0's
        for (int i = 0; i < individual.chromosome.Length; i++)
        {
            if(i < individual.chromosome.Length / 2f)
            {
                total += individual.chromosome[i];
            }
            else
            {
                total += 1 - individual.chromosome[i];
            }
        }
        return total;

        /*
        Thread.Sleep(10);
        float sum = 0;
        //Maximize number of 1's in the chromosome
        for (int i = 0; i < individual.chromosome.Length; i++)
        {
            sum += individual.chromosome[i]; // count number of one's
        }
        return sum;
        */
    }

    public static PotentialParameters ParseChromosome(int[] chromosome)
    {
        PotentialParameters p = new PotentialParameters();

        p.potentialDistanceThreshold = 2000f * chromosome[0] + 1000f * chromosome[1] + 500f * chromosome[2] + 250f * chromosome[3];
        
        p.attractionCoefficient = (8f * chromosome[4] + 4f * chromosome[5] + 2f * chromosome[6] + 1f * chromosome[7])
                                * Mathf.Pow(10f, 4f * chromosome[8] + 2f * chromosome[9] + 1f * chromosome[10]);

        p.attractiveExponent = -(8f * chromosome[11] + 4f * chromosome[12] + 2f * chromosome[13] + 1f * chromosome[14]
                             + 0.5f * chromosome[15] + 0.25f * chromosome[16] + 0.125f * chromosome[17]);

        p.repulsiveCoefficient = (8f * chromosome[18] + 4f * chromosome[19] + 2f * chromosome[20] + 1f * chromosome[21])
                        * Mathf.Pow(10f, 4f * chromosome[22] + 2f * chromosome[23] + 1f * chromosome[24]);

        p.repulsiveExponent = -(8f * chromosome[25] + 4f * chromosome[26] + 2f * chromosome[27] + 1f * chromosome[28]
                             + 0.5f * chromosome[29] + 0.25f * chromosome[30] + 0.125f * chromosome[31]);

        return p;
    }
}
