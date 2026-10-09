using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
    private sealed class SearchCell
    {
        public readonly Vector3Int Coords;
        public readonly Cell Cell;
        public SearchCell Parent;
        public int GCost = int.MaxValue;
        public int HCost;
        public int FCost => GCost + HCost;

        public SearchCell(Vector3Int coords)
        {
            Coords = coords;
            Cell = WorldManager.World.GetCellAt(coords);
        }
    }

    public static void Pathfind(Vector3Int start, Vector3Int goal, out List<Vector3Int> path)
    {
        path = new();
        Dictionary<Vector3Int, SearchCell> searchCells = new();
        List<SearchCell> openSet = new();
        HashSet<SearchCell> closedSet = new();

        SearchCell startCell = GetSearchCell(start, searchCells);
        SearchCell goalCell = GetSearchCell(goal, searchCells);

        startCell.GCost = 0;
        startCell.HCost = GetHeuristic(startCell.Coords, goalCell.Coords);
        openSet.Add(startCell);
        while (openSet.Count > 0)
        {
            var current = openSet[0];
            foreach (var cell in openSet)
                if (cell.FCost < current.FCost || cell.FCost == current.FCost && cell.HCost < current.HCost)
                    current = cell;

            openSet.Remove(current);
            closedSet.Add(current);

            if (current == goalCell)
            {
                path = RetracePath(current, startCell);
                return;
            }

            foreach (var neighbourCell in current.Cell.GetNeighbours())
            {
                SearchCell neighbour = GetSearchCell(neighbourCell.Coords, searchCells);
                var delta = neighbour.Coords - current.Coords;
                if (!neighbour.Cell.canWalkOn || closedSet.Contains(neighbour))
                    continue;
                if(
                    !GetSearchCell(new Vector3Int(current.Coords.x + delta.x, current.Coords.y, 0), searchCells).Cell.canWalkOn ||
                    !GetSearchCell(new Vector3Int(current.Coords.x, current.Coords.y + delta.y, 0), searchCells).Cell.canWalkOn
                )
                    continue;

                int newGCost = current.GCost + GetHeuristic(current.Coords, neighbour.Coords);
                if (newGCost < neighbour.GCost || !openSet.Contains(neighbour))
                {
                    neighbour.GCost = newGCost;
                    neighbour.HCost = GetHeuristic(neighbour.Coords, goalCell.Coords);
                    neighbour.Parent = current;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    static SearchCell GetSearchCell(Vector3Int coords, Dictionary<Vector3Int, SearchCell> searchCells)
    {
        if (!searchCells.TryGetValue(coords, out SearchCell searchCell))
        {
            searchCell = new SearchCell(coords);
            searchCells.Add(coords, searchCell);
        }

        return searchCell;
    }

    static int GetHeuristic(Vector3Int a, Vector3Int b)
    {
        int deltaX = Mathf.Abs(a.x - b.x);
        int deltaY = Mathf.Abs(a.y - b.y);

        if(deltaX > deltaY)
            return deltaY * 14 + (deltaX - deltaY) * 10;
        else
            return deltaX * 14 + (deltaY - deltaX) * 10;
    }
    static List<Vector3Int> RetracePath(SearchCell goal, SearchCell start)
    {
        var path = new List<Vector3Int>();
        SearchCell current = goal;

        while (current != start)
        {
            path.Add(current.Coords);

            if (current.Parent == null)
                return new List<Vector3Int>();

            current = current.Parent;
        }

        path.Add(start.Coords);
        path.Reverse();
        return path;
    }
    public static void pathfindAdiacente(Vector3Int start, Vector3Int obb, out List<Vector3Int> path)
    {
        path = new List<Vector3Int>();
        Cell obbCell = WorldManager.World.GetCellAt(obb);
        if (obbCell == null) return;
        if (obbCell.canWalkOn)
        {
            Pathfind(start, obb, out path);
            return;
        }
        foreach(var i in obbCell.GetNeighbours())
        {
            if(!i.canWalkOn) continue;
            Pathfind(start, i.Coords, out List<Vector3Int> candidati);
            if (candidati.Count == 0) continue;
            if (path.Count == 0 || candidati.Count < path.Count) path = candidati;
        }    
    }
}
