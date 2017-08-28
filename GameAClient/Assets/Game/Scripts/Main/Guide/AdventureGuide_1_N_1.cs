using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class AdventureGuide_1_N_1 : AdventureGuideBase
    {
        private TriggerUnitEventManager.EventRegister _eventRegister;
        private UICtrlUIGuideBubble _uiCtrlUIGuideBubble;
        
        private UMCtrlUIGuideBubble _moveGuideBubble;
        private bool _moveGuideFinish;
        
        public override void Init()
        {
            base.Init();
            _eventRegister = TriggerUnitEventManager.Instance.BeginRegistEvent()
                .RegistEvent("MoveGuide", MoveGuide)
                .End();
            _uiCtrlUIGuideBubble = SocialGUIManager.Instance.GetUI<UICtrlUIGuideBubble>();
        }

        private void MoveGuide(bool flag)
        {
            if (_moveGuideFinish)
            {
                return;
            }
            if (flag)
            {
                _moveGuideFinish = true;
                var inputUI = SocialGUIManager.Instance.GetUI<UICtrlMobileInputControl>();
                _moveGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                    inputUI.CachedView.JoyStickEx.RightArrowNormal.GetComponent<RectTransform>(), EDirectionType.Down,
                    "长按这里移动");
            }
            else
            {
                if (_moveGuideBubble != null)
                {
                    _uiCtrlUIGuideBubble.CloseBubble(_moveGuideBubble);
                    _moveGuideBubble = null;
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _eventRegister.UnregisterAllEvent();
        }
    }
}