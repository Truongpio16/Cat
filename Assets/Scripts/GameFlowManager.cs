using System.Collections.Generic;
using Cat.Core;
using UnityEngine;
using UnityEngine.UI;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField] private LevelCatalog levelCatalog;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private LevelButtonView levelButtonPrefab;
    [SerializeField] private Button backButton;

    private LevelProgress _progress;
    private readonly List<LevelButtonView> _buttonViews = new List<LevelButtonView>();
    private int _currentLevelIndex;

    private void Start()
    {
        SaveData save = SaveSystem.Load();
        _progress = new LevelProgress(save.completedLevels);

        gridManager.OnWin += OnLevelWon;
        backButton.onClick.AddListener(ShowLevelSelect);

        BuildLevelButtons();
        ShowLevelSelect();
    }

    private void BuildLevelButtons()
    {
        for (int i = 0; i < levelCatalog.levels.Length; i++)
        {
            LevelButtonView view = Instantiate(levelButtonPrefab, buttonContainer);
            int index = i;
            view.OnClicked += () => SelectLevel(index);
            _buttonViews.Add(view);
        }

        RefreshButtons();
    }

    private void RefreshButtons()
    {
        for (int i = 0; i < _buttonViews.Count; i++)
        {
            _buttonViews[i].SetLevel(i + 1, _progress.IsUnlocked(i), _progress.IsCompleted(i));
        }
    }

    private void SelectLevel(int index)
    {
        if (!_progress.IsUnlocked(index)) return;

        _currentLevelIndex = index;
        levelSelectPanel.SetActive(false);
        gameplayPanel.SetActive(true);
        gridManager.LoadLevel(levelCatalog.levels[index]);
    }

    private void ShowLevelSelect()
    {
        gameplayPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        RefreshButtons();
    }

    private void OnLevelWon()
    {
        _progress.MarkCompleted(_currentLevelIndex);
        SaveSystem.Save(new SaveData { completedLevels = new List<int>(_progress.CompletedLevels) });
    }
}
