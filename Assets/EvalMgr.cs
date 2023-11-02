using System.Collections;
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
        float timeStart;
        float timeEnd;

        timeStart = Time.realtimeSinceStartup;

        game = new GameMgr(potentialParameters);
        game.ExecuteGame(scenarioID);

        float closestDist = game.fitnessMgr.twoShipFitnessParameters[0][1].closestDist;
        float timePoint = Mathf.Max(game.fitnessMgr.oneShipFitnessParameters[0].timeToTarget, game.fitnessMgr.oneShipFitnessParameters[1].timeToTarget);
        float minAngle0 = game.fitnessMgr.oneShipFitnessParameters[0].minDesHeadingWP;
        float maxAngle0 = game.fitnessMgr.oneShipFitnessParameters[0].maxDesHeadingWP;
        float minAngle1 = game.fitnessMgr.oneShipFitnessParameters[1].minDesHeadingWP;
        float maxAngle1 = game.fitnessMgr.oneShipFitnessParameters[1].maxDesHeadingWP;
        float fitness = 0f;


        /*
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

            fitness = fcd + 1.0f * ftp + 1.0f * fmi0 * fma0 + fmi1 * fma1;
            if (fmi1 * fma1 == 0) { fitness = 0.1f; }
            if (fmi0 * fma0 == 0) { fitness = 0.1f; }
        }
        */

        float sumDist = 0f;
        bool allVisited = true;

        for (int i = 0; i < game.entityMgr.entities.Count; i++)
        {
            Entity381 ent = game.entityMgr.entities[i];
            for (int j = 0; j < ent.fitness.dists.Count; j++)
            {
                sumDist += ent.fitness.dists[j];
                if(ent.fitness.dists[j] < 0)
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

        timeEnd = Time.realtimeSinceStartup;

        Debug.Log(fitness.ToString() + " "  + sumDist.ToString() + " " + timePoint.ToString() + " " + (timeEnd -timeStart).ToString());

    }
}
