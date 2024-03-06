using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VelocityObstacle
{
    public Entity381 oEnt; // The other entity
    public float leftAng;
    public float rightAng;
    public bool giveWay = true;
}

[System.Serializable]
public class Move : Command
{
    public Vector3 movePosition;
    public Move(Entity381 ent, Vector3 pos) : base(ent)
    {
        movePosition = pos;
    }

    //public LineRenderer potentialLine;
    public override void Init()
    {
        //Debug.Log("MoveInit:\tMoving to: " + movePosition);
        //line = LineMgr.inst.CreateMoveLine(entity.position, movePosition);
        //line.gameObject.SetActive(false);
        //potentialLine = LineMgr.inst.CreatePotentialLine(entity.position);
        //line.gameObject.SetActive(true);
    }
    int aiTick = 0;
    int aiReset = 100;
    public override void Tick()
    {
        DHDS dhds;
        if (aiTick == 0)
        {
            if (entity.gameMgr.aiMgr.isPotentialFieldsMovement)
                dhds = ComputeVODHDS();
            else
                dhds = ComputeDHDS();
            entity.desiredHeading = dhds.dh;
            entity.desiredSpeed = dhds.ds;
            aiTick = aiReset;
            //line.SetPosition(1, movePosition);
        }
        aiTick--;
    }

    public Vector3 diff = Vector3.positiveInfinity;
    public float dhRadians;
    public float dhDegrees;
    public DHDS ComputeDHDS()
    {
        diff = movePosition - entity.position;
        dhRadians = Mathf.Atan2(diff.x, diff.z);
        dhDegrees = Utils.Degrees360(Mathf.Rad2Deg * dhRadians);
        return new DHDS(dhDegrees, entity.maxSpeed);

    }

    Entity381 prioEntity = null;

    public DHDS ComputeVODHDS()
    {
        diff = movePosition - entity.position;
        dhRadians = Mathf.Atan2(diff.x, diff.z);
        dhDegrees = Utils.Degrees360(Mathf.Rad2Deg * dhRadians);

        List<VelocityObstacle> obs = ComputeVOs();
        List<Entity381> riskEntities = WithinObstacles(entity.velocity, obs, true);

        float tcpaPrio = prioEntity != null ? Utils.tCPA(entity, prioEntity) : 0f;
        if(tcpaPrio < 0f)
        {
            prioEntity = null;
        }

        foreach(Entity381 ent in riskEntities)
        {
            float entTcpa = Utils.tCPA(entity, ent);
            if (entTcpa < tcpaLim && entTcpa >= 0f)
            {
                if (prioEntity == null || entTcpa < tcpaPrio)
                {
                    prioEntity = ent;
                    tcpaPrio = entTcpa;
                }
            }
        }

        if (prioEntity != null)
        {
            return ReplanDHDS(obs, prioEntity);
        }
        else
        {
            return new DHDS(dhDegrees, entity.maxSpeed);
        }
    }
    public float dh;
    public float angleDiff;
    public float cosValue;
    public float ds;
    public float entRad = 275f;
    public float tcpaLim = 200f;
    public float dcpaLim = 550f;

    public List<VelocityObstacle> ComputeVOs()
    {
        List<VelocityObstacle> obs = new List<VelocityObstacle>();

        foreach (Entity381 otherEntity in entity.gameMgr.entityMgr.entities)
        {
            if (entity == otherEntity) continue;

            Vector3 diffDist = otherEntity.position - entity.position;
            float dir = Mathf.Atan2(diffDist.x, diffDist.z);
            float ang = Mathf.Asin(2 * entRad / diffDist.magnitude);
            
            VelocityObstacle vo = new VelocityObstacle();
            vo.oEnt = otherEntity;
            vo.leftAng = dir - ang;
            vo.rightAng = dir + ang;
            vo.giveWay = GiveWay(entity, otherEntity);

            obs.Add(vo);
        }

        return obs;
    }
    
    // Returns true if ent1 is give way, and false if ent2 is give way.
    // Guarantees that at least one entity is trying to avoid the other.
    public bool GiveWay(Entity381 ent1, Entity381 ent2)
    {
        Vector3 diffDist = ent2.position - ent1.position;
        float dir1 = Mathf.Atan2(diffDist.x, diffDist.z);
        float relBearing1 = dir1 * Mathf.Rad2Deg - ent1.heading;

        float dir2 = Mathf.Atan2(-diffDist.x, -diffDist.z);
        float relBearing2 = dir2 * Mathf.Rad2Deg - ent2.heading;

        if(!Utils.AngleBetween(relBearing1, 112.5f, 350f))
        {
            return true;
        }
        return Utils.Degrees360(relBearing1 - 112.5f) > Utils.Degrees360(relBearing2 - 112.5f);
    }

    // Returns the list of entities that the ship will collide with
    public List<Entity381> WithinObstacles(Vector3 vel, List<VelocityObstacle> obs, bool forceGiveWay)
    {
        List<Entity381> ents = new List<Entity381>();

        foreach(VelocityObstacle ob in obs)
        {
            // Determine if the current velocity is inside a triangle
            float dist = Vector3.Distance(entity.position, ob.oEnt.position);
            Vector3 relVel = vel - ob.oEnt.velocity;
            float relAng = Mathf.Atan2(relVel.x, relVel.z);

            if (dist < 2 * entRad || Utils.AngleBetween(relAng * Mathf.Rad2Deg, ob.leftAng * Mathf.Rad2Deg, ob.rightAng * Mathf.Rad2Deg))
            {
                if (!forceGiveWay || ob.giveWay)
                {
                    ents.Add(ob.oEnt);
                }
            }
        }

        return ents;
    }

    public DHDS ReplanDHDS(List<VelocityObstacle> obs, Entity381 prioEnt)
    {
        float best_ang = entity.heading;
        float best_j = 0f;
        float best_dcpa = Mathf.Infinity;
        float best_tcpa = 0f;

        for(float i = 0; i < 120; i += 5) // Lower angles are better
        {
            for(float j = 1; j > 0; j -= 0.25f) // Higher speeds are better
            {
                float radAng = Utils.Degrees360(entity.heading + i) * Mathf.Deg2Rad;
                Vector3 newVel = entity.maxSpeed * j * new Vector3(Mathf.Sin(radAng), 0, Mathf.Cos(radAng));
                List<Entity381> obsEnts = WithinObstacles(newVel, obs, true);

                if (obsEnts.Count == 0 && j >= best_j)
                {
                    Vector3 p = prioEnt.position - entity.position;
                    Vector3 v = prioEnt.velocity - newVel;
                    float t = Mathf.Acos(Vector3.Dot(-p, v) / (p.magnitude * v.magnitude));
                    float dcpa = p.magnitude * Mathf.Sin(t);
                    float tcpa = p.magnitude * Mathf.Cos(t) / v.magnitude;

                    if (dcpa < best_dcpa && dcpa > dcpaLim && tcpa > 0)
                    {
                        best_ang = radAng * Mathf.Rad2Deg;
                        best_j = j;
                        best_dcpa = dcpa;
                        best_tcpa = tcpa;
                    }
                }
            }
        }

        return new DHDS(best_ang, entity.maxSpeed * best_j);
    }

    // Will leave this up for the visualization
    public List<Vector3> ComputePotentials(Vector3 pos)
    {
        List<Vector3> potentials = new List<Vector3>();
        PotentialParameters pf = entity.gameMgr.aiMgr.potentialParameters;

        potentials.Add(Vector3.zero);

        Vector3 attractiveDist = movePosition - pos;
        Vector3 attractivePotential = attractiveDist.normalized *
            pf.waypointCoefficient * Mathf.Pow(attractiveDist.magnitude, pf.waypointExponent);
        potentials[0] += attractivePotential;

        if(entity.role == EntityRole.Traffic || entity.role == EntityRole.None)
        {
            return potentials;
        }

        for (int i = 0; i < 5; i++)
        {
            potentials.Add(Vector3.zero); // i % 2 == 0 for positive potentials, i % 2 == 1 for negative potentials
        }

        foreach (Entity381 ent in entity.gameMgr.entityMgr.entities)
        {
            if (ent == entity) continue;
                    
            Vector3 posDiff = ent.position - pos;
            Vector3 posDiffCross = Vector3.Cross(posDiff, Vector3.down);
            Vector3 relVel = ent.velocity - entity.velocity;
            Vector3 relVelCross = Vector3.Cross(relVel, Vector3.up);

            Vector3 starboard = Vector3.Normalize(Vector3.Cross(entity.velocity, Vector3.down)); 

            float relBearing = Mathf.Atan2((ent.position - entity.position).x, (ent.position - entity.position).z) * Mathf.Rad2Deg - entity.heading;
            float targetAngle = Mathf.Atan2((entity.position - ent.position).x, (entity.position - ent.position).z) * Mathf.Rad2Deg - ent.heading;

            float bAngle = Mathf.Sin((relBearing + pf.bearingAngle) * Mathf.Deg2Rad);
            float tAngle = Mathf.Sin((targetAngle + pf.taAngle) * Mathf.Deg2Rad);

            Vector3 repField = Mathf.Pow(posDiff.magnitude, pf.repulsiveExponent) * pf.repulsiveCoefficient * -posDiff.normalized;
            Vector3 attField = Mathf.Pow(posDiff.magnitude, pf.attractiveExponent) * pf.attractiveCoefficient * posDiff.normalized;
            Vector3 crossPosField = Mathf.Pow(0.5f * (bAngle + 1), pf.bearingAngleExp) * Mathf.Pow(posDiff.magnitude, pf.bearingExponent) * pf.bearingCoefficient * starboard;
            Vector3 crossVelField = Mathf.Pow(0.5f * (tAngle + 1), pf.taAngleExp) * Mathf.Pow(posDiff.magnitude, pf.taExponent) * pf.taCoefficient * starboard;

            potentials[1] += repField;
            potentials[2] += attField;
            potentials[3] += crossPosField;
            potentials[4] += crossVelField;


            /*


            Vector3 basePotentialVal = bAngle * (Mathf.Pow(potDiff.magnitude, pf.baseExponent) * pf.baseCoefficient * potDiff.normalized + 
                                                    Mathf.Pow(r.magnitude, pf.bearingExponent) * pf.bearingCoefficient * r);

            Vector3 bearingPotentialVal = Mathf.Pow(potDiff.magnitude, pf.headingExponent) *
                pf.headingCoefficient * -(ent.position - pos);

            Vector3 headingPotentialVal = 0f * hAngle * Mathf.Pow(potDiff.magnitude, pf.headingExponent) *
                pf.headingCoefficient * -dir;

            if (pf.isAttractive)
                potentials[2] +=  1 * basePotentialVal;
            else
                potentials[1] += -1 * basePotentialVal;

            potentials[bAngle < 0 ? 4 : 3] += bearingPotentialVal;
            potentials[hAngle < 0 ? 6 : 5] += headingPotentialVal;
            */
        }

        return potentials;
    }



    public float doneDistanceSq = 1000;
    public override bool IsDone()
    {

        return ((entity.position - movePosition).sqrMagnitude < doneDistanceSq);
    }

    public override void Stop()
    {
        entity.desiredSpeed = 0;
        //LineMgr.inst.DestroyLR(line);
        //LineMgr.inst.DestroyLR(potentialLine);
    }
}
