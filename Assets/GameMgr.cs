﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class GameMgr : MonoBehaviour
{
    public static GameMgr inst;

    [SerializeField] private int numShipsOnJoin = 4;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Vector3 position = Vector3.zero;
            foreach (GameObject go in EntityMgr.inst.entityPrefabs)
            {
                Entity381 ent = EntityMgr.inst.CreateEntity(go.GetComponent<Entity381>().entityType, position, Vector3.zero);
                position.x += 200;
            }
        }

        int j = numShipsOnJoin;
        foreach (Entity381 e in EntityMgr.inst.entities)
        {
            if(j == 0)
            {
                break;
            }
            if (!e.hasOwner)
            {
                e.AssignOwner();
                j--;
            }
        }
    }

    public Vector3 position;
    public float spread = 20;
    public float colNum = 10;
    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(1) && SelectionMgr.inst.selectedEntities.Count == 0) {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out RaycastHit hit))
            {
                GameObject go = EntityMgr.inst.entityPrefabs[0];
                Entity381 ent = EntityMgr.inst.CreateEntity(go.GetComponent<Entity381>().entityType, hit.point, Vector3.zero);
                ent.AssignOwner();
                DistanceMgr.inst.Initialize();
            }
        }
    }
}
