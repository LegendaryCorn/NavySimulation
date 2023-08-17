using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;

public class MasterMgr : MonoBehaviour
{
    public static MasterMgr inst;
    public int chromosomeLength = 32;

    private void Awake()
    {
        inst = this;
        gaParameters = new GAParameters();
        gaParameters.chromosomeLength = chromosomeLength;
    }

    private Thread GAThread;
    private int GAResult;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    


    public GAParameters gaParameters;
    public void OnSubmit()
    {
        StartJob();
        //        GraphMgr.inst.SetAxisLimits(parameters.numberOfGenerations, 0, parameters.chromosomeLength);
    }
    //---------------------------------------------------------------------------------------

    void StartJob()
    {
        GAThread = new Thread(GAStarter);
        GAThread.Start();
    }
    GeneticAlgo ga;
    public void GAStarter()
    {
        ga = new GeneticAlgo(gaParameters);
        ga.Run();
        Debug.Log("GA done: ");

    }

    private void OnDestroy()
    {
        if (GAThread != null) GAThread.Join();
    }
    //---------------------------------------------------------------------------------------

    public string LogSemaphore = "1";
    public void ThreadLog(string msg)
    {
        lock (LogSemaphore)
        {
            Debug.Log("--->GA: " + msg);

        }
    }
    /*
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

    public GAParameters gaParameters;

    float timeStart;
    float timeEnd;

    private void Awake()
    {
        inst = this;
        gaParameters = new GAParameters();
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
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
    */
}
