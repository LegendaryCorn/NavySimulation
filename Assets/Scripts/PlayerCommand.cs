using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PlayerCommand : MonoBehaviourPunCallbacks, IPunObservable
{
    //private bool netInitialized = false;

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

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
