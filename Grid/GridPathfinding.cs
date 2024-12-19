using UnityEngine;
using ThiccTapeman.Grid;
using System;
using System.Collections.Generic;

namespace ThiccTapeman.Grid.Pathfinding
{
    public interface GridPathfindingObject
    {
        int gCost { get; set; }
        int hCost { get; set; }
        int fCost { get; set; }
        int xPos { get; set; }
        int yPos { get; set; }
        GridPathfindingObject cameFromNode { get; set; }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
    }

    public static class GridPathfinding
    {
        private const int MOVE_STRAIGHT_COST = 10;

        public static List<GridPathfindingObject> AStar(Grid grid, int startX, int startY, int endX, int endY, Func<Grid, int, int, bool> tileReachable)
        {
            GridPathfindingObject startObject = (GridPathfindingObject)grid.GetGridObject(startX, startY);
            GridPathfindingObject endObject = (GridPathfindingObject)grid.GetGridObject(endX, endY);

            List<GridPathfindingObject> openList = new List<GridPathfindingObject>() { startObject };
            List<GridPathfindingObject> closedList = new List<GridPathfindingObject>();

            for(int x = 0; x < grid.GetGridWidth(); x++)
            {
                for(int y = 0; y < grid.GetGridHeight(); y++)
                {
                    GridPathfindingObject obj = (GridPathfindingObject)grid.GetGridObject(x, y);
                    obj.gCost = int.MaxValue;
                    obj.CalculateFCost();
                    obj.cameFromNode = null;
                }
            }

            startObject.gCost = 0;
            startObject.hCost = CalculateDistanceCost(startObject, endObject);
            startObject.CalculateFCost();

            while(openList.Count > 0)
            {
                GridPathfindingObject currentNode = GetLowestFCostObject(openList);

                GridObjectAbstract obj = (GridObjectAbstract)currentNode;

                Debug.Log(obj.x);
                Debug.Log(obj.y);

                if (currentNode == endObject)
                {
                    return CalculatePath(endObject);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach(GridPathfindingObject neighbourObject in GetAdjecentGridObjects(grid, currentNode, tileReachable))
                {
                    if (closedList.Contains(neighbourObject)) continue;

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourObject);
                    if(tentativeGCost < neighbourObject.gCost)
                    {
                        neighbourObject.cameFromNode = currentNode;
                        neighbourObject.gCost = tentativeGCost;
                        neighbourObject.hCost = CalculateDistanceCost(neighbourObject, endObject);
                        neighbourObject.CalculateFCost();

                        if(!openList.Contains(neighbourObject))
                        {
                            openList.Add(neighbourObject);
                        }
                    }
                }
            }

            return null;
        }

        private static List<GridPathfindingObject> CalculatePath(GridPathfindingObject endObject)
        {
            List<GridPathfindingObject> path = new List<GridPathfindingObject>() { endObject };

            GridPathfindingObject currentObject = endObject;
            while(currentObject.cameFromNode != null)
            {
                path.Add(currentObject.cameFromNode);
                currentObject = currentObject.cameFromNode;
            }

            path.Reverse();

            return path;
        }

        private static GridPathfindingObject GetLowestFCostObject(List<GridPathfindingObject> gridObjectList)
        {
            GridPathfindingObject lowestFCostObject = gridObjectList[0];

            for(int i = 1; i < gridObjectList.Count; i++)
            {
                if (gridObjectList[i].fCost < lowestFCostObject.fCost)
                {
                    lowestFCostObject = gridObjectList[i];
                }
            }

            return lowestFCostObject;
        }

        private static int CalculateDistanceCost(GridPathfindingObject a, GridPathfindingObject b)
        {
            int xDistance = Mathf.Abs(a.xPos - b.xPos);
            int yDistance = Mathf.Abs(a.yPos - b.yPos);
            int remaining = Mathf.Abs(xDistance - yDistance);

            return MOVE_STRAIGHT_COST * remaining;
        }

        private static List<GridPathfindingObject> GetAdjecentGridObjects(Grid grid, GridPathfindingObject currentObject, Func<Grid, int, int, bool> tileReachable) 
        {
            List<GridPathfindingObject> list = new List<GridPathfindingObject>();

            if(tileReachable(grid, currentObject.xPos + 1, currentObject.yPos)) list.Add((GridPathfindingObject)grid.GetGridObject(currentObject.xPos + 1, currentObject.yPos));
            if(tileReachable(grid, currentObject.xPos - 1, currentObject.yPos)) list.Add((GridPathfindingObject)grid.GetGridObject(currentObject.xPos - 1, currentObject.yPos));
            if(tileReachable(grid, currentObject.xPos, currentObject.yPos + 1)) list.Add((GridPathfindingObject)grid.GetGridObject(currentObject.xPos, currentObject.yPos + 1));
            if(tileReachable(grid, currentObject.xPos, currentObject.yPos - 1)) list.Add((GridPathfindingObject)grid.GetGridObject(currentObject.xPos, currentObject.yPos - 1));

            return list;
        }
    }
}
