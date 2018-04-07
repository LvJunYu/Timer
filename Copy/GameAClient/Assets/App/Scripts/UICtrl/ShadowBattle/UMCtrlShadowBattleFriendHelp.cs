using UnityEngine.UI;

namespace GameA
{
    public class UMCtrlShadowBattleFriendHelp : UMCtrlWorldShareProject
    {
        protected override void OnHeadBtn()
        {
        }

        public void SetToggleGroup(ToggleGroup toggleGroup)
        {
            if (_cachedView != null)
            {
                _cachedView.SelectTog.group = toggleGroup;
            }
        }
    }
}