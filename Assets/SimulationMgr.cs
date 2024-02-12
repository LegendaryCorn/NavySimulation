using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationMgr : MonoBehaviour
{
    public static SimulationMgr inst;

    public int scenarioID;
    public EScenarioType scenarioType;
    public int scenarioCount;

    GameMgr gameMgr;

    public List<SimulatedEntity> simulatedEntityPrefabs;
    public GameObject entitiesRoot;
    public List<SimulatedEntity> simulatedEntities;
    public LineRenderer waypointLinePrefab;

    public LineRenderer simulatedBoundaryPrefab;
    public GameObject boundariesRoot;
    public List<LineRenderer> simulatedBoundaries;

    public GAParameters gaParameters;
    public float simSpeed;
    public string potentialChromosome;
    public PotentialParameters potentialParameters;

    float dt = 1f / 20f;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ScenarioMgr.inst.GenerateScenarios(scenarioType, scenarioCount);

        if(potentialChromosome != "")
        {
            int[] chromosome = new int[potentialChromosome.Length];
            for(int i = 0; i < potentialChromosome.Length; i++)
            {
                string c = potentialChromosome[i].ToString();
                chromosome[i] = int.Parse(c);
            }
            float[] parsedChromosome = Evaluator.ParseChromosome(chromosome, gaParameters);
            potentialParameters = new PotentialParameters(parsedChromosome); //Evaluator.ParseChromosome(chromosome);
        }

        gameMgr = new GameMgr(potentialParameters);
        gameMgr.LoadScenario(scenarioID); // Based on Scenario ID

        scenarioCount = ScenarioMgr.inst.scenarios.Count;

        // Load Scenario Entities
        foreach (Entity381 ent in gameMgr.entityMgr.entities)
        {
            SimulatedEntity simEnt = Instantiate<SimulatedEntity>(FindSimulatedEntityPrefab(ent.entityType));
            simEnt.id = ent.id;
            simEnt.ent = ent;
            simEnt.transform.parent = entitiesRoot.transform;
            simulatedEntities.Add(simEnt);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update Sim
        gameMgr.RunGame(dt, dt * simSpeed);

        // Change Sim if input
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            LoadNewScenario((scenarioID + 1) % scenarioCount);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            LoadNewScenario((scenarioID + scenarioCount - 1) % scenarioCount);
        }
    }

    SimulatedEntity FindSimulatedEntityPrefab(EntityType e)
    {
        foreach (SimulatedEntity simEnt in simulatedEntityPrefabs)
        {
            if (simEnt.type == e)
            {
                return simEnt;
            }
        }
        return null;
    }

    void LoadNewScenario(int id)
    {
        for(int i = 0; i < simulatedEntities.Count; i++)
        {
            Destroy(simulatedEntities[i].gameObject);
        }

        scenarioID = id;
        gameMgr = new GameMgr(potentialParameters);
        gameMgr.LoadScenario(scenarioID); // Based on Scenario ID
        simulatedEntities = new List<SimulatedEntity>();

        // Load Scenario Entities
        foreach (Entity381 ent in gameMgr.entityMgr.entities)
        {
            SimulatedEntity simEnt = Instantiate<SimulatedEntity>(FindSimulatedEntityPrefab(ent.entityType));
            simEnt.id = ent.id;
            simEnt.ent = ent;
            simEnt.transform.parent = entitiesRoot.transform;
            simulatedEntities.Add(simEnt);
        }
    }
}
