﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static PlayerManager Instance;

    void Awake()
    {
        Instance = this;
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
        throw new System.NotImplementedException();
    }

    public void Message()
    {
        Debug.Log("Message");
    }
}
