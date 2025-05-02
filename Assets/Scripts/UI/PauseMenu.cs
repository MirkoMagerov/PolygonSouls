using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject settingsCanvas;

    void Start()
    {
        pauseCanvas.SetActive(false);
        StarterAssetsInputs.OnPausePressed += TogglePause;
    }

    void OnDestroy()
    {
        StarterAssetsInputs.OnPausePressed -= TogglePause;
    }

    public void TogglePause() { pauseCanvas.SetActive(!pauseCanvas.activeInHierarchy); }

    public void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSettings()
    {
        settingsCanvas.SetActive(true);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Save()
    {
        // Save game
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
