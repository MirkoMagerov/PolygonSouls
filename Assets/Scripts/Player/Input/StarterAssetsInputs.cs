using UnityEngine;
using UnityEngine.InputSystem;

public class StarterAssetsInputs : MonoBehaviour
{
	public delegate void LockOnEventHandler(bool isPressed);
	public static event LockOnEventHandler OnLockOnPressed;

	public delegate void InteractHandler();
	public static event InteractHandler OnInteractPerformed;

	public delegate void PauseHandler();
	public static event PauseHandler OnPausePressed;

	public delegate void DodgeHandler();
	public event DodgeHandler OnDodgePerformed;

	public delegate void AttackHandler();
	public event AttackHandler OnAttackPerformed;

	public delegate void BlockHandler(bool isBlocking);
	public event BlockHandler OnBlockStateChanged;

	public delegate void HealHandler();
	public event HealHandler OnHealPerformed;

	[Header("Character Input Values")]
	public Vector2 move;
	public Vector2 look;
	public bool sprint;
	public bool dodge;
	public bool lockOn;
	public bool primaryAttack;
	public bool isBlocking;
	public bool isHealing;
	public bool isInteracting;
	public bool isPaused;

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

	public void OnSprint(InputValue value)
	{
		SprintInput(value.isPressed);
	}

	public void OnDodge(InputValue value)
	{
		DodgeInput(value.isPressed);
		if (value.isPressed)
		{
			OnDodgePerformed?.Invoke();
		}
	}

	public void OnLockOn(InputValue value)
	{
		LockOnInput(value.isPressed);
		OnLockOnPressed?.Invoke(value.isPressed);
	}

	public void OnPrimaryAttack(InputValue value)
	{
		PrimaryAttackInput(value.isPressed);
		if (value.isPressed)
		{
			OnAttackPerformed?.Invoke();
		}
	}

	public void OnShieldBlock(InputValue value)
	{
		bool previousBlockState = isBlocking;
		ShieldBlockInput(value.isPressed);

		if (previousBlockState != isBlocking)
		{
			OnBlockStateChanged?.Invoke(isBlocking);
		}
	}

	public void OnHeal(InputValue value)
	{
		HealInput(value.isPressed);
		if (value.isPressed)
		{
			OnHealPerformed?.Invoke();
		}
	}

	public void OnInteract(InputValue value)
	{
		InteractInput(value.isPressed);
		if (value.isPressed)
		{
			OnInteractPerformed?.Invoke();
		}
	}

	public void OnPause(InputValue value)
	{
		PauseInput(value.isPressed);
		if (value.isPressed)
		{
			OnPausePressed?.Invoke();
		}
	}

	public void MoveInput(Vector2 newMoveDirection)
	{
		move = newMoveDirection;
	}

	public void LookInput(Vector2 newLookDirection)
	{
		look = newLookDirection;
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

	public void InteractInput(bool newInteractState)
	{
		isInteracting = newInteractState;
	}

	public void PauseInput(bool newInteractState)
	{
		isPaused = newInteractState;
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
