using System;
using UnityEngine;

public enum PlayerStateType
{
    Idle,
    Moving,
    Attacking,
    Dodging,
    Healing,
    InAir
}

[Serializable]
public class PlayerStateInfo
{
    public PlayerStateType currentState = PlayerStateType.Idle;

    public bool CanMove => currentState != PlayerStateType.Attacking && currentState != PlayerStateType.Healing;

    public bool CanAttack => currentState == PlayerStateType.Idle || currentState == PlayerStateType.Moving || currentState == PlayerStateType.Attacking;

    public bool CanHeal => currentState == PlayerStateType.Idle;

    public bool CanDodge => currentState == PlayerStateType.Idle || currentState == PlayerStateType.Moving;
}

public class PlayerStateManager : MonoBehaviour
{
    public static PlayerStateManager Instance { get; private set; }

    [SerializeField] private PlayerStateInfo stateInfo = new PlayerStateInfo();

    public event Action<PlayerStateType> OnStateChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetState(PlayerStateType newState)
    {
        if (stateInfo.currentState != newState)
        {
            stateInfo.currentState = newState;
            OnStateChanged?.Invoke(newState);
        }
    }

    public PlayerStateInfo GetCurrentState() { return stateInfo; }
}
