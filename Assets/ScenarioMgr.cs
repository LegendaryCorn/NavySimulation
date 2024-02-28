using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ScenarioEntity
{
    public EntityType type;
    public Vector3 spawnPoint;
    public List<Vector3> wayPoints;
    public float heading;

    public List<Vector3> fitPoints;
    public float fitAxisHeading;
}


[System.Serializable]
public struct Scenario
{
    public EScenarioType scenarioType;
    public ScenarioEntity ownShipEntity;
    public ScenarioEntity targetShipEntity;
    public List<ScenarioEntity> trafficEntities;

    public float fitTimeMin;
    public float fitTimeMax;
}

// To be used by TargetShipGenerator
public enum EScenarioType
{
    HeadOn = 0,
    Overtaking,
    Crossing,
    Crossing90,
}


[System.Serializable]
public class ScenarioTypeData
{
    public EScenarioType scenarioType;
    public float minAngle;
    public float maxAngle;
    public float minDist, maxDist;
}
//

public class ScenarioMgr : MonoBehaviour
{
    public static ScenarioMgr inst;

    private void Awake()
    {
        inst = this;
    }

    [Header("Scenario Data")]
    public List<Entity381> entityData; // Needs to be set
    public List<ScenarioTypeData> scenarioTypeData;
    [Header("Scenario Variables")]
    public int seed;
    public int trafficCount;
    [Header("Keep These Empty")]
    public List<Scenario> scenarios; // Doesn't need to be set

    public Entity381 GetEntityData(EntityType e)
    {
        foreach (Entity381 ent in entityData)
        {
            if(ent.entityType == e)
            {
                return ent;
            }
        }
        return null;
    }

    public void GenerateScenarios(EScenarioType scenarioType, int scenarioCount)
    {
        Random.InitState(seed);

        for (int i = 0; i < scenarioCount; i++)
        {
            Scenario s = new Scenario();

            // Generate ownship
            ScenarioEntity ownShip = new ScenarioEntity();
            ownShip.spawnPoint = new Vector3(0, 0, -4000);
            ownShip.wayPoints = new List<Vector3>();
            ownShip.wayPoints.Add(new Vector3(0, 0, 8000));
            ownShip.fitPoints = new List<Vector3>();
            ownShip.type = EntityType.DDG51;
            s.ownShipEntity = ownShip;


            // Generate target ship
            ScenarioTypeData sd = scenarioTypeData.Find(x => x.scenarioType == scenarioType);
            ScenarioEntity targetShip = TargetShipGenerator.TargetShip(ownShip, sd);
            s.targetShipEntity = targetShip;

            // Generate traffic ships
            List<ScenarioEntity> traffic = TrafficShipGenerator.TrafficShips(trafficCount, ownShip, targetShip);
            s.trafficEntities = traffic;

            s.scenarioType = scenarioType;
            scenarios.Add(s);
        }
    }
}
