using GameA.Game;
using SoyEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame)]
    public class UICtrlMobileInputControl : UICtrlInGameBase<UIViewMobileInputControl>
    {
        private UMCtrlSkillBtn[] _umSkillBtns;
        private Table_Equipment[] _equipments = new Table_Equipment[3];

        public UIViewMobileInputControl CachedView
        {
            get { return _cachedView; }
        }

        public UMCtrlSkillBtn[] UmSkillBtns
        {
            get { return _umSkillBtns; }
        }

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
            {
                CreateUMSkillBtns();
            }
            _cachedView.JumpBtn.OnPress += OnJumpButtonDown;
            _cachedView.JumpBtn.OnPress += PlayClickParticle;
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

        protected override void OnDestroy()
        {
            for (int i = 0; i < _umSkillBtns.Length; i++)
            {
                _umSkillBtns[i].Dispose();
            }
            for (int i = 0; i < _equipments.Length; i++)
            {
                _equipments[i] = null;
            }
            _umSkillBtns = null;
            base.OnDestroy();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            RefreshSkillBtns();
            _cachedView.AssistBtn.gameObject.SetActive(false);
        }

        protected override void OnClose()
        {
            base.OnClose();
            for (int i = 0; i < _equipments.Length; i++)
            {
                _equipments[i] = null;
            }
        }

        public void SetSkillBtnVisible(int slot, bool visible)
        {
            if (null == _cachedView)
            {
                return;
            }
            if (null == _umSkillBtns)
            {
                CreateUMSkillBtns();
            }
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

        private void RefreshSkillBtns()
        {
            for (int i = 0; i < _cachedView.SkillRTFs.Length; i++)
            {
                if (_equipments[i] != null)
                {
                    _cachedView.SkillRTFs[i].gameObject.SetActive(true);
                    _umSkillBtns[i].SetData(_equipments[i]);
                }
                else
                {
                    _cachedView.SkillRTFs[i].gameObject.SetActive(false);

                }
            }
        }

        private void PlayClickParticle()
        {
            GameParticleManager.Instance.EmitUIParticle("UIEffectClickBig", _cachedView.JumpBtn.transform, _groupId,
                0.5f);
        }

        private void OnSkillSlotChanged(Table_Equipment tableSkill, int slot)
        {
            if (!_isViewCreated)
            {
                return;
            }
            if (null == tableSkill) return;
            if (0 > slot || slot > _equipments.Length - 1) return;
            _equipments[slot] = tableSkill;
            if (null == _cachedView) return;
            if (null == _umSkillBtns)
                CreateUMSkillBtns();
            _umSkillBtns[slot].SetData(tableSkill);
            RefreshSkillBtns();
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
                umCtrlSkillBtn.Init(_cachedView.SkillRTFs[i], ResScenary);
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
    }
}