using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 20f;
    Vector2 moveInput;
    Rigidbody2D myRigidBody;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Move()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * movementSpeed, 0f);
        myRigidBody.linearVelocity = playerVelocity;
    }

}
