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
        for (int sc = 0; sc < 2; sc++)
        {
            float timeStart;
            float timeEnd;

            timeStart = Time.realtimeSinceStartup;

            game = new GameMgr(potentialParameters);
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

                if (ent.entityType == EntityType.DDG51) // DDG51 will be the dedicated "main" ship for now
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

            float mainFit = mainCount == 0 ? 1f : Mathf.Pow(mainDist / mainCount, -0.2f);
            float sideFit = sideCount == 0 ? 1f : Mathf.Pow(sideDist / sideCount, -0.2f);
            float timeFit = 1f - (timePoint - game.fitnessMgr.timeMin) / (game.fitnessMgr.timeMax - game.fitnessMgr.timeMin);
            timeFit = Mathf.Clamp01(timeFit);
            float fitness = (0.4f * mainFit + 0.4f * sideFit + 0.2f * timeFit);

            timeEnd = Time.realtimeSinceStartup;

            Debug.Log(fitness.ToString() + " " + mainFit.ToString() + " " + sideFit.ToString() + " " + timeFit.ToString() + " " + (timeEnd - timeStart).ToString());
        }

    }
}
