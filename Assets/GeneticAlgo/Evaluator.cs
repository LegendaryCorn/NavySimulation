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

        GameMgr game = null;

        for(int i = 0; i < ScenarioMgr.inst.scenarios.Count; i++)
        {
            Scenario s = ScenarioMgr.inst.scenarios[i];

            game = new GameMgr(new PotentialParameters(vals));
            game.ExecuteGame(i);

            /*
            float total = 3f   * game.fitnessMgr.countHeadingManeuver
                        + 3f   * game.fitnessMgr.countSpeedManeuver
                        + 1f   * game.fitnessMgr.countNearbyShips
                        + 10f  * game.fitnessMgr.countShipInFront
                        + 100f * game.fitnessMgr.countCrash;

            sum += total / (s.scenarioEntities.Count);`

            */
            if(i == 0) { break; }
        }

        //return 1 / sum;

        float closestDist = game.fitnessMgr.twoShipFitnessParameters[0][1].closestDist;
        float timePoint = Mathf.Max(game.fitnessMgr.oneShipFitnessParameters[0].timeToTarget, game.fitnessMgr.oneShipFitnessParameters[1].timeToTarget);
        float minAngle = Mathf.Min(game.fitnessMgr.oneShipFitnessParameters[0].minDesHeadingWP, game.fitnessMgr.oneShipFitnessParameters[1].minDesHeadingWP);
        float maxAngle = Mathf.Max(game.fitnessMgr.oneShipFitnessParameters[0].maxDesHeadingWP, game.fitnessMgr.oneShipFitnessParameters[1].maxDesHeadingWP);
        float fitness = 0f;

        if (!game.fitnessMgr.oneShipFitnessParameters[0].reachedTarget || !game.fitnessMgr.oneShipFitnessParameters[1].reachedTarget || closestDist < 150f)
        {
            fitness *= 0f;
        }
        else
        {
            float fcd = Mathf.Max(0, 100f - 0.001f * (800f - closestDist) * (800f - closestDist));
            float ftp = Mathf.Max(0, 100f + (230f - timePoint));
            float fmi = Mathf.Clamp(0.2f * minAngle + 10f, 0f, 1f);
            float fma = Mathf.Clamp(-100f * (maxAngle - 75f) / 15f, 0f, 100f);

            fitness = fcd + 1.0f * ftp + 1.0f * fmi * fma;
        }

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
