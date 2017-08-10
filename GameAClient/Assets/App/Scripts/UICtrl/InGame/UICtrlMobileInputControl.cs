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
            RegisterEvent<Table_Skill, int>(EMessengerType.OnSkillSlotChanged, OnSkillSlotChanged);
            RegisterEvent<float, float>(EMessengerType.OnSkill1CDChanged, OnSkill1CDChanged);
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

        private void OnSkillSlotChanged(Table_Skill tableSkill, int slot)
        {
            if (null == _cachedView) return;
            if (null == tableSkill) return;
            if (0 > slot || slot > 2) return;
            int bgIdx = 0;
            int cdType = -1;
            if (tableSkill.CostType == (int) ECostType.Magic)
            {
                bgIdx = 0;
                cdType = 0;
            }
            else if (tableSkill.CostType == (int) ECostType.Paint)
            {
                bgIdx = 1;
                cdType = -1;
            }
            else if (tableSkill.CostType == (int) ECostType.Rage)
            {
                bgIdx = 2;
                cdType = 1;
            }

            if (slot == 0)
            {
                SetSkill1Type(bgIdx, cdType, tableSkill.Icon);
            }
            else if (slot == 1)
            {
                SetSkill2Type(bgIdx, cdType, tableSkill.Icon);
            }
            else if (slot == 2)
            {
                SetSkill3Type(bgIdx, cdType, tableSkill.Icon);
            }

        }
        private void SetSkill1Type(int bg, int cdType, string icon)
        {
            if (null == _cachedView) return;
            bg = Mathf.Clamp(bg, 0, 3);
            for (int i = 0; i < _cachedView.Btn1ColorBgArray.Length; i++)
            {
                if (i == bg)
                {
                    _cachedView.Btn1ColorBgArray[i].SetActive(true);
                }
                else
                {
                    _cachedView.Btn1ColorBgArray[i].SetActive(false);
                }
            }
            _cachedView.Btn1Icon.sprite = ResourcesManager.Instance.GetSprite(icon);
            _cachedView.Btn1Icon.gameObject.SetActive(true);
            _cachedView.Btn1CD1.gameObject.SetActive(cdType == 0);
            _cachedView.Btn1CD2.gameObject.SetActive(cdType == 1);
        }
        private void SetSkill2Type(int bg, int cdType, string icon)
        {
            if (null == _cachedView) return;
            bg = Mathf.Clamp(bg, 0, 3);
            for (int i = 0; i < _cachedView.Btn2ColorBgArray.Length; i++)
            {
                if (i == bg)
                {
                    _cachedView.Btn2ColorBgArray[i].SetActive(true);
                }
                else
                {
                    _cachedView.Btn2ColorBgArray[i].SetActive(false);
                }
            }
            _cachedView.Btn2Icon.sprite = ResourcesManager.Instance.GetSprite(icon);
            _cachedView.Btn2Icon.gameObject.SetActive(true);
            _cachedView.Btn2CD1.gameObject.SetActive(cdType == 0);
            _cachedView.Btn2CD2.gameObject.SetActive(cdType == 1);
        }
        private void SetSkill3Type(int bg, int cdType, string icon)
        {
            if (null == _cachedView) return;
            bg = Mathf.Clamp(bg, 0, 3);
            for (int i = 0; i < _cachedView.Btn3ColorBgArray.Length; i++)
            {
                if (i == bg)
                {
                    _cachedView.Btn3ColorBgArray[i].SetActive(true);
                }
                else
                {
                    _cachedView.Btn3ColorBgArray[i].SetActive(false);
                }
            }
            _cachedView.Btn3Icon.sprite = ResourcesManager.Instance.GetSprite(icon);
            _cachedView.Btn3Icon.gameObject.SetActive(true);
            _cachedView.Btn3CD1.gameObject.SetActive(cdType == 0);
            _cachedView.Btn3CD2.gameObject.SetActive(cdType == 0);
        }
        
//        private void SetSkill1Icon(string iconName)
//        {
//            if (null == _cachedView) return;
//            _cachedView.Btn1Icon.sprite = ResourcesManager.Instance.GetSprite(iconName);
//        }
//        private void SetSkill2Icon(string iconName)
//        {
//            if (null == _cachedView) return;
//            _cachedView.Btn2Icon.sprite = ResourcesManager.Instance.GetSprite(iconName);
//        }
//
//        private void SetSkill3Icon(string iconName)
//        {
//            if (null == _cachedView) return;
//            _cachedView.Btn3Icon.sprite = ResourcesManager.Instance.GetSprite(iconName);
//        }

        private void OnSkill1CDChanged(float leftTime, float totalTime)
        {
            if (null == _cachedView) return;
            _cachedView.Btn1CD1.fillAmount = leftTime / totalTime;
            _cachedView.Btn1CD2.fillAmount = leftTime / totalTime;
        }
        private void OnSkill2CDChanged(float leftTime, float totalTime)
        {
            if (null == _cachedView) return;
            _cachedView.Btn2CD1.fillAmount = leftTime / totalTime;
            _cachedView.Btn2CD2.fillAmount = leftTime / totalTime;
        }
        private void OnSkill3CDChanged(float current, float max)
        {
            if (null == _cachedView) return;
            _cachedView.Btn3CD1.fillAmount = current / max;
            _cachedView.Btn3CD2.fillAmount = current / max;
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