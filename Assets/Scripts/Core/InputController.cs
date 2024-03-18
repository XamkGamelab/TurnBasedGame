using UnityEngine;
using UniRx;

public class InputController : SingletonMono<InputController>
{
    public ReactiveProperty<Vector2> MoveInput = new ReactiveProperty<Vector2>();

    private GameInputAsset inputAsset;
    public void Init()
    {
        inputAsset = new GameInputAsset();
        inputAsset.Enable();

        inputAsset.Player.Move.performed += move => MoveInput.Value = move.ReadValue<Vector2>();
        inputAsset.Player.Move.canceled += move => MoveInput.Value = Vector2.zero;
    }
}
