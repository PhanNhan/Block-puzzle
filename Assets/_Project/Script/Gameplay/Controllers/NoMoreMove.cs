using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ClassicMode.Gameplay.Controllers
{
    public class NoMoreMove : MonoBehaviour
    {
        const float MinSize = 20f;
        const float MaxSize = 700f;

        public CanvasGroup Panel;
        public RectTransform Background;
        public Text Msg;
        public Transform Root;
        private Vector3 originalPos;
        private Sequence noMoreMoveSequence;

        public void Init()
        {
            originalPos = Root.position;
            OnReset();
        }

        public void OnReset()
        {
            Panel.alpha = 0f;
            Background.sizeDelta = new Vector2(MinSize, Background.sizeDelta.y);
            //Msg.text = "";
            Root.position = originalPos;
            gameObject.SetActive(false);
            noMoreMoveSequence.Kill();
        }

        public void Show(float delayShowInSeconds, System.Action onCompleted, string audioId)
		{
            Vector3 desPos = GetHeightOfGameObject(transform) * 0.5f * Vector3.down + new Vector3(0,-100f,0);
			float speed = 2f;
			gameObject.SetActive (true);
            noMoreMoveSequence= DOTween.Sequence();
            noMoreMoveSequence.Append(Panel.DOFade (1.0f, 0.2f * speed));
            noMoreMoveSequence.Append(Background.DOSizeDelta (new Vector2 (MaxSize, Background.sizeDelta.y), 0.3f * speed));
            noMoreMoveSequence.Append(Msg.transform.DOScale (Msg.transform.localScale, 0.3f));
            noMoreMoveSequence.Append(Root.DOLocalMove(desPos,0.3f));
            noMoreMoveSequence.OnStart(()=>{
                AudioPlayer.Instance.PlaySound (audioId);
            });
            noMoreMoveSequence.SetDelay(delayShowInSeconds);
            noMoreMoveSequence.OnComplete(()=>{
                if(onCompleted!=null)
                    onCompleted();
            });

		}

        private float GetHeightOfGameObject (Transform trans)
		{
			RectTransform rect = (RectTransform)trans;
			return rect.rect.height;
		}
    }
}
