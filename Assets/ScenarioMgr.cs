using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct WayPoint
{
    public Vector3 center;
}

[System.Serializable]
public struct ScenarioEntity
{
    public EntityType type;
    public WayPoint spawnPoint;
    public List<WayPoint> wayPoints;
    public float heading;

    public List<WayPoint> fitPoints;
    public float fitAxisHeading;
}

[System.Serializable]
public struct ScenarioBoundary
{
    public List<Vector3> points;
}

[System.Serializable]
public struct Scenario
{
    public List<ScenarioEntity> scenarioEntities;
    public List<ScenarioBoundary> scenarioBoundaries;

    public float fitTimeMin;
    public float fitTimeMax;
}

public class ScenarioMgr : MonoBehaviour
{
    public static ScenarioMgr inst;

    private void Awake()
    {
        inst = this;
    }

    public List<Entity381> entityData;
    public List<Scenario> scenarios;

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
}
