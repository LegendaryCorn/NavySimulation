using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientedPhysics
{
    // Start is called before the first frame update
    public OrientedPhysics(Entity381 ent)
    {
        entity = ent;
    }

    public Entity381 entity;

    public void OnUpdate(float dt)
    {
        if (Utils.ApproximatelyEqual(entity.speed, entity.desiredSpeed))
        {
            ;
        }
        else if (entity.speed < entity.desiredSpeed)
        {
            entity.speed = entity.speed + entity.acceleration * dt;
        }
        else if (entity.speed > entity.desiredSpeed)
        {
            entity.speed = entity.speed - entity.acceleration * dt;
        }
        entity.speed = Utils.Clamp(entity.speed, entity.minSpeed, entity.maxSpeed);

        //heading
        if (Utils.ApproximatelyEqual(entity.heading, entity.desiredHeading))
        {
            ;
        }
        else if (Utils.AngleDiffPosNeg(entity.desiredHeading, entity.heading) > 0)
        {
            entity.heading += entity.turnRate * dt;
        }
        else if (Utils.AngleDiffPosNeg(entity.desiredHeading, entity.heading) < 0)
        {
            entity.heading -= entity.turnRate * dt;
        }
        entity.heading = Utils.Degrees360(entity.heading);
        //
        entity.velocity.x = Mathf.Sin(entity.heading * Mathf.Deg2Rad) * entity.speed;
        entity.velocity.y = 0;
        entity.velocity.z = Mathf.Cos(entity.heading * Mathf.Deg2Rad) * entity.speed;

        entity.position = entity.position + entity.velocity * dt;
    }

    public Vector3 eulerRotation = Vector3.zero;


}
