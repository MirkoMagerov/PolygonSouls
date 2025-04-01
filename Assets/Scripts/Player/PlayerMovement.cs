using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 5f;

    private Vector2 movementInput;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        InputActionsManager.Instance.GetPlayerControls().Movement.Direction.performed += OnMoveReadValues;
        InputActionsManager.Instance.GetPlayerControls().Movement.Direction.canceled += OnMoveReadValues;
    }

    private void OnDisable()
    {
        InputActionsManager.Instance.GetPlayerControls().Movement.Direction.performed -= OnMoveReadValues;
        InputActionsManager.Instance.GetPlayerControls().Movement.Direction.canceled -= OnMoveReadValues;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnMoveReadValues(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }

    private void Move()
    {
        Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y);

        if (movement.magnitude > 1f) movement.Normalize();

        rb.velocity = new Vector3(movement.x * movementSpeed, rb.velocity.y, movement.z * movementSpeed);
    }
}
