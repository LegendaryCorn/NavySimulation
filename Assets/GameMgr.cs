using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class GameMgr : MonoBehaviour
{
    public static GameMgr inst;
    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Vector3 position = Vector3.zero;
            foreach (GameObject go in EntityMgr.inst.entityPrefabs)
            {
                Entity381 ent = EntityMgr.inst.CreateEntity(go.GetComponent<Entity381>().entityType, position, Vector3.zero);
                position.x += 200;
            }
        }
    }

    public Vector3 position;
    public float spread = 20;
    public float colNum = 10;
    // Update is called once per frame
    void Update()
    {
        
    }
}
