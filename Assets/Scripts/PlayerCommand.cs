using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerCommand : MonoBehaviourPunCallbacks, IPunObservable
{

    // Start is called before the first frame update
    void Start()
    {
        PlayerManager.Instance.Message();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
