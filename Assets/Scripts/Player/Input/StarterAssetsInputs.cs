using UnityEngine;
using UnityEngine.InputSystem;

public class StarterAssetsInputs : MonoBehaviour
{
	public delegate void LockOnEventHandler(bool isPressed);
	public static event LockOnEventHandler OnLockOnPressed;

	[Header("Character Input Values")]
	public Vector2 move;
	public Vector2 look;
	public bool jump;
	public bool sprint;
	public bool dodge;
	public bool lockOn;
	public bool primaryAttack;
	public bool isBlocking;
	public bool isHealing;

	[Header("Movement Settings")]
	public bool analogMovement;

	[Header("Mouse Cursor Settings")]
	public bool cursorLocked = true;
	public bool cursorInputForLook = true;

	public void OnMove(InputValue value)
	{
		MoveInput(value.Get<Vector2>());
	}

	public void OnLook(InputValue value)
	{
		if (cursorInputForLook)
		{
			LookInput(value.Get<Vector2>());
		}
	}

	public void OnJump(InputValue value)
	{
		JumpInput(value.isPressed);
	}

	public void OnSprint(InputValue value)
	{
		SprintInput(value.isPressed);
	}

	public void OnDodge(InputValue value)
	{
		DodgeInput(value.isPressed);
	}

	public void OnLockOn(InputValue value)
	{
		LockOnInput(value.isPressed);
		OnLockOnPressed?.Invoke(value.isPressed);
	}

	public void OnPrimaryAttack(InputValue value)
	{
		PrimaryAttackInput(value.isPressed);
	}

	public void OnShieldBlock(InputValue value)
	{
		ShieldBlockInput(value.isPressed);
	}

	public void OnHeal(InputValue value)
	{
		HealInput(value.isPressed);
	}

	public void MoveInput(Vector2 newMoveDirection)
	{
		move = newMoveDirection;
	}

	public void LookInput(Vector2 newLookDirection)
	{
		look = newLookDirection;
	}

	public void JumpInput(bool newJumpState)
	{
		jump = newJumpState;
	}

	public void SprintInput(bool newSprintState)
	{
		sprint = newSprintState;
	}

	public void DodgeInput(bool newDodgeState)
	{
		dodge = newDodgeState;
	}

	public void LockOnInput(bool newLockOnState)
	{
		lockOn = newLockOnState;
	}

	public void PrimaryAttackInput(bool newPrimaryAttackState)
	{
		primaryAttack = newPrimaryAttackState;
	}

	public void ShieldBlockInput(bool newShieldBlockState)
	{
		isBlocking = newShieldBlockState;
	}

	public void HealInput(bool newHealState)
	{
		isHealing = newHealState;
	}

	void OnApplicationFocus(bool hasFocus)
	{
		SetCursorState(cursorLocked);
	}

	private void SetCursorState(bool newState)
	{
		Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
	}
}
