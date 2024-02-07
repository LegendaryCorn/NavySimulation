using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class RunUIMgr : MonoBehaviour
{
    public static RunUIMgr inst;


    [Header("Start UI Elements")]
    [SerializeField] private InputField popSizeField;
    [SerializeField] private InputField genCountField;
    [SerializeField] private InputField crossProbField;
    [SerializeField] private InputField mutProbField;
    [SerializeField] private InputField seedField;
    [SerializeField] private InputField testCountField;
    [SerializeField] private InputField scenarioIDField;

    [Header("Graph UI Elements")]
    public RectTransform graphArea;
    public LineRenderer avgLine;
    public LineRenderer maxLine;

    public Text lowText;
    public Text highText;
    public Text avgText;
    public Text maxText;


    Dictionary<int, List<float>> avgLists;
    Dictionary<int, List<float>> maxLists;
    List<float> averageAvg;
    List<float> averageMax;
    List<string> bestLog;
    [Header("Other")]
    public int genCount;
    public string fileName;

    Object entryLock = new Object();

    [SerializeField] private List<GameObject> panels;

    private void Awake()
    {
        inst = this;

        avgLists = new Dictionary<int, List<float>>();
        maxLists = new Dictionary<int, List<float>>();
        averageAvg = new List<float>();
        averageMax = new List<float>();
        bestLog = new List<string>();
    }

    private void Start()
    {
        RefreshGAParams();
        ShowPanel(0);
    }

    private void Update()
    {
        if (averageAvg.Count > 0)
        {
            UpdateGraph();
        }
    }

    public void RefreshGAParams()
    {
        try
        {
            MasterMgr.inst.gaParameters.populationSize = int.Parse(popSizeField.text);
            MasterMgr.inst.gaParameters.numberOfGenerations = int.Parse(genCountField.text);
            MasterMgr.inst.gaParameters.pCross = float.Parse(crossProbField.text);
            MasterMgr.inst.gaParameters.pMut = float.Parse(mutProbField.text);
            MasterMgr.inst.gaParameters.seed = int.Parse(seedField.text);
            MasterMgr.inst.testCount = int.Parse(testCountField.text);
            MasterMgr.inst.gaParameters.scenario = int.Parse(scenarioIDField.text);
        }
        catch (System.Exception e)
        {

        }
    }

    public void NewListEntry(int id, int gen, Individual best)
    {
        bestLog.Add(id.ToString() + "." + gen.ToString() + ": " + best.ToString());
    }

    // Should be called by a button
    public void PrintNewList()
    {
        try
        {
            string fname = fileName + System.DateTime.UtcNow.ToFileTime() + ".txt";

            using (StreamWriter sw = File.CreateText(fname))
            {
                foreach (string bLogLine in bestLog)
                {
                    sw.Write(bLogLine + "\n");
                }
                sw.Write("---------------------------------------------------");
                for(int i = 0; i < averageAvg.Count; i++)
                {
                    sw.Write("\n" + i + ": " + averageAvg[i].ToString() + "|" + averageMax[i].ToString());
                }
            }
        }
        catch(System.Exception e)
        {
            Debug.LogError("Invalid file! Exception encountered: " + e);
        }
    }

    public void NewGraphEntry(float avg, float max, int id)
    {
        lock (entryLock)
        {
            if (!avgLists.ContainsKey(id))
            {
                avgLists[id] = new List<float>();
                maxLists[id] = new List<float>();
            }

            avgLists[id].Add(avg);
            maxLists[id].Add(max);
            UpdateAverages();
        }
    }

    void UpdateAverages()
    {
        averageAvg = new List<float>();
        averageMax = new List<float>();
        List<float> count = new List<float>();

        for(int i = 0; i < genCount; i++)
        {
            count.Add(0f);
        }
        foreach(List<float> avgL in avgLists.Values)
        {
            for(int i = 0; i < avgL.Count; i++)
            {
                if(i >= averageAvg.Count)
                {
                    averageAvg.Add(0f);
                }
                averageAvg[i] += avgL[i];
                count[i] += 1f;
            }
        }
        foreach (List<float> maxL in maxLists.Values)
        {
            for (int i = 0; i < maxL.Count; i++)
            {
                if (i >= averageMax.Count)
                {
                    averageMax.Add(0f);
                }
                averageMax[i] += maxL[i];
            }
        }
        for(int i = 0; i < averageAvg.Count; i++)
        {
            averageAvg[i] *= 1f / count[i];
            averageMax[i] *= 1f / count[i];
        }

        
    }

    void UpdateGraph()
    {
        lock (entryLock)
        {
            /*
            float minVal = averageAvg[0];
            foreach (float val in averageAvg)
                minVal = Mathf.Min(val, minVal);
            float maxVal = averageMax[0];
            foreach (float val in averageMax)
                maxVal = Mathf.Max(val, maxVal);
            */
            float minVal = 0f;
            float maxVal = 1f;

            float top = graphArea.rect.center.y + graphArea.rect.height * 0.5f;
            float bottom = graphArea.rect.center.y - graphArea.rect.height * 0.5f;
            float left = graphArea.rect.center.x - graphArea.rect.width * 0.5f;
            float right = graphArea.rect.center.x + graphArea.rect.width * 0.5f;
            int maxCount = averageMax.Count;

            avgLine.positionCount = maxCount;
            maxLine.positionCount = maxCount;

            float avgFinal = 0;
            float maxFinal = 0;
            for (int i = 0; i < maxCount; i++)
            {
                Vector3 a = new Vector3(i * 1f / (genCount-1) * (right - left) + left, (averageAvg[i] - minVal) / (maxVal - minVal) * (top - bottom) + bottom, 0);
                avgLine.SetPosition(i, a);
                Vector3 m = new Vector3(i * 1f / (genCount-1) * (right - left) + left, (averageMax[i] - minVal) / (maxVal - minVal) * (top - bottom) + bottom, 0);
                maxLine.SetPosition(i, m);

                if(i + 1 == maxCount)
                {
                    avgFinal = averageAvg[i];
                    maxFinal = averageMax[i];
                }
            }

            lowText.text = minVal.ToString("F3");
            highText.text = maxVal.ToString("F3");
            avgText.text = avgFinal.ToString("F3");
            maxText.text = maxFinal.ToString("F3");
        }
    }

    public void ShowPanel(int p)
    {
        for(int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(i == p);
        }
    }
}
