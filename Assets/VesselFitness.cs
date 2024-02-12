using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselFitness
{
    public Entity381 entity;
    private Vector3 fitPoint;
    private Entity381 targetEntity; // If null, the fitpoint will be placed on the world. Otherwise, it will be placed relative to the target entity.
    // X = point on axis
    // Y = Distance from axis

    public float dist; // To be used by an evaluator
    private int currIndex;

    private Vector2 prevPoint; // Previous point
    private Vector2 currPoint; // Current point


    public VesselFitness(Entity381 ent)
    {
        entity = ent;
    }

    public void SetAxisPoints(float axis, List<Vector3> wayPoints)
    {

        dist = Mathf.Infinity;

        currIndex = 0;
    }

    public void OnUpdate(float dt)
    {
        Vector3 fp = fitPoint;
        if(targetEntity != null)
        {
            float h = targetEntity.heading * Mathf.Deg2Rad;
            fp = new Vector3(fitPoint.x * Mathf.Cos(h) - fitPoint.z * Mathf.Sin(h), 0, fitPoint.x * Mathf.Sin(h) + fitPoint.z * Mathf.Cos(h)) + targetEntity.position;
        }
        dist = Mathf.Min(dist, Vector3.SqrMagnitude(fp - entity.position));
    }
}
