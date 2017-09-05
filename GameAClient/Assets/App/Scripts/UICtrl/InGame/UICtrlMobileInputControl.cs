using GameA.Game;
using SoyEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlMobileInputControl : UICtrlGenericBase<UIViewMobileInputControl>
    {
        #region fields

        private UMCtrlSkillBtn[] _umSkillBtns;

        #endregion

        #region properties

        public UIViewMobileInputControl CachedView
        {
            get { return _cachedView; }
        }

        #endregion

        #region methods

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InputCtrl;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<Table_Equipment, int>(EMessengerType.OnSkillSlotChanged, OnSkillSlotChanged);
            RegisterEvent<int, float, float>(EMessengerType.OnSkillCDTime, OnSkillCDTime);
            RegisterEvent<int, float, float>(EMessengerType.OnSkillChargeTime, OnSkillChargeTime);
            RegisterEvent<int, int, int>(EMessengerType.OnSkillBulletChanged, OnSkillBulletChanged);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            if (null == _umSkillBtns)
                CreateUMSkillBtns();
            _cachedView.JumpBtn.OnPress += OnJumpButtonDown;
            _cachedView.JumpBtn.OnRelease += OnJumpButtonUp;

            _umSkillBtns[0].CachedView.SkillBtn.OnPress += OnSkill1ButtonDown;
            _umSkillBtns[0].CachedView.SkillBtn.OnRelease += OnSkill1ButtonUp;

            _umSkillBtns[1].CachedView.SkillBtn.OnPress += OnSkill2ButtonDown;
            _umSkillBtns[1].CachedView.SkillBtn.OnRelease += OnSkill2ButtonUp;

            _umSkillBtns[2].CachedView.SkillBtn.OnPress += OnSkill3ButtonDown;
            _umSkillBtns[2].CachedView.SkillBtn.OnRelease += OnSkill3ButtonUp;

            _cachedView.AssistBtn.OnPress += OnAssistButtonDown;
            _cachedView.AssistBtn.OnRelease += OnAssistButtonUp;
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
//            for (int i = 0; i < _cachedView.SkillRTFs.Length; i++)
//            {
//                _cachedView.SkillRTFs[i].gameObject.SetActive(false);
//            }
            _cachedView.AssistBtn.gameObject.SetActive(false);
            OnSkillSlotChanged(TableManager.Instance.Table_EquipmentDic[101], 0);
            OnSkillSlotChanged(TableManager.Instance.Table_EquipmentDic[201], 1);
            OnSkillSlotChanged(TableManager.Instance.Table_EquipmentDic[203], 2);

        }

        public void SetSkillBtnVisible(int slot, bool visible)
        {
            if (null == _cachedView) return;
            if (null == _umSkillBtns)
                CreateUMSkillBtns();
            switch (slot)
            {
                case 0:
                    _umSkillBtns[0].CachedView.SkillBtn.gameObject.SetActive(visible);
                    break;
                case 1:
                    _umSkillBtns[1].CachedView.SkillBtn.gameObject.SetActive(visible);
                    break;
                case 2:
                    _umSkillBtns[2].CachedView.SkillBtn.gameObject.SetActive(visible);
                    break;
            }
        }

        public void SetAssistBtnVisible(bool visible)
        {
            if (null == _cachedView) return;
            _cachedView.AssistBtn.gameObject.SetActive(visible);
        }

        private void OnSkillSlotChanged(Table_Equipment tableSkill, int slot)
        {
            if (null == _cachedView) return;
            if (null == tableSkill) return;
            if (null == _umSkillBtns)
                CreateUMSkillBtns();
            if (0 > slot || slot > _umSkillBtns.Length - 1) return;
            _umSkillBtns[slot].SetData(tableSkill);
        }

        private void OnSkillChargeTime(int slot, float leftTime, float totalTime)
        {
            if (null == _cachedView) return;
            _umSkillBtns[slot].OnSkillChargeTime(leftTime, totalTime);
        }

        private void OnSkillCDTime(int slot, float leftTime, float totalTime)
        {
            if (null == _cachedView) return;
            _umSkillBtns[slot].OnSkillCDTime(leftTime, totalTime);
        }

        private void OnSkillBulletChanged(int slot, int leftCount, int totalCount)
        {
            if (null == _cachedView) return;
            _umSkillBtns[slot].OnSkillBulletChanged(leftCount, totalCount);
        }

        private void CreateUMSkillBtns()
        {
            int skillNum = _cachedView.SkillRTFs.Length;
            _umSkillBtns = new UMCtrlSkillBtn[skillNum];
            for (int i = 0; i < skillNum; i++)
            {
                UMCtrlSkillBtn umCtrlSkillBtn = new UMCtrlSkillBtn();
                umCtrlSkillBtn.Init(_cachedView.SkillRTFs[i]);
                _umSkillBtns[i] = umCtrlSkillBtn;
            }
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