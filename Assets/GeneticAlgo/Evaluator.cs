using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;



public class Evaluator
{

    public static float Evaluate(Individual individual, GAParameters gaParameters)
    {
        float[] vals = ParseChromosome(individual.chromosome, gaParameters);

        GameMgr game = null;
        float fitness = 0f;

        for (int sc = 0; sc < 2; sc++)
        {

            game = new GameMgr(new PotentialParameters(vals));
            game.ExecuteGame(sc);

            float timePoint = Mathf.Max(game.fitnessMgr.oneShipFitnessParameters[0].timeToTarget, game.fitnessMgr.oneShipFitnessParameters[1].timeToTarget);

            float sumDist = 0f;
            bool allVisited = true;
            timePoint = Mathf.Clamp(timePoint, game.fitnessMgr.timeMin, game.fitnessMgr.timeMax);

            for (int i = 0; i < game.entityMgr.entities.Count; i++)
            {
                float sumShip = 0f;
                Entity381 ent = game.entityMgr.entities[i];
                for (int j = 0; j < ent.fitness.dists.Count; j++)
                {
                    sumShip += Mathf.Sqrt(ent.fitness.dists[j]);
                }
                sumDist += sumShip / ent.fitness.dists.Count;
            }
            sumDist *= 1.0f / game.entityMgr.entities.Count;

            fitness += 2000f / (20 * sumDist + 0.5f * (timePoint - game.fitnessMgr.timeMin) * (timePoint - game.fitnessMgr.timeMin));
        }
        
        return fitness / 2;
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
