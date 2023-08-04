using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMgr : MonoBehaviour
{
    public static ControlMgr inst;
    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float deltaSpeed = 1;
    public float deltaHeading = 2;

    // Update is called once per frame
    void Update()
    {
        // Controls to Spawn
        if (Input.GetMouseButtonDown(1) && SelectionMgr.inst.selectedEntity == null)
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out RaycastHit hit))
            {
                SpawnCommand sp = new SpawnCommand(PlayerCommand.GenerateID(), EntityType.DDG51, hit.point, 0f);
                PlayerCommand.Instance.AddToCommList(sp);
            }
        }

        // Controls to add waypoint
        if(Input.GetMouseButtonDown(1) && SelectionMgr.inst.selectedEntity != null)
        {
            Vector3 pnt = Vector3.up * 1000;
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out RaycastHit hit))
            {
                pnt = hit.point;
            }
            List<int> ids = new List<int>();

            foreach(Entity381 ent in SelectionMgr.inst.selectedEntities)
            {
                ids.Add(ent.photonView.ViewID);
            }
            WaypointCommand wp = new WaypointCommand(PlayerCommand.GenerateID(), pnt, !Input.GetKey(KeyCode.LeftShift), ids);
            PlayerCommand.Instance.AddToCommList(wp);
        }


        // Controls to Change Speed/Heading
        if (SelectionMgr.inst.selectedEntity != null) {
            if (Input.GetKeyUp(KeyCode.UpArrow))
                SelectionMgr.inst.selectedEntity.desiredSpeed += deltaSpeed;
            if (Input.GetKeyUp(KeyCode.DownArrow))
                SelectionMgr.inst.selectedEntity.desiredSpeed -= deltaSpeed;
            SelectionMgr.inst.selectedEntity.desiredSpeed =
                Utils.Clamp(SelectionMgr.inst.selectedEntity.desiredSpeed, SelectionMgr.inst.selectedEntity.minSpeed, SelectionMgr.inst.selectedEntity.maxSpeed);

            if (Input.GetKeyUp(KeyCode.LeftArrow))
                SelectionMgr.inst.selectedEntity.desiredHeading -= deltaHeading;
            if (Input.GetKeyUp(KeyCode.RightArrow))
                SelectionMgr.inst.selectedEntity.desiredHeading += deltaHeading;
            SelectionMgr.inst.selectedEntity.desiredHeading = Utils.Degrees360(SelectionMgr.inst.selectedEntity.desiredHeading);
        }
    }

    
    //------------------------------------------
}
