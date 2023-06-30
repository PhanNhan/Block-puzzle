using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassicMode.Gameplay.Components;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using Profile;
using DG.Tweening;

namespace ClassicMode.Gameplay.Controllers
{
    public class DragableBlockController : MonoBehaviour, IRefreshable
    {
        [SerializeField]
        private BlockComponent _blockPrefab;
        [SerializeField]
        private Transform[] _dragableRoots;
        [SerializeField]
        private Transform _dragZoneRoot;

        public TileController _tileController;
        public Image[] BgImages;

        //private Vector2 _draggingBlockOrgPos;
        private BlockComponent[] _dragableBlocks;
        private List<int> _currentBlockTypes;

        public BlockComponent[] DragableBlocks { get { return _dragableBlocks; } }

        public bool IsTutorialMode;

        public void Init()
        {
            _dragableBlocks = new BlockComponent[_dragableRoots.Length];
            refresh();
        }

        public List<BlockComponent> SpawnNewBlocksWithType()
        {
            var results = new List<BlockComponent>();
            for (int i = 0, c = 3; i < c; ++i)
            {
                bool needGenerate = false;
                if (_dragableBlocks[i] == null)
                    needGenerate = true;
                else
                {
                    if (!_dragableBlocks[i].gameObject.activeSelf)
                    {
                        ZPool.Assets.Destroy(_dragableBlocks[i].gameObject);
                        _dragableBlocks[i] = null;
                        needGenerate = true;
                    }
                }
                if (needGenerate)
                {
                    _currentBlockTypes[i] = BlockHelper.RandomBlock();
                    var block = spawnNewBlock(i, _currentBlockTypes[i]);
                    results.Add(block);
                }
            }
            return results;
        }

        public List<BlockComponent> SpawnNewBlocks(List<int> _blockTypes)
        {
            var results = new List<BlockComponent>();
            _currentBlockTypes = _blockTypes;
            for (int i = 0, c = 3; i < c; ++i)
            {
                _currentBlockTypes[i] = BlockHelper.RandomBlock();
                var block = spawnNewBlock(i, _currentBlockTypes[i]);
                results.Add(block);
            }
            return results;
        }

        public List<BlockComponent> SpawnNewBlocksTutorial(List<int> _blockTypes)
        {
            var results = new List<BlockComponent>();
            _currentBlockTypes = _blockTypes;
            for (int i = 0, c = 3; i < c; ++i)
            {
                var block = spawnNewBlock(i, _currentBlockTypes[i]);
                results.Add(block);
            }
            return results;
        }

        private BlockComponent spawnNewBlock(int idx, int type, bool isInitStart = false)
        {
            var block = createBlock(idx, type);
            _dragableBlocks[idx] = block;
            return block;
        }

        public bool CheckBlocks()
        {
            foreach (var block in _dragableBlocks)
            {
                if (block == null)
                    return true;
                else
                {
                    if (!block.gameObject.activeSelf)
                        return true;
                }
            }
            return false;
        }

        public BlockComponent SpawnNewBlockAt(int index)
        {
            Debug.Log("Spam Block At " + index);
            _currentBlockTypes[index] = BlockHelper.RandomBlock();
            var block = createBlock(index, _currentBlockTypes[index]);
            _dragableBlocks[index] = block;

            return block;
        }

        public int PopDraggingBlock(BlockComponent block)
        {
            for (int i = 0, c = _dragableBlocks.Length; i < c; ++i)
            {
                if (_dragableBlocks[i] == block)
                {
                    _dragableBlocks[i] = null;
                    //_draggingBlockOrgPos = block.transform.localPosition;

                    return i;
                }
            }
            return -1;
        }

        public void SetParentDraggingBlock(BlockComponent block)
        {
            for (int i = 0; i < _dragableBlocks.Length; i++)
            {
                if (_dragableBlocks[i] == block)
                {
                    Debug.LogError("??????");
                }
            }

            block.transform.SetParent(_dragZoneRoot);
            block.transform.localScale = Vector3.one;
        }

        public void PutDraggingBlockBack(int draggingBlockIndex, BlockComponent currentDraggingBlock)
        {
            _dragableBlocks[draggingBlockIndex] = currentDraggingBlock;
            currentDraggingBlock.transform.SetParent(_dragableRoots[draggingBlockIndex]);
            currentDraggingBlock.transform.localPosition = Vector3.zero;//_draggingBlockOrgPos;
            currentDraggingBlock.transform.localScale = Vector2.one * 0.5f;
            currentDraggingBlock.ScaleBlockCellWhenDrag(1f);
        }

        public bool NeedSpawnNewBlock()
        {
            bool enableOneBlock = C.GameplayConfig.GameModeOneBlock; ;
            if (enableOneBlock)
            {
                for (int i = 0, c = _dragableBlocks.Length; i < c; ++i)
                {
                    if (_dragableBlocks[i] == null)
                    {
                        return true;
                    }
                    else
                    {
                        if (!_dragableBlocks[i].gameObject.activeSelf)
                            return true;
                    }
                }
                return false;
            }
            else
            {
                for (int i = 0, c = _dragableBlocks.Length; i < c; ++i)
                {
                    if (_dragableBlocks[i] != null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public void OnReset()
        {
            cleanUp();
        }

        public void CleanUp()
        {
            cleanUp();
        }

        public void OnRefresh()
        {
            refresh();
        }

        public void OnLoadGameData(GameData gameData)
        {
            cleanUp();
            var blocks = gameData.Blocks;
            for (int i = 0, c = blocks.Count; i < c; ++i)
            {
                var block = blocks[i];
                block.Type = (block.Type >= BlockHelper.GetBlockCount) ? 0 : block.Type;
                _currentBlockTypes[i] = block.Type;
                if (block.IsEmpty)
                {
                    _dragableBlocks[i] = null;
                }
                else
                {
                    _dragableBlocks[i] = spawnNewBlock(i, block.Type, true);
                }
            }
        }

        public void OnSaving(GameData gameData)
        {
            var blocks = new List<GameData.Block>();
            for (int i = 0, c = _currentBlockTypes.Count; i < c; ++i)
            {
                var block = new GameData.Block
                {
                    Type = _currentBlockTypes[i],
                    IsEmpty = _dragableBlocks[i] == null ? true : false
                };
                blocks.Add(block);
            }

            gameData.Blocks = blocks;
        }

        private void cleanUp()
        {
            for (int i = 0, c = _dragableBlocks.Length; i < c; ++i)
            {
                if (_dragableBlocks[i] != null)
                {
                    ZPool.Assets.Destroy(_dragableBlocks[i].gameObject);
                    _dragableBlocks[i] = null;
                }
            }

            if (_currentBlockTypes == null)
                _currentBlockTypes = new List<int>() { 0, 0, 0 };
        }

        private void refresh()
        {
            updateBlocks();
        }

        private void updateBlocks()
        {
            for (int i = 0, c = _dragableBlocks.Length; i < c; ++i)
            {
                if (_dragableBlocks[i] != null)
                {
                    _dragableBlocks[i].Refresh();
                }
            }
        }

        public BlockComponent createBlock(int index, int type)
        {
            GameObject objBlock = ZPool.Assets.Clone(_blockPrefab.gameObject);
            objBlock.transform.SetParent(_dragableRoots[index], false);
            var block = objBlock.GetComponent<BlockComponent>();
            block.Build(type);
            block.EnableCollider(true);

            return block;
        }

        public BlockComponent createBlock(int type)
        {
            GameObject objBlock = ZPool.Assets.Clone(_blockPrefab.gameObject);
            var block = objBlock.GetComponent<BlockComponent>();
            block.Build(type);
            return block;
        }
    }
}
