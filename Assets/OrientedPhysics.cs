using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientedPhysics : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        entity = GetComponentInParent<Entity381>();
        entity.position = transform.localPosition;
    }

    public Entity381 entity;


    // Update is called once per frame
    void Update()
    {
        if (entity.isRealtime)
        {
            OnUpdate(Time.deltaTime);
        }
    }

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
        transform.localPosition = entity.position;

        eulerRotation.y = entity.heading;
        transform.localEulerAngles = eulerRotation;
    }

    public Vector3 eulerRotation = Vector3.zero;


}
