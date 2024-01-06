using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselFitness
{
    public Entity381 entity;
    private float fitAxis;
    private float cosAxis, sinAxis; // Makes calculations significantly faster by not having to calculate the cos/sin at every frame
    private List<Vector3> worldPoints;
    private List<Vector2> fitAxisPoints;
    // X = point on axis
    // Y = Distance from axis

    public List<float> dists; // To be used by an evaluator
    private int currIndex;

    private Vector2 prevPoint; // Previous point
    private Vector2 currPoint; // Current point


    public VesselFitness(Entity381 ent)
    {
        entity = ent;
    }

    public void SetAxisPoints(float axis, List<Vector3> wayPoints)
    {
        fitAxis = axis * Mathf.Deg2Rad;
        cosAxis = Mathf.Cos(fitAxis);
        sinAxis = Mathf.Sin(fitAxis);
        worldPoints = new List<Vector3>();
        fitAxisPoints = new List<Vector2>();
        dists = new List<float>();

        prevPoint = WorldPointToAxisPoint(entity.position);
        currPoint = WorldPointToAxisPoint(entity.position);

        foreach (Vector3 wP in wayPoints)
        {
            worldPoints.Add(wP);
            fitAxisPoints.Add(WorldPointToAxisPoint(wP));
            dists.Add(Mathf.Infinity);
        }

        currIndex = 0;
    }

    public void OnUpdate(float dt)
    {
        for(int i = 0; i < worldPoints.Count; i++)
        {
            dists[i] = Mathf.Min(dists[i], Vector3.SqrMagnitude(worldPoints[i] - entity.position));
        }
        /*
        currPoint = WorldPointToAxisPoint(entity.position);

        // If the index is equal to this, then there's no more fit points
        if (currIndex != fitAxisPoints.Count)
        {
            
            bool a = currPoint.x > prevPoint.x && (fitAxisPoints[currIndex].x <= currPoint.x && fitAxisPoints[currIndex].x >= prevPoint.x);
            bool b = currPoint.x < prevPoint.x && (fitAxisPoints[currIndex].x >= currPoint.x && fitAxisPoints[currIndex].x <= prevPoint.x);

            if (a || b) // Basically checks if the fitaxispoint is within the current point and the previous point
            {
                float approxY = (currPoint.y + prevPoint.y) / 2;
                dists[currIndex] = Mathf.Abs(approxY - fitAxisPoints[currIndex].y);
                //Debug.Log(dists[currIndex].ToString() + " - Vessel " + entity.id.ToString());
                currIndex++;
            }
        }

        prevPoint = WorldPointToAxisPoint(entity.position);
        */
    }

    public Vector2 WorldPointToAxisPoint(Vector3 worldPoint)
    {
        return new Vector2(worldPoint.z * cosAxis + worldPoint.x * sinAxis, worldPoint.x * cosAxis - worldPoint.z * sinAxis);
    }
}
