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


[System.Serializable]
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
    public int id;

    public GameObject cameraRig;
    public GameObject selectionCircle;

    public GameMgr gameMgr;

    public UnitAI ai;
    public OrientedPhysics physics;

    public Entity381(GameMgr mgr, Entity381 entData, Vector3 position, float heading, int id)
    {
        gameMgr = mgr;
        this.position = position;
        this.heading = heading;
        this.desiredHeading = heading;

        this.acceleration = entData.acceleration;
        this.turnRate = entData.turnRate;
        this.maxSpeed = entData.maxSpeed;
        this.minSpeed = entData.minSpeed;
        this.mass = entData.mass;

        this.entityType = entData.entityType;
        this.id = id;

        ai = new UnitAI(this);
        physics = new OrientedPhysics(this);
    }
}
