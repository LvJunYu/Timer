using UnityEngine;

namespace GameA.Game
{
    public class AdventureGuide_1_N_1 : AdventureGuideBase
    {
        private UICtrlUIGuideBubble _uiCtrlUIGuideBubble;
        
        private UMCtrlUIGuideBubble _moveGuideBubble;
        private bool _moveGuideFinish;

        private UMCtrlUIGuideBubble _jumpGuideBubble;

        public override void Init()
        {
            base.Init();
            _eventRegister = TriggerUnitEventManager.Instance.BeginRegistEvent()
                .RegistEvent("MoveGuide", MoveGuide)
                .RegistEvent("JumpGuide", JumpGuide)
                .RegistEvent("JumpMoveGuide", JumpMoveGuide)
                .RegistEvent("JumpDoubleGuide", JumpDoubleGuide)
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
                var inputUI = SocialGUIManager.Instance.GetUI<UICtrlMobileInputControl>();
                if (inputUI.IsViewCreated)
                {
                    _moveGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                        inputUI.CachedView.JoyStickEx.RightArrowNormal.GetComponent<RectTransform>(), EDirectionType.Down,
                        "长按这里移动");
                }
            }
            else
            {
                if (_moveGuideBubble != null)
                {
                    _moveGuideFinish = true;
                    _uiCtrlUIGuideBubble.CloseBubble(_moveGuideBubble);
                    _moveGuideBubble = null;
                }
            }
        }
        
        private void JumpGuide(bool flag)
        {
            if (flag)
            {
                var inputUI = SocialGUIManager.Instance.GetUI<UICtrlMobileInputControl>();
                if (inputUI.IsViewCreated)
                {
                    _jumpGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                        inputUI.CachedView.JumpBtn.rectTransform(), EDirectionType.Down,
                        "长按这里跳跃");
                }
            }
            else
            {
                if (_jumpGuideBubble != null)
                {
                    _uiCtrlUIGuideBubble.CloseBubble(_jumpGuideBubble);
                    _jumpGuideBubble = null;
                }
            }
        }
        
        private void JumpMoveGuide(bool flag)
        {
            if (flag)
            {
                var inputUI = SocialGUIManager.Instance.GetUI<UICtrlMobileInputControl>();
                if (inputUI.IsViewCreated)
                {
                    _jumpGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                        inputUI.CachedView.JumpBtn.rectTransform(), EDirectionType.Down,
                        "长按这里跳跃");
                    _moveGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                        inputUI.CachedView.JoyStickEx.RightArrowNormal.GetComponent<RectTransform>(),
                        EDirectionType.Down,
                        "长按这里向前");
                }
            }
            else
            {
                if (_moveGuideBubble != null)
                {
                    _uiCtrlUIGuideBubble.CloseBubble(_moveGuideBubble);
                    _moveGuideBubble = null;
                }
                if (_jumpGuideBubble != null)
                {
                    _uiCtrlUIGuideBubble.CloseBubble(_jumpGuideBubble);
                    _jumpGuideBubble = null;
                }
            }
        }
        
        private void JumpDoubleGuide(bool flag)
        {
            if (flag)
            {
                var inputUI = SocialGUIManager.Instance.GetUI<UICtrlMobileInputControl>();
                if (inputUI.IsViewCreated)
                {
                    _jumpGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                        inputUI.CachedView.JumpBtn.rectTransform(), EDirectionType.Down,
                        "按两下跳跃");
                    _moveGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                        inputUI.CachedView.JoyStickEx.RightArrowNormal.GetComponent<RectTransform>(),
                        EDirectionType.Down,
                        "长按这里向前");
                }
            }
            else
            {
                if (_moveGuideBubble != null)
                {
                    _uiCtrlUIGuideBubble.CloseBubble(_moveGuideBubble);
                    _moveGuideBubble = null;
                }
                if (_jumpGuideBubble != null)
                {
                    _uiCtrlUIGuideBubble.CloseBubble(_jumpGuideBubble);
                    _jumpGuideBubble = null;
                }
            }
        }

        public override void Dispose()
        {
            if (_moveGuideBubble != null)
            {
                _uiCtrlUIGuideBubble.CloseBubble(_moveGuideBubble);
                _moveGuideBubble = null;
            }
            if (_jumpGuideBubble != null)
            {
                _uiCtrlUIGuideBubble.CloseBubble(_jumpGuideBubble);
                _jumpGuideBubble = null;
            }
            base.Dispose();
        }
    }
}