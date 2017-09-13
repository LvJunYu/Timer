using UnityEngine;

namespace GameA.Game
{
    public class AdventureGuide_1_N_1 : AdventureGuideBase
    {
        private UICtrlUIGuideBubble _uiCtrlUIGuideBubble;
        
        private UMCtrlUIGuideBubble _moveGuideBubble;
        private bool _moveGuideFinish;

        private UMCtrlUIGuideBubble _jumpGuideBubble;

        private EState _goalGuideState;
        
        public override void Init()
        {
            base.Init();
            _eventRegister = TriggerUnitEventManager.Instance.BeginRegistEvent()
                .RegistEvent("MoveGuide", MoveGuide)
                .RegistEvent("JumpGuide", JumpGuide)
                .RegistEvent("JumpMoveGuide", JumpMoveGuide)
                .RegistEvent("JumpDoubleGuide", JumpDoubleGuide)
                .RegistEvent("GoalGuide", GoalGuide)
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
                _moveGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                    inputUI.CachedView.JoyStickEx.RightArrowNormal.GetComponent<RectTransform>(), EDirectionType.Down,
                    "长按这里移动");
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
                _jumpGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                    inputUI.CachedView.JumpBtn.rectTransform(), EDirectionType.Down,
                    "长按这里跳跃");
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
                _jumpGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                    inputUI.CachedView.JumpBtn.rectTransform(), EDirectionType.Down,
                    "长按这里跳跃");
                _moveGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                    inputUI.CachedView.JoyStickEx.RightArrowNormal.GetComponent<RectTransform>(), EDirectionType.Down,
                    "长按这里向前");
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
                _jumpGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                    inputUI.CachedView.JumpBtn.rectTransform(), EDirectionType.Down,
                    "按两下跳跃");
                _moveGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                    inputUI.CachedView.JoyStickEx.RightArrowNormal.GetComponent<RectTransform>(), EDirectionType.Down,
                    "长按这里向前");
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
        
        private void GoalGuide(bool flag)
        {
            if (_goalGuideState == EState.Finish)
            {
                return;
            }
            if (flag)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlInGameUnitHandbook>(5001);
                GM2DGame.Instance.Pause();
                _goalGuideState = EState.Doing;
            }
        }

        public override void Update()
        {
            base.Update();
            if (_goalGuideState == EState.Doing && !SocialGUIManager.Instance.GetUI<UICtrlInGameUnitHandbook>().IsOpen)
            {
                _goalGuideState = EState.Finish;
                GM2DGame.Instance.Continue();
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
        
        private enum EState
        {
            None,
            Doing,
            Finish
        }
    }
}