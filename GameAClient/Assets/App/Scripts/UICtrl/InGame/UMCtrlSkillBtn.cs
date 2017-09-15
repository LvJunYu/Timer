using GameA.Game;
using NewResourceSolution;
using SoyEngine;

namespace GameA
{
    public class UMCtrlSkillBtn : UMCtrlBase<UMViewSkillBtn>
    {
        private int _curBulletCount;
        private UIParticleItem _fullParticleItem;
        private bool _press;
        private Table_Equipment _curTableEquipment;

        public UMViewSkillBtn CachedView
        {
            get { return _cachedView; }
        }

        public void SetData(Table_Equipment tableSkill)
        {
            if (null == _cachedView) return;
            _curTableEquipment = tableSkill;
            int bgIndex = tableSkill.InputType - 1;
            for (int i = 0; i < _cachedView.BtnColorBgs.Length; i++)
            {
                _cachedView.BtnColorBgs[i].SetActive(i == bgIndex);
            }
            _cachedView.BtnIcon.sprite = ResourcesManager.Instance.GetSprite(tableSkill.Icon);
            _cachedView.BtnIcon.gameObject.SetActive(true);
            PlayFullParticle(_curTableEquipment.InputType == (int) EWeaponInputType.GetKeyUp);
            _cachedView.BtnCD1.fillAmount = 0;
            _cachedView.BtnCD2.fillAmount = 0;
            _cachedView.TimeTxt.text = "";
        }

        public void OnSkillCDTime(float leftTime, float totalTime)
        {
            _cachedView.BtnCD1.fillAmount = leftTime / totalTime;
            _cachedView.TimeTxt.text = GetTimeString(leftTime);
            if (_press)
                _press = false;
        }

        public void OnSkillChargeTime(float leftTime, float totalTime)
        {
            if (_curBulletCount == 0)
            {
                _cachedView.BtnCD1.fillAmount = leftTime / totalTime;
                _cachedView.BtnCD2.fillAmount = 0;
                _cachedView.TimeTxt.text = GetTimeString(leftTime);
            }
            else
            {
                _cachedView.BtnCD1.fillAmount = 0;
                _cachedView.TimeTxt.text = "";
                _cachedView.BtnCD2.fillAmount = 1 - leftTime / totalTime;
            }
            if (_press)
                _press = false;
        }

        private string GetTimeString(float value)
        {
            value = value * ConstDefineGM2D.FixedDeltaTime;
            if (value < 0.1f)
                return "";
            return string.Format("{0:f1}", value);
        }

        private void PlayClickParticle()
        {
            GameParticleManager.Instance.EmitUIParticle("UIEffectClickSmall",
                _cachedView.SkillBtn.transform, (int) EUIGroupType.InputCtrl, 0.5f);
        }

        private void PlayFullParticle(bool play)
        {
            if (play)
            {
                if (null == _fullParticleItem)
                    _fullParticleItem = GameParticleManager.Instance.EmitUIParticle("UIEffectSkillFull",
                        _cachedView.SkillBtn.transform, (int) EUIGroupType.InputCtrl);
                else
                    _fullParticleItem.Particle.Play();
            }
            else
            {
                if (_fullParticleItem != null && _fullParticleItem.Particle.IsPlaying)
                    _fullParticleItem.Particle.Stop();
            }
        }

        public void OnSkillBulletChanged(int leftCount, int totalCount)
        {
            _curBulletCount = leftCount;
            //连射不会触发点击特效
            if (!_press && leftCount < totalCount)
            {
                PlayClickParticle();
                _press = true;
            }
            if (_curTableEquipment != null && _curTableEquipment.InputType == (int) EWeaponInputType.GetKeyUp)
                PlayFullParticle(leftCount == totalCount);
        }
    }
}