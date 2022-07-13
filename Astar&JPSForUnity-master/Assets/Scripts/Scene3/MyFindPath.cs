using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFindPath : MonoBehaviour
{
    public Transform startNode;
    public Transform endNode;
    private MyGridBase gridBase;
    private List<MyNode> openList = new List<MyNode> ();
    private HashSet<MyNode> closedList = new HashSet<MyNode> ();

    void Start()
    {
        gridBase = GetComponent<MyGridBase>();
        gridBase.createGrid();
    }

    // Update is called once per frame
    void Update()
    {
        gridBase.createGrid ();
        FindingPath(startNode, endNode);
    }

    void FindingPath(Transform startNode, Transform endNode)
    {
        MyNode start = gridBase.GetFromPosition(startNode.position);
        MyNode end = gridBase.GetFromPosition(endNode.position);
        openList.Clear ();
        closedList.Clear ();
        openList.Add(start);
        int count = openList.Count;
        while (count > 0)
        {
            MyNode currentNode = openList[0];
            for (int i = 0; i < openList.Count; i++)
            {
                if (currentNode.FCost > openList[i].FCost || currentNode.FCost == openList[i].FCost && currentNode.hCost > openList[i].hCost)
                {
                    currentNode = openList[i];
                }
            }
            
            closedList.Add(currentNode);
            openList.Remove(currentNode);
            if (currentNode == end)
            {
                GeneratePath(start, end);
                return;
            }

            List<MyNode> list = gridBase.FindNeigbor(currentNode);
            foreach (var node in list)
            {
                if (!node.m_canWalk || closedList.Contains(node))
                    continue;
                int gCost = currentNode.gCost + GetDistance(currentNode, node);
                if (gCost < node.gCost || !openList.Contains(node))
                {
                    node.gCost = gCost;
                    node.hCost = GetDistance(end, node);
                    node.parent = currentNode;
                    if (!openList.Contains(node))
                    {
                        openList.Add(node);
                    }
                }
            }
        }
    }

    void GeneratePath(MyNode start, MyNode end)
    {
        Stack<MyNode> path = new Stack<MyNode>();
        MyNode node = end;
        while (node != start)
        {
            path.Push(node);
            node = node.parent;
        }
        gridBase.m_Path = path;
    }

    int GetDistance(MyNode n1, MyNode n2)
    {
        int distanceX = Mathf.Abs(n1.m_GridX - n2.m_GridX);
        int distanceY = Mathf.Abs(n1.m_GridY - n2.m_GridY);
        if (distanceX < distanceY)
        {
            return distanceX * 14 + (distanceY - distanceX) * 10;
        }
        else
        {
            return distanceY * 14 + (distanceX - distanceY) * 10;
        }
    }

}
