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

        float[] p = new float[parameters.chromosomeParameters.Count];

        int j = 0;
        for(int i = 0; i < parameters.chromosomeParameters.Count; i++)
        {
            int chromoLen = parameters.chromosomeParameters[i].chromosomeLength;
            int[] shortChromosome = new int[chromoLen];
            System.Array.Copy(chromosome, j, shortChromosome, 0, chromoLen);

            p[i] = parameters.chromosomeParameters[i].chromosomeMin 
                + ((parameters.chromosomeParameters[i].chromosomeMax - parameters.chromosomeParameters[i].chromosomeMin) / (Mathf.Pow(2, chromoLen) - 1)) 
                * Utils.BinaryToDecimal(shortChromosome);

            j += chromoLen;
        }

        return p;
    }
}
