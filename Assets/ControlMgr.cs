﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMgr : MonoBehaviour
{
    public static ControlMgr inst;
    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float deltaSpeed = 1;
    public float deltaHeading = 2;

    // Update is called once per frame
    void Update()
    {
        // Controls to add waypoint
        if(Input.GetMouseButtonDown(1) && SelectionMgr.inst.selectedEntity != null)
        {
            Vector3 pnt = Vector3.up * 1000;
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out RaycastHit hit))
            {
                pnt = hit.point;
            }
            List<int> ids = new List<int>();

            foreach(Entity381 entity in SelectionMgr.inst.selectedEntities)
            {
                ids.Add(entity.photonView.ViewID);
            }
            WaypointCommand wp = new WaypointCommand(PlayerCommand.GenerateID(), pnt, !Input.GetKey(KeyCode.LeftShift), ids);
            PlayerCommand.Instance.AddToCommList(wp);
        }


        // Controls to Change Speed/Heading
        Entity381 ent = SelectionMgr.inst.selectedEntity;
        if (ent != null) {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                DesiredSpeedCommand ds = new DesiredSpeedCommand(PlayerCommand.GenerateID(), deltaSpeed, ent.photonView.ViewID);
                PlayerCommand.Instance.AddToCommList(ds);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                DesiredSpeedCommand ds = new DesiredSpeedCommand(PlayerCommand.GenerateID(), -deltaSpeed, ent.photonView.ViewID);
                PlayerCommand.Instance.AddToCommList(ds);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                DesiredHeadingCommand dh = new DesiredHeadingCommand(PlayerCommand.GenerateID(), -deltaHeading, ent.photonView.ViewID);
                PlayerCommand.Instance.AddToCommList(dh);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                DesiredHeadingCommand dh = new DesiredHeadingCommand(PlayerCommand.GenerateID(), deltaHeading, ent.photonView.ViewID);
                PlayerCommand.Instance.AddToCommList(dh);
            }
        }
    }

    
    //------------------------------------------
}
