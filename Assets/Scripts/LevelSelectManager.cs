using System.Collections.Generic;
using Cat.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    [SerializeField] private LevelCatalog levelCatalog;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private LevelButtonView levelButtonPrefab;
    [SerializeField] private Button backButton;
    [SerializeField] private string gameplaySceneName = "Main";

    private LevelProgress _progress;
    private readonly List<LevelButtonView> _buttonViews = new List<LevelButtonView>();

    private void Start()
    {
        SaveData save = SaveSystem.Load();
        _progress = new LevelProgress(save.completedLevels);

        BuildLevelButtons();
        backButton.onClick.AddListener(GoBack);
    }

    private void GoBack()
    {
        SceneManager.LoadScene(SessionState.ReturnSceneName);
    }

    private void BuildLevelButtons()
    {
        for (int i = 0; i < levelCatalog.levels.Length; i++)
        {
            LevelButtonView view = Instantiate(levelButtonPrefab, buttonContainer);
            int index = i;
            view.SetLevel(index + 1, _progress.IsUnlocked(index), _progress.IsCompleted(index));
            view.OnClicked += () => SelectLevel(index);
            _buttonViews.Add(view);
        }
    }

    private void SelectLevel(int index)
    {
        if (!_progress.IsUnlocked(index)) return;

        SessionState.SelectedLevel = levelCatalog.levels[index];
        SessionState.SelectedLevelIndex = index;
        SessionState.ReopenPauseOnReturn = false;
        SceneManager.LoadScene(gameplaySceneName);
    }
}
