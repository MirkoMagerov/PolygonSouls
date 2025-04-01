using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private void OnEnable()
    {
        InputActionsManager.Instance.GetPlayerControls().Attack.PrimaryAttack.performed += Attack;
        InputActionsManager.Instance.GetPlayerControls().Movement.Direction.canceled += Attack;
    }

    private void OnDisable()
    {
        InputActionsManager.Instance.GetPlayerControls().Movement.Direction.performed -= Attack;
        InputActionsManager.Instance.GetPlayerControls().Movement.Direction.canceled -= Attack;
    }

    private void Attack(InputAction.CallbackContext context)
    {
        Debug.Log("Attack");
    }
}
