using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static PlayerManager Instance;

    public List<PlayerCommand> pcList;
    public string commandResponses;

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
        if(Random.Range(0,1) > 0.5f)
        {
            commandResponses = "1234";
        }
        else
        {
            commandResponses = "12345";
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(commandResponses);
        }
        else
        {
            this.commandResponses = (string)stream.ReceiveNext();
            Debug.Log(this.commandResponses);
        }
    }
}
