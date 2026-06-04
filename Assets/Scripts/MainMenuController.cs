using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuRoot;
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        Time.timeScale = 1f;

        mainMenuRoot.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void Play()
    {
        SceneManager.LoadScene("Prototype_Level_01");
    }

    public void OpenSettings()
    {
        mainMenuRoot.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsToMainMenu()
    {
        settingsPanel.SetActive(false);
        mainMenuRoot.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}