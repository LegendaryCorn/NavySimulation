using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvalMgr : MonoBehaviour
{
    public static EvalMgr inst;
    GameMgr game;
    public PotentialParameters potentialParameters;
    public int scenarioCount;

    private void Awake()
    {
        inst = this;
    }

    private void Start()
    {

        float timeStart;
        float timeEnd;

        timeStart = Time.realtimeSinceStartup;

        // Generate scenarios
        foreach (ScenarioTypeData scData in ScenarioMgr.inst.scenarioTypeData)
        {
            ScenarioMgr.inst.GenerateScenarios(scData.scenarioType, scenarioCount);
        }

        game = new GameMgr(potentialParameters);
        game.ExecuteGame(0);

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
            Entity381 ent = game.entityMgr.entities[i];
            sumDist += ent.fitness.dist;
        }
        sumDist *= 1.0f / game.entityMgr.entities.Count;

        float timeVal = (timePoint - game.fitnessMgr.timeMin) / (game.fitnessMgr.timeMax - game.fitnessMgr.timeMin); 

        fitness += Mathf.Pow(0.01f * sumDist + 25f * timeVal * timeVal + 1f, -0.5f);

        timeEnd = Time.realtimeSinceStartup;

        Debug.Log(fitness.ToString() + " " + sumDist.ToString() + " " + timePoint.ToString() + " " + (timeEnd - timeStart).ToString());

        /*
        foreach(Entity381 ent in game.entityMgr.entities)
        {
            // Print the path points
            ScenarioEntity v = ScenarioMgr.inst.scenarios[0].ownShipEntity;
            string s1 =  v.spawnPoint.ToString() + "\n";
            foreach(Vector3 fp in v.fitPoints)
            {
                s1 += fp.ToString() + "\n";
            }
            s1 += v.wayPoints[0].ToString();
            Debug.Log(s1);
            // Print the path
            string s2 = v.spawnPoint.ToString();
            foreach (Vector3 vec in game.recordPos[ent])
            {
                s2 += "\n" + vec.ToString();
            }
            Debug.Log(s2);
        }
        */
    }
}
