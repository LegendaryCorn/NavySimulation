using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitnessParameters
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
    public Dictionary<int, Dictionary<int, FitnessParameters>> fitnessParameters;

    public FitnessMgr(GameMgr mgr)
    {
        gameMgr = mgr;
    }

    // Needs to be called every time a ship is added/removed (fine for now)
    public void LoadParameters()
    {
        List<Entity381> entities = gameMgr.entityMgr.entities;
        fitnessParameters = new Dictionary<int, Dictionary<int, FitnessParameters>>();

        foreach (Entity381 ent1 in entities)
        {
            fitnessParameters[ent1.id] = new Dictionary<int, FitnessParameters>();
            foreach (Entity381 ent2 in entities)
            {
                if (ent1 == ent2) { continue; }

                Vector3 p = ent2.position - ent1.position;
                float b = Utils.Degrees360(Mathf.Atan2(p.x, p.z) * Mathf.Rad2Deg - ent1.heading);
                Vector3 v = ent2.velocity - ent1.velocity;
                float t = Mathf.Acos(Vector3.Dot(-p, v) / (p.magnitude * v.magnitude));

                // Doing this over and over again is very computationally heavy!
                fitnessParameters[ent1.id][ent2.id] = new FitnessParameters
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
            foreach(Entity381 ent2 in entities)
            {
                if (ent1 == ent2) { continue; }

                Vector3 p = ent2.position - ent1.position;
                float b = Utils.Degrees360(Mathf.Atan2(p.x, p.z) * Mathf.Rad2Deg - ent1.heading);
                Vector3 v = ent2.velocity - ent1.velocity;
                float t = Mathf.Acos(Vector3.Dot(-p, v) / (p.magnitude * v.magnitude));

                // Update the values
                FitnessParameters f = fitnessParameters[ent1.id][ent2.id];

                f.relPos = p;
                f.range = p.magnitude;
                f.bearing = b;

                f.relVel = v;
                f.relHeading = Utils.Degrees360(ent2.heading - ent1.heading);

                f.dcpa = p.magnitude * Mathf.Sin(t);
                f.tcpa = p.magnitude * Mathf.Cos(t) / v.magnitude;
            }
        }
    }

    float ParametersToFitness(float dt)
    {
        List<Entity381> entities = gameMgr.entityMgr.entities;
        float total = entities.Count;

        foreach(Entity381 ent1 in entities)
        {
            bool noNearbyShips = true;
            bool noShipsInFront = true;
            bool noCrash = true;

            foreach(Entity381 ent2 in entities)
            {
                // Nearby Ships
                if (true)
                {

                }

                // Ships In Front
                if (true)
                {

                }

                // Crashes
                if (true)
                {

                }
            }

            float totalSub = 0;
            if (!noNearbyShips) totalSub += 0.25f;
            if (!noShipsInFront) totalSub += 0.5f;
            if (!noCrash) totalSub = 1;

            total -= totalSub;
        }
        
        return total;
    }

    public void FinalFitness()
    {
        //totalFitness += 36000f;
    }
}
