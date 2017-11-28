using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using System.Collections;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UICommon)]
    public class UICtrlShadowBattleSurprise : UICtrlResManagedBase<UIViewShadowBattle>
    {
        private Msg_SC_DAT_ShadowBattleData _shadowBattleData;
        private USCtrlGameFinishReward[] _rewardCtrl;
        private Reward _reward;
        private int _curCountDown;
        private bool _openGamePlaying;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _rewardCtrl = new USCtrlGameFinishReward [_cachedView.Rewards.Length];
            for (int i = 0; i < _cachedView.Rewards.Length; i++)
            {
                _rewardCtrl[i] = new USCtrlGameFinishReward();
                _rewardCtrl[i].Init(_cachedView.Rewards[i]);
            }
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            if (null == _shadowBattleData)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlShadowBattleSurprise>();
                return;
            }
            _openGamePlaying = false;
            if (GM2DGame.Instance != null)
            {
                if (GameRun.Instance.IsPlaying)
                {
                    _openGamePlaying = true;
                    GM2DGame.Instance.Pause();
                }
            }
            RefreshView();
            _curCountDown = 3;
            CoroutineProxy.Instance.StartCoroutine(CountDown());
        }

        protected override void OnClose()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserHead, _cachedView.DefaultHeadTexture);
            if (GM2DGame.Instance != null && _openGamePlaying)
            {
                GM2DGame.Instance.Continue();
                _openGamePlaying = false;
            }
            base.OnClose();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<Msg_SC_DAT_ShadowBattleData>(EMessengerType.OnShadowBattleStart, OnShadowBattleStart);
        }

        private void OnShadowBattleStart(Msg_SC_DAT_ShadowBattleData shadowBattleData)
        {
            _shadowBattleData = shadowBattleData;
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.PopUpDialog;
        }

        private IEnumerator CountDown()
        {
            _cachedView.CountDownTxt.text = _curCountDown.ToString();
            while (_curCountDown > 0)
            {
                yield return new WaitForSeconds(1);
                _curCountDown--;
                _cachedView.CountDownTxt.text = _curCountDown.ToString();
            }
            _shadowBattleData = null;
            SocialGUIManager.Instance.CloseUI<UICtrlShadowBattleSurprise>();
        }

        private void RefreshView()
        {
            var user = _shadowBattleData.Record.UserInfo;
            _cachedView.NickName.text = user.NickName;
            _cachedView.MaleIcon.SetActiveEx(user.Sex == ESex.S_Male);
            _cachedView.FemaleIcon.SetActiveEx(user.Sex == ESex.S_Female);
            _cachedView.AdvLevel.text = user.LevelData.PlayerLevel.ToString();
            _cachedView.CreatorLevel.text = user.LevelData.CreatorLevel.ToString();
            _cachedView.Score.text = _shadowBattleData.Record.Score.ToString();
            _reward = new Reward(_shadowBattleData.Reward);
            UpdateReward(_reward);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserHead, user.HeadImgUrl,
                _cachedView.DefaultHeadTexture);
        }

        private void UpdateReward(Reward reward)
        {
            if (null != reward && reward.IsInited)
            {
                int i = 0;
                for (; i < _rewardCtrl.Length && i < reward.ItemList.Count; i++)
                {
                    _rewardCtrl[i].Set(reward.ItemList[i].GetSprite(), reward.ItemList[i].Count.ToString());
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