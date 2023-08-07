using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static PlayerManager Instance;

    public List<PlayerCommand> pcList;

    public Dictionary<string, string> commandIDResolved;

    public string commandResponses;

    void Awake()
    {
        Instance = this;
        transform.parent = NetworkManager.Instance.playerRoot.transform;
        commandIDResolved = new Dictionary<string, string>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (PlayerCommand pc in pcList)
            {
                if (pc != null)
                {
                    string id = pc.photonView.ViewID.ToString();
                    if (!commandIDResolved.ContainsKey(id))
                    {
                        commandIDResolved[id] = "NaN";
                    }

                    if (pc.currCommand != null && pc.currCommand.id != commandIDResolved[id])
                    {
                        ExecuteCommand(pc.currCommand, pc.photonView.Owner);
                        commandIDResolved[id] = pc.currCommand.id;
                    }
                }
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerCommand pcToRemove = null;

        foreach(PlayerCommand pc in pcList)
        {
            pcToRemove = pc.photonView.Owner == otherPlayer ? pc : pcToRemove;
        }

        if(pcToRemove != null)
        {
            pcList.Remove(pcToRemove);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            commandResponses = ConvertDictToString();

            stream.SendNext(commandResponses);
        }
        else
        {
            this.commandResponses = (string)stream.ReceiveNext();

            commandIDResolved = ConvertStringToDict(commandResponses);
        }
    }

    private string ConvertDictToString()
    {
        string s = "";

        foreach(string key in commandIDResolved.Keys)
        {
            s += "$" + key + "&" + commandIDResolved[key];
        }

        return s;
    }

    private Dictionary<string, string> ConvertStringToDict(string s)
    {
        Dictionary<string, string> new_dict = new Dictionary<string, string>();
        string[] sub_s = s.Split('$');

        for(int i = 1; i < sub_s.Length; i++)
        {
            string[] split_sub_s = sub_s[i].Split('&');
            new_dict[split_sub_s[0]] = split_sub_s[1];
        }

        return new_dict;
    }

    private void ExecuteCommand(NetCommand c, Player p)
    {
        if(c.GetType() == typeof(SpawnCommand))
        {
            SpawnCommand cc = (SpawnCommand)c;
            Entity381 ent = EntityMgr.inst.CreateEntity(cc.type, cc.pos, Vector3.up * cc.heading, p.UserId);
            DistanceMgr.inst.Initialize();
        }

        if(c.GetType() == typeof(WaypointCommand))
        {
            WaypointCommand cc = (WaypointCommand)c;
            List<Entity381> entities = new List<Entity381>();
            foreach(Entity381 ent in EntityMgr.inst.entities)
            {
                if (cc.entityIDs.Contains(ent.photonView.ViewID))
                {
                    entities.Add(ent);
                }
            }

            AIMgr.inst.HandleMove(entities, cc.pos, cc.clear);
        }

        if (c.GetType() == typeof(DesiredSpeedCommand))
        {
            DesiredSpeedCommand cc = (DesiredSpeedCommand)c;
            foreach (Entity381 ent in EntityMgr.inst.entities)
            {
                if (cc.entityID.Equals(ent.photonView.ViewID))
                {
                    ent.desiredSpeed = Utils.Clamp(ent.desiredSpeed + cc.speed, ent.minSpeed, ent.maxSpeed); ;
                }
            }
        }

        if (c.GetType() == typeof(DesiredHeadingCommand))
        {
            DesiredHeadingCommand cc = (DesiredHeadingCommand)c;
            foreach (Entity381 ent in EntityMgr.inst.entities)
            {
                if (cc.entityID.Equals(ent.photonView.ViewID))
                {
                    ent.desiredHeading = Utils.Degrees360(ent.desiredHeading + cc.heading);
                }
            }
        }

        Debug.Log("Command " + c.id + " executed.");
    }
}
