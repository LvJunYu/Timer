using GameA.Game;
using SoyEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame)]
    public class UICtrlGameInputControl : UICtrlInGameBase<UIViewGameInputControl>
    {
        private USCtrlSkillBtn[] _usSkillBtns;
        private Table_Equipment[] _equipments = new Table_Equipment[3];

        public UIViewGameInputControl CachedViewGame
        {
            get { return _cachedView; }
        }

        public USCtrlSkillBtn[] UsSkillBtns
        {
            get { return _usSkillBtns; }
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
            if (null == _usSkillBtns)
            {
                CreateUMSkillBtns();
            }
            _cachedView.JumpBtn.OnPress += OnJumpButtonDown;
            _cachedView.JumpBtn.OnPress += PlayClickParticle;
            _cachedView.JumpBtn.OnRelease += OnJumpButtonUp;

            _usSkillBtns[0].CachedView.SkillBtn.OnPress += OnSkill1ButtonDown;
            _usSkillBtns[0].CachedView.SkillBtn.OnRelease += OnSkill1ButtonUp;

            _usSkillBtns[1].CachedView.SkillBtn.OnPress += OnSkill2ButtonDown;
            _usSkillBtns[1].CachedView.SkillBtn.OnRelease += OnSkill2ButtonUp;

            _usSkillBtns[2].CachedView.SkillBtn.OnPress += OnSkill3ButtonDown;
            _usSkillBtns[2].CachedView.SkillBtn.OnRelease += OnSkill3ButtonUp;

            _cachedView.AssistBtn.OnPress += OnAssistButtonDown;
            _cachedView.AssistBtn.OnRelease += OnAssistButtonUp;
        }

        protected override void OnDestroy()
        {
            for (int i = 0; i < _usSkillBtns.Length; i++)
            {
                _usSkillBtns[i].Dispose();
            }
            for (int i = 0; i < _equipments.Length; i++)
            {
                _equipments[i] = null;
            }
            _usSkillBtns = null;
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
            if (null == _usSkillBtns)
            {
                CreateUMSkillBtns();
            }
            switch (slot)
            {
                case 0:
                    _usSkillBtns[0].CachedView.SkillBtn.gameObject.SetActive(visible);
                    break;
                case 1:
                    _usSkillBtns[1].CachedView.SkillBtn.gameObject.SetActive(visible);
                    break;
                case 2:
                    _usSkillBtns[2].CachedView.SkillBtn.gameObject.SetActive(visible);
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
            for (int i = 0; i < _cachedView.USViewSkillBtns.Length; i++)
            {
                if (_equipments[i] != null)
                {
                    _cachedView.USViewSkillBtns[i].gameObject.SetActive(true);
                    _usSkillBtns[i].SetData(_equipments[i]);
                }
                else
                {
                    _cachedView.USViewSkillBtns[i].gameObject.SetActive(false);
                }
            }
        }

        private void PlayClickParticle()
        {
            GameParticleManager.Instance.EmitUIParticle("UIEffectClickBig", _cachedView.JumpBtn.transform, _groupId);
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
            if (null == _usSkillBtns)
                CreateUMSkillBtns();
            _usSkillBtns[slot].SetData(tableSkill);
            RefreshSkillBtns();
        }

        private void OnSkillChargeTime(int slot, float leftTime, float totalTime)
        {
            if (null == _cachedView) return;
            _usSkillBtns[slot].OnSkillChargeTime(leftTime, totalTime);
        }

        private void OnSkillCDTime(int slot, float leftTime, float totalTime)
        {
            if (null == _cachedView) return;
            _usSkillBtns[slot].OnSkillCDTime(leftTime, totalTime);
        }

        private void OnSkillBulletChanged(int slot, int leftCount, int totalCount)
        {
            if (null == _cachedView) return;
            _usSkillBtns[slot].OnSkillBulletChanged(leftCount, totalCount);
        }

        private void CreateUMSkillBtns()
        {
            int skillNum = _cachedView.USViewSkillBtns.Length;
            _usSkillBtns = new USCtrlSkillBtn[skillNum];
            for (int i = 0; i < skillNum; i++)
            {
                _usSkillBtns[i] = new USCtrlSkillBtn();
                _usSkillBtns[i].Init(_cachedView.USViewSkillBtns[i]);
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