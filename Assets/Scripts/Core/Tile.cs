using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType { Wall, Obstacle, Floor, Door }

    public TileType Type;
}
