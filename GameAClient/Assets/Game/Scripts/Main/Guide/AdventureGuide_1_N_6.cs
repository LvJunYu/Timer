using UnityEngine;

namespace GameA.Game
{
    public class AdventureGuide_1_N_6 : AdventureGuideBase
    {
        private UICtrlUIGuideBubble _uiCtrlUIGuideBubble;
        
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
        }

        private void SkillHGuide(bool flag)
        {
            if (_hFinish)
            {
                return;
            }
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
        
        private void SkillHGuideEnd(bool flag)
        {
            if (_hFinish)
            {
                return;
            }
            _hFinish = true;
            if (_skillGuideBubble != null)
            {
                _uiCtrlUIGuideBubble.CloseBubble(_skillGuideBubble);
                _skillGuideBubble = null;
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
                var inputUI = SocialGUIManager.Instance.GetUI<UICtrlGameInput>();
                if (inputUI.IsViewCreated)
                {
                    _skillGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                        inputUI.UsSkillBtns[0].CachedView.BtnIcon.rectTransform, EDirectionType.Down,
                        "点击这里喷水");
                    _skillDirGuideBubble = _uiCtrlUIGuideBubble.ShowBubble(
                        inputUI.CachedViewGame.JoyStickEx.UpArrowNormal.GetComponent<RectTransform>(), EDirectionType.Down,
                        "同时按下这里可以改变喷水方向");
                }
            }
            else
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
        }
        
        private void SkillVGuideEnd(bool flag)
        {
            if (_vFinish)
            {
                return;
            }
            _vFinish = true;
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
            base.Dispose();
        }
    }
}