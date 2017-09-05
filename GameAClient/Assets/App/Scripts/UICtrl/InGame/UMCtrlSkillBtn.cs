using GameA.Game;
using NewResourceSolution;

namespace GameA
{
    public class UMCtrlSkillBtn : UMCtrlBase<UMViewSkillBtn>
    {
        private int _curBulletCount;
        private int _totalBulletCount;

        public UMViewSkillBtn CachedView
        {
            get { return _cachedView; }
        }

        public void SetData(Table_Equipment tableSkill)
        {
            if (null == _cachedView) return;
            int bgIndex = tableSkill.InputType - 1;
            for (int i = 0; i < _cachedView.BtnColorBgs.Length; i++)
            {
                _cachedView.BtnColorBgs[i].SetActive(i == bgIndex);
            }
            _cachedView.BtnIcon.sprite = ResourcesManager.Instance.GetSprite(tableSkill.Icon);
            _cachedView.BtnIcon.gameObject.SetActive(true);
        }

        public void OnSkillCDTime(float leftTime, float totalTime)
        {
            _cachedView.BtnCD1.fillAmount = leftTime / totalTime;
            _cachedView.TimeTxt.text = GetTimeString(leftTime);
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
        }

        private string GetTimeString(float value)
        {
            value = value * ConstDefineGM2D.FixedDeltaTime; 
            if (value < 0.1f)
                return "";
            return string.Format("{0:f1}",value);
        }

        public void OnSkillBulletChanged(int leftCount, int totalCount)
        {
            _curBulletCount = leftCount;
            _totalBulletCount = totalCount;
        }
    }
}