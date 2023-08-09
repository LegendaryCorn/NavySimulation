using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum EntityType
{
    DDG51,
    Container,
    MineSweeper,
    OilServiceVessel,
    OrientExplorer,
    PilotVessel,
    SmitHouston,
    Tanker,
    TugBoat
}


public class Entity381
{
    //------------------------------
    // values that change while running
    //------------------------------
    public bool isSelected = false;
    public bool isRealtime = false;
    public Vector3 position = Vector3.zero;
    public Vector3 velocity = Vector3.zero;

    public float speed;
    public float desiredSpeed;
    public float heading; //degrees
    public float desiredHeading; //degrees

    //------------------------------
    // values that do not change
    //------------------------------
    public float acceleration;
    public float turnRate;
    public float maxSpeed;
    public float minSpeed;
    public float mass;

    public EntityType entityType;

    public GameObject cameraRig;
    public GameObject selectionCircle;

    public GameMgr gameMgr;

    public UnitAI ai;
    public OrientedPhysics physics;

    public Entity381(GameMgr mgr, Vector3 position, Vector3 eulerAngles)
    {
        gameMgr = mgr;
        this.position = position;
        this.heading = eulerAngles.y;

        ai = new UnitAI(this);
        physics = new OrientedPhysics(this);
    }
}
