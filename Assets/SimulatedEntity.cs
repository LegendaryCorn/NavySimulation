using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatedEntity : MonoBehaviour
{
    public EntityType type;
    public int id;
    public float speed;
    public float desiredSpeed;
    public float heading;
    public float desiredHeading;

    public bool isSelected;
    public GameObject selectionCircle;

    public void Select(bool selected)
    {
        isSelected = selected;
        selectionCircle.SetActive(selected);
    }
}
