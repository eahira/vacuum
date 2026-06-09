using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryScreenUI : MonoBehaviour
{
    public static VictoryScreenUI Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;

    [Header("Rating")]
    [SerializeField] private float minStarsForCompletion = 1f;
    [SerializeField] private float maxStars = 3f;

    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("Scenes")]
    [SerializeField] private string menuSceneName = "MainMenu";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);

        if (menuButton != null)
            menuButton.onClick.AddListener(GoToMenu);
    }

    public void ShowVictory()
    {
        Time.timeScale = 0f;

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        if (titleText != null)
            titleText.text = "Уровень завершён!";

        if (messageText == null)
            return;

        if (CollectiblesManager.Instance == null)
        {
            messageText.text = "Молодец! Ты завершил уровень.";
            Debug.LogError("На сцене нет CollectiblesManager.");
            return;
        }

        int collectedClockParts = CollectiblesManager.Instance.CollectedClockParts;
        int totalClockParts = CollectiblesManager.Instance.TotalClockParts;

        int collectedHats = CollectiblesManager.Instance.CollectedCurrency;
        int totalHats = CollectiblesManager.Instance.TotalCurrency;

        float stars = CalculateStars(collectedHats, totalHats);

        messageText.text =
            "Ты собрал все части часов и восстановил ход времени.\n\n" +
            $"Части часов: {collectedClockParts}/{totalClockParts}\n" +
            $"Шапочки: {collectedHats}/{totalHats}\n\n" +
            $"Оценка прохождения: {stars:0.0} / {maxStars:0} звезды";
    }

    private float CalculateStars(int collectedHats, int totalHats)
    {
        if (totalHats <= 0)
            return maxStars;

        float hatsRatio = Mathf.Clamp01((float)collectedHats / totalHats);

        return minStarsForCompletion + (maxStars - minStarsForCompletion) * hatsRatio;
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private void GoToMenu()
    {
        Time.timeScale = 1f;

        if (string.IsNullOrWhiteSpace(menuSceneName))
        {
            Debug.LogWarning("Не указано имя сцены меню.");
            return;
        }

        SceneManager.LoadScene(menuSceneName);
    }
}