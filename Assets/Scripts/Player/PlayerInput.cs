using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerInput : MonoBehaviour
{
    public FrameInput FrameInput { get; private set; }
    private InputSystem_Actions _playerInputActions;
    private InputAction _move, _jump, _shoot, _dash, _tnt;

    private void Awake()
    {
        _playerInputActions = new InputSystem_Actions();

        _move = _playerInputActions.Player.Move;
        _jump = _playerInputActions.Player.Jump;
        _shoot = _playerInputActions.Player.Shoot;
        _dash = _playerInputActions.Player.Dash;
        _tnt = _playerInputActions.Player.TNT;
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
            Dash = _dash.triggered,
            TNT = _tnt.triggered,
        };
    }
}

public struct FrameInput
{
    public Vector2 Move;
    public bool Jump;
    public bool Shoot;
    public bool Dash;
    public bool TNT;
}
