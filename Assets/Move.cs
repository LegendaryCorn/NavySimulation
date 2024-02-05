using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public override void Tick()
    {
        DHDS dhds;
        if (entity.gameMgr.aiMgr.isPotentialFieldsMovement)
            dhds = ComputePotentialDHDS();
        else
            dhds = ComputeDHDS();

        entity.desiredHeading = dhds.dh;
        entity.desiredSpeed = dhds.ds;
        //line.SetPosition(1, movePosition);
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

    public DHDS ComputePotentialDHDS()
    {
        List<Vector3> potentials = ComputePotentials(entity.position);

        Vector3 potentialSum = Vector3.zero;

        foreach(Vector3 pot in potentials)
        {
            potentialSum += pot;
        }

        dh = Utils.Degrees360(Mathf.Rad2Deg * Mathf.Atan2(potentialSum.x, potentialSum.z));

        angleDiff = Utils.Degrees360(Utils.AngleDiffPosNeg(dh, entity.heading));
        cosValue = (Mathf.Cos(angleDiff * Mathf.Deg2Rad) + 1) / 2.0f; // makes it between 0 and 1
        ds = entity.maxSpeed * cosValue;

        return new DHDS(dh, ds);
    }
    public float dh;
    public float angleDiff;
    public float cosValue;
    public float ds;

    public List<Vector3> ComputePotentials(Vector3 pos)
    {
        List<Vector3> potentials = new List<Vector3>();
        PotentialParameters pf = entity.gameMgr.aiMgr.potentialParameters;

        potentials.Add(Vector3.zero);
        for (int i = 0; i < 7; i++)
        {
            potentials.Add(Vector3.zero); // i % 2 == 0 for positive potentials, i % 2 == 1 for negative potentials
        }

        Vector3 attractiveDist = movePosition - pos;
        Vector3 attractivePotential = attractiveDist.normalized *
            pf.waypointCoefficient * Mathf.Pow(attractiveDist.magnitude, pf.waypointExponent);
        potentials[0] += attractivePotential;

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

            float attBAngle = Mathf.Sin((relBearing + pf.attractiveBearingAngle) * Mathf.Deg2Rad);
            float attTAngle = Mathf.Sin((targetAngle + pf.attractiveTAAngle) * Mathf.Deg2Rad);

            float repBAngle = Mathf.Sin((relBearing + pf.repulsiveBearingAngle) * Mathf.Deg2Rad);
            float repTAngle = Mathf.Sin((targetAngle + pf.repulsiveTAAngle) * Mathf.Deg2Rad);

            Vector3 repField = Mathf.Pow(posDiff.magnitude, pf.repulsiveExponent) * pf.repulsiveCoefficient * -posDiff.normalized;
            Vector3 attField = Mathf.Pow(posDiff.magnitude, pf.attractiveExponent) * pf.attractiveCoefficient * posDiff.normalized;
            Vector3 attCrossPosField = Mathf.Pow(0.5f * (attBAngle + 1), pf.attractiveBearingAngleExp) * Mathf.Pow(posDiff.magnitude, pf.attractiveBearingExponent) * pf.attractiveBearingCoefficient * starboard;
            Vector3 attCrossVelField = Mathf.Pow(0.5f * (attTAngle + 1), pf.attractiveTAAngleExp) * Mathf.Pow(posDiff.magnitude, pf.attractiveTAExponent) * pf.attractiveTACoefficient * starboard;
            Vector3 repCrossPosField = Mathf.Pow(0.5f * (repBAngle + 1), pf.repulsiveBearingAngleExp) * Mathf.Pow(posDiff.magnitude, pf.repulsiveBearingExponent) * pf.repulsiveBearingCoefficient * -starboard;
            Vector3 repCrossVelField = Mathf.Pow(0.5f * (repTAngle + 1), pf.repulsiveTAAngleExp) * Mathf.Pow(posDiff.magnitude, pf.repulsiveTAExponent) * pf.repulsiveTACoefficient * -starboard;


            potentials[1] += repField;
            potentials[2] += attField;
            potentials[3] += attCrossPosField;
            potentials[4] += attCrossVelField;
            potentials[5] += repCrossPosField;
            potentials[6] += repCrossVelField;

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
