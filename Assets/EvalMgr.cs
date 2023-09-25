using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvalMgr : MonoBehaviour
{
    public static EvalMgr inst;
    GameMgr gameMgr;
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

        gameMgr = new GameMgr(potentialParameters);
        gameMgr.ExecuteGame(scenarioID);

        float closestDist = gameMgr.fitnessMgr.twoShipFitnessParameters[0][1].closestDist;
        float timePoint = Mathf.Max(gameMgr.fitnessMgr.oneShipFitnessParameters[0].timeToTarget, gameMgr.fitnessMgr.oneShipFitnessParameters[1].timeToTarget);
        float fitness = 0f;

        if(!gameMgr.fitnessMgr.oneShipFitnessParameters[0].reachedTarget || !gameMgr.fitnessMgr.oneShipFitnessParameters[1].reachedTarget)
        {
            fitness *= 0f;
        }
        else
        {
            float fcd = Mathf.Max(0, 100f - 0.001f * (800f - closestDist) * (800f - closestDist));
            float ftp = Mathf.Max(0, 100f + (90f - timePoint));

            fitness = fcd + ftp;
        }

        timeEnd = Time.realtimeSinceStartup;

        Debug.Log(closestDist.ToString() + " " + timePoint.ToString() + " "  + fitness.ToString() + " " + (timeEnd - timeStart).ToString());

    }
}
