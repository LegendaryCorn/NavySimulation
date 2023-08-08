using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public static GameMgr inst;
    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 position = Vector3.zero;
        foreach(GameObject go in EntityMgr.inst.entityPrefabs) {
            Entity381 ent = EntityMgr.inst.CreateEntity(go.GetComponent<Entity381>().entityType, position, Vector3.zero);
            ent.ai.AddCommand(new Move(ent, position + Vector3.forward * 1000));
            //ent.isRealtime = true;
            position.x += 200;
        }

        float timeStart = Time.realtimeSinceStartup;
        RunGame(1f / 60f, 60f);
        float timeEnd = Time.realtimeSinceStartup;
        Debug.Log(timeEnd - timeStart);
    }

    public Vector3 position;
    public float spread = 20;
    public float colNum = 10;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F12)) {
            for (int i = 0; i < 10; i++) {
                for (int j = 0; j < 10; j++) {
                    Entity381 ent = EntityMgr.inst.CreateEntity(EntityType.PilotVessel, position, Vector3.zero);
                    position.z += spread;
                }
                position.x += spread;
                position.z = 0;
            }
            DistanceMgr.inst.Initialize();
        }
    }

    void RunGame(float dt, float t0) // t0 is in Seconds
    {
        for (float t = 0; t <= t0; t += dt)
        {
            DistanceMgr.inst.OnUpdate(dt);
            foreach(Entity381 ent in EntityMgr.inst.entities)
            {
                ent.ai.OnUpdate(dt);
            }
            foreach (Entity381 ent in EntityMgr.inst.entities)
            {
                ent.physics.OnUpdate(dt);
            }
        }
    }
}
