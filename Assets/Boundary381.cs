using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary381
{
    public List<Vector3> points;
    public GameMgr gameMgr;

    public Boundary381(GameMgr mgr, List<Vector3> pointList)
    {
        gameMgr = mgr;
        points = pointList;
    }
}
