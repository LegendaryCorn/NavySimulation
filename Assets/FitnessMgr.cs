using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShipFitnessParameters
{
    public float desSpeedDiff;
    public float prevDesSpeedDiff;

    public float desHeadingDiff;
    public float prevDesHeadingDiff;
}

public class TwoShipFitnessParameters
{
    public Vector3 relPos;
    public float range;
    public float bearing;

    public Vector3 relVel;
    public float relHeading;

    public float dcpa;
    public float tcpa;
}

public class FitnessMgr
{
    public GameMgr gameMgr;
    public float totalFitness = 0f;
    public Dictionary<int, OneShipFitnessParameters> oneShipFitnessParameters;
    public Dictionary<int, Dictionary<int, TwoShipFitnessParameters>> twoShipFitnessParameters;

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
                desSpeedDiff = 0,
                prevDesSpeedDiff = 0,
                desHeadingDiff = 0,
                prevDesHeadingDiff = 0
            };

            twoShipFitnessParameters[ent1.id] = new Dictionary<int, TwoShipFitnessParameters>();
            foreach (Entity381 ent2 in entities)
            {
                if (ent1 == ent2) { continue; }

                Vector3 p = ent2.position - ent1.position;
                float b = Utils.Degrees360(Mathf.Atan2(p.x, p.z) * Mathf.Rad2Deg - ent1.heading);
                Vector3 v = ent2.velocity - ent1.velocity;
                float t = Mathf.Acos(Vector3.Dot(-p, v) / (p.magnitude * v.magnitude));

                // Doing this over and over again is very computationally heavy!
                twoShipFitnessParameters[ent1.id][ent2.id] = new TwoShipFitnessParameters
                {
                    relPos = p,
                    range = p.magnitude,
                    bearing = b,

                    relVel = v,
                    relHeading = Utils.Degrees360(ent2.heading - ent1.heading),

                    dcpa = p.magnitude * Mathf.Sin(t),
                    tcpa = p.magnitude * Mathf.Cos(t) / v.magnitude,

                };
            }
        }
    }

    public void OnUpdate(float dt)
    {
        UpdateParameters();
        totalFitness += ParametersToFitness(dt);
    }

    void UpdateParameters()
    {
        List<Entity381> entities = gameMgr.entityMgr.entities;

        foreach(Entity381 ent1 in entities)
        {

            OneShipFitnessParameters f1 = oneShipFitnessParameters[ent1.id];

            f1.prevDesHeadingDiff = f1.desHeadingDiff;
            f1.prevDesSpeedDiff = f1.desSpeedDiff;

            f1.desHeadingDiff = Utils.AngleDiffPosNeg(ent1.desiredHeading, ent1.heading);
            f1.desSpeedDiff = ent1.desiredSpeed - ent1.speed;

            foreach(Entity381 ent2 in entities)
            {
                if (ent1 == ent2) { continue; }

                Vector3 p = ent2.position - ent1.position;
                float b = Utils.Degrees360(Mathf.Atan2(p.x, p.z) * Mathf.Rad2Deg - ent1.heading);
                Vector3 v = ent2.velocity - ent1.velocity;
                float t = Mathf.Acos(Vector3.Dot(-p, v) / (p.magnitude * v.magnitude));

                // Update the values
                TwoShipFitnessParameters f2 = twoShipFitnessParameters[ent1.id][ent2.id];

                f2.relPos = p;
                f2.range = p.magnitude;
                f2.bearing = b;

                f2.relVel = v;
                f2.relHeading = Utils.Degrees360(ent2.heading - ent1.heading);

                f2.dcpa = p.magnitude * Mathf.Sin(t);
                f2.tcpa = p.magnitude * Mathf.Cos(t) / v.magnitude;
            }
        }
    }

    float ParametersToFitness(float dt)
    {
        List<Entity381> entities = gameMgr.entityMgr.entities;
        float total = 0;

        foreach(Entity381 ent1 in entities)
        {
            bool noTurningPort = true;
            bool noHeadingManeuver = true;
            bool noSpeedManeuver = true;

            bool noNearbyShips = true;
            bool noShipsInFront = true;
            bool noCrash = true;

            OneShipFitnessParameters f1 = oneShipFitnessParameters[ent1.id];

            // Turning Port
            if(f1.desHeadingDiff < -0.1)
            {
                noTurningPort = false;
            }

            // Heading Maneuver
            if(f1.desHeadingDiff * f1.prevDesHeadingDiff < 0)
            {
                noHeadingManeuver = false;
            }

            // Speed Maneuver
            if (f1.desSpeedDiff * f1.prevDesSpeedDiff < 0)
            {
                noSpeedManeuver = false;
            }



            foreach (Entity381 ent2 in entities)
            {
                if (ent1 == ent2) { continue; }

                TwoShipFitnessParameters f2 = twoShipFitnessParameters[ent1.id][ent2.id];

                // Nearby Ships
                if (f2.range < 600 && f2.relHeading > 10 && f2.relHeading < 350)
                {
                    noNearbyShips = false;
                }

                // Ships In Front
                if (f2.range < 1200 && f2.bearing > 20 && f2.bearing < 340 && f2.relHeading > 10 && f2.relHeading < 350)
                {
                    noShipsInFront = false;
                }

                // Crashes
                if (f2.range < 150)
                {
                    noCrash = false;
                }
            }

            float totalSub = 0;
            //if (!noTurningPort) totalSub += 2f;
            //if (!noHeadingManeuver) totalSub += 1f;
            //if (!noSpeedManeuver) totalSub += 1f;
            //if (!noNearbyShips) totalSub += 50f;
            //if (!noShipsInFront) totalSub += 50f;
            if (!noCrash) totalSub += 100f;

            total -= totalSub;
        }
        
        return total;
    }

    public void FinalFitness()
    {
        float f = 0;
        foreach (Entity381 ent in gameMgr.entityMgr.entities)
        {
            if (ent.ai.commands.Count == 0)
            {
                f += 1f;
            }
            else
            {
                Move finalMove = (Move)ent.ai.commands[ent.ai.commands.Count - 1];
                float dist = Vector3.Distance(ent.position, finalMove.movePosition);
                f += 1f * (1f - dist / 3000f);
            }
        }
        totalFitness += f;
    }
}
