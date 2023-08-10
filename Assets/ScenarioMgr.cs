using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct RandomPointBox
{
    public float length;
    public float width;
    public float angle;
}

[System.Serializable]
public struct ScenarioEntity
{
    public EntityType type;
    public RandomPointBox spawnPoint;
    public List<RandomPointBox> wayPoints;
}

[System.Serializable]
public struct Scenario
{
    public List<ScenarioEntity> scenarioEntities;
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
}
