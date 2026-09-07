using Cat.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private LevelCatalog levelCatalog;
    [SerializeField] private Button playButton;
    [SerializeField] private TMP_Text playButtonLabel;
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button levelSelectButton;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private string gameplaySceneName = "Main";
    [SerializeField] private string levelSelectSceneName = "LevelSelect";

    private int _currentLevelIndex;

    private void Start()
    {
        RefreshCurrentLevel();

        playButton.onClick.AddListener(PlayCurrentLevel);
        settingsButton.onClick.AddListener(() =>
        {
            audioManager.PlayButtonClick();
            settingsPanel.SetActive(true);
        });
        levelSelectButton.onClick.AddListener(GoToLevelSelect);
    }

    private void RefreshCurrentLevel()
    {
        SaveData save = SaveSystem.Load();
        LevelProgress progress = new LevelProgress(save.completedLevels);

        _currentLevelIndex = 0;
        while (_currentLevelIndex < levelCatalog.levels.Length - 1 && progress.IsCompleted(_currentLevelIndex))
        {
            _currentLevelIndex++;
        }

        playButtonLabel.text = $"Cấp {_currentLevelIndex + 1}";
    }

    private void GoToLevelSelect()
    {
        audioManager.PlayButtonClick();
        SessionState.ReturnSceneName = "Home";
        SessionState.ReopenPauseOnReturn = false;
        SceneManager.LoadScene(levelSelectSceneName);
    }

    private void PlayCurrentLevel()
    {
        audioManager.PlayButtonClick();
        SessionState.SelectedLevel = levelCatalog.levels[_currentLevelIndex];
        SessionState.SelectedLevelIndex = _currentLevelIndex;
        SceneManager.LoadScene(gameplaySceneName);
    }
}
