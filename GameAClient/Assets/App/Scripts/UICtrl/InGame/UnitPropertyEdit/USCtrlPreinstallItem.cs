using SoyEngine;
using UnityEngine.Events;

namespace GameA
{
    public class USCtrlPreinstallItem : USCtrlBase<USViewPreinstallItem>
    {
        private UnitPreinstall _data;

        public void AddInputEndEditListener(UnityAction<string> onEndEdit)
        {
            _cachedView.InputField.onEndEdit.AddListener(onEndEdit);
        }

        public void AddListener(UnityAction onBtn)
        {
            _cachedView.Btn.onClick.AddListener(onBtn);
        }

        public void SetEnable(bool value)
        {
            _cachedView.SetActiveEx(value);
        }

        public void Set(UnitPreinstall data)
        {
            _data = data;
            SetText(_data.PreinstallData.Name);
        }

        public void SetText(string content)
        {
            _cachedView.NameTxt.text = _cachedView.InputField.text = content;
        }

        public void SetSelected(bool value)
        {
            _cachedView.InputField.SetActiveEx(value);
        }
    }
}