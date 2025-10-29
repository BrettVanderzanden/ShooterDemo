using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerInput : MonoBehaviour
{
    public FrameInput FrameInput { get; private set; }
    public DebugFrameInput DebugFrameInput { get; private set; }
    private InputSystem_Actions _playerInputActions;
    private InputAction _move, _jump, _shoot, _dash, _tnt;
    private InputAction _reload;

    private bool _debugMode = false;

    private void Awake()
    {
        _debugMode = Application.isEditor || Debug.isDebugBuild;

        _playerInputActions = new InputSystem_Actions();

        _move = _playerInputActions.Player.Move;
        _jump = _playerInputActions.Player.Jump;
        _shoot = _playerInputActions.Player.Shoot;
        _dash = _playerInputActions.Player.Dash;
        _tnt = _playerInputActions.Player.TNT;

        _reload = _playerInputActions.Debug.Reload;
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

        if (_debugMode)
        {
            DebugFrameInput = GatherDebugInput();
        }
    }

    private FrameInput GatherInput()
    {
        return new FrameInput
        {
            Move = _move.ReadValue<Vector2>(),
            Jump = _jump.IsPressed(),
            Shoot = _shoot.triggered,
            Dash = _dash.triggered,
            TNT = _tnt.triggered,
        };
    }

    private DebugFrameInput GatherDebugInput()
    {
        return new DebugFrameInput
        {
            Reload = _reload.triggered
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

public struct DebugFrameInput
{
    public bool Reload;
}
