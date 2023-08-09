using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;

public class MasterMgr : MonoBehaviour
{
    struct GameJob : IJob
    {
        public readonly GameMgr gameMgr;
        public float fitness;

        public GameJob(GameMgr g)
        {
            gameMgr = g;
            fitness = 0;
        }

        public void Execute()
        {
            gameMgr.RunGame();
        }
    }

    public static MasterMgr inst;

    public List<Entity381> entityData;
    public GameObject entitiesRoot;

    float timeStart;
    float timeEnd = -1f;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        timeStart = Time.realtimeSinceStartup;
        List<JobHandle> jobHandles = new List<JobHandle>();
        List<GameJob> jobs = new List<GameJob>();

        for (int i = 0; i < 15; i++)
        {
            var job = new GameJob(new GameMgr());

            jobs.Add(job);
            jobHandles.Add(job.Schedule<GameJob>());
        }

        for (int i = 0; i < jobHandles.Count; i++)
        {
            JobHandle jobHandle = jobHandles[i];
            jobHandle.Complete();
            
            foreach(Entity381 ent in jobs[i].gameMgr.entityMgr.entities)
            {
                Debug.Log(ent.position);
            }
        }

        timeEnd = Time.realtimeSinceStartup;
        Debug.Log(timeEnd - timeStart);
    }
}
