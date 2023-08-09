using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMgr
{
    public EntityMgr(GameMgr mgr)
    {
        entities = new List<Entity381>();
        gameMgr = mgr;
    }

    public List<Entity381> entities;

    public int entityId = 0;

    public GameMgr gameMgr;

    public Entity381 CreateEntity(EntityType et, Vector3 position, Vector3 eulerAngles)
    {
        Entity381 entity = null;
        //GameObject entityPrefab = MasterMgr.inst.entityPrefabs.Find(x => (x.GetComponent<Entity381>().entityType == et));
        entity = new Entity381(gameMgr, position, eulerAngles);
        entities.Add(entity);
        return entity;
    }
}
