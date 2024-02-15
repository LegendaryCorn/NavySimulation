using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr
{

    public EntityMgr entityMgr;
    public DistanceMgr distanceMgr;
    public AIMgr aiMgr;
    //public FitnessMgr fitnessMgr;

    public Dictionary<Entity381, List<Vector3>> recordPos;

    public GameMgr(PotentialParameters p)
    {
        entityMgr = new EntityMgr(this);
        distanceMgr = new DistanceMgr(this);
        aiMgr = new AIMgr(this, p);
        //fitnessMgr = new FitnessMgr(this);
    }

    public void ExecuteGame(int scenario)
    {
        LoadScenario(scenario);
        RunGame(1f / 10f, 800f);
    }

    public Vector3 position;
    public float spread = 20;
    public float colNum = 10;

    public void LoadScenario(int scenarioID)
    {
        Scenario s = ScenarioMgr.inst.scenarios[scenarioID];
        recordPos = new Dictionary<Entity381, List<Vector3>>();

        List<ScenarioEntity> entities = new List<ScenarioEntity>();
        entities.Add(s.ownShipEntity);
        entities.Add(s.targetShipEntity);
        foreach (ScenarioEntity trEnt in s.trafficEntities)
        {
            entities.Add(trEnt);
        }

        for(int i = 0; i < entities.Count; i++)
        {
            ScenarioEntity scEnt = entities[i];
            EntityRole role = i == 0 ? EntityRole.Own : EntityRole.Traffic;
            role = i == 1 ? EntityRole.Target : role;
            Entity381 eD = ScenarioMgr.inst.GetEntityData(scEnt.type);
            Entity381 ent = entityMgr.CreateEntity(eD, scEnt.spawnPoint, scEnt.heading, role);
            recordPos[ent] = new List<Vector3>();

            foreach(Vector3 waypoint in scEnt.wayPoints)
            {
                ent.ai.AddCommand(new Move(ent, waypoint));
            }
        }

        for(int i = 0; i < 2; i++) // Load the fitness points
        {
            entityMgr.entities[i].fitness.SetFitPoints(s.scenarioType, i == 0, entityMgr.entities[1 - i]);
        }

        //fitnessMgr.LoadParameters();
        //fitnessMgr.timeMin = s.fitTimeMin;
        //fitnessMgr.timeMax = s.fitTimeMax;
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
            //fitnessMgr.OnUpdate(dt);

            /* Disabled if it isn't being used
            foreach(Entity381 ent in entityMgr.entities)
            {
                if (!fitnessMgr.oneShipFitnessParameters[ent.id].reachedTarget)
                {
                    recordPos[ent].Add(ent.position);
                }
            }
            */

            counter += 1;
        }
    }
}
