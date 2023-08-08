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

    public List<GameObject> entityPrefabs;
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

        for (int i = 0; i < 20; i++)
        {
            var job = new GameJob(new GameMgr());

            jobHandles.Add(job.Schedule<GameJob>());
        }

        foreach (JobHandle jobHandle in jobHandles)
        {
            jobHandle.Complete();
        }

        timeEnd = Time.realtimeSinceStartup;
        Debug.Log(timeEnd - timeStart);
    }
}
