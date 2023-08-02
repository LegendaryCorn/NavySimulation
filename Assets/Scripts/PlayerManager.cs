using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static PlayerManager Instance;

    public List<PlayerCommand> pcList;

    void Awake()
    {
        Instance = this;
        transform.parent = NetworkManager.Instance.playerRoot.transform;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(pcList);
        }
        else
        {
            this.pcList = (List<PlayerCommand>)stream.ReceiveNext();
        }
    }

    public void Message()
    {
        Debug.Log("Message");
    }
}
