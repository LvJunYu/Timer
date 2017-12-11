using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    public class USCtrlPreinstallItem : USCtrlBase<USViewPreinstallItem>
    {
        private UnitPreinstall _data;
        private int _index;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Tog.onValueChanged.AddListener(OnTogValueChanged);
        }

        private void OnTogValueChanged(bool arg0)
        {
            if (arg0)
            {
                Messenger<int>.Broadcast(EMessengerType.OnPreinstallRead, _index);
            }
        }

        public void SetTogGroup(ToggleGroup toggleGroup)
        {
            _cachedView.Tog.group = toggleGroup;
        }

        public void SetEnable(bool value)
        {
            _cachedView.SetActiveEx(value);
        }

        public void Set(UnitPreinstall data,int index)
        {
            _data = data;
            _index = index;
            _cachedView.NameTxt1.text = _cachedView.NameTxt2.text = _data.PreinstallData.Name;
        }
    }
}