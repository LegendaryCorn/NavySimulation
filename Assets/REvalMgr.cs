using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class REvalMgr : MonoBehaviour
{
    public static REvalMgr inst;
    GameMgr game;
    public GAParameters gaParameters;
    public string potentialChromosome;
    public PotentialParameters potentialParameters;

    public float numRuns;
    public float angMin;
    public float angMax;
    public float distMin;
    public float distMax;
    public Vector3 shipOffset;
    public int seed;

    void Awake()
    {
        inst = this;
        Random.InitState(seed);
    }

    void Start()
    {
        if (potentialChromosome != "")
        {
            int[] chromosome = new int[potentialChromosome.Length];
            for (int i = 0; i < potentialChromosome.Length; i++)
            {
                string c = potentialChromosome[i].ToString();
                chromosome[i] = int.Parse(c);
            }
            float[] parsedChromosome = Evaluator.ParseChromosome(chromosome, gaParameters);
            potentialParameters = new PotentialParameters(parsedChromosome); //Evaluator.ParseChromosome(chromosome);
        }

        List<float> evals = new List<float>();
        float sum = 0;

        for (int i = 0; i < numRuns; i++)
        {
            GenerateScenario();
            float e = EvalGame();
            evals.Add(e);
            sum += e;
        }

        float avg = sum / evals.Count;
        float sumSq = 0;
        float min = 1f;
        float max = 0f;
        foreach (float eval in evals)
        {
            sumSq += (eval - avg) * (eval - avg);
            min = Mathf.Min(min, eval);
            max = Mathf.Max(max, eval);
        }
        float stD = Mathf.Sqrt(sumSq / evals.Count);

        Debug.Log("Minimum: " + min.ToString() + "\nMaximum: " + max.ToString() + "\nAverage: " + avg.ToString() + "\nStDev: " + stD.ToString());
    }

    public void GenerateScenario()
    {
        // Generate the random variables
        float ang = Mathf.Deg2Rad * Random.Range(angMin, angMax);
        float dist = Random.Range(distMin, distMax);

        // Calculate the starting, ending, and cross points
        Vector3 spawnP = new Vector3(dist * Mathf.Sin(ang), 0, dist * Mathf.Cos(ang) - 3000f);
        Vector3 wayP = new Vector3(-spawnP.x * 7f/6f, 0, -spawnP.z * 7f / 6f);
        float cAng = Mathf.Atan2(spawnP.x, spawnP.z);
        Vector3 crossP = new Vector3(750f * Mathf.Sin(cAng), 0, 750f * Mathf.Cos(cAng));

        float speed = 30f * spawnP.magnitude / 3000f;

        // Calculate the waypoints
        ScenarioEntity v1 = ScenarioMgr.inst.scenarios[0].scenarioEntities[0];
        ScenarioEntity v2 = ScenarioMgr.inst.scenarios[0].scenarioEntities[1];
        Vector3 sp1 = new Vector3(0, 0, -3000);
        Vector3 wp1 = new Vector3(0, 0, 3500);

        v1.fitPoints[0] = Vector3.Lerp(sp1, crossP, 1f / 3f);
        v1.fitPoints[1] = Vector3.Lerp(sp1, crossP, 2f / 3f);
        v1.fitPoints[2] = crossP;
        v1.fitPoints[3] = Vector3.Lerp(crossP, wp1, 1f / 3f);
        v1.fitPoints[4] = Vector3.Lerp(crossP, wp1, 2f / 3f);

        v2.fitPoints[0] = Vector3.Lerp(spawnP, wayP, 1f / 6f);
        v2.fitPoints[1] = Vector3.Lerp(spawnP, wayP, 2f / 6f);
        v2.fitPoints[2] = Vector3.Lerp(spawnP, wayP, 3f / 6f);
        v2.fitPoints[3] = Vector3.Lerp(spawnP, wayP, 4f / 6f);
        v2.fitPoints[4] = Vector3.Lerp(spawnP, wayP, 5f / 6f);

        // Add to the scenario
        ScenarioMgr.inst.entityData[1].maxSpeed = speed;
        v2.spawnPoint = spawnP;
        v2.wayPoints[0] = wayP;
        v2.heading = cAng * Mathf.Rad2Deg + 180f;

        ScenarioMgr.inst.scenarios[0].scenarioEntities[0] = v1;
        ScenarioMgr.inst.scenarios[0].scenarioEntities[1] = v2;
    }

    public float EvalGame()
    {

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
        float sumShip = 0f;
        Entity381 ent = game.entityMgr.entities[i];
        for (int j = 0; j < ent.fitness.dists.Count; j++)
        {
            sumShip += Mathf.Sqrt(ent.fitness.dists[j]);
        }
        sumDist += sumShip / ent.fitness.dists.Count;
        }
        sumDist *= 1.0f / game.entityMgr.entities.Count;

        float timeVal = (timePoint - game.fitnessMgr.timeMin) / (game.fitnessMgr.timeMax - game.fitnessMgr.timeMin);

        fitness += Mathf.Pow(0.01f * sumDist + 25f * timeVal * timeVal + 1f, -0.5f);

        return fitness;
    }
}
