using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PlayerCommand : MonoBehaviourPunCallbacks, IPunObservable
{
    public List<NetCommand> commList;
    public NetCommand currCommand;

    public string commString;

    public static PlayerCommand Instance;

    void Awake()
    {
        transform.parent = NetworkManager.Instance.playerRoot.transform;

        if (PhotonNetwork.IsMasterClient)
        {
            PlayerManager.Instance.pcList.Add(this);
        }
        if (photonView.IsMine)
        {
            Instance = this;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        currCommand = null;
        commList = new List<NetCommand>();
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine && commList.Count > 0 && PlayerManager.Instance && PlayerManager.Instance.commandIDResolved.ContainsKey(photonView.ViewID.ToString()))
        {
            string lastComm = PlayerManager.Instance.commandIDResolved[photonView.ViewID.ToString()];
            if (commList[0].id.Equals(lastComm))
            {
                Debug.Log("Command " + lastComm + " resolved.");
                commList.RemoveAt(0);
                currCommand = commList.Count > 0 ? commList[0] : null;
            }
        }
    }

    public void AddToCommList(NetCommand c)
    {
        commList.Add(c);
        if(commList.Count == 1)
        {
            currCommand = commList[0];
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (commList.Count > 0)
            {
                commString = (currCommand).ConvToString();
            }
            else
            {
                commString = "";
            }

            stream.SendNext(commString);
        }
        else
        {
            this.commString = (string)stream.ReceiveNext();

            currCommand = NetCommand.ConvToCommand(commString);
        }
    }

    public static string GenerateID()
    {
        string new_id = "";
        for(int i = 0; i < 6; i++)
        {
            int id_int = Random.Range(0, 36);
            char c = id_int > 9 ? (char)(id_int + 55) : id_int.ToString()[0];
            new_id += c;
        }
        return new_id;
    }
}
