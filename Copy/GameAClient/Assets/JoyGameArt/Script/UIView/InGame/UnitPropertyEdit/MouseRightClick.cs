using GameA.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameA
{
    public class MouseRightClick : MonoBehaviour, IPointerClickHandler
    {
        public UnityAction RightMouseCallback;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right && RightMouseCallback != null)
            {
                RightMouseCallback.Invoke();
            }
        }
    }
}