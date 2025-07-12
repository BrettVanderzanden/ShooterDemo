using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public FrameInput FrameInput { get; private set; }
    private InputSystem_Actions _playerInputActions;
    private InputAction _move, _jump, _shoot;

    private void Awake()
    {
        _playerInputActions = new InputSystem_Actions();

        _move = _playerInputActions.Player.Move;
        _jump = _playerInputActions.Player.Jump;
        _shoot = _playerInputActions.Player.Shoot;
    }

    private void OnEnable()
    {
        _playerInputActions.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
    }

    private void Update()
    {
        FrameInput = GatherInput();
    }

    private FrameInput GatherInput()
    {
        return new FrameInput
        {
            Move = _move.ReadValue<Vector2>(),
            Jump = _jump.triggered,
            Shoot = _shoot.IsPressed(),
        };
    }
}

public struct FrameInput
{
    public Vector2 Move;
    public bool Jump;
    public bool Shoot;
}
