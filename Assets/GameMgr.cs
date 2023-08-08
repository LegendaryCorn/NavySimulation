using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public EntityMgr entityMgr;
    public DistanceMgr distanceMgr;
    public AIMgr aiMgr;

    private void Awake()
    {
        entityMgr = this.gameObject.AddComponent<EntityMgr>();
        distanceMgr = this.gameObject.AddComponent<DistanceMgr>();
        aiMgr = this.gameObject.AddComponent<AIMgr>();

        entityMgr.gameMgr = this;
        distanceMgr.gameMgr = this;
        aiMgr.gameMgr = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadScenario();

        float timeStart = Time.realtimeSinceStartup;
        RunGame(1f / 60f, 60f);
        float timeEnd = Time.realtimeSinceStartup;
        Debug.Log(timeEnd - timeStart);
    }

    public Vector3 position;
    public float spread = 20;
    public float colNum = 10;

    void LoadScenario()
    {
        Vector3 position = Vector3.zero;
        foreach (GameObject go in MasterMgr.inst.entityPrefabs)
        {
            Entity381 ent = entityMgr.CreateEntity(go.GetComponent<Entity381>().entityType, position, Vector3.zero);
            ent.ai.AddCommand(new Move(ent, position + Vector3.forward * 1000));
            //ent.isRealtime = true;
            position.x += 200;
        }
    }

    void RunGame(float dt, float t0) // t0 is in Seconds
    {
        for (float t = 0; t <= t0; t += dt)
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
