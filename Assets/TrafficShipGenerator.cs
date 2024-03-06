using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficShipGenerator
{
    public static List<ScenarioEntity> TrafficShips(int trafficCount, ScenarioEntity ownShip, ScenarioEntity targetShip)
    {
        float laneWidth = 1000f;
        float laneBorder = 20f;
        int numLanesPerRoad = 3;
        int middleLane = 1;

        float laneRange = 8000f; // How close or far away from ownship that the vessels can spawn vertically
        float randAngDeg = 10f; // The directions the ships can face
        float shipDist = 1000f; // Don't spawn on top of other vessels

        List<ScenarioEntity> scEntities = new List<ScenarioEntity>();

        for(int i = 0; i < trafficCount; i++)
        {
            // Decide if forwards or backwards
            bool trBackwards = Random.Range(0f, 1f) > 0.5f;

            // Decide lane (ownship is centered at the middle lane, forwards)
            int trLane = Random.Range(0, numLanesPerRoad);
            trLane -= trBackwards ? numLanesPerRoad : 0;
            float locationLane = Random.Range(laneBorder, laneWidth - laneBorder);

            // Place ship (if not too close)
            float relXPos = (trLane - middleLane) * laneWidth - (laneWidth / 2) + locationLane;
            float relZPos = 0;
            int counter = 15;
            Vector3 spawn = Vector3.zero;
            float oShipHeading = ownShip.heading * Mathf.Deg2Rad;
            for(int c = counter; c > 0; c--) // Close check
            {
                relZPos = Random.Range(-laneRange, laneRange);
                bool tooClose = false;
                spawn = new Vector3(relXPos * Mathf.Cos(oShipHeading) - relZPos * Mathf.Sin(oShipHeading), 0, relXPos * Mathf.Sin(oShipHeading) + relZPos * Mathf.Cos(oShipHeading));
                
                if(Vector3.Distance(spawn, ownShip.spawnPoint) < shipDist || Vector3.Distance(spawn, targetShip.spawnPoint) < shipDist)
                {
                    tooClose = true;
                }

                foreach(ScenarioEntity sc in scEntities)
                {
                    if (Vector3.Distance(spawn, sc.spawnPoint) < shipDist)
                    {
                        tooClose = true;
                    }
                }

                if (!tooClose)
                {
                    c = 0;
                }
            }

            // Decide waypoints (20000 units away)
            float trHeading = ownShip.heading + Random.Range(-randAngDeg, randAngDeg);
            trHeading += trBackwards ? 180f : 0f;
            Vector3 wp = spawn + 40000f * new Vector3(Mathf.Sin(trHeading * Mathf.Deg2Rad), 0, Mathf.Cos(trHeading * Mathf.Deg2Rad));

            // Store
            ScenarioEntity trShip = new ScenarioEntity();
            trShip.type = (EntityType)Random.Range(0, 9); // Will need to be changed if more types are added
            trShip.spawnPoint = spawn;
            trShip.heading = trHeading;
            trShip.wayPoints = new List<Vector3>();
            trShip.wayPoints.Add(wp);
            trShip.fitPoints = new List<Vector3>();
            scEntities.Add(trShip);
        }

        return scEntities;
    }
}
