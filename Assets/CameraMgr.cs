using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMgr : MonoBehaviour
{
    public static CameraMgr inst;
    public bool followEntity = false;
    private void Awake()
    {
        inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public GameObject RTSCameraRig;
    public GameObject YawNode;   // Child of RTSCameraRig
    public GameObject PitchNode; // Child of YawNode
    public GameObject RollNode;  // Child of PitchNode

    public GameObject followObject;
    public Vector3 offset;
    //Camera is child of RollNode

    public float cameraMoveSpeed = 500;
    public float cameraTurnRate = 10;
    public Vector3 currentYawEulerAngles = Vector3.zero;
    public Vector3 currentPitchEulerAngles = Vector3.zero;
    // Update is called once per frame
    void Update()
    {
        if(followEntity && followObject != null)
        {
            YawNode.transform.position = followObject.transform.position + offset;
        }

        if (Input.GetKey(KeyCode.W))
            YawNode.transform.Translate(Vector3.forward * Time.deltaTime * cameraMoveSpeed);
        if (Input.GetKey(KeyCode.S))
            YawNode.transform.Translate(Vector3.back * Time.deltaTime * cameraMoveSpeed);
        if (Input.GetKey(KeyCode.A))
            YawNode.transform.Translate(Vector3.left * Time.deltaTime * cameraMoveSpeed);
        if (Input.GetKey(KeyCode.D))
            YawNode.transform.Translate(Vector3.right * Time.deltaTime * cameraMoveSpeed);
        if (Input.GetKey(KeyCode.R))
            YawNode.transform.Translate(Vector3.up * Time.deltaTime * cameraMoveSpeed);
        if (Input.GetKey(KeyCode.F))
            YawNode.transform.Translate(Vector3.down * Time.deltaTime * cameraMoveSpeed);

        currentYawEulerAngles = YawNode.transform.eulerAngles;
        if (Input.GetKey(KeyCode.Q))
            currentYawEulerAngles.y -= cameraTurnRate * Time.deltaTime;
        if (Input.GetKey(KeyCode.E))
            currentYawEulerAngles.y += cameraTurnRate * Time.deltaTime;
        YawNode.transform.eulerAngles = currentYawEulerAngles;

        currentPitchEulerAngles = PitchNode.transform.eulerAngles;
        if (Input.GetKey(KeyCode.Z))
            currentPitchEulerAngles.x -= cameraTurnRate * Time.deltaTime;
        if (Input.GetKey(KeyCode.X))
            currentPitchEulerAngles.x += cameraTurnRate * Time.deltaTime;
        PitchNode.transform.eulerAngles = currentPitchEulerAngles;

        
        if (Input.GetKeyDown(KeyCode.C)) {
            followEntity = !followEntity;
            if (followEntity && SelectionMgr.inst.selectedEntity != null) {
                followObject = SelectionMgr.inst.selectedEntity.gameObject;
            }
        }

        if (followEntity && followObject != null)
        {
            offset = YawNode.transform.position - followObject.transform.position;
        }
        if(followEntity && followObject == null)
        {
            followObject = SimulationMgr.inst.simulatedEntities[0].gameObject;
            YawNode.transform.localPosition = new Vector3(0, 0, 2500) + followObject.transform.position;
            PitchNode.transform.eulerAngles = new Vector3(0, 0, 0);
            offset = YawNode.transform.position - followObject.transform.position;
        }
    }
}
