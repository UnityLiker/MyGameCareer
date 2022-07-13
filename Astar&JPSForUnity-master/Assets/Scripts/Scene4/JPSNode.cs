using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPSNode
{
    public Vector3 m_worldPos;
    public bool m_canWalk;
    public int m_GridX;
    public int m_GridY;
    public JPSNode parent;
    public int gCost;
    public int hCost;

    public int FCost
    {
        get { return gCost + hCost; }
    }

    public JPSNode(bool canWalk, Vector3 worldPos, int gridX, int gridY)
    {
        m_canWalk = canWalk;
        m_worldPos = worldPos;
        m_GridX = gridX;
        m_GridY = gridY;
    }
}
