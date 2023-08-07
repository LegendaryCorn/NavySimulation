using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class GameMgr : MonoBehaviourPunCallbacks
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
                Entity381 ent = EntityMgr.inst.CreateEntity(go.GetComponent<Entity381>().entityType, position, Vector3.zero, "");
                position.x += 200;
            }

            StarterShips(PhotonNetwork.LocalPlayer);
        }
    }

    public Vector3 position;
    public float spread = 20;
    public float colNum = 10;

    public int numShips = 5;
    // Update is called once per frame
    void Update()
    {
        
    }

    public void StarterShips(Player p)
    {
        int i = numShips;
        foreach (Entity381 ent in EntityMgr.inst.entities)
        {
            if (i == 0)
            {
                break;
            }
            if (ent.idOwner.Equals(""))
            {
                ent.SetOwner(p.UserId);
                i--;
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        StarterShips(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        foreach (Entity381 ent in EntityMgr.inst.entities)
        {
            if (ent.idOwner.Equals(otherPlayer.UserId))
            {
                ent.SetOwner("");
            }
        }
    }
}
