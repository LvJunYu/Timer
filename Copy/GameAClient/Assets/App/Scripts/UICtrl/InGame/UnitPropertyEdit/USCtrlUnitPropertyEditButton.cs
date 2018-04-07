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

        public USViewUnitPropertyEditRotateMenuButton RotateMenuView
        {
            get { return _cachedView as USViewUnitPropertyEditRotateMenuButton; }
        }
        
        public USViewUnitPropertyEditSpawnButton SpawnMenuView
        {
            get { return _cachedView as USViewUnitPropertyEditSpawnButton; }
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
        }
        
        public void SetFgImage(Sprite sprite, bool native, int width, int height)
        {
            _cachedView.FgImage.sprite = sprite;
            if (native)
            {
                _cachedView.FgImage.SetNativeSize();
            }
            else
            {
                _cachedView.FgImage.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                _cachedView.FgImage.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
        }

        public void SetFgImage(bool isRotation)
        {
            if (isRotation)
            {
                _cachedView.FgImage.rectTransform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                _cachedView.FgImage.rectTransform.localEulerAngles = new Vector3(0, 180, 0);
            }
        }

        public void SetBgImage(Sprite sprite)
        {
            _cachedView.BgImage.sprite = sprite;
        }

        public void SetFgImageAngle(float degree)
        {
            _cachedView.FgImage.rectTransform.localEulerAngles = new Vector3(0, 0, -degree);
        }

        public void SetBgImageAngle(float degree)
        {
            _cachedView.BgImage.rectTransform.parent.localEulerAngles = new Vector3(0, 0, -degree);
        }

        public void SetText(string text)
        {
            DictionaryTools.SetContentText(_cachedView.FgText, text);
        }

        public void SetText2(string text)
        {
            USViewUnitPropertyEditMenuButton btn = _cachedView as USViewUnitPropertyEditMenuButton;
            if (btn != null)
            {
                DictionaryTools.SetContentText(btn.FgText2, text);
            }
        }

        public void SetSelected(bool selected)
        {
            _cachedView.Button.interactable = !selected;
        }

        public void SetEnable(bool enable)
        {
            _cachedView.SetActiveEx(enable);
        }

        public void SetColor(Color color)
        {
            _cachedView.BgImage.color = color;
        }

        public void AddDeleteBtnListener(UnityAction callback)
        {
            _cachedView.DeleteBtn.onClick.AddListener(callback);
        }

        public void SetDeleteBtnActive(bool value)
        {
            _cachedView.DeleteBtn.SetActiveEx(value);
        }

        public void SetBtnInteractable(bool value)
        {
            _cachedView.Button.interactable = value;
        }

        public bool GetBtnInteractable()
        {
            return _cachedView.Button.interactable;
        }
    }
}