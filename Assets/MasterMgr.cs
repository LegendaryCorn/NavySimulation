using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterMgr : MonoBehaviour
{
    public static MasterMgr inst;

    public List<GameObject> entityPrefabs;
    public GameObject entitiesRoot;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject game = new GameObject();
        game.transform.parent = this.transform.parent;
        game.name = "GameMgr";
        game.AddComponent<GameMgr>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
