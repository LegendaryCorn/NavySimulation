﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvalMgr : MonoBehaviour
{
    public static EvalMgr inst;
    GameMgr game;
    public PotentialParameters potentialParameters;
    public int scenarioID;

    private void Awake()
    {
        inst = this;
    }

    private void Start()
    {
        for (int sc = 0; sc < 2; sc++)
        {
            float timeStart;
            float timeEnd;

            timeStart = Time.realtimeSinceStartup;

            game = new GameMgr(potentialParameters);
            game.ExecuteGame(sc);

            float closestDist = game.fitnessMgr.twoShipFitnessParameters[0][1].closestDist;
            float timePoint = Mathf.Max(game.fitnessMgr.oneShipFitnessParameters[0].timeToTarget, game.fitnessMgr.oneShipFitnessParameters[1].timeToTarget);
            float minAngle0 = game.fitnessMgr.oneShipFitnessParameters[0].minDesHeadingWP;
            float maxAngle0 = game.fitnessMgr.oneShipFitnessParameters[0].maxDesHeadingWP;
            float minAngle1 = game.fitnessMgr.oneShipFitnessParameters[1].minDesHeadingWP;
            float maxAngle1 = game.fitnessMgr.oneShipFitnessParameters[1].maxDesHeadingWP;
            float fitness = 0f;

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

            fitness = 2000f / (20 * sumDist + 0.5f * (timePoint - game.fitnessMgr.timeMin) * (timePoint - game.fitnessMgr.timeMin));

            timeEnd = Time.realtimeSinceStartup;

            Debug.Log(fitness.ToString() + " " + sumDist.ToString() + " " + timePoint.ToString() + " " + (timeEnd - timeStart).ToString());
        }

    }
}
