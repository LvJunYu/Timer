using SoyEngine;

namespace GameA
{
    public class USCtrlGameSettingItem : USCtrlBase<USViewGameSettingItem>
    {
        private bool _isOn;
        private System.Action<bool> _cb;
        protected override void OnViewCreated ()
        {
            base.OnViewCreated ();
            _cachedView.Btn.onClick.AddListener (OnBtn);
        }

        public void SetData (bool isOn, System.Action<bool> cb)
        {
            _isOn = isOn;
            _cb = cb;
            RefreshView();
        }
        
        private void OnBtn ()
        {
            _isOn = !_isOn;
            RefreshView();
            if (null != _cb) {
                _cb.Invoke (_isOn);
            }
        }

        private void RefreshView()
        {
            _cachedView.On.SetActiveEx(_isOn);
            _cachedView.Off.SetActiveEx(!_isOn);
        }

        public void SetEnable(bool value)
        {
            _cachedView.SetActiveEx(value);
        }
    }
}