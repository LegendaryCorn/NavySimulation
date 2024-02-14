using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselFitness
{
    public Entity381 entity;
    private Vector3 fitPoint;
    private Entity381 targetEntity; // If null, the fitpoint will be placed on the world. Otherwise, it will be placed relative to the target entity.
    private bool wpOnTarget; // Whether or not the waypoint is attached to the target

    public float fitPDist; // Closest to the fitpoint the vessel gets
    public float cpaDist; // Closest the vessels get to each other


    public VesselFitness(Entity381 ent)
    {
        entity = ent;
    }

    public void SetFitPoints(EScenarioType scenarioType, bool isOwnShip, Entity381 targetEntity)
    {

        fitPDist = Mathf.Infinity;
        cpaDist = Mathf.Infinity;

        this.targetEntity = targetEntity;

        if (isOwnShip)
        {
            wpOnTarget = true;
            switch (scenarioType)
            {
                case EScenarioType.HeadOn: // 500 to the left
                    fitPoint = new Vector3(0, 0, -500);
                    break;
                case EScenarioType.Overtaking: // 500 to the right
                    fitPoint = new Vector3(0, 0, 500);
                    break;
                case EScenarioType.Crossing:
                case EScenarioType.Crossing90: // 500 Behind
                    fitPoint = new Vector3(-500, 0, 0);
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (scenarioType)
            {
                case EScenarioType.HeadOn: // 500 to the left
                    fitPoint = new Vector3(-500, 0, 0);
                    wpOnTarget = true;
                    break;
                case EScenarioType.Overtaking:
                case EScenarioType.Crossing:
                case EScenarioType.Crossing90: // Keep course
                    fitPoint = (entity.position + ((Move)entity.ai.commands[0]).movePosition) / 2;
                    wpOnTarget = false;
                    break;
                default:
                    break;
            }
        }
    }

    public void OnUpdate(float dt)
    {
        if (targetEntity != null)
        {
            Vector3 fp = fitPoint;
            if (wpOnTarget)
            {
                float h = targetEntity.heading * Mathf.Deg2Rad;
                fp = new Vector3(fitPoint.x * Mathf.Cos(h) - fitPoint.z * Mathf.Sin(h), 0, fitPoint.x * Mathf.Sin(h) + fitPoint.z * Mathf.Cos(h)) + targetEntity.position;
            }
            fitPDist = Mathf.Min(fitPDist, Vector3.SqrMagnitude(fp - entity.position));
            cpaDist = Mathf.Min(cpaDist, Vector3.SqrMagnitude(entity.position - targetEntity.position));
        }
    }
}
