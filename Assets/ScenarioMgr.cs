using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct RandomPointBox
{
    public Vector3 center;
    public float length;
    public float width;
    public float angle;

    public Vector3 GenerateRandomPoint()
    {
        float randomL = Random.Range(-length / 2f, length / 2f);
        float randomW = Random.Range(-width / 2f, width / 2f);
        float angleRad = angle * Mathf.Deg2Rad;

        Vector3 new_point = new Vector3(
            center.x + randomL * Mathf.Cos(angleRad) + randomW * Mathf.Sin(angleRad),
            center.y,
            center.z - randomL * Mathf.Sin(angleRad) + randomW * Mathf.Cos(angleRad)
            );

        return new_point;
    }
}

[System.Serializable]
public struct ScenarioEntity
{
    public EntityType type;
    public RandomPointBox spawnPoint;
    public List<RandomPointBox> wayPoints;
    public float heading;
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
