using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GraphMgr : MonoBehaviour
{
    public static GraphMgr inst;
    public GameObject graph;
    public float maxMag;
    public Vector2 size;
    public Vector3 position;
    public int resolution;

    private GameObject plane;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Destroy(plane);
            if (SelectionMgr.inst.selectedEntity != null && SelectionMgr.inst.selectedEntities.Count > 0)
                CreateGraph(SelectionMgr.inst.selectedEntity);
        }
    }

    public void DeleteAllGraphs()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Graph");
        foreach (GameObject go in gos)
            Destroy(go);
    }

    public void CreateGraph(SimulatedEntity sEntity)
    {
        plane = Instantiate(graph, sEntity.transform);
        plane.transform.localPosition = new Vector3(position.x, 0, position.z);
        plane.GetComponent<GraphPlane>().sEntity = sEntity;
    }

    public void SetSizeX(string x)
    {
        float floatx;
        float.TryParse(x, out floatx);
        size.x = floatx;
    }

    public void SetSizeY(string y)
    {
        float floaty;
        float.TryParse(y, out floaty);
        size.y = floaty;
    }

    public void SetPosX(string x)
    {
        float floatx;
        float.TryParse(x, out floatx);
        position.x = floatx;
    }

    public void SetPosZ(string z)
    {
        float floatz;
        float.TryParse(z, out floatz);
        position.z = floatz;
    }

    public void SetRes(string res)
    {
        int intres;
        int.TryParse(res, out intres);
        resolution = intres;
    }



}
