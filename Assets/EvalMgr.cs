using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EvalPrint
{
    public int scenarioID;
    public float ownTargetCPA;
    public float sMinCPA;
    public float sShip1, sShip2;
    public float pMinCPA;
    public float pShip1, pShip2;
    public float evalTime;
}

public class EvalMgr : MonoBehaviour
{
    public static EvalMgr inst;
    GameMgr game;
    public PotentialParameters potentialParameters;
    public EScenarioType scenarioType;
    public int scenarioCount;
    public int scenarioToRecord;
    public bool usePotentials;
    public bool monoType;

    private void Awake()
    {
        inst = this;
    }

    private void Start()
    {

        float timeStart;
        float timeEnd;
        List<EvalPrint> evPrints = new List<EvalPrint>();

        int scCount = 0;
        // Generate scenarios
        if (monoType)
        {
            ScenarioMgr.inst.GenerateScenarios(scenarioType, scenarioCount);
            ScenarioMgr.inst.trafficCount *= 2;
            ScenarioMgr.inst.GenerateScenarios(scenarioType, scenarioCount);
            scCount = scenarioCount * 2;
        }
        else
        {
            foreach (ScenarioTypeData scData in ScenarioMgr.inst.scenarioTypeData)
            {
                ScenarioMgr.inst.GenerateScenarios(scData.scenarioType, scenarioCount);
                scCount += scenarioCount;
            }
        }


        for (int x = 0; x < scCount; x++) {
            timeStart = Time.realtimeSinceStartup;

            game = new GameMgr(potentialParameters);
            game.aiMgr.isPotentialFieldsMovement = usePotentials;
            if(x == scenarioToRecord)
            {
                game.fitnessMgr.recordLocations = true;
            }
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

            EvalPrint eP = new EvalPrint();
            eP.scenarioID = x;
            eP.ownTargetCPA = Mathf.Sqrt(game.entityMgr.entities[0].fitness.cpaDist);
            eP.sMinCPA = sMinCPA;
            eP.sShip1 = sMin1;
            eP.sShip2 = sMin2;
            eP.pMinCPA = pMinCPA;
            eP.pShip1 = pMin1;
            eP.pShip2 = pMin2;
            eP.evalTime = timeEnd - timeStart;
            evPrints.Add(eP);

            // Print paths
            if (x == scenarioToRecord)
            {
                CSVPositions(x);
            }
        }

        // Print
        try
        {
            string fname = "Output/VOResults/results_" + scenarioCount.ToString() + "_" + ScenarioMgr.inst.trafficCount.ToString() + "_" + System.DateTime.UtcNow.ToFileTime() + ".csv";

            using (StreamWriter sw = File.CreateText(fname))
            {
                sw.Write("Scenario ID,Own/Target Ship CPA,Closest Distance (Skewed),Ship 1,Ship 2,Closest Distance (Parallel),Ship 1,Ship 2,Evaluation Time\n");
                foreach (EvalPrint evalPrint in evPrints)
                {
                    sw.Write(EvalCSVLine(evalPrint));
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Invalid file! Exception encountered: " + e);
        }
    }

    public static string EvalCSVLine(EvalPrint eP)
    {
        string s = "";
        s += eP.scenarioID.ToString() + ",";
        s += eP.ownTargetCPA.ToString() + ",";
        s += eP.sMinCPA.ToString() + "," + eP.sShip1.ToString() + "," + eP.sShip2.ToString() + ",";
        s += eP.pMinCPA.ToString() + "," + eP.pShip1.ToString() + "," + eP.pShip2.ToString() + ",";
        s += eP.evalTime + "\n";
        return s;
    }

    public void CSVPositions(int x)
    {
        int timeSteps = game.fitnessMgr.oneShipFitnessParameters[0].positions.Count;
        // Print
        try
        {
            string fname = "Output/VOResults/positions_" + x.ToString() + "_" + ScenarioMgr.inst.trafficCount.ToString() + "_" + System.DateTime.UtcNow.ToFileTime() + ".csv";

            using (StreamWriter sw = File.CreateText(fname))
            {
                string header = "Time Step";
                foreach (Entity381 ship in game.entityMgr.entities)
                {
                    header += ",Ship " + ship.id.ToString() + " X Pos";
                    header += ",Ship " + ship.id.ToString() + " Z Pos";
                }
                sw.Write(header + "\n");
                for (int i = 0; i < timeSteps; i++)
                {
                    string s = i.ToString();
                    foreach(Entity381 ship in game.entityMgr.entities)
                    {
                        s += "," + game.fitnessMgr.oneShipFitnessParameters[ship.id].positions[i].x.ToString();
                        s += "," + game.fitnessMgr.oneShipFitnessParameters[ship.id].positions[i].z.ToString();
                    }
                    sw.Write(s + "\n");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Invalid file! Exception encountered: " + e);
        }
    }
}
