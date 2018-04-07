using UnityEngine;
using SoyEngine;

namespace GameA
{
    public class UMCtrlStayHint : UMCtrlBase<UMViewStayHint>
    {
        public void Set(string content, int width, int topDis, TextAnchor textAnchor)
        {
            _cachedView.Content.text = content;
            _cachedView.PannelRtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            _cachedView.Trans.anchoredPosition = new Vector2(0, -topDis);
            _cachedView.LayoutElement.enabled = false;
            _cachedView.Content.alignment = textAnchor;
            Show();
            Canvas.ForceUpdateCanvases();
            //单行时的处理
            if (_cachedView.Content.rectTransform.rect.height + _cachedView.VerticalLayoutGroup.padding.bottom + 10 <
                _cachedView.PannelRtf.rect.height)
            {
                _cachedView.LayoutElement.enabled = true;
            }
        }

        public void SetVertical(int topPadding, int bottomPadding, int leftPadding, int rightPadding)
        {
            _cachedView.VerticalLayoutGroup.padding.top = topPadding;
            _cachedView.VerticalLayoutGroup.padding.bottom = bottomPadding;
            _cachedView.VerticalLayoutGroup.padding.left = leftPadding;
            _cachedView.VerticalLayoutGroup.padding.right = rightPadding;
        }

        public void Show()
        {
            _cachedView.SetActiveEx(true);
        }

        public void Hide()
        {
            _cachedView.SetActiveEx(false);
        }
    }
}