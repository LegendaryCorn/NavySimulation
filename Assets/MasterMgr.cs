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
            gameMgr.ExecuteGame();
        }
    }

    public static MasterMgr inst;

    float timeStart;
    float timeEnd;

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

        PotentialParameters parameters = new PotentialParameters
        {
            potentialDistanceThreshold = 1000,
            attractionCoefficient = 20000,
            attractiveExponent = -1,
            repulsiveCoefficient = 1000,
            repulsiveExponent = -2.0f,
        };

        for (int i = 0; i < 15; i++)
        {
            var job = new GameJob(new GameMgr(parameters));

            jobs.Add(job);
            jobHandles.Add(job.Schedule<GameJob>());
        }

        for (int i = 0; i < jobHandles.Count; i++)
        {
            JobHandle jobHandle = jobHandles[i];
            jobHandle.Complete();

            Debug.Log(jobs[i].gameMgr.fitnessMgr.totalFitness);
        }

        timeEnd = Time.realtimeSinceStartup;
        Debug.Log(timeEnd - timeStart);
    }
}
