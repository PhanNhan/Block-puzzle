using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using ClassicMode.Gameplay.Controllers;
using ClassicMode.Gameplay.Components;
using MyEventSystem;
using Util;
using UI;

namespace Scenes
{
    public class Gameplay : MonoBehaviour
    {
        public GameObject Board;
        public ScoreController ScoreController;
        public TileController TileController;
        public DragableBlockController DragableBlockController;
        public NoMoreMove NoMoreMoveController;
        public XText Coin;

        [SerializeField]
        private RectTransform _canvasRectTransform;

        private GameState _gameState;
        private int _draggingBlockIndex;
        private BlockComponent _currentDraggingBlock = null;
        private bool _isBreakBestScore;
        private Sequence _blockMoveTweener = null;
        private bool _isShowBestScoreEffect = false;
        private Util.Timer _timer;
        private bool _isPause = false;
        private float _timeWait = 0;
        private float _timeWaitCreateCombo = 0;
        private int oldCoin;
        private int coin;
        private List<int> _currentBlockTypes;
        private int _timeGameovers;
        private Profile.GameData _gameoverData;

        private bool _needShowWhenCloseIntestitalAds = false;

        private void Start()
        {
            InitGame();
        }

        private void InitGame()
        {
            DragableBlockController.Init();
            TileController.Init(C.GameplayConfig.Rows, C.GameplayConfig.Columns);
            ScoreController.Init();
            NoMoreMoveController.Init();
            updateCoin();
            onReset();
            updateDraggableObjects();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (_currentDraggingBlock == null || _draggingBlockIndex < 0)
                return;

            _currentDraggingBlock.ScaleBlockCellWhenDrag(1f);
            putDraggingBlockBack();
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (_currentDraggingBlock == null || _draggingBlockIndex < 0)
                return;

            _currentDraggingBlock.ScaleBlockCellWhenDrag(1f);
            putDraggingBlockBack();
        }

        void Update()
        {
            if (_gameState == GameState.Playing)
            {
                _timeWait += Time.deltaTime;
                _timeWaitCreateCombo += Time.deltaTime;
            }
            if (_timer != null && !_isPause)
            {
                _timer.TakeTime(Time.deltaTime);
            }
        }

        private void onBeginBlockDrag(BlockComponent block, PointerEventData eventData)
        {
            if (_currentDraggingBlock != null)
                return;

            cancelCurrentDragBlockTween();
            _draggingBlockIndex = -1;
            _draggingBlockIndex = DragableBlockController.PopDraggingBlock(block);
            if (_draggingBlockIndex < 0)
                return;
            DragableBlockController.SetParentDraggingBlock(block);
            block.StopFailAnimation();
            block.ScaleBlockCellWhenDrag(0.9f);
            Vector2 curScreenPoint = new Vector2(eventData.position.x, eventData.position.y);
            Vector2 viewportPoint = Camera.main.ScreenToViewportPoint(curScreenPoint);
            float canvasX = (viewportPoint.x - 0.5f) * _canvasRectTransform.sizeDelta.x;
            float canvasY = (viewportPoint.y - 0.5f) * _canvasRectTransform.sizeDelta.y;
            canvasY += getDeltaMoveForBlock(block.Type);

            _blockMoveTweener = DOTween.Sequence();
            _blockMoveTweener.Append(block.transform.DOScale(Vector3.one, 0.15f))
                .Join(block.transform.DOLocalMove(new Vector2(canvasX, canvasY), 0.15f));


            _currentDraggingBlock = block;
        }

        private void onBlockDrag(BlockComponent block, PointerEventData eventData)
        {
            cancelCurrentDragBlockTween();
            if (_draggingBlockIndex < 0)
                return;

            Vector2 curScreenPoint = new Vector2(eventData.position.x, eventData.position.y);
            Vector2 viewportPoint = Camera.main.ScreenToViewportPoint(curScreenPoint);
            float canvasX = (viewportPoint.x - 0.5f) * _canvasRectTransform.sizeDelta.x;
            float canvasY = (viewportPoint.y - 0.5f) * _canvasRectTransform.sizeDelta.y;
            canvasY += getDeltaMoveForBlock(block.Type);

            block.transform.localPosition = new Vector2(canvasX, canvasY);
            _currentDraggingBlock = block;

            Vector2 index = TileController.IndexAtPoint(block.transform.localPosition);
            if (block.Width % 2 == 0)
                index.y -= 0.5f;
            if (block.Height % 2 == 0)
                index.x -= 0.5f;
            int row = Mathf.RoundToInt(index.x);
            int col = Mathf.RoundToInt(index.y);
            if (TileController.CanPutBlockAt(block, row, col))
            {
                TileController.UpdateHighlightTiles(block);
            }
            else
            {
                TileController.SetEnableHighlightTiles(false);
            }
        }
        private void onEndBlockDrag(BlockComponent block, PointerEventData eventData)
        {

            cancelCurrentDragBlockTween();
            if (_draggingBlockIndex < 0)
                return;

            Vector2 index = TileController.IndexAtPoint(block.transform.localPosition);

            if (block.Width % 2 == 0)
                index.y -= 0.5f;
            if (block.Height % 2 == 0)
                index.x -= 0.5f;

            int row = Mathf.RoundToInt(index.x);
            int col = Mathf.RoundToInt(index.y);
            TileController.SetEnableHighlightTiles(false);
            if (TileController.CanPutBlockAt(block, row, col))
            {

                block.transform.localScale = Vector3.one;
                int removedLineCount = 0;
                TileController.PutBlockAt(block, row, col, out removedLineCount);
                block.EnableCollider(false);
                updateDraggableObjects();
                checkAvailableSlots(removedLineCount > 0 ? C.GameplayConfig.RemoveCellDelayInSeconds * C.GameplayConfig.Rows + 0.6f : 0);
            }
            else
            {
                putDraggingBlockBack();
                block.ShowDropFailedAnimation();
                block.ScaleBlockCellWhenDrag(1f);
            }
            _currentDraggingBlock = null;
            _draggingBlockIndex = -1;
        }

        private void cancelCurrentDragBlockTween()
        {
            if (_blockMoveTweener != null) _blockMoveTweener.Complete();
            _blockMoveTweener = null;
        }

        private void putDraggingBlockBack()
        {
            if (_currentDraggingBlock == null || _draggingBlockIndex < 0)
                return;

            if (_currentDraggingBlock.IsEmptyBlock())
                return;

            DragableBlockController.PutDraggingBlockBack(_draggingBlockIndex, _currentDraggingBlock);
            TileController.SetEnableHighlightTiles(false);
            _currentDraggingBlock = null;
            _draggingBlockIndex = -1;
            TileController.SetEnableHighlightTiles(false);
        }

        private void checkAvailableSlots(float delayShowGameoverIfAny, bool onlyCheckUI = false)
        {
            bool canPutAnyBlock = false;
            var dragableBlocks = DragableBlockController.DragableBlocks;
            for (int i = 0; i < dragableBlocks.Length; i++)
            {
                if (dragableBlocks[i] != null)
                {
                    List<Vector2> availableIndexes = TileController.GetAvailableIndexes(dragableBlocks[i]);
                    if (availableIndexes != null && availableIndexes.Count > 0)
                    {
                        canPutAnyBlock = true;
                        dragableBlocks[i].Refresh();
                        //break;
                    }
                    else
                    {
                        dragableBlocks[i].SetGrayBlockNotAvailable();
                    }
                }
            }

            if (onlyCheckUI)
                return;

            if (!canPutAnyBlock)
            {
                _gameState = GameState.Gameover;
                saveGameData(ref _gameoverData);
                TileController.SetGrayAllCells();
                //				StartCoroutine(screenShotBoard());
                StartCoroutine(showNoMoreMoveController(delayShowGameoverIfAny));
            }
        }

        private IEnumerator showNoMoreMoveController(float delayShowGameoverIfAny)
        {
            yield return new WaitForSeconds(1f);
            Debug.LogError("No More Move");
        }

        private IEnumerator showGameOverCoroutine()
        {

            Debug.LogError("Tracking Gameover ");
            yield return new WaitForSeconds(0.2f);
            showGameOver();
        }

        private void updateDraggableObjects()
        {
            if (DragableBlockController.NeedSpawnNewBlock())
            {

                bool enableOneBlock = C.GameplayConfig.GameModeOneBlock;
                if (enableOneBlock)
                {
                    var blocks = DragableBlockController.SpawnNewBlocksWithType();
                    registerEventsForDragableBlocks(blocks);
                }
                else
                {
                    _currentBlockTypes = BlockHelper.GetRandomBlocks(_currentBlockTypes);
                    var blocks = DragableBlockController.SpawnNewBlocks(_currentBlockTypes);
                    registerEventsForDragableBlocks(blocks);
                }
            }
        }

        private void registerEventsForDragableBlocks(IList<BlockComponent> blocks)
        {
            for (int i = 0, c = blocks.Count; i < c; ++i)
            {
                var block = blocks[i];
                if (block != null)
                {
                    block.OnBeginDragCB = onBeginBlockDrag;
                    block.OnDragCB = onBlockDrag;
                    block.OnEndDragCB = onEndBlockDrag;
                }
            }
        }

        private void onReset()
        {

            _timeGameovers = 0;
            _isBreakBestScore = false;
            DragableBlockController.OnReset();
            _currentDraggingBlock = null;
            _draggingBlockIndex = -1;
            _timeWait = 0;
            _timeWaitCreateCombo = 0;
            _needShowWhenCloseIntestitalAds = false;

            ScoreController.OnReset();
            TileController.OnReset();
            NoMoreMoveController.OnReset();

            _gameState = GameState.Playing;
            _timer = new Util.Timer();
            _timer.Start();
        }

        private void cleanUp()
        {
            DragableBlockController.CleanUp();
            ScoreController.CleanUp();
            TileController.CleanUp();
        }


        private void showGameOver()
        {
            _timeGameovers++;

            Debug.LogError("Show GameOver");
        }

        private IEnumerator onResetCoroutine(bool isReplayGameOver)
        {

            Debug.LogError("Tracking Reset");
            yield return new WaitForSeconds(0.2f);
            onReset();
            updateDraggableObjects();
        }

        private IEnumerator onOneMoreChangeCoroutine()
        {
            DragableBlockController.OnReset();
            _currentDraggingBlock = null;
            _draggingBlockIndex = -1;
            TileController.OnRefresh();

            _timer = new Util.Timer();
            _timer.Start();
            ScoreController.ResetComboCount();

            yield return new WaitForSeconds(0.2f);

            int blockCount = ClassicMode.Gameplay.Controllers.BlockHelper.GetBlockCount;
            List<int> availabeBlock = new List<int>();
            List<int> listTypeBlock = new List<int>();
            for (int type = 0; type < blockCount; type++)
                listTypeBlock.Add(type);

            while (availabeBlock.Count < 3)
            {
                if (listTypeBlock.Count == 0) break;
                int type = Random.Range(0, listTypeBlock.Count);
                BlockComponent block = DragableBlockController.createBlock(type);
                List<Vector2> availableIndexes = TileController.GetAvailableIndexes(block);
                if (availableIndexes != null && availableIndexes.Count > 0)
                {
                    availabeBlock.Add(type);
                }
                listTypeBlock.Remove(type);
                ZPool.Assets.Destroy(block.gameObject);
            }

            var blocks = DragableBlockController.SpawnNewBlocksTutorial(availabeBlock);
            registerEventsForDragableBlocks(blocks);
            _gameState = GameState.Playing;
            checkAvailableSlots(0.25f);
        }

        private void OnCoinChanged()
        {
            updateCoin();
            Coin.GetComponent<RectTransform>().sizeDelta = new Vector2(100, Coin.GetComponent<RectTransform>().sizeDelta.y);
        }

        private void updateCoin()
        {
            // coin = oldCoin = G.ProfileService.TotalCoins;
            // Coin.Value = G.ProfileService.TotalCoins.ToString();
            // updateCointText(G.ProfileService.TotalCoins - oldCoin);
        }

        private void updateCointText(int deltaScore)
        {
            //ScoreText.Value = Score.ToString();
            oldCoin = int.Parse(Coin.Value); ;
            coin += deltaScore;
            Coin.transform.DOScale(Vector3.one * 1.5f, C.GameplayConfig.ScoreChangingTime).SetEase(Ease.InOutQuad).OnComplete(() =>
                {
                    DOTween.To(() => oldCoin, x => updateCoinTextAnim(x), coin, C.GameplayConfig.ScoreChangingTime).OnComplete(() =>
                       {
                           Coin.transform.DOScale(Vector3.one, C.GameplayConfig.ScoreChangingTime).SetEase(Ease.Linear);
                       });
                });

        }

        private void updateCoinTextAnim(float value)
        {
            oldCoin = (int)value;
            int tempScore = (int)value;
            Coin.Value = tempScore.ToString();
        }

        private void showPauseMenu()
        {
            if (_gameState == GameState.Gameover)
                return;
            _isPause = true;
            Debug.LogError("Show Pause Menu Game");
        }

        private void loadGameData(Profile.GameData gameData)
        {
            ScoreController.OnLoadGameData(gameData);
            TileController.OnLoadGameData(gameData);
            DragableBlockController.OnLoadGameData(gameData);
            _isBreakBestScore = gameData.IsBreakBestScore;
            _timeGameovers = gameData.TimesGameOver;
            registerEventsForDragableBlocks(DragableBlockController.DragableBlocks);
            if (_timer != null)
            {
                _timer.TakeTime(gameData.Duration);
            }
            checkAvailableSlots(0.25f, true);
        }

        private void saveGameData(ref Profile.GameData gameData)
        {
            if (gameData == null)
                gameData = new Profile.GameData();

            ScoreController.OnSaving(gameData);
            TileController.OnSaving(gameData);
            DragableBlockController.OnSaving(gameData);
            gameData.TimesGameOver = _timeGameovers;
            //PlayerPrefs.SetInt("TimeGameovers",_timeGameovers);
            if (_timer != null)
            {
                gameData.Duration = _timer.Seconds;
            }
            gameData.HasSaved = true;
        }

        private void onSaving(Profile.GameData gameData)
        {
            if (_gameState == GameState.Gameover)
            {
                Debug.LogError("Clear GameData");
                return;
            }

            ScoreController.OnSaving(gameData);
            TileController.OnSaving(gameData);
            DragableBlockController.OnSaving(gameData);
            gameData.TimesGameOver = _timeGameovers;
            if (_timer != null)
            {
                gameData.Duration = _timer.Seconds;
            }
            gameData.HasSaved = true;
        }

        private void showNewBestScore()
        {
            _isShowBestScoreEffect = true;
            _isBreakBestScore = true;
            AudioPlayer.Instance.PlaySound(C.AudioIds.Sound.BestScore);
            StartCoroutine(showBestScoreEffect());
        }

        private IEnumerator showBestScoreEffect()
        {
            yield return new WaitForEndOfFrame();
            Debug.LogError("Show best score anim");
        }

        private float getDeltaMoveForBlock(int blockType)
        {
            return C.GameplayConfig.BlockCellHeight * 0.5f * C.BlockHeights[blockType] + 145f;
        }

        public enum GameState
        {
            Playing,
            Gameover
        }
    }
}
