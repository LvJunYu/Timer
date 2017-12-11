using SoyEngine;
using UnityEngine.Events;

namespace GameA
{
    public class USCtrlPreinstallItem : USCtrlBase<USViewPreinstallItem>
    {
        private UnitPreinstall _data;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.InputField.onEndEdit.AddListener(OnInputEndEdit);
        }

        private void OnInputEndEdit(string arg0)
        {
            
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
            _cachedView.NameTxt.text = _cachedView.InputField.text = _data.PreinstallData.Name;
        }

        public void SetSelected(bool value)
        {
            _cachedView.InputField.SetActiveEx(value);
        }
    }
}