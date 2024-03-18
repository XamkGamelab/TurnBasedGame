using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class InputCharacter : CharacterBase
{
    [Header("Input Character")]
    public float InitConstMoveDelay = .5f;
    public float ConstMoveMultiplier = .5f;
    public float MinConstMoveDelay = .05f;
    public float MoveDelay = 0f;

    protected AStarPathFinding aStarRef;

    protected override void Awake()
    {
        ApplicationController.Instance.AstarUpdated.Subscribe(astar => aStarRef = astar);

        InputController.Instance.MoveInput.Subscribe(moveVec2 =>
        {
            MoveDelay = InitConstMoveDelay;
            StopAllCoroutines();
            StartCoroutine(Moving(moveVec2));
        });
    }

    private IEnumerator Moving(Vector2 moveInput)
    {
        while (true)
        {
            if (moveInput != Vector2.zero)
            {
                ProgressInternalTime();
                GameSettings.ProgressGlobalTimeBySpeed(Speed);
                if (!aStarRef.NonWalkablePositions.Contains((transform.position.x + moveInput.x, transform.position.z + moveInput.y)))
                {
                    //turn model to walking dir
                    Quaternion rotation = Quaternion.LookRotation(new Vector3(moveInput.x, 0, moveInput.y), Vector3.up);
                    CharacterModel.transform.rotation = rotation;
                    //animate walk
                    anim.SetTrigger("Walk");
                    transform.Translate(new Vector3(moveInput.x, 0, moveInput.y) * GameSettings.GridUnitSize);
                }
                else
                    Debug.Log("non walkable position");
            }

            
            MoveDelay = Mathf.Clamp(MoveDelay * ConstMoveMultiplier, MinConstMoveDelay, Mathf.Infinity);
            yield return new WaitForSeconds(MoveDelay);
            //animation back to idle
            anim.SetTrigger("Idle");
        }
    }
}
