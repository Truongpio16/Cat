using System;
using System.Collections;
using System.Collections.Generic;
using Cat.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [SerializeField] private LevelData debugLevel;
    [SerializeField] private CellView cellPrefab;
    [SerializeField] private RectTransform gridContainer;
    [SerializeField] private RectTransform gridFrame;
    [SerializeField] private float gridFramePadding = 28f;
    [SerializeField] private float cellSize = 110f;
    [SerializeField] private float cellGap = 10f;
    [SerializeField] private float safeAreaLeftX = -480f;
    [SerializeField] private float safeAreaRightX = 480f;
    [SerializeField] private float safeAreaTopY = 300f;
    [SerializeField] private float safeAreaBottomY = -420f;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text winText;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button hintButton;
    [SerializeField] private float hintDuration = 1.2f;
    [SerializeField] private float staggerPerDistance = 0.02f;
    [SerializeField] private float winPulseStagger = 0.03f;
    [SerializeField] private float winTextPopDuration = 0.25f;
    [SerializeField] private float winTextOvershoot = 1.15f;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private HapticManager hapticManager;

    public event Action OnWin;
    public event Action OnNextLevelClicked;

    private GridData _gridData;
    private CellView[,] _cellViews;
    private Coroutine _refreshRoutine;
    private bool _hasWon;

    private void Awake()
    {
        hintButton.onClick.AddListener(OnHintButtonClicked);
        nextLevelButton.onClick.AddListener(() => OnNextLevelClicked?.Invoke());
    }

    public void SetNextLevelButtonVisible(bool visible)
    {
        nextLevelButton.gameObject.SetActive(visible);
    }

    private void Start()
    {
        if (debugLevel != null)
        {
            LoadLevel(debugLevel);
        }
    }

    public void LoadLevel(LevelData level)
    {
        StopAllCoroutines();
        _refreshRoutine = null;
        _hasWon = false;

        winPanel.SetActive(false);
        winText.rectTransform.localScale = Vector3.one;
        nextLevelButton.gameObject.SetActive(true);

        ClearGrid();

        _gridData = new GridData(level.width, level.height);

        foreach (LevelData.WallEntry wall in level.walls)
        {
            CellData cell = _gridData.GetCell(wall.x, wall.y);
            cell.Type = wall.hasNumber ? CellType.WallNumbered : CellType.Wall;
            cell.WallNumber = wall.hasNumber ? wall.number : -1;
        }

        BuildGrid();
    }

    private void ClearGrid()
    {
        if (_cellViews == null) return;

        foreach (CellView view in _cellViews)
        {
            if (view != null)
            {
                Destroy(view.gameObject);
            }
        }
    }

    private void BuildGrid()
    {
        _cellViews = new CellView[_gridData.Width, _gridData.Height];

        float availableWidth = safeAreaRightX - safeAreaLeftX;
        float availableHeight = safeAreaTopY - safeAreaBottomY;
        float activeCellSize = Mathf.Min(
            cellSize,
            availableWidth / _gridData.Width,
            availableHeight / _gridData.Height);

        float gridPixelWidth = _gridData.Width * activeCellSize;
        float gridPixelHeight = _gridData.Height * activeCellSize;
        float gridLeft = safeAreaLeftX + (availableWidth - gridPixelWidth) / 2f;
        float gridTop = safeAreaTopY - (availableHeight - gridPixelHeight) / 2f;

        gridContainer.anchoredPosition = new Vector2(
            gridLeft + activeCellSize / 2f,
            gridTop - activeCellSize / 2f);

        if (gridFrame != null)
        {
            gridFrame.anchoredPosition = new Vector2(
                (_gridData.Width - 1) * activeCellSize / 2f,
                -(_gridData.Height - 1) * activeCellSize / 2f);
            gridFrame.sizeDelta = new Vector2(
                gridPixelWidth + gridFramePadding * 2f,
                gridPixelHeight + gridFramePadding * 2f);
        }

        float cellVisualSize = activeCellSize - cellGap;

        for (int x = 0; x < _gridData.Width; x++)
        {
            for (int y = 0; y < _gridData.Height; y++)
            {
                CellView view = Instantiate(cellPrefab, gridContainer);
                RectTransform rt = view.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(x * activeCellSize, -y * activeCellSize);
                rt.sizeDelta = new Vector2(cellVisualSize, cellVisualSize);

                CellData data = _gridData.GetCell(x, y);
                if (data.Type == CellType.Empty)
                {
                    view.SetEmpty();
                }
                else
                {
                    view.SetWall(data.Type == CellType.WallNumbered, data.WallNumber);
                }

                int cx = x;
                int cy = y;
                view.OnClicked += () => OnCellClicked(cx, cy);

                _cellViews[x, y] = view;
            }
        }
    }

    private void OnCellClicked(int x, int y)
    {
        if (_hasWon) return;

        CellData cell = _gridData.GetCell(x, y);
        bool isActionable = cell.Type == CellType.Empty;

        _gridData.ToggleBulb(x, y);
        _gridData.RecalculateLight();

        if (isActionable)
        {
            if (cell.HasBulb)
            {
                audioManager.PlayPlaceCat();
                hapticManager.PlaceFeedback();
                if (cell.HasConflict)
                {
                    audioManager.PlayConflict();
                    hapticManager.ConflictFeedback();
                }
            }
            else
            {
                audioManager.PlayRemoveCat();
            }
        }

        if (_refreshRoutine != null)
        {
            StopCoroutine(_refreshRoutine);
        }
        _refreshRoutine = StartCoroutine(RefreshViewsStaggered(x, y));

        if (_gridData.CheckWin())
        {
            ShowWin();
        }
    }

    private IEnumerator RefreshViewsStaggered(int originX, int originY)
    {
        List<(int x, int y, int distance)> cells = new List<(int, int, int)>();

        for (int x = 0; x < _gridData.Width; x++)
        {
            for (int y = 0; y < _gridData.Height; y++)
            {
                if (_gridData.GetCell(x, y).Type != CellType.Empty) continue;

                int distance = Mathf.Abs(x - originX) + Mathf.Abs(y - originY);
                cells.Add((x, y, distance));
            }
        }

        cells.Sort((a, b) => a.distance.CompareTo(b.distance));

        int currentDistance = -1;
        foreach ((int x, int y, int distance) cell in cells)
        {
            if (cell.distance != currentDistance)
            {
                if (currentDistance >= 0)
                {
                    yield return new WaitForSeconds(staggerPerDistance);
                }
                currentDistance = cell.distance;
            }

            RefreshCell(cell.x, cell.y);
        }

        _refreshRoutine = null;
    }

    private void RefreshViews()
    {
        for (int x = 0; x < _gridData.Width; x++)
        {
            for (int y = 0; y < _gridData.Height; y++)
            {
                if (_gridData.GetCell(x, y).Type != CellType.Empty) continue;
                RefreshCell(x, y);
            }
        }
    }

    private void RefreshCell(int x, int y)
    {
        CellData data = _gridData.GetCell(x, y);
        _cellViews[x, y].SetBulb(data.HasBulb);
        _cellViews[x, y].SetLit(data.IsLit);
        _cellViews[x, y].SetConflict(data.HasConflict);
    }

    private void ShowWin()
    {
        _hasWon = true;
        OnWin?.Invoke();
        StartCoroutine(WinSequence());
    }

    private IEnumerator WinSequence()
    {
        audioManager.PlayWin();
        hapticManager.WinFeedback();
        yield return StartCoroutine(RippleBoard());

        winPanel.SetActive(true);
        yield return StartCoroutine(PopWinText());
    }

    private IEnumerator RippleBoard()
    {
        int centerX = _gridData.Width / 2;
        int centerY = _gridData.Height / 2;

        List<(int x, int y, int distance)> cells = new List<(int, int, int)>();

        for (int x = 0; x < _gridData.Width; x++)
        {
            for (int y = 0; y < _gridData.Height; y++)
            {
                if (_gridData.GetCell(x, y).Type != CellType.Empty) continue;

                int distance = Mathf.Abs(x - centerX) + Mathf.Abs(y - centerY);
                cells.Add((x, y, distance));
            }
        }

        cells.Sort((a, b) => a.distance.CompareTo(b.distance));

        int currentDistance = -1;
        foreach ((int x, int y, int distance) cell in cells)
        {
            if (cell.distance != currentDistance)
            {
                if (currentDistance >= 0)
                {
                    yield return new WaitForSeconds(winPulseStagger);
                }
                currentDistance = cell.distance;
            }

            _cellViews[cell.x, cell.y].PlayWinPulse();
        }
    }

    private IEnumerator PopWinText()
    {
        RectTransform rt = winText.rectTransform;
        rt.localScale = Vector3.zero;
        float half = winTextPopDuration * 0.5f;

        float elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / half);
            rt.localScale = Vector3.one * Mathf.Lerp(0f, winTextOvershoot, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / half);
            rt.localScale = Vector3.one * Mathf.Lerp(winTextOvershoot, 1f, t);
            yield return null;
        }

        rt.localScale = Vector3.one;
    }

    public void OnHintButtonClicked()
    {
        if (_hasWon) return;

        audioManager.PlayButtonClick();
        hapticManager.LightTap();

        HintMove hint = _gridData.FindHint();
        if (hint == null)
        {
            Debug.Log("Khong tim thay goi y ro rang luc nay.");
            return;
        }

        Debug.Log(hint.Reason);
        audioManager.PlayHint();
        StartCoroutine(FlashHint(hint.X, hint.Y, hint.Type));
    }

    private IEnumerator FlashHint(int x, int y, HintType type)
    {
        _cellViews[x, y].SetHint(type);
        yield return new WaitForSeconds(hintDuration);
        RefreshViews();
    }
}
