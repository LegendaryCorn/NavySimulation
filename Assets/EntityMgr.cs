using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMgr : MonoBehaviour
{
    private void Awake()
    {
        entities = new List<Entity381>();
    }

    public List<Entity381> entities;

    public static int entityId = 0;

    public GameMgr gameMgr;

    public Entity381 CreateEntity(EntityType et, Vector3 position, Vector3 eulerAngles)
    {
        Entity381 entity = null;
        GameObject entityPrefab = MasterMgr.inst.entityPrefabs.Find(x => (x.GetComponent<Entity381>().entityType == et));
        if (entityPrefab != null) {
            GameObject entityGo = Instantiate(entityPrefab, position, Quaternion.Euler(eulerAngles), MasterMgr.inst.entitiesRoot.transform);
            if (entityGo != null) {
                entity = entityGo.GetComponent<Entity381>();
                entityGo.name = et.ToString() + entityId++;
                entity.gameMgr = gameMgr;
                entities.Add(entity);
            }
        }
        return entity;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
