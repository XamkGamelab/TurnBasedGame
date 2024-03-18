using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Generic A* path finding algorithm
/// </summary>
public class AStarPathFinding
{
    public List<(float, float)> NonWalkablePositions = new List<(float, float)>();
    public List<(float, float)> PreviousPaths { get; private set; } = new List<(float, float)>();

    private Rect searchBounds;

    #region Public

    public void SetSearchBoundsRect(Rect searchBoundsRect)
    {
        searchBounds = searchBoundsRect;
    }

    /// <summary>
    /// Add many position to "non-walkable" list
    /// </summary>
    /// <param name="positions"></param>
    public void AddManyToNonWalkable(List<(float, float)> positions)
    {
        positions.ForEach(pos => NonWalkablePositions.Add(pos));
    }

    /// <summary>
    /// Remove single position from "non-walkable" list
    /// </summary>
    /// <param name="position"></param>
    public void RemoveFromNonWalkable((float, float) position)
    {
        if (NonWalkablePositions.Contains(position))
            NonWalkablePositions.Remove(position);
        else
            UnityEngine.Debug.LogWarning(position + " was not found from NonWalkablePositions");
    }

    /// <summary>
    /// Find path
    /// </summary>
    /// <param name="startPos">Start position</param>
    /// <param name="targetPos">Target position</param>
    /// <param name="canCargetToPreviousPaths">If true, can find target from all cached paths</param>    
    /// <param name="renderAllCheckedTiles">Whether to render all checked tiles or not</param>
    /// <returns></returns>
    public Stack<AStarNode> FindPath((float x, float y) startPos, (float x, float y) targetPos, bool canCargetToPreviousPaths)
    {
        AStarNode start = new AStarNode() { X = startPos.x, Y = startPos.y };
        AStarNode target = new AStarNode() { X = targetPos.x, Y = targetPos.y };
        //Set distance to target
        start.SetDistance(target.X, target.Y);

        List<AStarNode> activeTiles = new List<AStarNode>() { start };
        //activeTiles.Add(start);

        List<AStarNode> visitedTiles = new List<AStarNode>();

        //Stack for full path
        Stack<AStarNode> path = new Stack<AStarNode>();

        if (!start.Walkable || !target.Walkable)
        {
            Debug.LogError("Pathfinding issue, start & target should be walkable!");
            return null;
        }

        //Path finding
        while (activeTiles.Any())
        {
            var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();

            if (checkTile.X == target.X && checkTile.Y == target.Y || canCargetToPreviousPaths && PreviousPaths.Contains((checkTile.X, checkTile.Y)))
            {
                //Start with current checkTile
                AStarNode node = checkTile;

                //Rectrace back by looping through the parents of each tile to get the full path. Push all nodes to stack.                 
                while (node != null)
                {
                    if ((node.X != start.X || node.Y != start.Y) && (node.X != target.X || node.Y != target.Y))
                    {
                        //Add to previous paths (so we can do intersecting paths later)
                        PreviousPaths.Add((node.X, node.Y));
                        //Push to stack
                        path.Push(node);
                    }
                    node = node.Parent;
                }

                //Return path stack
                return path;
            }

            visitedTiles.Add(checkTile);
            activeTiles.Remove(checkTile);

            var walkableTiles = GetWalkableNodes(checkTile, target);

            foreach (var walkableTile in walkableTiles)
            {
                //We have already visited this tile so we don't need to do so again!
                if (visitedTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                    continue;

                //It's already in the active list, but that's OK, maybe this new tile has a better value (e.g. We might zigzag earlier but this is now straighter). 
                if (activeTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                {
                    var existingTile = activeTiles.First(x => x.X == walkableTile.X && x.Y == walkableTile.Y);
                    if (existingTile.CostDistance > checkTile.CostDistance)
                    {
                        activeTiles.Remove(existingTile);
                        activeTiles.Add(walkableTile);
                    }
                }
                else
                {
                    //We've never seen this tile before so add it to the list. 
                    activeTiles.Add(walkableTile);
                }
            }
        }

        return null;
    }

    #endregion

    #region Private
    private List<AStarNode> GetWalkableNodes(AStarNode currentTile, AStarNode targetTile)
    {
        var possibleNodes = new List<AStarNode>();

        AddPossibleNode(currentTile, possibleNodes, 0, -1);
        AddPossibleNode(currentTile, possibleNodes, 0, 1);
        AddPossibleNode(currentTile, possibleNodes, -1, 0);
        AddPossibleNode(currentTile, possibleNodes, 1, 0);

        possibleNodes.ForEach(node => node.SetDistance(targetTile.X, targetTile.Y));

        return possibleNodes.
            Where(node => node.X >= searchBounds.x && node.X <= searchBounds.width).
            Where(node => node.Y >= searchBounds.y && node.Y <= searchBounds.height).
            ToList();
    }

    private void AddPossibleNode(AStarNode currentNode, List<AStarNode> possibleNodes, int deltaX, int deltaY)
    {
        if (!NonWalkablePositions.Contains((currentNode.X + deltaX, currentNode.Y + deltaY)))
            possibleNodes.Add(new AStarNode { X = currentNode.X + deltaX, Y = currentNode.Y + deltaY, Parent = currentNode, Cost = currentNode.Cost + 1 });

    }
    #endregion
}
