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
            RegisterEvent(EMessengerType.OnInputKeyCodeChanged, UpdateInputKeys);
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
            SetPlatform(CrossPlatformInputManager.Platform);
            RefreshSkillBtns();
            UpdateInputKeys();
            _cachedView.AssistBtn.gameObject.SetActive(false);
            _cachedView.AssistBtn_PC.SetActive(false);
        }

        protected override void OnClose()
        {
            base.OnClose();
            for (int i = 0; i < _equipments.Length; i++)
            {
                _equipments[i] = null;
            }
        }

        private void UpdateInputKeys()
        {
            if (_cachedView == null || CrossPlatformInputManager.Platform == EPlatform.Moblie) return;
            _cachedView.AssistInputKeyTxt.text =
                GetStringRaw(CrossPlatformInputManager.GetButtonPositiveKey(InputManager.TagAssist).ToString());
            _cachedView.USViewSkillBtns_PC[0].KeyTxt.text =
                GetStringRaw(CrossPlatformInputManager.GetButtonPositiveKey(InputManager.TagSkill1).ToString());
            _cachedView.USViewSkillBtns_PC[1].KeyTxt.text =
                GetStringRaw(CrossPlatformInputManager.GetButtonPositiveKey(InputManager.TagSkill2).ToString());
            _cachedView.USViewSkillBtns_PC[2].KeyTxt.text =
                GetStringRaw(CrossPlatformInputManager.GetButtonPositiveKey(InputManager.TagSkill3).ToString());
        }

        private string GetStringRaw(string str)
        {
            if (str.Length > 2)
            {
                return str.Substring(0, 2);
            }
            return str;
        }

        public void SetPlatform(EPlatform ePlatform)
        {
            _cachedView.MobilePannel.SetActive(ePlatform == EPlatform.Moblie);
            _cachedView.PcPannel.SetActive(ePlatform == EPlatform.Standalone);
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
            _cachedView.AssistBtn_PC.SetActive(visible);
        }

        private void RefreshSkillBtns()
        {
            for (int i = 0; i < _cachedView.USViewSkillBtns.Length; i++)
            {
                _usSkillBtns[i].SetData(_equipments[i]);
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
            if (CrossPlatformInputManager.Platform == EPlatform.Moblie)
            {
                int skillNum = _cachedView.USViewSkillBtns.Length;
                _usSkillBtns = new USCtrlSkillBtn[skillNum];
                for (int i = 0; i < skillNum; i++)
                {
                    _usSkillBtns[i] = new USCtrlSkillBtn();
                    _usSkillBtns[i].Init(_cachedView.USViewSkillBtns[i]);
                }
            }
            else
            {
                int skillNum = _cachedView.USViewSkillBtns_PC.Length;
                _usSkillBtns = new USCtrlSkillBtn[skillNum];
                for (int i = 0; i < skillNum; i++)
                {
                    _usSkillBtns[i] = new USCtrlSkillBtn();
                    _usSkillBtns[i].Init(_cachedView.USViewSkillBtns_PC[i]);
                }
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