using UnityEngine;

/// <summary>
/// A single node in A* path finding
/// </summary>
public class AStarNode
{
    #region Public
    public float X { get; set; }
    public float Y { get; set; }
    public float Cost { get; set; }
    public float Distance { get; set; }
    public float CostDistance => Cost + Distance;    
    public bool Walkable { get; set; } = true;
    public AStarNode Parent { get; set; } = null;

    public AStarNode() { }
    public AStarNode(float x, float y)
    {
        
        X = x;
        Y = y;
        
    }

    //The distance is essentially the estimated distance, ignoring walls to our target. 
    //So how many tiles left and right, up and down, ignoring walls, to get there. 
    public void SetDistance(float targetX, float targetY)
    {
        Distance = Mathf.Abs(targetX - X) + Mathf.Abs(targetY - Y);
        //CostDistance = Cost + Distance;
    }

    #endregion
}
