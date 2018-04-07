using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace GameA.Game
{
    public class AdventureGuide_1_N_1 : AdventureGuideBase
    {
        private UICtrlUIGuideBubble _uiCtrlUIGuideBubble;
        private UICtrlGameBottomTip _uiCtrlGameBottomTip;

        private UMCtrlUIGuideBubble _moveGuideBubble;
        private bool _moveGuideFinish;
        private bool _inJumpGuide;
        private bool _inJumpMoveGuide;
        private bool _inJumpDoubleGuide;
        private const float _waitToShowTime = 0.5f;
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
            _uiCtrlGameBottomTip = SocialGUIManager.Instance.GetUI<UICtrlGameBottomTip>();
        }

        private void MoveGuide(bool flag)
        {
            if (_moveGuideFinish)
            {
                return;
            }
            if (flag)
            {
                if (Application.isMobilePlatform)
                {
                    var inputUI = SocialGUIManager.Instance.GetUI<UICtrlGameInput>();
                    if (inputUI.IsViewCreated)
                    {
                        _moveGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                            inputUI.CachedViewGame.JoyStickEx.RightArrowNormal.GetComponent<RectTransform>(),
                            EDirectionType.Down,
                            "长按这里移动");
                    }
                }
                else
                {
                    _uiCtrlGameBottomTip.ShowTip(string.Format("长按 ‘{0}’键 移动",
                        CrossPlatformInputManager.GetButtonPositiveKey(InputManager.TagHorizontal)));
                }
            }
            else
            {
                if (Application.isMobilePlatform)
                {
                    if (_moveGuideBubble != null)
                    {
                        _uiCtrlUIGuideBubble.CloseBubble(_moveGuideBubble);
                        _moveGuideBubble = null;
                    }
                }
                else
                {
                    _uiCtrlGameBottomTip.CloseTip();
                }
                _moveGuideFinish = true;
            }
        }

        private void JumpGuide(bool flag)
        {
            if (flag)
            {
                _inJumpGuide = true;
                CoroutineProxy.Instance.StartCoroutine(
                    CoroutineProxy.RunWaitForSeconds(_waitToShowTime, () =>
                    {
                        if (!_inJumpGuide) return;
                        if (Application.isMobilePlatform)
                        {
                            var inputUI = SocialGUIManager.Instance.GetUI<UICtrlGameInput>();
                            if (inputUI.IsViewCreated)
                            {
                                _jumpGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                                    inputUI.CachedViewGame.JumpBtn.rectTransform(), EDirectionType.Down,
                                    "长按这里跳跃");
                            }
                        }
                        else
                        {
                            _uiCtrlGameBottomTip.ShowTip(string.Format("长按 ‘{0}’键 跳跃",
                                CrossPlatformInputManager.GetButtonPositiveKey(InputManager.TagJump)));
                        }
                    }));
            }
            else
            {
                _inJumpGuide = false;
                if (Application.isMobilePlatform)
                {
                    if (_jumpGuideBubble != null)
                    {
                        _uiCtrlUIGuideBubble.CloseBubble(_jumpGuideBubble);
                        _jumpGuideBubble = null;
                    }
                }
                else
                {
                    _uiCtrlGameBottomTip.CloseTip();
                }
            }
        }

        private void JumpMoveGuide(bool flag)
        {
            _inJumpMoveGuide = flag;
            if (flag)
            {
                CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitForSeconds(_waitToShowTime, () =>
                {
                    if (!_inJumpMoveGuide) return;
                    if (Application.isMobilePlatform)
                    {
                        var inputUI = SocialGUIManager.Instance.GetUI<UICtrlGameInput>();
                        if (inputUI.IsViewCreated)
                        {
                            _jumpGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                                inputUI.CachedViewGame.JumpBtn.rectTransform(), EDirectionType.Down,
                                "长按这里跳跃");
                            _moveGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                                inputUI.CachedViewGame.JoyStickEx.RightArrowNormal.GetComponent<RectTransform>(),
                                EDirectionType.Down,
                                "长按这里向前");
                        }
                    }
                    else
                    {
                        _uiCtrlGameBottomTip.ShowTip(string.Format("同时长按‘{0}’键移动和‘{1}’键跳跃可以向前跳跃",
                            CrossPlatformInputManager.GetButtonPositiveKey(InputManager.TagHorizontal),
                            CrossPlatformInputManager.GetButtonPositiveKey(InputManager.TagJump)));
                    }
                }));
            }
            else
            {
                if (Application.isMobilePlatform)
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
                else
                {
                    _uiCtrlGameBottomTip.CloseTip();
                }
            }
        }

        private void JumpDoubleGuide(bool flag)
        {
            _inJumpDoubleGuide = flag;
            if (flag)
            {
                CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitForSeconds(_waitToShowTime, () =>
                {
                    if (!_inJumpDoubleGuide) return;
                    if (Application.isMobilePlatform)
                    {
                        var inputUI = SocialGUIManager.Instance.GetUI<UICtrlGameInput>();
                        if (inputUI.IsViewCreated)
                        {
                            _jumpGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                                inputUI.CachedViewGame.JumpBtn.rectTransform(), EDirectionType.Down,
                                "按两下跳跃");
                            _moveGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                                inputUI.CachedViewGame.JoyStickEx.RightArrowNormal.GetComponent<RectTransform>(),
                                EDirectionType.Down,
                                "长按这里向前");
                        }
                    }
                    else
                    {
                        _uiCtrlGameBottomTip.ShowTip(string.Format("长按‘{0}’键移动同时按两次‘{1}’键跳跃可以向前二连跳",
                            CrossPlatformInputManager.GetButtonPositiveKey(InputManager.TagHorizontal),
                            CrossPlatformInputManager.GetButtonPositiveKey(InputManager.TagJump)));
                    }
                }));
            }
            else
            {
                if (Application.isMobilePlatform)
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
                else
                {
                    _uiCtrlGameBottomTip.CloseTip();
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
            if (_uiCtrlGameBottomTip != null)
            {
                _uiCtrlGameBottomTip.CloseTip();
            }
            base.Dispose();
        }
    }
}