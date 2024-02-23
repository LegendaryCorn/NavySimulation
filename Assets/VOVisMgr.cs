using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VOVisMgr : MonoBehaviour
{
    public static VOVisMgr inst;
    bool visualize = false;
    public float velMult; // Increases the size of things
    public float entRad; // Radius of entities
    public bool trafficEnable; // Enables the lines for traffic

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            visualize = !visualize;
        }

        if (visualize && SelectionMgr.inst.selectedEntities.Count > 0 && SelectionMgr.inst.selectedEntity != null)
        {
            PrintObstacles();
        }
    }

    void PrintObstacles()
    {
        Entity381 selEntity = SelectionMgr.inst.selectedEntity.ent;

        Debug.DrawRay(selEntity.position, velMult * selEntity.velocity, Color.green, Time.fixedDeltaTime); // Velocity Line

        foreach(SimulatedEntity simEnt in SimulationMgr.inst.simulatedEntities)
        {
            if (simEnt.ent == selEntity) continue;
            if (!trafficEnable && (simEnt.ent.role == EntityRole.Traffic || simEnt.ent.role == EntityRole.None)) continue;

            Entity381 otherEntity = simEnt.ent;

            Vector3 diffDist = otherEntity.position - selEntity.position;
            float dir = Mathf.Atan2(diffDist.x, diffDist.z);
            float ang = Mathf.Asin(2 * entRad / diffDist.magnitude);

            float leftAng = dir - ang;
            float rightAng = dir + ang;

            Vector3 leftVec = new Vector3(Mathf.Sin(leftAng), 0, Mathf.Cos(leftAng));
            Vector3 rightVec = new Vector3(Mathf.Sin(rightAng), 0, Mathf.Cos(rightAng));

            Color c = simEnt.ent.role == EntityRole.Traffic || simEnt.ent.role == EntityRole.None ? Color.magenta : Color.red;

            Debug.DrawRay(selEntity.position + velMult * otherEntity.velocity, velMult * 10000f * leftVec, c, Time.fixedDeltaTime); // Left Line
            Debug.DrawRay(selEntity.position + velMult * otherEntity.velocity, velMult * 10000f * rightVec, c, Time.fixedDeltaTime); // Right Line

            // Determine if the current velocity is inside a triangle
            Vector3 relVel = selEntity.velocity - otherEntity.velocity;
            float relAng = Mathf.Atan2(relVel.x, relVel.z);
            
            if(Utils.AngleBetween(relAng * Mathf.Rad2Deg, leftAng * Mathf.Rad2Deg, rightAng * Mathf.Rad2Deg))
            {
                Debug.Log("Ship " + selEntity.id.ToString() + " will collide with Ship " + otherEntity.id.ToString());
            }
        }
    }
}
