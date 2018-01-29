using System.Collections;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UICommon)]
    public class UICtrlShadowBattleHelp : UICtrlShadowBattleSurprise
    {
        protected override IEnumerator CountDown()
        {
            _cachedView.CountDownTxt.text = _curCountDown.ToString();
            while (_curCountDown > 0)
            {
                yield return new WaitForSeconds(1);
                _curCountDown--;
                _cachedView.CountDownTxt.text = _curCountDown.ToString();
            }
            _shadowBattleData = null;
            SocialGUIManager.Instance.CloseUI<UICtrlShadowBattleHelp>();
        }

        protected override void UpdateReward(Reward reward)
        {
            if (null != reward && reward.IsInited)
            {
                int i = 0;
                for (; i < _rewardCtrl.Length && i < reward.ItemList.Count; i++)
                {
                    _rewardCtrl[i].Set(reward.ItemList[i].GetSprite(), (reward.ItemList[i].Count / 2).ToString());
                }
                for (; i < _rewardCtrl.Length; i++)
                {
                    _rewardCtrl[i].Hide();
                }
            }
            else
            {
                for (int i = 0; i < _rewardCtrl.Length; i++)
                {
                    _rewardCtrl[i].Hide();
                }
            }
        }
    }
}