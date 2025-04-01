using UnityEngine;

public class InputActionsManager : MonoBehaviour
{
    public static InputActionsManager Instance;
    private PlayerControls playerControls;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            playerControls = new PlayerControls();
            playerControls.Enable();
        }
        else
        {
            Destroy(this);
        }
    }

    public PlayerControls GetPlayerControls() => playerControls;
}
