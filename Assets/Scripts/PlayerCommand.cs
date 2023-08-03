using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PlayerCommand : MonoBehaviourPunCallbacks, IPunObservable
{
    public NetCommand comm;

    public string commString;

    void Awake()
    {
        transform.parent = NetworkManager.Instance.playerRoot.transform;

        if (PhotonNetwork.IsMasterClient)
        {
            PlayerManager.Instance.pcList.Add(this);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        comm = new NetCommand(GenerateID());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if(photonView.IsMine && Random.Range(0f,10f) < 0.02f)
        {
            comm = new NetCommand(GenerateID());
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            commString = comm.ConvToString();

            stream.SendNext(commString);
        }
        else
        {
            this.commString = (string)stream.ReceiveNext();

            comm = NetCommand.ConvToCommand(commString);
        }
    }

    private string GenerateID()
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
