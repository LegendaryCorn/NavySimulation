using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatedEntity : MonoBehaviour
{
    public EntityType type;
    public int id;
    public float speed;
    public float desiredSpeed;
    public float heading;
    public float desiredHeading;

    public bool isSelected;
    public GameObject selectionCircle;
    public LineRenderer waypointLine;

    public Entity381 ent = null;

    private void Awake()
    {
        waypointLine = Instantiate<LineRenderer>(SimulationMgr.inst.waypointLinePrefab);
        waypointLine.transform.parent = transform;
        waypointLine.enabled = false;
    }

    private void Update()
    {
        transform.position = ent.position;
        transform.eulerAngles = new Vector3(0, ent.heading, 0);

        speed = ent.speed;
        desiredSpeed = ent.desiredSpeed;
        heading = ent.heading;
        desiredHeading = ent.desiredHeading;

        waypointLine.transform.position = Vector3.zero;
        waypointLine.transform.rotation = Quaternion.identity;
        waypointLine.positionCount = ent.ai.moves.Count + 1;
        waypointLine.SetPosition(0, ent.position);
        for(int i = 0; i < ent.ai.commands.Count; i++)
        {
            Move m = (Move)ent.ai.commands[i];
            waypointLine.SetPosition(i+1, m.movePosition);
        }
    }

    public void Select(bool selected)
    {
        isSelected = selected;
        selectionCircle.SetActive(selected);
        waypointLine.enabled = selected;
    }
}
