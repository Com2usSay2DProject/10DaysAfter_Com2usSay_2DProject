using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public static class Pathfinding
{
    public static List<Vector3> FindPath(Vector3 startWorld, Vector3 targetWorld)
    {
        TestNode start = TestGridManager.Instance.GetNodeFromWorldPos(startWorld);
        TestNode target = TestGridManager.Instance.GetNodeFromWorldPos(targetWorld);

        List<TestNode> openSet = new List<TestNode> { start };
        HashSet<TestNode> closedSet = new HashSet<TestNode>();

        while (openSet.Count > 0)
        {
            TestNode current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < current.fCost ||
                   (openSet[i].fCost == current.fCost && openSet[i].hCost < current.hCost))
                {
                    current = openSet[i];
                }
            }

            openSet.Remove(current);
            closedSet.Add(current);

            if (current == target)
                return RetracePath(start, target);

            foreach (TestNode neighbor in TestGridManager.Instance.GetNeighbors(current))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                    continue;

                int newCost = current.gCost + GetDistance(current, neighbor);
                if (newCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCost;
                    neighbor.hCost = GetDistance(neighbor, target);
                    neighbor.parent = current;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }

    private static List<Vector3> RetracePath(TestNode start, TestNode end)
    {
        List<Vector3> path = new List<Vector3>();
        TestNode current = end;

        while (current != start)
        {
            path.Add(TestGridManager.Instance.GetWorldPosition(current.x, current.y));
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    private static int GetDistance(TestNode a, TestNode b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}