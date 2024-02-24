using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr
{

    public EntityMgr entityMgr;
    public DistanceMgr distanceMgr;
    public AIMgr aiMgr;
    public FitnessMgr fitnessMgr;

    public GameMgr(PotentialParameters p)
    {
        entityMgr = new EntityMgr(this);
        distanceMgr = new DistanceMgr(this);
        aiMgr = new AIMgr(this, p);
        fitnessMgr = new FitnessMgr(this);
    }

    public void ExecuteGame(int scenario)
    {
        LoadScenario(scenario);
        RunGame(1f / 20f, 4000f);
    }

    public Vector3 position;
    public float spread = 20;
    public float colNum = 10;

    public void LoadScenario(int scenarioID)
    {
        Scenario s = ScenarioMgr.inst.scenarios[scenarioID];

        foreach (ScenarioEntity scEnt in s.scenarioEntities)
        {
            Entity381 eD = ScenarioMgr.inst.GetEntityData(scEnt.type);
            Entity381 ent = entityMgr.CreateEntity(eD, scEnt.spawnPoint, scEnt.heading);

            foreach(Vector3 waypoint in scEnt.wayPoints)
            {
                ent.ai.AddCommand(new Move(ent, waypoint));
            }
            ent.fitness.SetAxisPoints(scEnt.fitAxisHeading, scEnt.fitPoints);
        }

        foreach (ScenarioBoundary scBound in s.scenarioBoundaries)
        {
            Boundary381 bound = new Boundary381(this, scBound.points);
            entityMgr.boundaries.Add(bound);
        }

        /*
        List<ScenarioEntity> trEnts = TrafficShipGenerator.TrafficShips(13, s.scenarioEntities[0], s.scenarioEntities[1]);

        foreach (ScenarioEntity scEnt in trEnts)
        {
            Entity381 eD = ScenarioMgr.inst.GetEntityData(scEnt.type);
            Entity381 ent = entityMgr.CreateEntity(eD, scEnt.spawnPoint, scEnt.heading);

            foreach (Vector3 waypoint in scEnt.wayPoints)
            {
                ent.ai.AddCommand(new Move(ent, waypoint));
            }
            ent.fitness.SetAxisPoints(scEnt.fitAxisHeading, scEnt.fitPoints);
        }
        */

        fitnessMgr.LoadParameters();
        fitnessMgr.timeMin = s.fitTimeMin;
        fitnessMgr.timeMax = s.fitTimeMax;
    }

    public void RunGame(float dt, float tf) // tf is in Seconds
    {
        int counter = 0;
        for (float t = 0; t < tf; t += dt)
        {
            distanceMgr.OnUpdate(dt);
            foreach(Entity381 ent in entityMgr.entities)
            {
                ent.ai.OnUpdate(dt);
            }
            foreach (Entity381 ent in entityMgr.entities)
            {
                ent.physics.OnUpdate(dt);
            }
            foreach (Entity381 ent in entityMgr.entities)
            {
                ent.fitness.OnUpdate(dt);
            }
            //if(counter % 10 == 9)
            fitnessMgr.OnUpdate(dt);
            counter += 1;
        }
    }
}
