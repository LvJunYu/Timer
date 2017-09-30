using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace GameA.Game
{
    public class AdventureGuide_1_N_6 : AdventureGuideBase
    {
        private UICtrlUIGuideBubble _uiCtrlUIGuideBubble;
        private UICtrlGameBottomTip _uiCtrlGameBottomTip;
        
        private UMCtrlUIGuideBubble _skillDirGuideBubble;
        private UMCtrlUIGuideBubble _skillGuideBubble;

        private bool _hFinish;
        private bool _vFinish;

        public override void Init()
        {
            base.Init();
            _eventRegister = TriggerUnitEventManager.Instance.BeginRegistEvent()
                .RegistEvent("SkillHGuide", SkillHGuide)
                .RegistEvent("SkillHGuideEnd", SkillHGuideEnd)
                .RegistEvent("SkillVGuide", SkillVGuide)
                .RegistEvent("SkillVGuideEnd", SkillVGuideEnd)
                .End();
            _uiCtrlUIGuideBubble = SocialGUIManager.Instance.GetUI<UICtrlUIGuideBubble>();
            _uiCtrlGameBottomTip = SocialGUIManager.Instance.GetUI<UICtrlGameBottomTip>();
        }

        private void SkillHGuide(bool flag)
        {
            if (_hFinish)
            {
                return;
            }
            if (Application.isMobilePlatform)
            {
                if (flag && _skillGuideBubble == null)
                {
                    var inputUI = SocialGUIManager.Instance.GetUI<UICtrlGameInput>();
                    if (inputUI.IsViewCreated)
                    {
                        _skillGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                            inputUI.UsSkillBtns[0].CachedView.BtnIcon.rectTransform, EDirectionType.Down,
                            "点击这里喷水");
                    }
                }
            }
            else
            {
                if (flag && !_uiCtrlGameBottomTip.IsOpen)
                {
                    _uiCtrlGameBottomTip.ShowTip(string.Format("按 ‘{0}’键 喷水’",
                        CrossPlatformInputManager.GetButtonPositiveKey(InputManager.TagSkill1)));
                }
            }
        }
        
        private void SkillHGuideEnd(bool flag)
        {
            if (_hFinish)
            {
                return;
            }
            _hFinish = true;
            if (Application.isMobilePlatform)
            {
                if (_skillGuideBubble != null)
                {
                    _uiCtrlUIGuideBubble.CloseBubble(_skillGuideBubble);
                    _skillGuideBubble = null;
                }
            }
            else
            {
                if (_uiCtrlGameBottomTip.IsOpen)
                {
                    _uiCtrlGameBottomTip.CloseTip();
                }
            }
        }
        
        private void SkillVGuide(bool flag)
        {
            if (_vFinish)
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
                        _skillGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                            inputUI.UsSkillBtns[0].CachedView.BtnIcon.rectTransform, EDirectionType.Down,
                            "点击这里喷水");
                        _skillDirGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                            inputUI.CachedViewGame.JoyStickEx.UpArrowNormal.GetComponent<RectTransform>(),
                            EDirectionType.Down,
                            "同时按下这里可以改变喷水方向");
                    }
                }
                else
                {
                    _uiCtrlGameBottomTip.ShowTip(string.Format("按‘{0}’键喷水同时按下‘{1}’键可以改变喷水方向",
                        CrossPlatformInputManager.GetButtonPositiveKey(InputManager.TagSkill1),
                        CrossPlatformInputManager.GetButtonPositiveKey(InputManager.TagVertical)));
                }
            }
            else
            {
                if (Application.isMobilePlatform)
                {
                    if (_skillGuideBubble != null)
                    {
                        _uiCtrlUIGuideBubble.CloseBubble(_skillGuideBubble);
                        _skillGuideBubble = null;
                    }
                    if (_skillDirGuideBubble != null)
                    {
                        _uiCtrlUIGuideBubble.CloseBubble(_skillDirGuideBubble);
                        _skillDirGuideBubble = null;
                    }
                }
                else
                {
                    _uiCtrlGameBottomTip.CloseTip();
                }
            }
        }
        
        private void SkillVGuideEnd(bool flag)
        {
            if (_vFinish)
            {
                return;
            }
            _vFinish = true;
            if (Application.isMobilePlatform)
            {
                if (_skillGuideBubble != null)
                {
                    _uiCtrlUIGuideBubble.CloseBubble(_skillGuideBubble);
                    _skillGuideBubble = null;
                }
                if (_skillDirGuideBubble != null)
                {
                    _uiCtrlUIGuideBubble.CloseBubble(_skillDirGuideBubble);
                    _skillDirGuideBubble = null;
                }
            }
            else
            {
                if (_uiCtrlGameBottomTip.IsOpen)
                {
                    _uiCtrlGameBottomTip.CloseTip();
                }
            }
        }
        
        public override void Dispose()
        {
            if (_skillGuideBubble != null)
            {
                _uiCtrlUIGuideBubble.CloseBubble(_skillGuideBubble);
                _skillGuideBubble = null;
            }
            if (_skillDirGuideBubble != null)
            {
                _uiCtrlUIGuideBubble.CloseBubble(_skillDirGuideBubble);
                _skillDirGuideBubble = null;
            }
            if (_uiCtrlGameBottomTip != null)
            {
                _uiCtrlGameBottomTip.CloseTip();
            }
            base.Dispose();
        }
    }
}