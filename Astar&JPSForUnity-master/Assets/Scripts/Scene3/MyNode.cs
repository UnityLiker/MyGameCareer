using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNode
{
    public bool m_canWalk;
    public Vector3 m_worldPos;
    public int m_GridX;
    public int m_GridY;
    public int gCost;
    public int hCost;
    public MyNode parent;

    public int FCost
    {
        get{ return gCost + hCost; }
    }

    public MyNode(bool canWalk, Vector3 worldPos, int gridX, int gridY)
    {
        m_canWalk = canWalk;
        m_worldPos = worldPos;
        m_GridX = gridX;
        m_GridY = gridY;
        parent = null;
    }


}
