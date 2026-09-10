using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneController : MonoBehaviour
{
    [SerializeField] private LevelCatalog levelCatalog;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Button menuButton;
    [SerializeField] private string levelSelectSceneName = "LevelSelect";
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private string homeSceneName = "Home";

    private void Start()
    {
        menuButton.onClick.AddListener(() => pausePanel.SetActive(true));
        resumeButton.onClick.AddListener(() => pausePanel.SetActive(false));
        exitButton.onClick.AddListener(GoToLevelSelect);
        homeButton.onClick.AddListener(GoToHome);
        settingsButton.onClick.AddListener(() => settingsPanel.SetActive(true));

        gridManager.OnWin += OnLevelWon;
        gridManager.OnNextLevelClicked += OnNextLevelClicked;
        if (SessionState.SelectedLevel != null)
        {
            gridManager.LoadLevel(SessionState.SelectedLevel);
        }

        if (SessionState.ReopenPauseOnReturn)
        {
            SessionState.ReopenPauseOnReturn = false;
            pausePanel.SetActive(true);
        }
    }

    private void OnLevelWon()
    {
        SaveData save = SaveSystem.Load();
        if (!save.completedLevels.Contains(SessionState.SelectedLevelIndex))
        {
            save.completedLevels.Add(SessionState.SelectedLevelIndex);
        }
        SaveSystem.Save(save);

        bool hasNext = SessionState.SelectedLevelIndex + 1 < levelCatalog.levels.Length;
        gridManager.SetNextLevelButtonVisible(hasNext);
    }

    private void OnNextLevelClicked()
    {
        int nextIndex = SessionState.SelectedLevelIndex + 1;
        if (nextIndex >= levelCatalog.levels.Length) return;

        SessionState.SelectedLevelIndex = nextIndex;
        SessionState.SelectedLevel = levelCatalog.levels[nextIndex];
        gridManager.LoadLevel(SessionState.SelectedLevel);
    }

    public void GoToLevelSelect()
    {
        audioManager.PlayButtonClick();
        SessionState.ReturnSceneName = "Main";
        SessionState.ReopenPauseOnReturn = true;
        SceneManager.LoadScene(levelSelectSceneName);
    }

    public void GoToHome()
    {
        audioManager.PlayButtonClick();
        SceneManager.LoadScene(homeSceneName);
    }
}
