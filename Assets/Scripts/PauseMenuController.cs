using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    private bool isPaused;

    private void Start()
    {
        Time.timeScale = 1f;

        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        isPaused = false;
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (!Keyboard.current.escapeKey.wasPressedThisFrame)
            return;

        if (settingsPanel.activeSelf)
        {
            CloseSettingsToPause();
            return;
        }

        if (isPaused)
            ResumeGame();
        else
            OpenPause();
    }

    public void OpenPause()
    {
        isPaused = true;

        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;

        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void OpenSettingsFromPause()
    {
        isPaused = true;

        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void CloseSettingsToPause()
    {
        isPaused = true;

        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}