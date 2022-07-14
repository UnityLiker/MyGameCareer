using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPSGridBase : MonoBehaviour
{
    public JPSNode[,] m_Grid;
    public float m_NodeRadius;
    public Vector2 m_GridSize;
    public LayerMask m_Layer;
    public Stack<JPSNode> m_Path = new Stack<JPSNode>();
    private float m_NodeDiameter;
    private int m_GridCountX;
    private int m_GridCountY;
    // Start is called before the first frame update
    void Start()
    {
        m_NodeDiameter = m_NodeRadius * 2;
        m_GridCountX = Mathf.RoundToInt(m_GridSize.x / m_NodeDiameter);
        m_GridCountY = Mathf.RoundToInt(m_GridSize.y / m_NodeDiameter);
        m_Grid = new JPSNode[m_GridCountX, m_GridCountY];
        CreateGrid();
    }

    // Update is called once per frame

    public void CreateGrid()
    {
        Vector3 worldPos = transform.position;
        worldPos.x = worldPos.x - m_GridSize.x / 2;
        worldPos.z = worldPos.z - m_GridSize.y / 2;
        for (int i = 0; i < m_GridCountX; i++)
        {
            for (int j = 0; j < m_GridCountY; j++)
            {
                Vector3 tempPos = worldPos;
                tempPos.x = tempPos.x + i * m_NodeDiameter + m_NodeRadius;
                tempPos.z = tempPos.z + j * m_NodeDiameter + m_NodeRadius;
                bool canWalk = !Physics.CheckSphere(tempPos, m_NodeRadius, m_Layer);
                m_Grid[i, j] = new JPSNode(canWalk, tempPos, i, j);
            }
        }
    }

    public List<JPSNode> GetNeigbour(JPSNode node)
    {
        List<JPSNode> neighbours = new List<JPSNode>();
        int gridX = node.m_GridX;
        int gridY = node.m_GridY;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                int x = gridX + i;
                int y = gridY + j;
                if (x > 0 && x < m_GridCountX && y > 0 && y < m_GridCountY)
                {
                    neighbours.Add(m_Grid[i, j]);
                }
            }
        }
        return neighbours;
    }

    public JPSNode GetFromPosition(Vector3 pos)
    {
        float percentX = (pos.x + m_GridSize.x / 2) / m_GridSize.x;
        float percentY = (pos.z + m_GridSize.y / 2) / m_GridSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int gridX = Mathf.RoundToInt(percentX * (m_GridCountX - 1));
        int gridY = Mathf.RoundToInt(percentY * (m_GridCountY - 1));
        return m_Grid[gridX, gridY];
    }

    public bool IsInBound(int x, int y)
    {
        return (x > 0 && x < m_GridCountX && y > 0 && y < m_GridCountY);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(m_GridSize.x, 1, m_GridSize.y));
        if (m_Grid == null)
            return;
        for (int i = 0; i < m_GridCountX; i++)
        {
            for (int j = 0; j < m_GridCountY; j++)
            {
                Gizmos.color = m_Grid[i, j].m_canWalk ? Color.white : Color.red;
                Gizmos.DrawCube(m_Grid[i, j].m_worldPos, Vector3.one * (m_NodeDiameter - 0.1f));
            }
        }
        if (m_Path != null)
        {
            JPSNode tempNode = null;
            foreach (var node in m_Path)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(node.m_worldPos, Vector3.one * (m_NodeDiameter - 0.1f));
                if (tempNode != null)
                    Gizmos.DrawLine(node.m_worldPos, tempNode.m_worldPos);
                tempNode = node;
            }
        }
    }
}
