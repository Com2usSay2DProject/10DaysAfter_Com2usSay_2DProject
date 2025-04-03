using CityBuilderCore;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;
public static class Pathfinding
{
    public static List<Vector3> FindPath(Vector3 startWorld, Vector3 targetWorld)
    {
        TileNode start = TileManager.Instance.GetNodeInfo(startWorld);
        TileNode target = TileManager.Instance.GetNodeInfo(targetWorld);

        //새로운거
        List<TileNode> openSet = new List<TileNode> { start };
        //탐색했던거
        HashSet<TileNode> closedSet = new HashSet<TileNode>();

        while (openSet.Count > 0)
        {
            TileNode current = openSet[0];
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

            foreach (TileNode neighbor in GetNeighbors(current))
            {
                if (neighbor .HasObstacle|| !neighbor.IsWalkable || closedSet.Contains(neighbor))
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

    private static List<Vector3> RetracePath(TileNode start, TileNode end)
    {
        List<Vector3> path = new List<Vector3>();
        TileNode current = end;

        while (current != start)
        {
            path.Add(TileManager.Instance.GetNodeInfo(current.X, current.Y).WorldPositon);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    private static int GetDistance(TileNode a, TileNode b)
    {
        return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);
    }

    public static List<TileNode> GetNeighbors(TileNode node)
    {
        List<TileNode> neighbors = new List<TileNode>();

        int[,] offsets = new int[,]
        {
            { 0, 1 },   // 상
            { 1, 0 },   // 우
            { 0, -1 },  // 하
            { -1, 0 },  // 좌
            { 1, 1 },   // 우상
            { 1, -1 },  // 우하
            { -1, 1 },  // 좌상
            { -1, -1 }  // 좌하
        };
        for (int i = 0; i < offsets.GetLength(0); i++)
        {
            int newX = node.X + offsets[i, 0];
            int newY = node.Y + offsets[i, 1];

            if (newX >= 0 && newX <  TileManager.Instance.GridArray.GetLength(0)&&
                newY >= 0 && newY < TileManager.Instance.GridArray.GetLength(1))
            {
                neighbors.Add(TileManager.Instance.GridArray[newX, newY]);
            }

        }

        return neighbors;
    }
}