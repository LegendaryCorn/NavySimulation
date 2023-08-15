using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationMgr : MonoBehaviour
{
    public static SimulationMgr inst;

    public int scenarioID;

    GameMgr gameMgr;

    public List<SimulatedEntity> simulatedEntityPrefabs;
    public GameObject entitiesRoot;
    public List<SimulatedEntity> simulatedEntities;

    public LineRenderer simulatedBoundaryPrefab;
    public GameObject boundariesRoot;
    public List<LineRenderer> simulatedBoundaries;

    public float simSpeed;
    public PotentialParameters potentialParameters;

    float dt = 1f / 120f;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameMgr = new GameMgr(potentialParameters);
        gameMgr.LoadScenario(scenarioID); // Based on Scenario ID

        // Load Scenario Entities
        foreach (Entity381 ent in gameMgr.entityMgr.entities)
        {
            SimulatedEntity simEnt = Instantiate<SimulatedEntity>(FindSimulatedEntityPrefab(ent.entityType));
            simEnt.id = ent.id;
            simEnt.transform.parent = entitiesRoot.transform;
            simulatedEntities.Add(simEnt);
        }

        // Load Lines
        foreach (Boundary381 bound in gameMgr.entityMgr.boundaries)
        {
            LineRenderer line = Instantiate<LineRenderer>(simulatedBoundaryPrefab);
            line.transform.parent = boundariesRoot.transform;
            line.positionCount = bound.points.Count;
            for (int i = 0; i < bound.points.Count; i++)
            {
                Vector3 point = bound.points[i];
                line.SetPosition(i, point);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Update Sim
        gameMgr.RunGame(dt, dt * 2f * simSpeed);

        // Update Ships
        for(int i = 0; i < simulatedEntities.Count; i++)
        {
            SimulatedEntity simEnt = simulatedEntities[i];
            Entity381 ent = gameMgr.entityMgr.entities[i];

            simEnt.transform.position = ent.position;
            simEnt.transform.eulerAngles = new Vector3(0, ent.heading, 0);

            simEnt.speed = ent.speed;
            simEnt.desiredSpeed = ent.desiredSpeed;
            simEnt.heading = ent.heading;
            simEnt.desiredHeading = ent.desiredHeading;
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
}
