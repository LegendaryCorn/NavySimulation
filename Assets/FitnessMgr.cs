using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShipFitnessParameters
{
    public bool reachedTarget;
    public float timeToTarget;
    public float minDesHeadingWP;
    public float maxDesHeadingWP;

    public List<Vector3> positions;
    /*
    public float desSpeedDiff;
    public float prevDesSpeedDiff;

    public float desHeadingDiff;
    public float prevDesHeadingDiff;
    */
}

public class TwoShipFitnessParameters
{
    public float closestDistSkw;
    public float closestDistPar;
    /*
    public Vector3 relPos;
    public float range;
    public float bearing;

    public Vector3 relVel;
    public float relHeading;

    public float dcpa;
    public float tcpa;
    */
}

public class FitnessMgr
{
    public GameMgr gameMgr;
    public Dictionary<int, OneShipFitnessParameters> oneShipFitnessParameters;
    public Dictionary<int, Dictionary<int, TwoShipFitnessParameters>> twoShipFitnessParameters;

    // Vars
    public float countHeadingManeuver = 0;
    public float countSpeedManeuver = 0;
    public float countNearbyShips = 0;
    public float countShipInFront = 0;
    public float countCrash = 0;

    public bool recordLocations = false;

    public float timeMin;
    public float timeMax;

    public FitnessMgr(GameMgr mgr)
    {
        gameMgr = mgr;
    }

    // Needs to be called every time a ship is added/removed (fine for now)
    public void LoadParameters()
    {

        List<Entity381> entities = gameMgr.entityMgr.entities;
        oneShipFitnessParameters = new Dictionary<int, OneShipFitnessParameters>();
        twoShipFitnessParameters = new Dictionary<int, Dictionary<int, TwoShipFitnessParameters>>();

        foreach (Entity381 ent1 in entities)
        {

            oneShipFitnessParameters[ent1.id] = new OneShipFitnessParameters
            {
                reachedTarget = false,
                timeToTarget = 0f,
                minDesHeadingWP = 0f,
                maxDesHeadingWP = 0f,

                positions = new List<Vector3>(),
                /*
                desSpeedDiff = 0,
                prevDesSpeedDiff = 0,
                desHeadingDiff = 0,
                prevDesHeadingDiff = 0
                */
            };

            twoShipFitnessParameters[ent1.id] = new Dictionary<int, TwoShipFitnessParameters>();
            foreach (Entity381 ent2 in entities)
            {
                if (ent1 == ent2) { continue; }

                Vector3 p = ent2.position - ent1.position;
                //float b = Utils.Degrees360(Mathf.Atan2(p.x, p.z) * Mathf.Rad2Deg - ent1.heading);
                //Vector3 v = ent2.velocity - ent1.velocity;
                //float t = Mathf.Acos(Vector3.Dot(-p, v) / (p.magnitude * v.magnitude));

                // Doing this over and over again is very computationally heavy!
                twoShipFitnessParameters[ent1.id][ent2.id] = new TwoShipFitnessParameters
                {
                    closestDistSkw = Mathf.Infinity,
                    closestDistPar = Mathf.Infinity
                    /*
                    relPos = p,
                    range = p.magnitude,
                    bearing = b,

                    relVel = v,
                    relHeading = Utils.Degrees360(ent2.heading - ent1.heading),

                    dcpa = p.magnitude * Mathf.Sin(t),
                    tcpa = p.magnitude * Mathf.Cos(t) / v.magnitude,
                    */

                };
            }
        }
    }

    public void OnUpdate(float dt)
    {
        UpdateParameters(dt);
        //UpdateFitnessInfo();
    }

    void UpdateParameters(float dt)
    {
        List<Entity381> entities = gameMgr.entityMgr.entities;

        foreach(Entity381 ent1 in entities)
        {

            OneShipFitnessParameters f1 = oneShipFitnessParameters[ent1.id];

            if (recordLocations)
            {
                f1.positions.Add(ent1.position);
            }

            f1.reachedTarget = ent1.ai.commands.Count == 0;
            if (!f1.reachedTarget) { 
                f1.timeToTarget += dt;

                Vector3 wayDiff = ((Move)(ent1.ai.commands[0])).movePosition - ent1.position;
                float wayAng = Utils.AngleDiffPosNeg(ent1.desiredHeading, Mathf.Atan2(wayDiff.x, wayDiff.z) * Mathf.Rad2Deg);
                f1.minDesHeadingWP = Mathf.Min(f1.minDesHeadingWP, wayAng);
                f1.maxDesHeadingWP = Mathf.Max(f1.maxDesHeadingWP, wayAng);
            }
            /*
            f1.prevDesHeadingDiff = f1.desHeadingDiff;
            f1.prevDesSpeedDiff = f1.desSpeedDiff;

            f1.desHeadingDiff = Utils.AngleDiffPosNeg(ent1.desiredHeading, ent1.heading);
            f1.desSpeedDiff = ent1.desiredSpeed - ent1.speed;
            */

            foreach(Entity381 ent2 in entities)
            {
                if (ent1 == ent2) { continue; }

                Vector3 p = ent2.position - ent1.position;
                //float b = Utils.Degrees360(Mathf.Atan2(p.x, p.z) * Mathf.Rad2Deg - ent1.heading);
                //Vector3 v = ent2.velocity - ent1.velocity;
                //float t = Mathf.Acos(Vector3.Dot(-p, v) / (p.magnitude * v.magnitude));

                // Update the values
                TwoShipFitnessParameters f2 = twoShipFitnessParameters[ent1.id][ent2.id];

                if(Mathf.Abs(Utils.AngleDiffPosNeg(ent1.heading, ent2.heading)) < 10f)
                {
                    f2.closestDistPar = Mathf.Min(f2.closestDistPar, p.magnitude);
                }
                else
                {
                    f2.closestDistSkw = Mathf.Min(f2.closestDistSkw, p.magnitude);
                }
                

                
                //f2.relPos = p;
                //f2.range = p.magnitude;
                //f2.bearing = b;

                //f2.relVel = v;
                //f2.relHeading = Utils.Degrees360(ent2.heading - ent1.heading);

                //f2.dcpa = p.magnitude * Mathf.Sin(t);
                //f2.tcpa = p.magnitude * Mathf.Cos(t) / v.magnitude;
                
            }
        }
    }

    void UpdateFitnessInfo()
    {
        /*
        List<Entity381> entities = gameMgr.entityMgr.entities;

        foreach(Entity381 ent1 in entities)
        {

            OneShipFitnessParameters f1 = oneShipFitnessParameters[ent1.id];

            // Heading Maneuver
            if(f1.desHeadingDiff * f1.prevDesHeadingDiff < 0)
            {
                countHeadingManeuver += 1;
            }

            // Speed Maneuver
            if (f1.desSpeedDiff * f1.prevDesSpeedDiff < 0)
            {
                countSpeedManeuver += 1;
            }



            foreach (Entity381 ent2 in entities)
            {
                if (ent1 == ent2) { continue; }

                TwoShipFitnessParameters f2 = twoShipFitnessParameters[ent1.id][ent2.id];

                // Nearby Ships
                if (f2.range < 600 && f2.relHeading > 10 && f2.relHeading < 350)
                {
                    countNearbyShips += 1;
                }

                // Ships In Front
                if (f2.range < 1200 && f2.bearing > 20 && f2.bearing < 340 && f2.relHeading > 10 && f2.relHeading < 350)
                {
                    countShipInFront += 1;
                }

                // Crashes
                if (f2.range < 150)
                {
                    countCrash += 1;
                }
            }
        }
        */
    }
}
