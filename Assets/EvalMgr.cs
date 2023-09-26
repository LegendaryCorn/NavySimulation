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
            float ftp = Mathf.Max(0, 100f + (90f - timePoint));
            float fmi = Mathf.Clamp(0.2f * minAngle + 10f, 0f, 1f);
            float fma = Mathf.Clamp(-100f * (maxAngle - 75f) / 15f, 0f, 100f);

            fitness = fcd + 0.1f * ftp + fmi * fma;
        }

        timeEnd = Time.realtimeSinceStartup;

        Debug.Log(closestDist.ToString() + " " + maxAngle.ToString() + " "  + fitness.ToString() + " " + (timeEnd - timeStart).ToString());

    }
}
