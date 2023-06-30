using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{
    public class XHoldableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event System.Action<bool> OnHold;

		public List<Image> Images;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (OnHold != null) OnHold(true);
			foreach (var image in Images)
				image.enabled = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (OnHold != null) OnHold(false);
			foreach (var image in Images)
				image.enabled = true;
        }
    }
}
