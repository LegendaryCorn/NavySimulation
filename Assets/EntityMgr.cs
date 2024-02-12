using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMgr
{
    public EntityMgr(GameMgr mgr)
    {
        entities = new List<Entity381>();
        boundaries = new List<Boundary381>();
        gameMgr = mgr;
    }

    public List<Entity381> entities;
    public List<Boundary381> boundaries;

    public int entityId = 0;

    public GameMgr gameMgr;

    public Entity381 CreateEntity(Entity381 entityData, Vector3 position, float heading, EntityRole eRole)
    {
        Entity381 entity = null;
        //GameObject entityPrefab = MasterMgr.inst.entityPrefabs.Find(x => (x.GetComponent<Entity381>().entityType == et));
        entity = new Entity381(gameMgr, entityData, position, heading, entityId, eRole);
        entityId++;
        entities.Add(entity);
        return entity;
    }
}
