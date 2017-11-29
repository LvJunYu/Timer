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
        protected ShadowBattleData _shadowBattleData;
        protected USCtrlGameFinishReward[] _rewardCtrl;
        protected Reward _reward;
        protected int _curCountDown;
        protected bool _openGamePlaying;

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
                Close();
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
            RegisterEvent<ShadowBattleData>(EMessengerType.OnShadowBattleStart, OnShadowBattleStart);
        }

        protected void OnShadowBattleStart(ShadowBattleData shadowBattleData)
        {
            _shadowBattleData = shadowBattleData;
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.PopUpDialog;
        }

        protected virtual IEnumerator CountDown()
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

        protected void RefreshView()
        {
            var user = _shadowBattleData.Record.UserInfo;
            _cachedView.NickName.text = user.NickName;
            _cachedView.MaleIcon.SetActiveEx(user.Sex == ESex.S_Male);
            _cachedView.FemaleIcon.SetActiveEx(user.Sex == ESex.S_Female);
            _cachedView.AdvLevel.text = user.LevelData.PlayerLevel.ToString();
            _cachedView.CreatorLevel.text = user.LevelData.CreatorLevel.ToString();
            _cachedView.Score.text = _shadowBattleData.Record.Score.ToString();
            _reward = _shadowBattleData.Reward;
            UpdateReward(_reward);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserHead, user.HeadImgUrl,
                _cachedView.DefaultHeadTexture);
            user.BlueVipData.RefreshBlueVipView(_cachedView.BlueVipDock,
                _cachedView.BlueImg, _cachedView.SuperBlueImg, _cachedView.BlueYearVipImg);
        }

        protected virtual void UpdateReward(Reward reward)
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