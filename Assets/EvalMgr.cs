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

        

        // Generate scenarios
        foreach (ScenarioTypeData scData in ScenarioMgr.inst.scenarioTypeData)
        {
            ScenarioMgr.inst.GenerateScenarios(scData.scenarioType, scenarioCount);
        }
        for (int x = 0; x < ScenarioMgr.inst.scenarioTypeData.Count * scenarioCount; x++) {
            timeStart = Time.realtimeSinceStartup;

            game = new GameMgr(potentialParameters);
            game.ExecuteGame(x);

            //float closestDist = game.fitnessMgr.twoShipFitnessParameters[0][1].closestDist;
            //float timePoint = Mathf.Max(game.fitnessMgr.oneShipFitnessParameters[0].timeToTarget, game.fitnessMgr.oneShipFitnessParameters[1].timeToTarget);
            //float minAngle0 = game.fitnessMgr.oneShipFitnessParameters[0].minDesHeadingWP;
            //float maxAngle0 = game.fitnessMgr.oneShipFitnessParameters[0].maxDesHeadingWP;
            //float minAngle1 = game.fitnessMgr.oneShipFitnessParameters[1].minDesHeadingWP;
            //float maxAngle1 = game.fitnessMgr.oneShipFitnessParameters[1].maxDesHeadingWP;
            float fitness = 0f;

            float cpaFit = 0f;
            float fpFit = 0f;
            //bool allVisited = true;
            //timePoint = Mathf.Clamp(timePoint, game.fitnessMgr.timeMin, game.fitnessMgr.timeMax);

            float sumDist = 0;
            for (int i = 0; i < 2; i++)
            {
                Entity381 ent = game.entityMgr.entities[i];
                sumDist += ent.fitness.fitPDist;
            }

            sumDist *= 1.0f / game.entityMgr.entities.Count;
            cpaFit = 1 / (Mathf.Abs(Mathf.Sqrt(game.entityMgr.entities[0].fitness.cpaDist) - 500f) + 1); // Remember that cpaDist and fitPDist are squared distances
            fpFit = 1.0f / (sumDist + 1);

            //float timeVal = (timePoint - game.fitnessMgr.timeMin) / (game.fitnessMgr.timeMax - game.fitnessMgr.timeMin); 

            fitness += 0.5f * cpaFit + 0.5f * fpFit;

            timeEnd = Time.realtimeSinceStartup;

            Debug.Log(fitness.ToString() + " " + cpaFit.ToString() + " " + fpFit.ToString() + " " + (timeEnd - timeStart).ToString());
        }
    }
}
