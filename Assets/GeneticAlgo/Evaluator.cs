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

        for(int i = 0; i < 1; i++)//ScenarioMgr.inst.scenarios.Count; i++)
        {
            Scenario s = ScenarioMgr.inst.scenarios[i];

            game = new GameMgr(new PotentialParameters(vals));
            game.ExecuteGame(1);

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

        float fitness = 0f;
        float sumDist = 0f;
        float timePoint = Mathf.Max(game.fitnessMgr.oneShipFitnessParameters[0].timeToTarget, game.fitnessMgr.oneShipFitnessParameters[1].timeToTarget);
        bool allVisited = true;

        for (int i = 0; i < game.entityMgr.entities.Count; i++)
        {
            Entity381 ent = game.entityMgr.entities[i];
            for (int j = 0; j < ent.fitness.dists.Count; j++)
            {
                sumDist += ent.fitness.dists[j];
                if (ent.fitness.dists[j] < 0)
                {
                    allVisited = false;
                }
            }
        }

        fitness = 2000f / (sumDist + 0.5f * (timePoint - 200) * (timePoint - 200));

        if (!allVisited)
        {
            fitness = 0f;
        }


        return fitness;

        //////////////////////////////////////////////////////////////////////////////////////
        float closestDist = game.fitnessMgr.twoShipFitnessParameters[0][1].closestDist;
        //float timePoint = Mathf.Max(game.fitnessMgr.oneShipFitnessParameters[0].timeToTarget, game.fitnessMgr.oneShipFitnessParameters[1].timeToTarget);
        float minAngle0 = game.fitnessMgr.oneShipFitnessParameters[0].minDesHeadingWP;
        float maxAngle0 = game.fitnessMgr.oneShipFitnessParameters[0].maxDesHeadingWP;
        float minAngle1 = game.fitnessMgr.oneShipFitnessParameters[1].minDesHeadingWP;
        float maxAngle1 = game.fitnessMgr.oneShipFitnessParameters[1].maxDesHeadingWP;
        //float fitness = 0f;

        if (!game.fitnessMgr.oneShipFitnessParameters[0].reachedTarget || !game.fitnessMgr.oneShipFitnessParameters[1].reachedTarget || closestDist < 150f)
        {
            fitness *= 0f;
        }
        else
        {
            float fcd = Mathf.Max(0, 100f - 0.001f * (800f - closestDist) * (800f - closestDist));
            float ftp = Mathf.Clamp(0.5f * (3400f - timePoint), 0, 100);
            float fmi0 = Mathf.Clamp(0.1f * (minAngle0 + 10f), 0f, 1f);
            float fma0 = Mathf.Clamp(-100f * (maxAngle0 - 75f) / 15f, 0f, 100f);
            float fmi1 = Mathf.Clamp(10f - (maxAngle1 / 5f), 0f, 10f);
            float fma1 = Mathf.Clamp(10f - (-minAngle1 / 5f), 0f, 10f);

            if(fmi1 * fma1 == 0) { return 0.1f; }
            if(fmi0 * fma0 == 0) { return 0.1f; }

            fitness = fcd + 1.0f * ftp + 1.0f * fmi0 * fma0 + fmi1 * fma1;
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
