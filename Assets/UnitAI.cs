﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Photon.Pun;

public class UnitAI : MonoBehaviourPunCallbacks, IPunObservable
{
    public Entity381 entity; //public only for ease of debugging
    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponentInParent<Entity381>();
        commands = new List<Command>();
        intercepts = new List<Intercept>();
        moves = new List<Move>();
    }

    public List<Move> moves;
    public List<Command> commands;
    public List<Intercept> intercepts;
    public List<Vector3> waypoints;

    // Update is called once per frame
    void Update()
    {
        if (commands.Count > 0) {
            if (PhotonNetwork.IsMasterClient)
            {
                if (commands[0].IsDone())
                {
                    StopAndRemoveCommand(0);
                }
                else
                {
                    commands[0].Tick();
                    commands[0].isRunning = true;
                    DecorateAll();
                }
            }
            else
            {
                DecorateAll();
            }
        }
    }

    void StopAndRemoveCommand(int index)
    {
        commands[index].Stop();
        commands.RemoveAt(index);
    }
    
    public void StopAndRemoveAllCommands()
    {
        for(int i = commands.Count - 1; i >= 0; i--) {
            StopAndRemoveCommand(i);
        }
    }

    public void AddCommand(Command c)
    {
        //print("Adding command; " + c.ToString());
        c.Init();
        commands.Add(c);
        if (c is Intercept)
            intercepts.Add(c as Intercept);
        else if (c is Follow)
            ;
        else
            moves.Add(c as Move);
    }

    public void SetCommand(Command c)
    {
        //print("Setting command: " + c.ToString());
        StopAndRemoveAllCommands();
        commands.Clear();
        moves.Clear();
        intercepts.Clear();
        AddCommand(c);

    }
    //---------------------------------

    public void DecorateAll()
    {
        Command prior = null;
        foreach(Command c in commands) {
            Decorate(prior, c);
            prior = c;
        }
    }

    //decoration logic (UI logic) in general is always convoluted. Ugh
    public void Decorate(Command prior, Command current)
    {
        if (current.line != null) {
            current.line.gameObject.SetActive(entity.isSelected);
            if (prior == null)
                current.line.SetPosition(0, entity.position);
            else
                current.line.SetPosition(0, prior.line.GetPosition(1));

            if (current is Intercept) { //Most specific
                Intercept intercept = current as Intercept;
                if (intercept.isRunning)// 
                    intercept.line.SetPosition(1, intercept.predictedMovePosition);
                else
                    intercept.line.SetPosition(1, intercept.targetEntity.position);
                intercept.line.SetPosition(2, intercept.targetEntity.position);

            } else if (current is Follow) { // Less specific
                Follow f = current as Follow;
                f.line.SetPosition(1, f.targetEntity.position + f.offset);
                f.line.SetPosition(2, f.targetEntity.position);
                //f.line.SetPosition(1, f.predictedMovePosition);
            }
            //Moveposition never changes
        }

        //potential fields lines
        if(!(current is Follow) && !(current is Intercept) && AIMgr.inst.isPotentialFieldsMovement){ 
            Move m = current as Move;
            m.potentialLine.SetPosition(0, entity.position);
            Vector3 newpos = Vector3.zero;
            newpos.x = Mathf.Sin(entity.desiredHeading * Mathf.Deg2Rad) * entity.desiredSpeed;
            newpos.z = Mathf.Cos(entity.desiredHeading * Mathf.Deg2Rad) * entity.desiredSpeed;
            newpos *= 20;
            newpos.y = 1;
            m.potentialLine.SetPosition(1, entity.position + newpos);
            m.potentialLine.gameObject.SetActive(entity.isSelected);
        }


    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            List<float> waypointsList = new List<float>();

            foreach(Vector3 waypoint in waypoints)
            {
                waypointsList.Add(waypoint.x);
                waypointsList.Add(waypoint.y);
                waypointsList.Add(waypoint.z);
            }

            string waypointsJson = JsonUtility.ToJson(waypointsList);

            stream.SendNext(waypointsJson);
        }
        else
        {
            string waypointsJson = (string)stream.ReceiveNext();
            waypoints = new List<Vector3>();
            List<float> waypointsList = JsonUtility.FromJson<List<float>>(waypointsJson);
            for(int i = 0; i < waypointsList.Count; i += 3)
            {
                waypoints.Add(new Vector3(waypointsList[i], waypointsList[i+1], waypointsList[i+2]));
            }

        }
    }

}
