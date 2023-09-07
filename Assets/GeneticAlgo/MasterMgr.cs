using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;

public class MasterMgr : MonoBehaviour
{
    public static MasterMgr inst;
    public List<ChromoParameters> chromosomeParameters;

    public int testCount;
    int testID = 0;

    private void Awake()
    {
        inst = this;
        gaParameters = new GAParameters();
        gaParameters.chromosomeParameters = chromosomeParameters;
        gaParameters.chromosomeTotal = 0;

        foreach(ChromoParameters chromoP in chromosomeParameters)
        {
            gaParameters.chromosomeTotal += chromoP.chromosomeLength;
        }
    }

    private List<Thread> GAThreads = new List<Thread>();
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
        RandomMgr r = new RandomMgr(gaParameters.seed);

        for (int i = 0; i < testCount; i++)
        {
            StartJob();
        }
        //        GraphMgr.inst.SetAxisLimits(parameters.numberOfGenerations, 0, parameters.chromosomeLength);
    }
    //---------------------------------------------------------------------------------------

    void StartJob()
    {
        Thread GAThread = new Thread(GAStarter);
        GAThreads.Add(GAThread);
        GAThread.Start();
    }

    public string InitSemaphore = "1";
    public void GAStarter()
    {

        GeneticAlgo ga;
        lock (InitSemaphore)
        {

            gaParameters.seed = RandomMgr.inst.RandInt(0, 1000000, -1);
            gaParameters.id = testID;

            ga = new GeneticAlgo(gaParameters);
            testID++;
        }
        ga.Run();
        Debug.Log("GA done: ");

    }

    private void OnDestroy()
    {
        if (GAThreads.Count != 0)
        {
            foreach (Thread thread in GAThreads)
            {
                thread.Abort();
            }
        }
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
