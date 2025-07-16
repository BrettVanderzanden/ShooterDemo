using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private Transform _bodySpriteTransform;
    [SerializeField] private Transform _headSpriteTransform;

    Animator _bodyAnimator;

    private bool _isFalling = false;
    private bool _isJumping = false;
    private bool _hasHorizontalSpeed = false;

    private void Awake()
    {
        _bodyAnimator = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        PlayerHealth.OnDeath += Death;
    }

    void OnDisable()
    {
        PlayerHealth.OnDeath -= Death;
    }

    private void Update()
    {
        AirMove();
        Move();
    }

    private void Move()
    {
        _hasHorizontalSpeed = PlayerController.Instance.MoveInput.x != 0;

        if (!_isJumping && !_isFalling && _hasHorizontalSpeed)
        {
            _bodyAnimator.SetBool("isRunning", true);
        }
        else
        {
            _bodyAnimator.SetBool("isRunning", false);
        }
    }

    private void AirMove()
    {
        _isJumping = !PlayerController.Instance.CheckGrounded();
        _bodyAnimator.SetBool("isJumping", _isJumping);
        _bodyAnimator.SetBool("isFalling", _isFalling);
    }

    private void Death(PlayerHealth health)
    {
        // TEMP solution - add death anim here
        var sprites = gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sprite in sprites)
        {
            sprite.gameObject.SetActive(false);
        }
    }

}
