using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PotentialParameters
{
    public float potentialDistanceThreshold;// = 1000;
    public float attractionCoefficient;// = 20000;
    public float attractiveExponent;// = -1;
    public float repulsiveCoefficient;// = 1000;
    public float repulsiveExponent;// = -2.0f;
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
