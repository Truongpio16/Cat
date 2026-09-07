using System.Collections.Generic;

namespace Cat.Core
{
    public class GridData
    {
        public readonly int Width;
        public readonly int Height;
        private readonly CellData[,] _cells;

        public GridData(int width, int height)
        {
            Width = width;
            Height = height;
            _cells = new CellData[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _cells[x, y] = new CellData();
                }
            }
        }

        public CellData GetCell(int x, int y)
        {
            return _cells[x, y];
        }

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public void ToggleBulb(int x, int y)
        {
            CellData cell = GetCell(x, y);
            if (cell.Type != CellType.Empty) return;
            cell.HasBulb = !cell.HasBulb;
        }

        public void RecalculateLight()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    _cells[x, y].IsLit = false;
                    _cells[x, y].HasConflict = false;
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (!_cells[x, y].HasBulb) continue;

                    _cells[x, y].IsLit = true;
                    LightDirection(x, y, 1, 0);
                    LightDirection(x, y, -1, 0);
                    LightDirection(x, y, 0, 1);
                    LightDirection(x, y, 0, -1);
                }
            }
        }

        private void LightDirection(int startX, int startY, int dx, int dy)
        {
            int x = startX + dx;
            int y = startY + dy;

            while (IsInBounds(x, y) && _cells[x, y].Type == CellType.Empty)
            {
                _cells[x, y].IsLit = true;

                if (_cells[x, y].HasBulb)
                {
                    _cells[x, y].HasConflict = true;
                    _cells[startX, startY].HasConflict = true;
                }

                x += dx;
                y += dy;
            }
        }

        public bool CheckWin()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    CellData cell = _cells[x, y];

                    if (cell.HasConflict) return false;

                    if (cell.Type == CellType.Empty && !cell.IsLit) return false;

                    if (cell.Type == CellType.WallNumbered && CountAdjacentBulbs(x, y) != cell.WallNumber)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private int CountAdjacentBulbs(int x, int y)
        {
            int count = 0;
            count += BulbAt(x + 1, y);
            count += BulbAt(x - 1, y);
            count += BulbAt(x, y + 1);
            count += BulbAt(x, y - 1);
            return count;
        }

        private int BulbAt(int x, int y)
        {
            if (!IsInBounds(x, y)) return 0;
            return _cells[x, y].HasBulb ? 1 : 0;
        }

        public HintMove FindHint()
        {
            HintMove isolatedHint = FindIsolatedCell();
            if (isolatedHint != null) return isolatedHint;

            HintMove placeHint = FindForcedPlacement();
            if (placeHint != null) return placeHint;

            return FindForcedAvoid();
        }

        private HintMove FindIsolatedCell()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (_cells[x, y].Type != CellType.Empty) continue;
                    if (_cells[x, y].HasBulb) continue;

                    bool leftBlocked = !IsInBounds(x - 1, y) || _cells[x - 1, y].Type != CellType.Empty;
                    bool rightBlocked = !IsInBounds(x + 1, y) || _cells[x + 1, y].Type != CellType.Empty;
                    bool upBlocked = !IsInBounds(x, y - 1) || _cells[x, y - 1].Type != CellType.Empty;
                    bool downBlocked = !IsInBounds(x, y + 1) || _cells[x, y + 1].Type != CellType.Empty;

                    if (leftBlocked && rightBlocked && upBlocked && downBlocked)
                    {
                        return new HintMove
                        {
                            X = x,
                            Y = y,
                            Type = HintType.PlaceBulb,
                            Reason = $"O ({x},{y}) bi khoa kin ca hang lan cot boi tuong/mep ban co - day la vi tri bat buoc phai co meo."
                        };
                    }
                }
            }

            return null;
        }

        private HintMove FindForcedPlacement()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (_cells[x, y].Type != CellType.WallNumbered) continue;

                    List<(int, int)> emptyNeighbors = GetEmptyNeighbors(x, y);
                    int bulbCount = 0;
                    List<(int, int)> missing = new List<(int, int)>();

                    foreach ((int nx, int ny) in emptyNeighbors)
                    {
                        if (_cells[nx, ny].HasBulb)
                        {
                            bulbCount++;
                        }
                        else
                        {
                            missing.Add((nx, ny));
                        }
                    }

                    int needed = _cells[x, y].WallNumber - bulbCount;
                    if (needed > 0 && needed == missing.Count)
                    {
                        (int hx, int hy) = missing[0];
                        return new HintMove
                        {
                            X = hx,
                            Y = hy,
                            Type = HintType.PlaceBulb,
                            Reason = $"O tuong ({x},{y}) can du {_cells[x, y].WallNumber} meo ke canh - day la vi tri bat buoc."
                        };
                    }
                }
            }

            return null;
        }

        private HintMove FindForcedAvoid()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (_cells[x, y].Type != CellType.WallNumbered) continue;

                    List<(int, int)> emptyNeighbors = GetEmptyNeighbors(x, y);
                    int bulbCount = 0;
                    List<(int, int)> missing = new List<(int, int)>();

                    foreach ((int nx, int ny) in emptyNeighbors)
                    {
                        if (_cells[nx, ny].HasBulb)
                        {
                            bulbCount++;
                        }
                        else
                        {
                            missing.Add((nx, ny));
                        }
                    }

                    if (bulbCount == _cells[x, y].WallNumber && missing.Count > 0)
                    {
                        (int hx, int hy) = missing[0];
                        return new HintMove
                        {
                            X = hx,
                            Y = hy,
                            Type = HintType.AvoidBulb,
                            Reason = $"O tuong ({x},{y}) da du {_cells[x, y].WallNumber} meo - khong duoc dat them meo o day."
                        };
                    }
                }
            }

            return null;
        }

        private List<(int, int)> GetEmptyNeighbors(int x, int y)
        {
            List<(int, int)> list = new List<(int, int)>();
            TryAddEmpty(list, x + 1, y);
            TryAddEmpty(list, x - 1, y);
            TryAddEmpty(list, x, y + 1);
            TryAddEmpty(list, x, y - 1);
            return list;
        }

        private void TryAddEmpty(List<(int, int)> list, int x, int y)
        {
            if (IsInBounds(x, y) && _cells[x, y].Type == CellType.Empty)
            {
                list.Add((x, y));
            }
        }
    }
}
