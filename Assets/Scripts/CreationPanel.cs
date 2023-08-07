using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreationPanel : MonoBehaviour
{
    [SerializeField] private RectTransform shipPanel;
    [SerializeField] private RectTransform rotatePanel;
    [SerializeField] private Button sampleButton;
    [SerializeField] private Text angText;

    private Vector3 pointToSpawn;
    private EntityType entityToSpawn = EntityType.DDG51;
    private float headingToSpawn = 0f;

    // Start is called before the first frame update
    void Start()
    {
        float y = 0;
        foreach(GameObject g in EntityMgr.inst.entityPrefabs)
        {
            Button b = Instantiate<Button>(sampleButton, shipPanel);
            b.gameObject.SetActive(true);
            b.transform.position = new Vector3(0, y, 0);
            y -= b.GetComponent<RectTransform>().rect.height;
            b.GetComponentInChildren<Text>().text = g.name;
            b.onClick.AddListener(() => ButtonPress(g.GetComponent<Entity381>().entityType));
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Controls to Spawn
        if (Input.GetMouseButtonDown(1) && SelectionMgr.inst.selectedEntity == null)
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out RaycastHit hit))
            {
                pointToSpawn = hit.point;
                shipPanel.gameObject.SetActive(true);
                shipPanel.anchoredPosition = Input.mousePosition;
            }
            else
            {
                shipPanel.gameObject.SetActive(false);
            }
        }
        else if(Input.GetMouseButtonDown(1) || SelectionMgr.inst.selectedEntity != null)
        {
            shipPanel.gameObject.SetActive(false);
        }
    }

    public void ButtonPress(EntityType e)
    {
        entityToSpawn = e;
        rotatePanel.gameObject.SetActive(true);
        shipPanel.gameObject.SetActive(false);
        rotatePanel.position = shipPanel.anchoredPosition;

    }

    public void SliderChange(float f)
    {
        headingToSpawn = f;
        angText.text = f.ToString();
    }

    public void SpawnShip()
    {
        SpawnCommand sp = new SpawnCommand(PlayerCommand.GenerateID(), entityToSpawn, pointToSpawn, headingToSpawn);
        PlayerCommand.Instance.AddToCommList(sp);
    }
}
