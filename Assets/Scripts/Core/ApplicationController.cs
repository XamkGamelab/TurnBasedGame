using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UniRx;

/// <summary>
/// Instantiate and initialize necessary objects.
/// </summary>
public class ApplicationController : SingletonMono<ApplicationController>
{
    public ReactiveProperty<InputCharacter> PlayerCharacter = new ReactiveProperty<InputCharacter>(null);

    public ReactiveProperty<AStarPathFinding> AstarUpdated = new ReactiveProperty<AStarPathFinding>(null);
    public AStarPathFinding AStar { get; private set; } = new AStarPathFinding();

    [HideInInspector]
    public CameraController CamController;

    private List<Tile> currentSceneTiles = new List<Tile>();


    [RuntimeInitializeOnLoadMethod]
    static void OnInit()
    {
        Instance.Init();
    }

    #region public
    public void Init()
    {
        InputController.Instance.Init();

        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        SceneManager.sceneLoaded += OnSceneLoaded;

        
    }
    #endregion
    #region private

    private void InstantiatePlayer()
    {
        PlayerSpawnPoint playerSpawnPoint = FindObjectOfType<PlayerSpawnPoint>();
        if (playerSpawnPoint != null)
        {
            PlayerCharacter.Value = Instantiate<InputCharacter>(Resources.Load<InputCharacter>("Player"));
            PlayerCharacter.Value.transform.position = playerSpawnPoint.transform.position;
        }
        else
            Debug.LogError("Scene is missing PlayerSpawnPoint!");
    }
    private void InitializeAStarForLoadedScene()
    {
        currentSceneTiles = FindObjectsOfType<Tile>(true).ToList();

        Tile minTile = currentSceneTiles.Aggregate((minItem, nextItem) =>
        minItem.transform.position.x + minItem.transform.position.z < nextItem.transform.position.x + nextItem.transform.position.z ? minItem : nextItem);

        Tile maxTile = currentSceneTiles.Aggregate((maxItem, nextItem) =>
        maxItem.transform.position.x + maxItem.transform.position.z > nextItem.transform.position.x + nextItem.transform.position.z ? maxItem : nextItem);

        Debug.Log("Min tile: " + minTile.name + " pos: " + minTile.transform.position);
        Debug.Log("Max tile: " + maxTile.name + " pos: " + maxTile.transform.position);

        Rect searchBoundsRect = new Rect(
            minTile.transform.position.x,
            minTile.transform.position.z,
            maxTile.transform.position.x - minTile.transform.position.x,
            maxTile.transform.position.z - minTile.transform.position.z
            );

        List<Tile> wallTiles = currentSceneTiles.Where(tile => tile.Type == Tile.TileType.Wall || tile.Type == Tile.TileType.Obstacle).ToList();        
        List<(float, float)> wallPositions = new List<(float, float)>();
        wallTiles.ForEach(wallTile => wallPositions.Add((wallTile.transform.position.x, wallTile.transform.position.z)));

        AStar.AddManyToNonWalkable(wallPositions);
        AStar.SetSearchBoundsRect(searchBoundsRect);

        //Notify that AStar has been updated and can now be used by NPCs
        AstarUpdated.Value = AStar;
    }

    #endregion
    #region Unity
    private void OnSceneLoaded(Scene loadedScene, LoadSceneMode arg1)
    {
        Debug.Log("Scene loaded: " + loadedScene.name);
        InitializeAStarForLoadedScene();
        InstantiatePlayer();
        //TODO: cam should be instantiated, not in scene
        CamController = Camera.main.GetComponentInParent<CameraController>();
        CamController.Target = PlayerCharacter.Value.transform;
    }
    #endregion
}