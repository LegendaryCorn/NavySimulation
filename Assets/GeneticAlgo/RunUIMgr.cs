using System.Collections;
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

    [SerializeField] private List<GameObject> panels;

    private void Awake()
    {
        inst = this;
    }

    private void Start()
    {
        RefreshGAParams();
        ShowPanel(0);
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

    public void ShowPanel(int p)
    {
        for(int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(i == p);
        }
    }
}
