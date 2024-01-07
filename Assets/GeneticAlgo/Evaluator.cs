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
        List<float> fitness = new List<float>();

        for (int sc = 0; sc < 2; sc++)
        {

            game = new GameMgr(new PotentialParameters(vals));
            game.ExecuteGame(sc);

            float timePoint = Mathf.Max(game.fitnessMgr.oneShipFitnessParameters[0].timeToTarget, game.fitnessMgr.oneShipFitnessParameters[1].timeToTarget);

            float mainDist = 0f;
            int mainCount = 0;
            float sideDist = 0f;
            int sideCount = 0;

            for (int i = 0; i < game.entityMgr.entities.Count; i++)
            {
                float sumShip = 0f;
                Entity381 ent = game.entityMgr.entities[i];
                for (int j = 0; j < ent.fitness.dists.Count; j++)
                {
                    sumShip += Mathf.Sqrt(ent.fitness.dists[j]);
                }

                if(ent.entityType == EntityType.DDG51) // DDG51 will be the dedicated "main" ship for now
                {
                    mainDist += sumShip / ent.fitness.dists.Count;
                    mainCount += 1;
                }
                else
                {
                    sideDist += sumShip / ent.fitness.dists.Count;
                    sideCount += 1;
                }
            }

            float mainFit = mainCount == 0 ? 1f : Mathf.Pow(mainDist / mainCount, -0.05f);
            float sideFit = sideCount == 0 ? 1f : Mathf.Pow(sideDist / sideCount, -0.05f);
            float timeFit = 1f - (timePoint - game.fitnessMgr.timeMin) / (game.fitnessMgr.timeMax - game.fitnessMgr.timeMin);
            timeFit = Mathf.Clamp01(timeFit);

            fitness.Add(0.45f * mainFit + 0.45f * sideFit + 0.1f * timeFit);
        }

        float lowerFit = fitness[0] > fitness[1] ? fitness[1] : fitness[0];
        float higherFit = fitness[1] > fitness[0] ? fitness[1] : fitness[0];

        return 0.7f * lowerFit + 0.3f * higherFit;
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
