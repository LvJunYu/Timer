using System.Collections;
using System.Collections.Generic;
using GameA.Game;
using NewResourceSolution;
using UnityEngine;
using SoyEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlMobileInputControl : UICtrlGenericBase<UIViewMobileInputControl>
    {
        #region fields

        #endregion

        #region properties

        #endregion

        #region methods
        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InputCtrl;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<string>(EMessengerType.SetSkill2Icon, SetSkill2Icon);
            RegisterEvent<string>(EMessengerType.SetSkill3Icon, SetSkill3Icon);
            RegisterEvent<float, float>(EMessengerType.OnSkill2CDChanged, OnSkill2CDChanged);
            RegisterEvent<float, float>(EMessengerType.OnSkill3CDChanged, OnSkill3CDChanged);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.JumpBtn.OnPress += OnJumpButtonDown;
            _cachedView.JumpBtn.OnRelease+= OnJumpButtonUp;
            
            _cachedView.SkillBtn1.OnPress += OnSkill1ButtonDown;
            _cachedView.SkillBtn1.OnRelease+= OnSkill1ButtonUp;
            
            _cachedView.SkillBtn2.OnPress += OnSkill2ButtonDown;
            _cachedView.SkillBtn2.OnRelease+= OnSkill2ButtonUp;
            
            _cachedView.SkillBtn3.OnPress += OnSkill3ButtonDown;
            _cachedView.SkillBtn3.OnRelease+= OnSkill3ButtonUp;
            
            _cachedView.AssistBtn.OnPress += OnAssistButtonDown;
            _cachedView.AssistBtn.OnRelease+= OnAssistButtonUp;
        }

        private void SetSkill2Icon(string iconName)
        {
            _cachedView.SkillBtn2Icon.sprite = ResourcesManager.Instance.GetSprite(iconName);
        }

        private void SetSkill3Icon(string iconName)
        {
            _cachedView.SkillBtn3Icon.sprite = ResourcesManager.Instance.GetSprite(iconName);
        }

        private void OnSkill2CDChanged(float leftTime, float totalTime)
        {
            _cachedView.SkillBtn2CD.fillAmount = leftTime / totalTime;
        }
        
        private void OnSkill3CDChanged(float current, float max)
        {
            _cachedView.SkillBtn3CD.fillAmount = current / max;
        }
        

        private void OnJumpButtonDown()
        {
            CrossPlatformInputManager.SetButtonDown(InputManager.TagJump);
        }

        private void OnJumpButtonUp()
        {
            CrossPlatformInputManager.SetButtonUp(InputManager.TagJump);
        }
        private void OnSkill1ButtonUp()
        {
            CrossPlatformInputManager.SetButtonUp(InputManager.TagSkill1);
        }
        private void OnSkill1ButtonDown()
        {
            CrossPlatformInputManager.SetButtonDown(InputManager.TagSkill1);
        }

        private void OnSkill2ButtonUp()
        {
            CrossPlatformInputManager.SetButtonUp(InputManager.TagSkill2);
        }
        private void OnSkill2ButtonDown()
        {
            CrossPlatformInputManager.SetButtonDown(InputManager.TagSkill2);
        }

        private void OnSkill3ButtonUp()
        {
            CrossPlatformInputManager.SetButtonUp(InputManager.TagSkill3);
        }
        private void OnSkill3ButtonDown()
        {
            CrossPlatformInputManager.SetButtonDown(InputManager.TagSkill3);
        }

        private void OnAssistButtonUp()
        {
            CrossPlatformInputManager.SetButtonUp(InputManager.TagAssist);
        }
        private void OnAssistButtonDown()
        {
            CrossPlatformInputManager.SetButtonDown(InputManager.TagAssist);
        }

        #endregion
    }
}