﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunUIMgr : MonoBehaviour
{
    public static RunUIMgr inst;

    [SerializeField] private InputField popSizeField;
    [SerializeField] private InputField genCountField;
    [SerializeField] private InputField chromoLenField;
    [SerializeField] private InputField crossProbField;
    [SerializeField] private InputField mutProbField;
    [SerializeField] private InputField seedField;

    public RectTransform graphArea;
    public LineRenderer avgLine;
    public LineRenderer maxLine;

    List<float> avgList;
    List<float> maxList;
    public int genCount;

    [SerializeField] private List<GameObject> panels;

    private void Awake()
    {
        inst = this;

        avgList = new List<float>();
        maxList = new List<float>();
    }

    private void Start()
    {
        RefreshGAParams();
        ShowPanel(0);
    }

    private void Update()
    {
        if (avgList.Count > 0)
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
            //MasterMgr.inst.gaParameters.chromosomeLength = int.Parse(chromoLenField.text);
            MasterMgr.inst.gaParameters.pCross = float.Parse(crossProbField.text);
            MasterMgr.inst.gaParameters.pMut = float.Parse(mutProbField.text);
            MasterMgr.inst.gaParameters.seed = int.Parse(seedField.text);
        }
        catch (System.Exception e)
        {

        }
    }

    public void NewGraphEntry(float avg, float max)
    {
        avgList.Add(avg);
        maxList.Add(max);
    }

    void UpdateGraph()
    {
        float minVal = avgList[0];
        foreach (float val in avgList)
            minVal = Mathf.Min(val, minVal);
        float maxVal = maxList[0];
        foreach (float val in maxList)
            maxVal = Mathf.Max(val, maxVal);

        float top = graphArea.rect.center.y + graphArea.rect.height * 0.5f;
        float bottom = graphArea.rect.center.y - graphArea.rect.height * 0.5f;
        float left = graphArea.rect.center.x - graphArea.rect.width * 0.5f;
        float right = graphArea.rect.center.x + graphArea.rect.width * 0.5f;
        int maxCount = maxList.Count;

        avgLine.positionCount = maxCount;
        maxLine.positionCount = maxCount;

        for(int i = 0; i < maxCount; i++)
        {
            Vector3 a = new Vector3(i * 1f / genCount * (right - left) + left, (avgList[i] - minVal) / (maxVal - minVal) * (top - bottom) + bottom, 0);
            avgLine.SetPosition(i, a);
            Vector3 m = new Vector3(i * 1f / genCount * (right - left) + left, (maxList[i] - minVal) / (maxVal - minVal) * (top - bottom) + bottom, 0);
            maxLine.SetPosition(i, m);
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
