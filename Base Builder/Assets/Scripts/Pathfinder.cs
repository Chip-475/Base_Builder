using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Pathfinder
{
    public static void Pathfind(Cell start, Cell goal, out List<Cell> path)
    {
        path = new();
        List<Cell> openSet = new();
        HashSet<Cell> closedSet = new();
        Dictionary<Cell, Cell> cameFrom = new();

        openSet.Add(start);
        while (openSet.Count > 0)
        {
            var current = openSet[0];
            foreach (var cell in openSet)
                if (cell.F_Cost < current.F_Cost || cell.F_Cost == current.F_Cost && cell.hCost < current.hCost)
                    current = cell;

            openSet.Remove(current);
            closedSet.Add(current);

            if (current == goal)
            {
                path = RetracePath(cameFrom, start, goal);
                return;
            }

            foreach(var neighbour in current.GetNeighbours())
            {
                if (!neighbour.canWalkOn || closedSet.Contains(neighbour))
                    continue;

                int newGCost = current.gCost + GetHeuristic(current, neighbour);
                if(newGCost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newGCost;
                    neighbour.hCost = GetHeuristic(neighbour, goal);
                    cameFrom[neighbour] = current;

                    if(!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    static int GetHeuristic(Cell a, Cell b)
    {
        int deltaX = Mathf.Abs(a.Coords.x - b.Coords.x);
        int deltaY = Mathf.Abs(a.Coords.y - b.Coords.y);

        if(deltaX > deltaY)
            return deltaY * 14 + (deltaX - deltaY) * 10;
        else
            return deltaX * 14 + (deltaY - deltaX) * 10;
    }
    static List<Cell> RetracePath(Dictionary<Cell, Cell> cameFrom, Cell start, Cell goal)
    {
        var path = new List<Cell>();
        Cell current = goal;

        while (current != start)
        {
            path.Add(current);

            if (!cameFrom.TryGetValue(current, out Cell parent))
                return new List<Cell>();

            current = parent;
        }

        path.Add(start);
        path.Reverse();
        return path;
    }
}
