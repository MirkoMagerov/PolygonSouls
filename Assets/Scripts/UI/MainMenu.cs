using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI continueButtonText;
    [SerializeField] private EventTrigger leftTrigger;
    [SerializeField] private EventTrigger rightTrigger;
    [SerializeField] private Color disabledColor;
    [SerializeField] private Color enabledColor;

    void Start()
    {
        SetupContinueButton(SaveSystem.Instance.HasSaveFile());
    }

    public void ContinueGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameManager.Instance.LoadExistingGame();
    }

    public void StartNewGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameManager.Instance.CreateNewGame();
    }

    public void Settings()
    {
        settingsCanvas.SetActive(true);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    void SetupContinueButton(bool hasSaveFile)
    {
        if (hasSaveFile)
        {
            continueButton.interactable = true;
            continueButtonText.color = enabledColor;
            SetButtonMarkers(true);
        }
        else
        {
            continueButton.interactable = false;
            continueButtonText.color = disabledColor;
            SetButtonMarkers(false);
        }
    }

    void SetButtonMarkers(bool isEnabled)
    {
        leftTrigger.enabled = isEnabled;
        rightTrigger.enabled = isEnabled;
    }
}