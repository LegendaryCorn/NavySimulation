using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PF
{
    public bool isAttractive;
    public float baseCoefficient;
    public float baseExponent;
    public float bearingCoefficient;
    public float bearingExponent;
    public float bearingSinAngle;
    public float headingCoefficient;
    public float headingExponent;
    public float headingSinAngle;
    public float verticalOffset;
    public float horizontalOffset;
    public float minAngle;
    public float maxAngle;
}

[System.Serializable]
public struct PotentialParameters
{
    public float potentialDistanceThreshold;// = 1000;
    public PF waypointPotential;
    public List<PF> shipPotentials;

    public PotentialParameters(float[] parameters)
    {
        potentialDistanceThreshold = 100000000f;
        waypointPotential.isAttractive = true;
        waypointPotential.baseCoefficient = parameters[0];
        waypointPotential.baseExponent = parameters[1];
        waypointPotential.bearingCoefficient = 0;
        waypointPotential.bearingExponent = 1;
        waypointPotential.bearingSinAngle = 0;
        waypointPotential.headingCoefficient = 0;
        waypointPotential.headingExponent = 1;
        waypointPotential.headingSinAngle = 0;
        waypointPotential.verticalOffset = 0;
        waypointPotential.horizontalOffset = 0;
        waypointPotential.minAngle = 0f;
        waypointPotential.maxAngle = 360f;

        shipPotentials = new List<PF>();
        for(int i = 2; i < parameters.Length; i+= 10)
        {
            PF p;
            p.isAttractive = true;//i + 4 * 1 >= parameters.Length;
            p.baseCoefficient = parameters[i];
            p.baseExponent = parameters[i+1];
            p.bearingCoefficient = parameters[i + 2];
            p.bearingExponent = parameters[i + 3];
            p.bearingSinAngle = parameters[i + 4];
            p.headingCoefficient = parameters[i + 5];
            p.headingExponent = parameters[i + 6];
            p.headingSinAngle = parameters[i + 7];
            p.verticalOffset = parameters[i+8];
            p.horizontalOffset = parameters[i+9];
            p.minAngle = 0f;
            p.maxAngle = 0f;
            shipPotentials.Add(p);
        }
    }
}

public class AIMgr
{

    // Start is called before the first frame update
    public AIMgr(GameMgr mgr, PotentialParameters parameters)
    {
        layerMask = 1 << 9;// LayerMask.GetMask("Water");
        gameMgr = mgr;
        potentialParameters = parameters;
    }

    public bool isPotentialFieldsMovement = true;

    public PotentialParameters potentialParameters;

    public RaycastHit hit;
    public int layerMask;

    public GameMgr gameMgr;

    public void HandleMove(List<Entity381> entities, Vector3 point)
    {
        foreach (Entity381 entity in entities) {
            Move m = new Move(entity, hit.point);
            UnitAI uai = entity.ai;
            AddOrSet(m, uai);
        }
    }

    void AddOrSet(Command c, UnitAI uai)
    {
        if (Input.GetKey(KeyCode.LeftShift))
            uai.AddCommand(c);
        else
            uai.SetCommand(c);
    }



    public void HandleFollow(List<Entity381> entities, Entity381 ent)
    {
        foreach (Entity381 entity in entities) {
            Follow f = new Follow(entity, ent, new Vector3(100, 0, 0));
            UnitAI uai = entity.ai;
            AddOrSet(f, uai);
        }
    }

    void HandleIntercept(List<Entity381> entities, Entity381 ent)
    {
        foreach (Entity381 entity in entities) {
            Intercept intercept = new Intercept(entity, ent);
            UnitAI uai = entity.ai;
            AddOrSet(intercept, uai);
        }

    }

    public float rClickRadiusSq = 10000;
    public Entity381 FindClosestEntInRadius(Vector3 point, float rsq)
    {
        Entity381 minEnt = null;
        float min = float.MaxValue;
        foreach (Entity381 ent in gameMgr.entityMgr.entities) {
            float distanceSq = (ent.position - point).sqrMagnitude;
            if (distanceSq < rsq) {
                if (distanceSq < min) {
                    minEnt = ent;
                    min = distanceSq;
                }
            }    
        }
        return minEnt;
    }
}
