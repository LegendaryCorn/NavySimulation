using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetShipGenerator
{
    public static ScenarioEntity TargetShip(ScenarioEntity ownShip, ScenarioTypeData sctData)
    {
        ScenarioEntity targetShip;

        switch (sctData.scenarioType)
        {
            case EScenarioType.HeadOn:
                targetShip = HeadOn(ownShip, sctData);
                break;
            case EScenarioType.Overtaking:
                targetShip = Overtaking(ownShip, sctData);
                break;
            case EScenarioType.Crossing:
            case EScenarioType.Crossing90:
                targetShip = Crossing(ownShip, sctData);
                break;
            default:
                targetShip = new ScenarioEntity();
                break;
        }

        return targetShip;
    }

    public static ScenarioEntity HeadOn(ScenarioEntity ownShip, ScenarioTypeData sctData)
    {
        ScenarioEntity tShip = new ScenarioEntity();
        tShip.type = (EntityType)Random.Range(0, 9); // Will need to be changed if more types are added
        Entity381 oShipData = ScenarioMgr.inst.GetEntityData(ownShip.type);
        Entity381 tShipData = ScenarioMgr.inst.GetEntityData(tShip.type);

        // Crossing Point
        float rDist = Random.Range(sctData.minDist, sctData.maxDist);
        float ownTravelDist = Vector3.Distance(ownShip.spawnPoint, ownShip.wayPoints[0]);
        Vector3 crossP = Vector3.Lerp(ownShip.spawnPoint, ownShip.wayPoints[0], rDist / ownTravelDist);
        float ownCrossPDist = Vector3.Distance(ownShip.spawnPoint, crossP); // d1

        // Spawn Point
        //Gamma = sin^-1(sin Alpha * d1/d2)
        // Beta = Alpha + Gamma
        float targetTravelDist = ownCrossPDist * tShipData.maxSpeed / oShipData.maxSpeed; // d2
        float rAngle = Random.Range(sctData.minAngle, sctData.maxAngle) * Mathf.Deg2Rad; // Alpha
        float gammaAngle = Mathf.Asin(Mathf.Sin(rAngle) * ownCrossPDist / targetTravelDist);
        float betaAngle = rAngle + gammaAngle + ownShip.heading * Mathf.Deg2Rad;
        Vector3 spawnP = crossP + targetTravelDist * new Vector3(Mathf.Sin(betaAngle), 0,  Mathf.Cos(betaAngle));

        // Heading
        Vector3 spawnDiff = crossP - spawnP;
        float targetHeading = Mathf.Atan2(spawnDiff.x, spawnDiff.z);

        // Waypoint
        Vector3 targetWayP = crossP + 4 * spawnDiff;

        // Finish
        tShip.spawnPoint = spawnP;
        tShip.heading = targetHeading * Mathf.Rad2Deg;
        tShip.wayPoints = new List<Vector3>();
        tShip.wayPoints.Add(targetWayP);
        tShip.fitPoints = new List<Vector3>();

        return tShip;
    }

    public static ScenarioEntity Crossing(ScenarioEntity ownShip, ScenarioTypeData sctData)
    {
        ScenarioEntity tShip = new ScenarioEntity();
        tShip.type = (EntityType)Random.Range(0, 9); // Will need to be changed if more types are added
        Entity381 oShipData = ScenarioMgr.inst.GetEntityData(ownShip.type);
        Entity381 tShipData = ScenarioMgr.inst.GetEntityData(tShip.type);

        // Crossing Point
        float rDist = Random.Range(sctData.minDist, sctData.maxDist);
        float ownTravelDist = Vector3.Distance(ownShip.spawnPoint, ownShip.wayPoints[0]);
        Vector3 crossP = Vector3.Lerp(ownShip.spawnPoint, ownShip.wayPoints[0], rDist / ownTravelDist);
        float ownCrossPDist = Vector3.Distance(ownShip.spawnPoint, crossP); // d1

        float targetTravelDist = ownCrossPDist * tShipData.maxSpeed / oShipData.maxSpeed; // d2
        float rAngle = Random.Range(sctData.minAngle, sctData.maxAngle) * Mathf.Deg2Rad; // Alpha
        float rrAngle = rAngle + ownShip.heading + Mathf.Deg2Rad;
        Vector3 spawnP = crossP + targetTravelDist * new Vector3(Mathf.Sin(rrAngle), 0, Mathf.Cos(rrAngle));

        // Heading
        Vector3 spawnDiff = crossP - spawnP;
        float targetHeading = Mathf.Atan2(spawnDiff.x, spawnDiff.z);

        // Waypoint
        Vector3 targetWayP = crossP + 100 * spawnDiff;

        // Finish
        tShip.spawnPoint = spawnP;
        tShip.heading = targetHeading * Mathf.Rad2Deg;
        tShip.wayPoints = new List<Vector3>();
        tShip.wayPoints.Add(targetWayP);
        tShip.fitPoints = new List<Vector3>();

        return tShip;
    }

    public static ScenarioEntity Overtaking(ScenarioEntity ownShip, ScenarioTypeData sctData)
    {
        ScenarioEntity tShip = new ScenarioEntity();
        tShip.type = (EntityType)Random.Range(0, 9); // Will need to be changed if more types are added
        Entity381 oShipData = ScenarioMgr.inst.GetEntityData(ownShip.type);
        Entity381 tShipData = ScenarioMgr.inst.GetEntityData(tShip.type);

        while(tShipData.maxSpeed >= oShipData.maxSpeed * 0.6f) // Guarantee a slow ship
        {
            tShip.type = (EntityType)Random.Range(0, 9); // Will need to be changed if more types are added
            tShipData = ScenarioMgr.inst.GetEntityData(tShip.type);
        }

        // Crossing Point
        float rDist = Random.Range(sctData.minDist, sctData.maxDist);
        float ownTravelDist = Vector3.Distance(ownShip.spawnPoint, ownShip.wayPoints[0]);
        Vector3 crossP = Vector3.Lerp(ownShip.spawnPoint, ownShip.wayPoints[0], rDist / ownTravelDist);
        float ownCrossPDist = Vector3.Distance(ownShip.spawnPoint, crossP); // d1

        float targetTravelDist = ownCrossPDist * tShipData.maxSpeed / oShipData.maxSpeed; // d2
        float rAngle = Random.Range(sctData.minAngle, sctData.maxAngle) * Mathf.Deg2Rad; // Alpha
        float rrAngle = rAngle + ownShip.heading + Mathf.Deg2Rad;
        Vector3 spawnP = crossP + targetTravelDist * new Vector3(Mathf.Sin(rrAngle), 0, Mathf.Cos(rrAngle));

        // Heading
        Vector3 spawnDiff = crossP - spawnP;
        float targetHeading = Mathf.Atan2(spawnDiff.x, spawnDiff.z);

        // Waypoint
        Vector3 targetWayP = crossP + spawnDiff;

        // Finish
        tShip.spawnPoint = spawnP;
        tShip.heading = targetHeading * Mathf.Rad2Deg;
        tShip.wayPoints = new List<Vector3>();
        tShip.wayPoints.Add(targetWayP);
        tShip.fitPoints = new List<Vector3>();

        return tShip;
    }
}
