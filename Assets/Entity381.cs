using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public enum EntityType
{
    DDG51,
    Container,
    MineSweeper,
    OilServiceVessel,
    OrientExplorer,
    PilotVessel,
    SmitHouston,
    Tanker,
    TugBoat
}


public class Entity381 : MonoBehaviourPunCallbacks, IPunObservable, IPunOwnershipCallbacks
{
    //------------------------------
    // values that change while running
    //------------------------------
    public bool hasOwner = false;
    public bool isSelected = false;
    public Vector3 position = Vector3.zero;
    public Vector3 velocity = Vector3.zero;

    public float speed;
    public float desiredSpeed;
    public float heading; //degrees
    public float desiredHeading; //degrees
    //------------------------------
    // values that do not change
    //------------------------------
    public float acceleration;
    public float turnRate;
    public float maxSpeed;
    public float minSpeed;
    public float mass;

    public EntityType entityType;

    public GameObject cameraRig;
    public GameObject selectionCircle;
    public GameObject ownerCircle;

    public void AssignOwner()
    {
        hasOwner = true;
        photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
    }

    public void RemoveOwner()
    {
        hasOwner = false;
        photonView.TransferOwnership(PhotonNetwork.MasterClient);
    }

    // Start is called before the first frame update
    void Start()
    {
        cameraRig = transform.Find("CameraRig").gameObject;
        selectionCircle = transform.Find("Decorations").Find("SelectionCylinder").gameObject;

        selectionCircle.SetActive(false);
        ownerCircle.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        targetView.TransferOwnership(requestingPlayer);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (photonView.AmOwner && hasOwner)
        {
            ownerCircle.SetActive(true);
        }
        else
        {
            ownerCircle.SetActive(false);
            if (SelectionMgr.inst.selectedEntities.Contains(this))
            {
                SelectionMgr.inst.selectedEntities.Remove(this);
            }
        }
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {

    }
}
