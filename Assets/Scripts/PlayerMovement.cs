using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform feetTransform;
    [SerializeField] Vector2 groundCheck;
    [SerializeField] float movementSpeed = 20f;
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float jumpAnimDelay = 0.1f;

    Vector2 moveInput;
    Rigidbody2D myRigidBody;
    Animator bodyAnimator;
    CapsuleCollider2D myCapsuleCollider;
    float airtime = 0f;
    float airborneStartTime;
    bool isJumping = false;
    bool isFalling = false;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myCapsuleCollider = GetComponent<CapsuleCollider2D>();
        bodyAnimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        AirMove();
        Move();
        FlipSprite();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed && myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Collision")))
        {
            myRigidBody.linearVelocityY += jumpSpeed;
        }
    }

    void Move()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * movementSpeed, myRigidBody.linearVelocityY);
        myRigidBody.linearVelocity = playerVelocity;

        bool hasHorizontalSpeed = Mathf.Abs(myRigidBody.linearVelocityX) > Mathf.Epsilon;

        if (!isJumping && !isFalling && hasHorizontalSpeed)
        {
            bodyAnimator.SetBool("isRunning", true);
        }
        else
        {
            bodyAnimator.SetBool("isRunning", false);
        }
    }

    void AirMove()
    {
        if (myRigidBody.linearVelocityY > Mathf.Epsilon || myRigidBody.linearVelocityY < -Mathf.Epsilon)
        {
            airtime += Time.deltaTime;
        }
        else
        {
            airtime = 0f;
        }
        if (airtime > jumpAnimDelay)
        {
            isJumping = myRigidBody.linearVelocityY > Mathf.Epsilon;
            isFalling = myRigidBody.linearVelocityY < -Mathf.Epsilon;
        }
        else
        {
            isJumping = false;
            isFalling = false;
        }

        bodyAnimator.SetBool("isJumping", isJumping);
        bodyAnimator.SetBool("isFalling", isFalling);

    }

    bool CheckGrounded()
    {
        Collider2D isGrounded = Physics2D.OverlapBox(feetTransform.position, groundCheck, 0f, LayerMask.GetMask("Collision"));
        return isGrounded;
    }

    void FlipSprite()
    {
        bool hasHorizontalSpeed = Mathf.Abs(myRigidBody.linearVelocityX) > Mathf.Epsilon;
        if (hasHorizontalSpeed)
        {
            float xScale = Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.linearVelocityX) * xScale, transform.localScale.y);
        }
    }

}
