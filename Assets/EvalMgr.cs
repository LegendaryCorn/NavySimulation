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

        float sum = gameMgr.fitnessMgr.twoShipFitnessParameters[0][1].closestDist;
        float fitness = 0f;

        if (sum <= 500f)
            fitness = 0.2f * (sum - 500f) + 100f;
        else if (sum <= 1400f)
            fitness = -0.125f * (sum - 500f) + 100f;

        if(!gameMgr.fitnessMgr.oneShipFitnessParameters[0].reachedTarget || !gameMgr.fitnessMgr.oneShipFitnessParameters[1].reachedTarget)
        {
            fitness *= 0f;
        }

        timeEnd = Time.realtimeSinceStartup;

        Debug.Log(sum.ToString() + " " + fitness.ToString() + " " + (timeEnd - timeStart).ToString());

    }
}
