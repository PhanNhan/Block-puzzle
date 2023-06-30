using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ClassicMode.Gameplay.Controllers;
using DG.Tweening;
using Util;

namespace ClassicMode.Gameplay.Components
{
    public class BlockComponent : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, ZPool.IPoolListener
    {
        public Action<BlockComponent, PointerEventData> OnBeginDragCB;
        public Action<BlockComponent, PointerEventData> OnDragCB;
        public Action<BlockComponent, PointerEventData> OnEndDragCB;

        [SerializeField]
        private BlockCellComponent _blockCellPrefab;
        [SerializeField]
        private Image _imageCollider;

        public int Type;
        public RectTransform RectTransform;
        public Transform BlockRoot;
        public Vector2 Anchor;
        public int Width;
        public int Height;

        private int[][] _blockMatrix;
        private Vector3 _startLocalScale = Vector3.zero;
        private Tweener _dropFailed = null;
        private Tweener _scaleTweener = null;

        private List<BlockCellComponent> _cells = new List<BlockCellComponent>();

        public int[][] GetBlockMatrix()
        {
            return _blockMatrix;
        }

        public void OnSpawn() { }

        public void OnRecycle()
        {
            transform.localPosition = Vector3.zero;
            transform.localScale = _startLocalScale;
            for (int i = 0, c = _cells.Count; i < c; ++i)
            {
                if (_cells[i] != null)
                    ZPool.Assets.Destroy(_cells[i].gameObject);
            }
            _cells.Clear();
        }

        public void RotateBlock()
        {
            OnRecycle();
            Type = ClassicMode.Gameplay.Controllers.BlockHelper.GetBlockCount;
            _blockMatrix = ClassicMode.Gameplay.Controllers.BlockHelper.CloneBlockMatrix(Type);

            updateShape();
        }

        public void Build(int type)
        {
            Type = type % ClassicMode.Gameplay.Controllers.BlockHelper.GetBlockCount;
            _blockMatrix = ClassicMode.Gameplay.Controllers.BlockHelper.CloneBlockMatrix(Type);

            updateShape();
            if (_startLocalScale == Vector3.zero)
                _startLocalScale = transform.localScale;
            showSpawningAnimation();
        }

        public BlockCellComponent CellAt(int row, int col)
        {
            return _cells[row * _blockMatrix[0].Length + col];
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            stopCurrentTweenIfAny();
            if (OnBeginDragCB != null) OnBeginDragCB(this, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (OnEndDragCB != null) OnEndDragCB(this, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (OnDragCB != null) OnDragCB(this, eventData);
        }

        public void ShowDropFailedAnimation()
        {
            stopCurrentTweenIfAny();
            _dropFailed = transform.DOPunchScale(_startLocalScale, 0.5f).SetEase(Ease.InOutBounce);
        }

        public void StopFailAnimation()
        {
            if (_dropFailed != null)
                _dropFailed.Kill();
        }

        public void EnableCollider(bool enable)
        {
            _imageCollider.raycastTarget = enable;
        }

        public int CellCount()
        {
            if (_blockMatrix == null)
                return 0;
            int count = 0;
            int rowCount = _blockMatrix.Length;
            int columnCount = _blockMatrix[0].Length;
            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < columnCount; col++)
                {
                    if (_blockMatrix[row][col] != 0)
                        count++;
                }
            }
            return count;
        }

        public bool IsEmptyBlock()
        {
            return _cells.Count <= 0;
        }

        public void ScaleBlockCellWhenDrag(float value)
        {
            if (_scaleTweener != null)
            {
                _scaleTweener.Kill(true);
                _scaleTweener = null;
            }

            int quantumBlockCells = transform.childCount;
            for (int i = 0; i < quantumBlockCells; i++)
            {
                transform.GetChild(i).localScale = Vector3.one * value;
            }
        }

        public void SetGrayBlockNotAvailable()
        {
            for (int i = 0, c = _cells.Count; i < c; ++i)
            {
                if (_cells[i] != null)
                {
                    _cells[i].SetGray();
                }
            }
        }

        // public void SetOriginalBlockNotAvailable()
        // {
        //     int quantumBlockCells = transform.childCount;
        //     for (int i = 0; i < quantumBlockCells; i++)
        //     {
        //         if(_cells[i] != null)
        //             _cells[i].Refresh();
        //     }
        // }

        public void OnPutIntoGrid()
        {
            _cells.Clear();
        }

        public void Refresh()
        {
            for (int i = 0, c = _cells.Count; i < c; ++i)
            {
                if (_cells[i] != null)
                {
                    _cells[i].Refresh();
                }
            }
        }

        private void stopCurrentTweenIfAny()
        {
            if (_dropFailed != null)
            {
                _dropFailed.Kill();
                _dropFailed = null;
            }
        }

        private void showSpawningAnimation()
        {
            gameObject.transform.localScale = 0.05f * Vector3.one;
            _scaleTweener = gameObject.transform.DOScale(_startLocalScale, 0.5f).SetEase(Ease.InOutBack);
            _scaleTweener.OnComplete(() =>
            {
                _scaleTweener.Kill(true);
                _scaleTweener = null;
            });

            for (int i = 0, c = _cells.Count; i < c; i++)
            {
                var cell = _cells[i];
                if (cell != null)
                {
                    cell.Show();
                }
            }
        }

        private void updateShape()
        {
            updateAnchorPoint();
            _cells.Clear();

            int rowCount = _blockMatrix.Length;
            int columnCount = _blockMatrix[0].Length;

            float centerX = Anchor.x * columnCount;
            float centerY = Anchor.y * rowCount;
            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < columnCount; col++)
                {
                    if (_blockMatrix[row][col] != 0)
                    {
                        var objCell = ZPool.Assets.Clone(_blockCellPrefab.gameObject);
                        objCell.transform.SetParent(BlockRoot, false);

                        Vector3 localPos = objCell.transform.localPosition;
                        localPos.x = (col - centerX) * (C.GameplayConfig.BlockCellWidth + 5);
                        localPos.y = (centerY - row) * (C.GameplayConfig.BlockCellHeight + 5);
                        objCell.transform.localPosition = localPos;
                        var cell = objCell.GetComponent<BlockCellComponent>();
                        cell.Init(Type);
                        cell.GetComponent<RectTransform>().pivot = (Vector2.zero);

                        _cells.Add(objCell.GetComponent<BlockCellComponent>());
                    }
                    else
                    {
                        _cells.Add(null);
                    }
                }
            }
            RectTransform.sizeDelta = new Vector2(5 * C.GameplayConfig.BlockCellWidth, 5 * C.GameplayConfig.BlockCellHeight);
        }

        private void updateAnchorPoint()
        {
            int currentWidth = 0;
            float maxWidth = 0;
            float maxHeight = 0;
            Anchor = new Vector2();

            int rowCount = _blockMatrix.Length;
            int columnCount = _blockMatrix[0].Length;
            for (int row = 0; row < rowCount; row++)
            {
                currentWidth = 0;
                for (int col = columnCount - 1; col >= 0; col--)
                {
                    if (_blockMatrix[row][col] != 0)
                    {
                        currentWidth = col + 1;
                        break;
                    }
                }

                if (currentWidth != 0)
                {
                    ++maxHeight;
                }
                if (currentWidth > maxWidth)
                {
                    maxWidth = currentWidth;
                }
            }
            Anchor.x = (maxWidth - 1) / columnCount / 2f;
            Anchor.y = (maxHeight - 1) / rowCount / 2f;
            Width = (int)maxWidth;
            Height = (int)maxHeight;
        }
    }
}
