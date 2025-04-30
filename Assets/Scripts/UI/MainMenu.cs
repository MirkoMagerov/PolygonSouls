using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("New Game");
    }

    public void ContinueGame()
    {
        Debug.Log("Continue game");
    }

    public void Settings()
    {
        Debug.Log("Settings");
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}