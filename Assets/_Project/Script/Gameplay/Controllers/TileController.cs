using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ClassicMode.Gameplay.Components;
using Profile;
using System.Linq;

namespace ClassicMode.Gameplay.Controllers
{
    public class TileController : MonoBehaviour, IRefreshable
    {
        public delegate void TileChangeHandler(BlockComponent block, int row, int col, int removedLineCount);
        public delegate void HurrayMomentHandler(int removeLineColCount, int removeLineRowCount);
        public event TileChangeHandler OnTileChanged;
        public event HurrayMomentHandler OnHurrayMoment;

        private int Rows;
        private int Columns;

        public GridLayoutGroup TileGrip;

        [SerializeField]
        private Transform _blockCellRoot;
        [SerializeField]
        private Transform _destroyZone;
        [SerializeField]
        private RectTransform _highlightZone;

        [SerializeField]
        private TileComponent _tilePrefab;
        [SerializeField]
        private BlockCellComponent _blockCellPrefab;

        private List<BlockCellComponent> highlightTiles = new List<BlockCellComponent>();

        private int _removedLineCount;
        private TileComponent[][] _currentGrid;
        private BlockCellComponent[][] _currentCells;
        private int removeLineRowCount = 0;
        private int removeLineColCount = 0;
        private List<List<Vector2Int>> listSquare;

        public void Init(int rows, int cols)
        {
            Rows = rows;
            Columns = cols;
            initTiles();
            initCells();
            updateView();
            initMiniSquare();
        }

        private void initMiniSquare()
        {
            listSquare = new List<List<Vector2Int>>();
            int rowCount = 0;
            while (rowCount < 9)
            {
                int colCount = 0;
                while (colCount < 9)
                {
                    var listIndexCell = new List<Vector2Int>();
                    for (int row = rowCount; row < rowCount + 3; row++)
                    {
                        for (int col = colCount; col < colCount + 3; col++)
                        {
                            listIndexCell.Add(new Vector2Int(row, col));
                        }
                    }
                    colCount += 3;
                    listSquare.Add(listIndexCell);
                }
                rowCount += 3;
            }
            Debug.LogError(listSquare.Count);
        }


        public void OnReset()
        {
            StopAllCoroutines();
            resetGrid();
        }

        public void CleanUp()
        {
            resetGrid();
            cleanUpTiles();
        }

        public Vector2 IndexAtPoint(Vector2 canvasPoint)
        {
            Vector2 index = new Vector2(0, 0);

            Vector2 rootGrid = TileGrip.transform.localPosition;
            index.y = (canvasPoint.x - rootGrid.x) / (TileGrip.cellSize.x + TileGrip.spacing.x);
            index.x = -(canvasPoint.y - rootGrid.y) / (TileGrip.cellSize.y + TileGrip.spacing.y);
            return index;
        }

        public Vector2 PointAtIndex(int row, int col)
        {
            Vector2 rootGrid = TileGrip.transform.localPosition;
            float x = rootGrid.x + col * (TileGrip.cellSize.x + TileGrip.spacing.x) - 3.5f;
            float y = rootGrid.y - row * (TileGrip.cellSize.y + TileGrip.spacing.y) + 3.5f;
            return new Vector2(x, y);
        }

        public bool IsEmptyAt(int row, int col)
        {
            if (row < 0 || row >= Rows ||
                col < 0 || col >= Columns)
                return false;
            if (_cellDisable != null && _cellDisable.Contains(col)) return false;

            return _currentCells[row][col] == null;
        }

        List<int> _cellDisable = null;
        public void CheatForTutorial_EnableCells(int colIdx, int length, bool enable)
        {
            _cellDisable = new List<int>();
            for (int i = colIdx, c = colIdx + length; i < c; ++i)
            {
                if (!enable)
                    _cellDisable.Add(i);
            }
        }

        public bool CanPutBlockAt(BlockComponent block, int row, int col)
        {
            if (row < 0 || row >= Rows ||
                col < 0 || col >= Columns)
                return false;
            int blockColumnCount = block.Width;
            int blockRowCount = block.Height;

            int halfColumnCount = blockColumnCount / 2;
            int halfRowCount = blockRowCount / 2;
            if (blockColumnCount % 2 == 0)
                halfColumnCount--;
            if (blockRowCount % 2 == 0)
                halfRowCount--;

            for (int tmpRow = 0; tmpRow < blockRowCount; tmpRow++)
            {
                for (int tmpCol = 0; tmpCol < blockColumnCount; tmpCol++)
                {
                    if (block.CellAt(tmpRow, tmpCol) != null)
                    {
                        int cellRow = row + (tmpRow - halfRowCount);
                        int cellCol = col + (tmpCol - halfColumnCount);
                        if (!IsEmptyAt(cellRow, cellCol))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public List<Vector2> GetAvailableIndexes(BlockComponent block)
        {
            List<Vector2> indexes = new List<Vector2>();
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (CanPutBlockAt(block, row, col))
                    {
                        indexes.Add(new Vector2(row, col));
                    }
                }
            }
            return indexes;
        }

        public bool PutBlockAt(BlockComponent block, int row, int col, out int removedLines)
        {
            if (CanPutBlockAt(block, row, col))
            {
                int blockColumnCount = block.Width;
                int blockRowCount = block.Height;
                int halfColumnCount = blockColumnCount / 2;
                int halfRowCount = blockRowCount / 2;
                if (blockColumnCount % 2 == 0)
                    halfColumnCount--;
                if (blockRowCount % 2 == 0)
                    halfRowCount--;

                Vector2 center = Vector2.zero;
                int totalPoint = 0;

                for (int tmpRow = 0; tmpRow < blockRowCount; tmpRow++)
                {
                    for (int tmpCol = 0; tmpCol < blockColumnCount; tmpCol++)
                    {
                        var cell = block.CellAt(tmpRow, tmpCol);
                        if (cell != null)
                        {
                            int cellRow = row + (tmpRow - halfRowCount);
                            int cellCol = col + (tmpCol - halfColumnCount);
                            setCellAtIndex(cellRow, cellCol, cell, true);
                            center += PointAtIndex(cellRow, cellCol);
                            totalPoint++;
                        }
                    }
                }
                block.OnPutIntoGrid();


                center = center / totalPoint;
                List<int> filledRows = new List<int>();
                List<int> filledColumns = new List<int>();
                List<Vector2Int> filledSquare = new List<Vector2Int>();
                int removedLineCount = 0;

                removedLineCount = checkGrid(ref filledRows, ref filledColumns);
                checkFillSquare3x3(ref filledSquare);

                if (OnTileChanged != null)
                {
                    OnTileChanged(block, row, col, removedLineCount);
                }

                ZPool.Assets.Destroy(block.gameObject);

                removeRows(filledRows, center);
                removeColumns(filledColumns, center);
                removeSquare(filledSquare, center);

                removedLines = removedLineCount;
                return true;
            }

            removedLines = 0;
            return false;
        }

        private void checkFillSquare3x3(ref List<Vector2Int> filledSquare)
        {
            foreach (var listCell in listSquare)
            {
                if (IsFilledSquare(listCell))
                {
                    filledSquare.AddRange(listCell);
                }
            }
        }

        private bool IsFilledSquare(List<Vector2Int> listCell)
        {
            foreach (var index in listCell)
            {
                if (IsEmptyAt(index.x, index.y))
                {
                    return false;
                }
            }
            return true;
        }

        private void removeSquare(List<Vector2Int> listIndexCell, Vector2 center)
        {
            if (listIndexCell.Any())
            {
                foreach (var indexCell in listIndexCell)
                {
                    if (_currentCells[indexCell.x][indexCell.y] != null)
                    {
                        BlockCellComponent cell = _currentCells[indexCell.x][indexCell.y];
                        _currentCells[indexCell.x][indexCell.y] = null;
                        cell.transform.SetParent(_destroyZone);
                        float delayTime = Vector2.Distance(PointAtIndex(indexCell.x, indexCell.y), center) / C.GameplayConfig.BlockCellHeight;
                        delayTime *= C.GameplayConfig.RemoveCellDelayInSeconds;
                        removeCell(cell, delayTime);
                    }
                }
            }
        }

        private void createHighlightTile(BlockComponent block)
        {
            int needCreater = block.CellCount() - highlightTiles.Count;
            if (needCreater > 0)
            {
                for (int i = 0; i < needCreater; i++)
                {
                    GameObject objHighlight = ZPool.Assets.Clone(_blockCellPrefab.gameObject);
                    BlockCellComponent cellHighlight = objHighlight.GetComponent<BlockCellComponent>();
                    objHighlight.transform.SetParent(_highlightZone);
                    objHighlight.transform.localScale = Vector3.one;
                    highlightTiles.Add(cellHighlight);
                }
            }

            foreach (var obj in highlightTiles)
            {
                BlockCellComponent cellHighlight = obj.GetComponent<BlockCellComponent>();
                cellHighlight.SetHighlight(block.Type);
                obj.gameObject.SetActive(false);
            }
        }

        public void UpdateHighlightTiles(BlockComponent block)
        {
            createHighlightTile(block);

            Vector2 index = IndexAtPoint(block.transform.localPosition);

            if (block.Width % 2 == 0)
                index.y -= 0.5f;
            if (block.Height % 2 == 0)
                index.x -= 0.5f;

            int row = Mathf.RoundToInt(index.x);
            int col = Mathf.RoundToInt(index.y);

            int blockColumnCount = block.Width;
            int blockRowCount = block.Height;

            int halfColumnCount = blockColumnCount / 2;
            int halfRowCount = blockRowCount / 2;
            if (blockColumnCount % 2 == 0)
                halfColumnCount--;
            if (blockRowCount % 2 == 0)
                halfRowCount--;

            int indexObj = 0;
            for (int tmpRow = 0; tmpRow < blockRowCount; tmpRow++)
            {
                for (int tmpCol = 0; tmpCol < blockColumnCount; tmpCol++)
                {
                    var _blockMatrix = block.GetBlockMatrix();
                    if (_blockMatrix[tmpRow][tmpCol] != 0)
                    {
                        int cellRow = row + (tmpRow - halfRowCount);
                        int cellCol = col + (tmpCol - halfColumnCount);

                        if (cellRow < 0 || cellRow >= Rows || cellCol < 0 || cellCol >= Columns)
                            continue;

                        Vector2 posObject = PointAtIndex(cellRow, cellCol);
                        var objHighlight = highlightTiles[indexObj];
                        objHighlight.gameObject.SetActive(true);
                        objHighlight.transform.localPosition = posObject;
                        indexObj++;
                    }
                }
            }
        }

        public void SetEnableHighlightTiles(bool enable)
        {
            foreach (var objectEffect in highlightTiles)
                objectEffect.gameObject.SetActive(false);
        }

        public void OnLoadGameData(GameData gameData)
        {
            resetGrid();
            for (int row = 0; row < Rows; ++row)
            {
                for (int col = 0; col < Columns; ++col)
                {
                    var data = gameData.Grid.Cells[row].Cells[col];
                    if (data.Type == -1)
                    {
                        _currentCells[row][col] = null;
                    }
                    else
                    {
                        var objCell = ZPool.Assets.Clone(_blockCellPrefab.gameObject);
                        var cell = objCell.GetComponent<BlockCellComponent>();
                        int newData = (data.Type > BlockHelper.GetBlockCount) ? 0 : data.Type;
                        cell.Init(newData);
                        setCellAtIndex(row, col, cell);
                    }
                }
            }
        }

        public void OnSaving(GameData gameData)
        {
            var grid = new GameData.Board();

            grid.Cells = new List<GameData.Board.Row>();
            for (int row = 0; row < Rows; ++row)
            {
                var newRow = new GameData.Board.Row();
                newRow.Cells = new List<GameData.Board.Row.Cell>();
                grid.Cells.Add(newRow);
                for (int col = 0; col < Columns; ++col)
                {
                    newRow.Cells.Add(new GameData.Board.Row.Cell
                    {
                        Type = _currentCells[row][col] == null ? -1 : _currentCells[row][col].Type
                    });
                }
            }

            gameData.Grid = grid;
        }

        private void updateView()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    _currentGrid[row][col].Refresh();

                    if (_currentCells[row][col] != null)
                    {
                        _currentCells[row][col].Refresh();
                    }
                }
            }
        }

        private int checkGrid(ref List<int> filledRows, ref List<int> filledColumns)
        {
            for (int row = 0; row < Rows; row++)
            {
                if (isFulfilledRow(row))
                    filledRows.Add(row);
            }
            removeLineRowCount = filledRows.Count;

            for (int col = 0; col < Columns; col++)
            {
                if (isFulfilledColumn(col))
                    filledColumns.Add(col);
            }
            removeLineColCount = filledColumns.Count;

            _removedLineCount = filledRows.Count + filledColumns.Count;

            return _removedLineCount;
        }

        private bool isFulfilledRow(int row)
        {
            for (int col = 0; col < Columns; col++)
            {
                if (IsEmptyAt(row, col))
                {
                    return false;
                }
            }
            return true;
        }

        private bool isFulfilledColumn(int col)
        {
            if (Rows <= 1) return false;
            for (int row = 0; row < Rows; row++)
            {
                if (IsEmptyAt(row, col))
                {
                    return false;
                }
            }
            return true;
        }

        public void OnRefresh()
        {
            updateView();
        }

        public void LogGrid()
        {
            string log = "";
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (_currentCells[row][col] != null)
                    {
                        log += "1 ";
                    }
                    else
                        log += "0 ";
                }
                log += "\n";
            }
            Debug.LogError(Time.time + "Grid: \n" + log);
        }

        private void removeRows(List<int> rowList, Vector2 center)
        {
            if (rowList == null || rowList.Count == 0)
                return;

            foreach (int row in rowList)
            {
                if (row < 0 || row >= Rows)
                    continue;
                for (int col = 0; col < Columns; col++)
                {
                    if (_currentCells[row][col] != null)
                    {
                        BlockCellComponent cell = _currentCells[row][col];
                        _currentCells[row][col] = null;
                        cell.transform.SetParent(_destroyZone);
                        float delayTime = Vector2.Distance(PointAtIndex(row, col), center) / C.GameplayConfig.BlockCellWidth;
                        delayTime *= C.GameplayConfig.RemoveCellDelayInSeconds;
                        removeCell(cell, delayTime);
                    }
                }
            }
        }

        private void removeColumns(List<int> columnList, Vector2 center)
        {
            if (columnList == null || columnList.Count == 0)
                return;

            foreach (int col in columnList)
            {
                if (col < 0 || col >= Columns)
                    continue;
                for (int row = 0; row < Rows; row++)
                {
                    if (_currentCells[row][col] != null)
                    {
                        BlockCellComponent cell = _currentCells[row][col];
                        _currentCells[row][col] = null;
                        cell.transform.SetParent(_destroyZone);
                        float delayTime = Vector2.Distance(PointAtIndex(row, col), center) / C.GameplayConfig.BlockCellHeight;
                        delayTime *= C.GameplayConfig.RemoveCellDelayInSeconds;
                        removeCell(cell, delayTime);
                    }
                }
            }
        }

        private void removeCell(BlockCellComponent cell, float delayTime)
        {
            stopAnimPutBlockAt();
            cell.Explosion(delayTime, () =>
            {
                ZPool.Assets.Destroy(cell.gameObject);
            });
        }

        private void initCells()
        {
            _currentCells = new BlockCellComponent[Rows][];
            for (int row = 0; row < Rows; row++)
            {
                _currentCells[row] = new BlockCellComponent[Columns];
            }
        }

        public float SetGrayAllCells()
        {
            int index = 0;
            float totalTimeDelay = 0;
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (_currentCells[row][col] != null)
                    {
                        index++;
                        _currentCells[row][col].SetGray(0.03f * index);
                    }

                }
            }
            return totalTimeDelay;
        }

        private void initTiles()
        {
            _currentGrid = new TileComponent[Rows][];
            for (int row = 0; row < Rows; row++)
            {
                _currentGrid[row] = new TileComponent[Columns];
            }

            Utils.RemoveAllChildren(TileGrip.transform);

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    TileComponent tile = spawnTile();
                    tile.GetComponent<RectTransform>().pivot = (Vector2.zero);
                    tile.Init();
                    _currentGrid[row][col] = tile;
                }
            }
        }

        private void cleanUpTiles()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    _currentGrid[row][col] = null;
                }
            }
            Utils.RemoveAllChildren(TileGrip.transform);
        }

        private TileComponent spawnTile()
        {
            GameObject objTile = GameObject.Instantiate(_tilePrefab.gameObject, TileGrip.transform);
            TileComponent tileComponent = objTile.GetComponent<TileComponent>();
            return tileComponent;
        }

        private Sequence animPutBlock = null;
        private void stopAnimPutBlockAt()
        {
            if (animPutBlock != null)
                animPutBlock.Complete();
        }
        private void setCellAtIndex(int row, int col, BlockCellComponent cell, bool isHaveAnim = false)
        {
            if (row < 0 || col < 0 || row >= Rows || col >= Columns)
            {
                return;
            }

            _currentCells[row][col] = cell;
            _currentCells[row][col].transform.SetParent(_blockCellRoot, isHaveAnim);
            _currentCells[row][col].SetPivot(Vector2.one * 0.5f);
            if (isHaveAnim)
            {
                animPutBlock = DOTween.Sequence();
                animPutBlock.Append(_currentCells[row][col].transform.DOLocalMove(PointAtIndex(row, col), 0.25f).SetEase(Ease.OutQuart));
                animPutBlock.Append(cell.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutCirc));
                // _currentCells[row][col].transform.DOLocalMove(PointAtIndex(row,col),0.25f).OnComplete(()=>{
                //     cell.transform.DOScale(Vector3.one, 0.2f);
                // });                        
            }

            else
                _currentCells[row][col].transform.localPosition = PointAtIndex(row, col);
        }

        private void resetGrid()
        {
            if (_currentCells == null)
                return;

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (_currentCells[row][col] != null)
                    {
                        ZPool.Assets.Destroy(_currentCells[row][col].gameObject);
                        _currentCells[row][col] = null;
                    }
                }
            }
            highlightTiles.Clear();
            Utils.RemoveAllChildren(_destroyZone);
        }
    }
}
