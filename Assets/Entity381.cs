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


public class Entity381 : MonoBehaviourPunCallbacks, IPunObservable
{
    //------------------------------
    // values that change while running
    //------------------------------
    public string idOwner = ""; // "" means no owner.

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

    void Awake()
    {
        cameraRig = transform.Find("CameraRig").gameObject;
        selectionCircle = transform.Find("Decorations").Find("SelectionCylinder").gameObject;

        selectionCircle.SetActive(false);
        ownerCircle.SetActive(false);

    }

    // Start is called before the first frame update
    void Start()
    {
        transform.parent = EntityMgr.inst.entitiesRoot.transform;

        if (!EntityMgr.inst.entities.Contains(this))
        {
            EntityMgr.inst.entities.Add(this);
            DistanceMgr.inst.Initialize();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(idOwner);
            stream.SendNext(position);
            stream.SendNext(velocity);
            stream.SendNext(speed);
            stream.SendNext(heading);
            stream.SendNext(desiredSpeed);
            stream.SendNext(desiredHeading);
        }
        else
        {
            string prevOwner = this.idOwner;

            this.idOwner = (string)stream.ReceiveNext();
            this.position = (Vector3)stream.ReceiveNext();
            this.velocity = (Vector3)stream.ReceiveNext();
            this.speed = (float)stream.ReceiveNext();
            this.heading = (float)stream.ReceiveNext();
            this.desiredSpeed = (float)stream.ReceiveNext();
            this.desiredHeading = (float)stream.ReceiveNext();

            if (!prevOwner.Equals(this.idOwner))
            {
                ownerCircle.SetActive(this.OwnsThisEntity());
            }
        }
    }

    public void SetOwner(string id)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            this.idOwner = id;
            ownerCircle.SetActive(this.OwnsThisEntity());
        }
    }

    public bool OwnsThisEntity()
    {
        return !this.idOwner.Equals("") && this.idOwner.Equals(PhotonNetwork.LocalPlayer.UserId);
    }
}
