using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class JPSFindPath : MonoBehaviour
{
    public Transform start;
    public Transform end;
    private JPSGridBase m_GridBase;
    private List<JPSNode> openList = new List<JPSNode> ();
    private HashSet<JPSNode> closedList = new HashSet<JPSNode> ();

    private void Start()
    {
        m_GridBase = GetComponent<JPSGridBase>();
    }

    private void Update()
    {
        m_GridBase.CreateGrid();
        FindingPath();
    }

    
    void FindingPath()
    {
        JPSNode startNode = m_GridBase.GetFromPosition(start.position);
        JPSNode endNode = m_GridBase.GetFromPosition(end.position);

        openList.Clear();
        closedList.Clear();
        
        openList.Add(startNode);

        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();
        while (openList.Count > 0)
        {
            int fromDirX = 0;
            int fromDirY = 0;

            JPSNode currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentNode.FCost || openList[i].FCost == currentNode.FCost && openList[i].hCost < currentNode.hCost)
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);
            if (currentNode == endNode)
            {
                GeneratePath(startNode, endNode);
                //stopwatch.Stop();
                //UnityEngine.Debug.LogError("JPSѰ·���� " + stopwatch.ElapsedMilliseconds);
                return;
            }

            if (currentNode.parent != null)
            {
                fromDirX = Mathf.Clamp(currentNode.parent.m_GridX - currentNode.m_GridX, -1, 1);
                fromDirY = Mathf.Clamp(currentNode.parent.m_GridY - currentNode.m_GridY, -1, 1);
            }

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if ((x != 0 || y != 0) && (x != fromDirX || y != fromDirY))
                    {
                        //ֱ�߷���
                        if (x == 0 || y == 0)
                        {
                            JPSNode node = lineFind(x, y, currentNode);
                            if (node != null)
                            {
                                if (!closedList.Contains(node))
                                {
                                    int gCost = currentNode.gCost + GetDistance(currentNode, node);
                                    if (!openList.Contains(node))
                                    {
                                        node.parent = currentNode;
                                        node.hCost = GetDistance(node, endNode);
                                        node.gCost = gCost;
                                        openList.Add(node);
                                    }
                                    else
                                    {
                                        if (gCost < node.gCost)
                                        {
                                            node.parent = currentNode;
                                            node.gCost = gCost;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //������
                            JPSNode node = biasFind(x, y, currentNode);
                            if (node != null)
                            {
                                if (!closedList.Contains(node))
                                {
                                    int gCost = GetDistance(node, currentNode) + currentNode.gCost;
                                    if (!openList.Contains(node))
                                    {
                                        node.parent = currentNode;
                                        node.hCost = GetDistance(node, endNode);
                                        node.gCost = gCost;
                                        openList.Add(node);
                                    }
                                    else
                                    {
                                        if (gCost < node.gCost)
                                        {
                                            node.parent = currentNode;
                                            node.gCost = gCost;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    

    /*
    public void findPath2()
    {
        JPSNode startNode = m_GridBase.GetFromPosition(start.position);
        JPSNode endNode = m_GridBase.GetFromPosition(end.position);
        int startX = startNode.m_GridX;
        int startY = startNode.m_GridY;
        int endX = endNode.m_GridX;
        int endY = endNode.m_GridY;
        if (!m_GridBase.IsInBound(startX, startY) || !m_GridBase.IsInBound(endX, endY))
        {
            UnityEngine.Debug.LogError("���or�յ� ���ڸ�����");
            return;
        }
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        closedList.Clear();
        openList.Clear();

        startNode.parent = null;
        closedList.Add(startNode);

        JPSNode currentNode = startNode;
        while (!currentNode.Equals(endNode))
        {
            int fromDirX = 0;
            int fromDirY = 0;
            //���ķ���
            if (currentNode.parent != null)
            {
                fromDirX = clamp(currentNode.parent.m_GridX - currentNode.m_GridX, -1, 1);
                fromDirY = clamp(currentNode.parent.m_GridY - currentNode.m_GridX, -1, 1);
            }
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if ((x != 0 || y != 0) && (x != fromDirX || y != fromDirY))
                    {
                        if (x == 0 || y == 0)
                        {
                        //ֱ��
                            JPSNode node = lineFind(x, y, currentNode);
                            if (node != null)
                            {
                                if (!closedList.Contains(node) && node.m_canWalk)
                                {
                                    int gCost = getGCostInLine(node, currentNode) + currentNode.gCost;
                                    if (!openList.Contains(node))
                                    {
                                        node.parent = currentNode;
                                        node.hCost = getHCost(node, endNode);
                                        node.gCost = gCost;
                                        openList.Add(node);
                                    }
                                    else
                                    {
                                        if (gCost < node.gCost)
                                        {
                                            node.parent = currentNode;
                                            node.gCost = gCost;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //б��
                            JPSNode node = biasFind(x, y, currentNode);
                            if (node != null)
                            {
                                if (!closedList.Contains(node) && node.m_canWalk)
                                {
                                    int gCost = getGCostInBias(node, currentNode) + currentNode.gCost;
                                    if (!openList.Contains(node))
                                    {
                                        node.parent = currentNode;
                                        node.hCost = getHCost(node, endNode);
                                        node.gCost = gCost;
                                        openList.Add(node);
                                    }
                                    else
                                    {
                                        if (gCost < node.gCost)
                                        {
                                            node.parent = currentNode;
                                            node.gCost = gCost;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (openList.Count <= 0)
            {
                UnityEngine.Debug.LogError("JPSѰ·ʧ��");
                return;
            }

            //openSet��ȥ��СFcost
            var ier = openList.GetEnumerator();
            int minCost = int.MaxValue;
            JPSNode newCurCell = null;
            while (ier.MoveNext())
            {
                JPSNode cell = ier.Current;
                int f = cell.gCost + cell.hCost;
                if (f < minCost)
                {
                    minCost = f;
                    newCurCell = cell;
                }
            }
            openList.Remove(newCurCell);
            closedList.Add(newCurCell);
            currentNode = newCurCell;
        }

        Stack<JPSNode> path = new Stack<JPSNode>();
        JPSNode loopNode = endNode;
        while (loopNode != null)
        {
            path.Push(loopNode);
            loopNode = loopNode.parent;
        }
        stopwatch.Stop();
        m_GridBase.m_Path = path;
    }

    */

    private int clamp(int value, int min, int max)
    {
        if (value < min)
            value = min;
        else if (value > max)
            value = max;
        return value;
    }

    private bool GetIsCanCross(int x, int y, int dirX, int dirY)
    {
        int horX = x - dirX;
        int horY = y - dirY;
        //������б��
        return m_GridBase.IsInBound(x, y) && m_GridBase.m_Grid[x, y].m_canWalk && ((m_GridBase.IsInBound(horX, y) && m_GridBase.m_Grid[horX, y].m_canWalk) || (m_GridBase.IsInBound(x, horY) && m_GridBase.m_Grid[x, horY].m_canWalk));
        //������б��
        //return m_GridBase.IsInBound(x, y) && m_GridBase.m_Grid[x,y].m_canWalk && m_GridBase.IsInBound(horX, y) && m_GridBase.m_Grid[horX, y].m_canWalk && m_GridBase.IsInBound(x, horY) && m_GridBase.m_Grid[x, horY].m_canWalk;
    }

    //�Խ�����������
    private JPSNode biasFind(int dirX, int dirY, JPSNode currentNode)
    {
        int nextX = currentNode.m_GridX + dirX;
        int nextY = currentNode.m_GridY + dirY;
        JPSNode endNode = m_GridBase.GetFromPosition(end.position);

        if (!GetIsCanCross(nextX, nextY, dirX, dirY))
        {
            return null;
        }

        JPSNode nextNode = m_GridBase.m_Grid[nextX, nextY];

        if (!nextNode.m_canWalk)
        {
            return null;
        }
        if (nextNode.Equals(endNode))
        {
            return nextNode;
        }
        //�ж�next�Ƿ�Ϊ����
        JPSNode horNode = lineFind(dirX, 0, nextNode);
        if (horNode != null) return nextNode;
        JPSNode verNode = lineFind(0, dirY, nextNode);
        if (verNode != null) return nextNode;

        return biasFind(dirX, dirY, nextNode);
    }

    //ֱ����������
    private JPSNode lineFind(int dirX, int dirY, JPSNode currentNode)
    {
        int nextX = currentNode.m_GridX + dirX;
        int nextY = currentNode.m_GridY + dirY;
        JPSNode endNode = m_GridBase.GetFromPosition(end.position);
        if (!m_GridBase.IsInBound(nextX, nextY))
        {
            return null;
        }
        JPSNode nextNode = m_GridBase.m_Grid[nextX, nextY];
        if (!nextNode.m_canWalk)
        {
            return null;
        }
        //�ж�next�Ƿ�Ϊ����
        //1.next == end
        if (nextNode.Equals(endNode))
        {
            return nextNode;
        }
        //2.next��ǿ���ھ�
        if (dirX != 0)
        {
            //���µ��Ƿ�������
            int upY = nextY + 1;
            if (m_GridBase.IsInBound(nextX, upY))
            {
                JPSNode node = m_GridBase.m_Grid[nextX, upY];
                if (node.m_canWalk && !m_GridBase.m_Grid[nextX - dirX, upY].m_canWalk)
                {
                    return nextNode;
                }
            }
            int downY = nextY - 1;
            if (m_GridBase.IsInBound(nextX, downY))
            {
                JPSNode node = m_GridBase.m_Grid[nextX, downY];
                if (node.m_canWalk && !m_GridBase.m_Grid[nextX - dirX, downY].m_canWalk)
                {
                    return nextNode;
                }
            }
        }
        else
        {
            int rightX = nextX + 1;
            if (m_GridBase.IsInBound(rightX, nextY))
            {
                JPSNode node = m_GridBase.m_Grid[rightX, nextY];
                if (node.m_canWalk && !m_GridBase.m_Grid[rightX, nextY - dirY].m_canWalk)
                {
                    return nextNode;
                }
            }
            int leftX = nextX - 1;
            if (m_GridBase.IsInBound(leftX,nextY))
            {
                JPSNode node = m_GridBase.m_Grid[leftX, nextY];
                if (node.m_canWalk && !m_GridBase.m_Grid[leftX, nextY - dirY].m_canWalk)
                {
                    return nextNode;
                }
            }
        }
        return lineFind(dirX, dirY, nextNode);
    }

    void GeneratePath(JPSNode startNode, JPSNode endNode)
    {
        Stack<JPSNode> path = new Stack<JPSNode>();
        JPSNode node = endNode;
        while (node != startNode)
        {
            path.Push(node);
            node = node.parent;
        }
        if (node == startNode)
        {
            path.Push(node);
        }
        m_GridBase.m_Path = path;
    }

    private int getHCost(JPSNode curCell, JPSNode endCell)
    {
        int x = Mathf.Abs(curCell.m_GridX - endCell.m_GridX);
        int y = Mathf.Abs(curCell.m_GridY - endCell.m_GridY);
        return (x + y) * 10;
        //int min = Math.Min(x, y);
        //return min * 14 + Math.Abs(x - y) * 10;
    }

    private int getGCostInLine(JPSNode curCell, JPSNode endCell)
    {
        int x = Mathf.Abs(curCell.m_GridX - endCell.m_GridX);
        int y = Mathf.Abs(curCell.m_GridY - endCell.m_GridY);
        return (x + y) * 10;
    }
    private int getGCostInBias(JPSNode curCell, JPSNode endCell)
    {
        int x = Mathf.Abs(curCell.m_GridX - endCell.m_GridX);
        return x * 14;
    }

    int GetDistance(JPSNode n1, JPSNode n2)
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
