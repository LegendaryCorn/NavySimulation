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
        foreach(PlayerCommand pc in pcList)
        {
            if (pc != null)
            {
                string id = pc.photonView.Owner.UserId;
                if (id != null && !commandIDResolved.ContainsKey(id))
                {
                    commandIDResolved[id] = "NaN";
                }

                if (pc.currCommand != null && pc.currCommand.id != commandIDResolved[id])
                {
                    ExecuteCommand(pc.currCommand);
                    commandIDResolved[id] = pc.currCommand.id;
                }
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        foreach (PlayerCommand pc in pcList)
        {
            if(pc == null) { pcList.Remove(pc); }
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

    private void ExecuteCommand(NetCommand c)
    {
        Debug.Log("Command " + c.id + " executed.");
    }
}
