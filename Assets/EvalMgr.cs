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

            float pMinCPA = Mathf.Infinity;
            int pMin1 = 0;
            int pMin2 = 0;
            float sMinCPA = Mathf.Infinity;
            int sMin1 = 0;
            int sMin2 = 0;
            foreach (int ship1 in game.fitnessMgr.twoShipFitnessParameters.Keys)
            {
                Dictionary<int, TwoShipFitnessParameters> shipDict1 = game.fitnessMgr.twoShipFitnessParameters[ship1];
                foreach (int ship2 in shipDict1.Keys)
                {
                    TwoShipFitnessParameters shipDict2 = shipDict1[ship2];
                    if (shipDict2.closestDistPar < pMinCPA)
                    {
                        pMinCPA = shipDict2.closestDistPar;
                        pMin1 = ship1;
                        pMin2 = ship2;
                    }
                    if (shipDict2.closestDistSkw < sMinCPA)
                    {
                        sMinCPA = shipDict2.closestDistSkw;
                        sMin1 = ship1;
                        sMin2 = ship2;
                    }
                }
            }

            timeEnd = Time.realtimeSinceStartup;

            Debug.Log(
                "Scenario " + x.ToString() + "\n" +
                "All Ships Closest (Skewed): " + sMinCPA.ToString() + " (" + sMin1.ToString() + ',' + sMin2.ToString() + ")" + "\n" +
                "All Ships Closest (Parallel): " + pMinCPA.ToString() + " (" + pMin1.ToString() + ',' + pMin2.ToString() + ")" + "\n" +
                "Ownship/Targetship Closest: " + Mathf.Sqrt(game.entityMgr.entities[0].fitness.cpaDist).ToString() + "\n" +
                "Evaluation Time: " + (timeEnd - timeStart).ToString());
        }
    }
}
