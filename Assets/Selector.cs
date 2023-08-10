using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    public SimulatedEntity entity;
    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponentInParent<SimulatedEntity>();
        entity.selectionCircle.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        //if (Input.GetMouseButtonDown(0)) {
            //SelectionMgr.inst.SelectEntity(entity);
        //}
    }


}
