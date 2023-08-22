using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;



public class Evaluator
{

    public static float Evaluate(Individual individual, GAParameters gaParameters)
    {
        float[] vals = ParseChromosome(individual.chromosome, gaParameters);

        float objective_val = 0f;
        for (int i = 0; i < vals.Length; i++)
        {
            objective_val += vals[i] * vals[i];
        }

        float fitness = 1 / (objective_val + 1);

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
        return fitness;
        
    }

    public static float[] ParseChromosome(int[] chromosome, GAParameters parameters)
    {

        float[] p = new float[parameters.chromosomeLength.Count];

        int j = 0;
        for(int i = 0; i < parameters.chromosomeLength.Count; i++)
        {
            int[] shortChromosome = new int[parameters.chromosomeLength[i]];
            System.Array.Copy(chromosome, j, shortChromosome, 0, parameters.chromosomeLength[i]);

            p[i] = parameters.chromoMin[i] 
                + ((parameters.chromoMax[i] - parameters.chromoMin[i]) / (Mathf.Pow(2, parameters.chromosomeLength[i]) - 1)) 
                * Utils.BinaryToDecimal(shortChromosome);

            j += parameters.chromosomeLength[i];
        }

        return p;
    }
}
