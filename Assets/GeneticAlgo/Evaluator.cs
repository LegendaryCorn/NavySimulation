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

        /*
        // De Jong 1
        for (int i = 0; i < vals.Length; i++)
        {
            objective_val += vals[i] * vals[i];
        }
        */

        /*
        // De Jong 2
        for (int i = 0; i < vals.Length - 1; i++)
        {
            objective_val += 100f * Mathf.Pow(vals[i+1] - vals[i] * vals[i], 2) + Mathf.Pow(1 - vals[i], 2) ;
        }


        float fitness = 1 / (objective_val + 1);
        */

        
        float sum = 0;

        for(int i = 0; i < ScenarioMgr.inst.scenarios.Count; i++)
        {
            Scenario s = ScenarioMgr.inst.scenarios[i];

            GameMgr game = new GameMgr(new PotentialParameters(vals));
            game.ExecuteGame(i);

            sum = game.fitnessMgr.twoShipFitnessParameters[0][1].closestDist;

            /*
            float total = 3f   * game.fitnessMgr.countHeadingManeuver
                        + 3f   * game.fitnessMgr.countSpeedManeuver
                        + 1f   * game.fitnessMgr.countNearbyShips
                        + 10f  * game.fitnessMgr.countShipInFront
                        + 100f * game.fitnessMgr.countCrash;

            sum += total / (s.scenarioEntities.Count);
            */
        }

        //return 1 / sum;

        float fitness = 0;

        if (sum <= 500f)
            fitness = 0.2f * (sum - 500f) + 100f;
        else if (sum <= 1400f)
            fitness = -0.125f * (sum - 500f) + 100f;
        else
            fitness = 0;

        return Mathf.Max(fitness, 0f);
        
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
