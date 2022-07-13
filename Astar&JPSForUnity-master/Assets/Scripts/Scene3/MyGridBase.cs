using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGridBase : MonoBehaviour
{
    private MyNode[,] m_Grid;
    public Vector2 m_GridSize;
    public float m_GridRadius;
    public LayerMask m_LayerMask;
    private float m_GridDiameter;
    private int m_GridCountX;
    private int m_GridCountY;
    public Stack<MyNode> m_Path = new Stack<MyNode>();



    // Start is called before the first frame update
    void Start()
    {
        m_GridDiameter = m_GridRadius * 2;
        m_GridCountX = Mathf.RoundToInt(m_GridSize.x / m_GridDiameter);
        m_GridCountY = Mathf.RoundToInt(m_GridSize.y / m_GridDiameter);
        m_Grid = new MyNode[m_GridCountX, m_GridCountY];
        createGrid();
    }

    // Update is called once per frame
    public void createGrid()
    {
        Vector3 worldPos = transform.position;
        worldPos.x = worldPos.x - m_GridSize.x / 2;
        worldPos.z = worldPos.z - m_GridSize.y / 2;
        for (int i = 0; i < m_GridCountX; i++)
        {
            for (int j = 0; j < m_GridCountY; j++)
            {
                Vector3 tempPos = worldPos;
                tempPos.x = tempPos.x + i * m_GridDiameter + m_GridRadius;
                tempPos.z = tempPos.z + j * m_GridDiameter + m_GridRadius;
                bool canWalk = !Physics.CheckSphere(tempPos, m_GridRadius,m_LayerMask);
                m_Grid[i, j] = new MyNode(canWalk, tempPos, i, j);
            }
        }
    }

    public List<MyNode> FindNeigbor(MyNode node)
    {
        List<MyNode> list = new List<MyNode> ();
        for (int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j)
            {
                if (i == 0 && j == 0)
                    continue;
                int gridX = node.m_GridX + i;
                int gridY = node.m_GridY + j;
                if (gridX > 0 && gridX < m_GridCountX && gridY > 0 && gridY < m_GridCountY)
                {
                    list.Add(m_Grid[gridX, gridY]);
                }
            }
        }
        return list;
    }

    public MyNode GetFromPosition(Vector3 pos)
    {
        float percentX = (pos.x + m_GridSize.x / 2) / m_GridSize.x;
        float percentY = (pos.z + m_GridSize.y / 2) / m_GridSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int countX = Mathf.RoundToInt((m_GridCountX - 1) * percentX);
        int countY = Mathf.RoundToInt((m_GridCountY - 1) * percentY);
        return m_Grid[countX, countY];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(m_GridSize.x, 1 ,m_GridSize.y));
        //Debug.Log(m_GridCountX.ToString() + m_GridCountY.ToString());
        if (m_Grid == null)
            return;
        foreach(var node in m_Grid)
        {
            Gizmos.color = node.m_canWalk ? Color.white : Color.red;
            Gizmos.DrawCube(node.m_worldPos, Vector3.one * (m_GridDiameter - 0.1f));
        }
        if (m_Path != null)
        {
            foreach (var node in m_Path)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(node.m_worldPos, Vector3.one * (m_GridDiameter - 0.1f));
            }
        }
    }
}
