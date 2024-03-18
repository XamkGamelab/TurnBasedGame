using UnityEngine;
using UniRx;
using System.Collections.Generic;
using Unity.VisualScripting;

public class NPCCharacter : CharacterBase
{
    protected InputCharacter playerCharacterRef;
    protected AStarPathFinding aStarRef;
    protected Stack<AStarNode> pathToPlayer;
    protected AStarNode previousPathSegment;
    protected override void Awake()
    {
        base.Awake();
        ApplicationController.Instance.PlayerCharacter.Subscribe(player => playerCharacterRef = player);
        ApplicationController.Instance.AstarUpdated.Subscribe(astar => aStarRef = astar);
    }
    protected override void PerformActions()
    {
        base.PerformActions();

        if (playerCharacterRef != null && aStarRef != null)
        {
            //NPC could only look for player at some interval instead of every time...
            pathToPlayer = aStarRef.FindPath(
            (transform.position.x, transform.position.z),
            (playerCharacterRef.transform.position.x, playerCharacterRef.transform.position.z),
            false);

            //Move NPC one step towards player
            if (pathToPlayer != null && pathToPlayer.Count > 0)
            {
                //get next path segment
                AStarNode pathSegment = pathToPlayer.Pop();                
                //turn model to correct direction
                if (previousPathSegment != null)
                {
                    Vector3 dir = new Vector3(pathSegment.X - previousPathSegment.X, 0, pathSegment.Y - previousPathSegment.Y);
                    Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
                    transform.rotation = rotation;                    
                }
                previousPathSegment = pathSegment;
                //move
                transform.position = new Vector3(pathSegment.X, 0, pathSegment.Y);
            }
        }
    }
}
