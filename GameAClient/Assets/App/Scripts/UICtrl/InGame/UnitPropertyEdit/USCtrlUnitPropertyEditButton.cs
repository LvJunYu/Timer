using SoyEngine;
using UnityEngine;
using UnityEngine.Events;

namespace GameA
{
    public class USCtrlUnitPropertyEditButton : USCtrlBase<USViewUnitPropertyEditButton>
    {
        public USViewUnitPropertyEditButton View
        {
            get { return _cachedView; }
        }
        
        public void AddClickListener(UnityAction callback)
        {
            _cachedView.Button.onClick.AddListener(callback);
        }

        public void SetPosAngle(float degree, float radius)
        {
            _cachedView.ContentDock.anchoredPosition =
                new Vector2(Mathf.Sin(degree * Mathf.Deg2Rad), Mathf.Cos(degree * Mathf.Deg2Rad)) * radius;
        }

        public void SetFgImage(Sprite sprite)
        {
            _cachedView.FgImage.sprite = sprite;
            _cachedView.FgImage.SetNativeSize();
        }

        public void SetFgImageAngle(float degree)
        {
            _cachedView.FgImage.rectTransform.localEulerAngles = new Vector3(0, 0, -degree);
        }

        public void SetText(string text)
        {
            DictionaryTools.SetContentText(_cachedView.FgText, text);
        }

        public void SetSelected(bool selected)
        {
            _cachedView.Button.interactable = !selected;
        }

        public void SetEnable(bool enable)
        {
            _cachedView.SetActiveEx(enable);
        }
    }
}
