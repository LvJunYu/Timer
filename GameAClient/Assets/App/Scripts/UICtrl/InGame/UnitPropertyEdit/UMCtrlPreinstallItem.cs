using UnityEngine.UI;

namespace GameA
{
    public class UMCtrlPreinstallItem : UMCtrlBase<UMViewPreinstallItem>
    {
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Tog.onValueChanged.AddListener(OnTogValueChanged);
        }

        private void OnTogValueChanged(bool arg0)
        {
        }

        public void SetName(string name)
        {
            _cachedView.NameTxt1.text = _cachedView.NameTxt2.text = name;
        }
        
        public void SetTogGroup(ToggleGroup toggleGroup)
        {
            _cachedView.Tog.group = toggleGroup;
        }
    }
}