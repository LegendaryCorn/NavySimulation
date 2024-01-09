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
