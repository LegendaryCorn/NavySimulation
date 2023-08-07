using System.Collections;
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
        waypoints = new List<Vector3>();

        waypointsMoveLine = LineMgr.inst.CreateMoveLine(Vector3.zero, Vector3.zero);
        waypointsPotentialLine = LineMgr.inst.CreatePotentialLine(Vector3.zero);
        waypointsMoveLine.gameObject.SetActive(false);
        waypointsPotentialLine.gameObject.SetActive(false);

    }

    public List<Move> moves;
    public List<Command> commands;
    public List<Intercept> intercepts;
    public List<Vector3> waypoints;

    public LineRenderer waypointsMoveLine;
    public LineRenderer waypointsPotentialLine;

    // Update is called once per frame
    void Update()
    {
        if (commands.Count > 0) {

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

        if(!PhotonNetwork.IsMasterClient)
        {
            DecorateWaypoints();
        }
    }

    void StopAndRemoveCommand(int index)
    {
        commands[index].Stop();
        commands.RemoveAt(index);
        waypoints.RemoveAt(index);
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
        {
            moves.Add(c as Move);
            waypoints.Add(moves[moves.Count - 1].movePosition);
        }
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

    public void DecorateWaypoints()
    {
        int wMax = waypoints.Count;
        waypointsMoveLine.positionCount = wMax;
        waypointsMoveLine.SetPosition(0, entity.position);
        for(int i = 0; i < wMax; i++)
        {
            waypointsMoveLine.SetPosition(i + 1, waypoints[i]);
        }

        waypointsPotentialLine.SetPosition(0, entity.position);
        Vector3 newpos = Vector3.zero;
        newpos.x = Mathf.Sin(entity.desiredHeading * Mathf.Deg2Rad) * entity.desiredSpeed;
        newpos.z = Mathf.Cos(entity.desiredHeading * Mathf.Deg2Rad) * entity.desiredSpeed;
        newpos *= 20;
        newpos.y = 1;
        waypointsPotentialLine.SetPosition(1, entity.position + newpos);

        waypointsMoveLine.gameObject.SetActive(entity.isSelected && commands.Count != 0);
        waypointsPotentialLine.gameObject.SetActive(entity.isSelected && commands.Count != 0);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            string waypointString = "";

            foreach(Vector3 waypoint in waypoints)
            {
                waypointString += "$" + waypoint.x + "&" + waypoint.y + "&" + waypoint.z;
            }

            stream.SendNext(waypointString);
        }
        else
        {
            string waypointString = (string)stream.ReceiveNext();

            string[] splitString = waypointString.Split('$');
            waypoints = new List<Vector3>();

            for(int i = 1; i < splitString.Length; i++)
            {
                string[] xyz = splitString[i].Split('&');
                waypoints.Add(new Vector3(float.Parse(xyz[0]), float.Parse(xyz[1]), float.Parse(xyz[2])));
            }

        }
    }

}
