﻿using System.Collections;
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
        PotentialParameters potParams = entity.gameMgr.aiMgr.potentialParameters;

        Vector3 attractiveDist = movePosition - pos;
        Vector3 attractivePotential = attractiveDist.normalized *
            potParams.waypointPotential.coefficient * Mathf.Pow(attractiveDist.magnitude, potParams.waypointPotential.exponent);
        potentials.Add(attractivePotential);

        foreach (Entity381 ent in entity.gameMgr.entityMgr.entities)
        {
            if (ent == entity) continue;

            if ((ent.position - entity.position).magnitude < entity.gameMgr.aiMgr.potentialParameters.potentialDistanceThreshold)
            {

                float h = ent.heading * Mathf.Deg2Rad;
                float sinHeading = Mathf.Sin(h);
                float cosHeading = Mathf.Cos(h);

                foreach (PF pf in potParams.shipPotentials)
                {
                    //bool c1 = pf.minAngle > pf.maxAngle && (p.targetRelHeading >= pf.minAngle || p.targetRelHeading <= pf.maxAngle);
                    //bool c2 = pf.minAngle < pf.maxAngle && (p.targetRelHeading >= pf.minAngle && p.targetRelHeading <= pf.maxAngle);
                    //if (c1 || c2 || true)
                    //{
                    Vector3 potPos = ent.position + new Vector3(pf.verticalOffset * sinHeading + pf.horizontalOffset * cosHeading, 0,
                                                                pf.verticalOffset * cosHeading - pf.horizontalOffset * sinHeading);
                    Vector3 potDiff = potPos - pos;

                    Vector3 potentialVal = Mathf.Pow(potDiff.magnitude, pf.exponent) * entity.mass *
                        pf.coefficient * potDiff.normalized;

                    if (pf.isAttractive)
                        potentials.Add(1 * potentialVal);
                    else
                        potentials.Add(-1 * potentialVal);
                    //}
                }
            }
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
