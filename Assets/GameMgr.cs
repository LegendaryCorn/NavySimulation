using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr
{

    public EntityMgr entityMgr;
    public DistanceMgr distanceMgr;
    public AIMgr aiMgr;

    public GameMgr()
    {
        entityMgr = new EntityMgr(this);
        distanceMgr = new DistanceMgr(this);
        aiMgr = new AIMgr(this);
    }

    public void ExecuteGame()
    {
        LoadScenario();
        RunGame(1f / 60f, 600f);
    }

    public Vector3 position;
    public float spread = 20;
    public float colNum = 10;

    public void LoadScenario()
    {
        Vector3 position = Vector3.zero;
        foreach (Entity381 eD in ScenarioMgr.inst.entityData)
        {
            Entity381 ent = entityMgr.CreateEntity(eD, position, Vector3.zero);
            ent.ai.AddCommand(new Move(ent, position + Vector3.forward * 1000));
            //ent.isRealtime = true;
            position.x += 200;
        }
    }

    public void RunGame(float dt, float t0) // t0 is in Seconds
    {
        for (float t = 0; t < t0; t += dt)
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
        }
    }
}
