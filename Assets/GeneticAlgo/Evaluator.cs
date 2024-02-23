﻿using System.Collections;
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

        /*
        game = new GameMgr(new PotentialParameters(vals));
        game.ExecuteGame(gaParameters.scenario);

        float timePoint = Mathf.Max(game.fitnessMgr.oneShipFitnessParameters[0].timeToTarget, game.fitnessMgr.oneShipFitnessParameters[1].timeToTarget);

        float sumDist = 0f;
        bool allVisited = true;
        timePoint = Mathf.Clamp(timePoint, game.fitnessMgr.timeMin, game.fitnessMgr.timeMax);

        for (int i = 0; i < game.entityMgr.entities.Count; i++)
        {
            Entity381 ent = game.entityMgr.entities[i];
            sumDist += ent.fitness.fitPDist;
        }
        sumDist *= 1.0f / game.entityMgr.entities.Count;

        float timeVal = (timePoint - game.fitnessMgr.timeMin) / (game.fitnessMgr.timeMax - game.fitnessMgr.timeMin);

        fitness += Mathf.Pow(0.01f * sumDist + 25f * timeVal * timeVal + 1f, -0.5f);
        */
        for (int x = 0; x < ScenarioMgr.inst.scenarioTypeData.Count * gaParameters.scenarioCount; x++)
        {
            game = new GameMgr(new PotentialParameters(vals));
            game.ExecuteGame(x);

            float cpaFit = 0f;
            float fpFit = 0f;

            float sumDist = 0;
            for (int i = 0; i < 2; i++)
            {
                Entity381 ent = game.entityMgr.entities[i];
                sumDist += ent.fitness.fitPDist;
            }

            sumDist *= 1.0f / game.entityMgr.entities.Count;
            cpaFit = 1 / (Mathf.Abs(Mathf.Sqrt(game.entityMgr.entities[0].fitness.cpaDist) - 500f) + 1); // Remember that cpaDist and fitPDist are squared distances
            fpFit = 1.0f / (sumDist + 1);

            fitness += 0.7f * cpaFit + 0.3f * fpFit;
        }

        return fitness / (ScenarioMgr.inst.scenarioTypeData.Count * gaParameters.scenarioCount);
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
