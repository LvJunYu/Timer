using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

        public override void Init (USViewGameSettingItem view)
        {
            base.Init (view);
        }
        public void SetData (bool isOn, System.Action<bool> cb)
        {
            _isOn = isOn;
            _cb = cb;
        }
        private void OnBtn ()
        {
            _isOn = !_isOn;
            _cachedView.On.SetActive (_isOn);
            _cachedView.Off.SetActive (!_isOn);
            if (null != _cb) {
                _cb.Invoke (_isOn);
            }
        }
    }
}