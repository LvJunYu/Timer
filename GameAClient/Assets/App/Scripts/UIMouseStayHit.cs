using GameA;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoyEngine
{
    public class UIMouseStayHit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string Content = "请输入内容";
        public int Width = 200;
        public int TopDistance = 40;
        public TextAnchor TextAnchor = TextAnchor.UpperLeft;
        public EResScenary ResScenary;
        private UMCtrlStayHint _umCtrlStayHint;

        public void OnPointerEnter(PointerEventData eventData)
        {
            ShowHint();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HideHint();
        }

        private void HideHint()
        {
            if (_umCtrlStayHint != null)
            {
                _umCtrlStayHint.Hide();
            }
        }

        private void ShowHint()
        {
            if (_umCtrlStayHint == null)
            {
                _umCtrlStayHint = new UMCtrlStayHint();
                _umCtrlStayHint.Init(GetComponent<RectTransform>(), ResScenary);
            }
            _umCtrlStayHint.Set(Content, Width, TopDistance, TextAnchor);
        }
    }
}