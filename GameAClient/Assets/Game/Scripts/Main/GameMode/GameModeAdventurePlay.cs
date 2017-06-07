using SoyEngine;
using System;
using SoyEngine.Proto;

namespace GameA.Game
{
    public class GameModeAdventurePlay : GameModePlay, ISituationAdventure
    {
        private SituationAdventureParam _adventureLevelInfo;


        public SituationAdventureParam GetLevelInfo()
        {
            return _adventureLevelInfo;
        }

        public override bool Init(Project project, object param, GameManager.EStartType startType)
        {
            if (!base.Init(project, param, startType))
            {
                return false;
			}
            _gameSituation = EGameSituation.Adventure;
            _adventureLevelInfo = param as SituationAdventureParam;
            return true;
		}

        public override void OnGameFailed()
		{
            UICtrlGameFinish.EShowState showState
                            = _startType == GameManager.EStartType.AdventureNormalPlay
                ?
                UICtrlGameFinish.EShowState.AdvNormalLose
                :
                UICtrlGameFinish.EShowState.AdvBonusLose;

			SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "提交成绩中...");
            CommitGameResult(() =>{
				SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(showState);
			},
			code =>
			{
				SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
				SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(showState);
			});
        }

        public override void OnGameSuccess()
		{
			UICtrlGameFinish.EShowState showState
							= _startType == GameManager.EStartType.AdventureNormalPlay
				?
                UICtrlGameFinish.EShowState.AdvNormalWin
				:
                UICtrlGameFinish.EShowState.AdvBonusWin;
			SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "提交成绩中...");
            CommitGameResult(() => {
				SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
				SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(showState);
			},
			code =>
			{
				SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
				SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(showState);
					//CommonTools.ShowPopupDialog (
					//    "游戏成绩提交失败",
					//    null,
					// new System.Collections.Generic.KeyValuePair<string, Action>("重试", ()=>{
					//   CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnGameFinishSuccess));
					// }), 
					// new System.Collections.Generic.KeyValuePair<string, Action>("跳过", ()=>{
					//   Messenger.Broadcast(EMessengerType.GameFinishSuccessShowUI);
					// })
					//);
			});
        }

        public override void QuitGame(Action successCB, Action<int> failureCB, bool forceQuitWhenFailed = false)
        {
            CommitGameResult (() => {
                if (null != successCB) {
                    successCB.Invoke ();
                }
                SocialApp.Instance.ReturnToApp ();
            },
            code => {
                if (null != failureCB) {
                    failureCB.Invoke ((int)code);
                }
                SocialApp.Instance.ReturnToApp ();
            });
        }

        public override bool Restart (Action successCb, Action failedCb)
        {

            var tableLevel = AppData.Instance.AdventureData.GetAdvLevelTable (
                AppData.Instance.AdventureData.LastPlayedChapterIdx + 1,
                AppData.Instance.AdventureData.LastPlayedLevelIdx + 1,
                AppData.Instance.AdventureData.LastPlayedLevelType
            );
            if (null == tableLevel) return false;
            if (GameATools.CheckEnergy (tableLevel.EnergyCost)) {
//                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (
//                    this, "...");
                AppData.Instance.AdventureData.RetryAdvLevel (
                    () => {
//                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                        // set local energy data
                        GameATools.LocalUseEnergy (tableLevel.EnergyCost);
//                        if (null != successCb) {
//                            successCb.Invoke ();
//                        }
                        base.Restart (successCb, failedCb);
                    },
                    (error) => {
                        if (null != failedCb) {
                            failedCb.Invoke ();
                        }
//                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                    }
                );
                return true;
            } else {
                SocialGUIManager.ShowPopupDialog(
                    "体力不够了",
                    null,
                    new System.Collections.Generic.KeyValuePair<string, Action> (
                        "确定",
                        () => {
                            if (null != failedCb) {
                                failedCb.Invoke ();
                            }
                        }
                    )
                );
                return false;
            }
            return false;
        }


		private void CommitGameResult(Action successCB, Action<ENetResultCode> failureCB)
		{
			float usedTime = PlayMode.Instance.GameSuccessFrameCnt * ConstDefineGM2D.FixedDeltaTime;

			//SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "提交成绩中...");
			AppData.Instance.AdventureData.CommitLevelResult(
				PlayMode.Instance.SceneState.GameSucceed,
                usedTime,
                PlayMode.Instance.SceneState.CurScore,
                PlayMode.Instance.SceneState.GemGain,
                PlayMode.Instance.SceneState.MonsterKilled,
                PlayMode.Instance.SceneState.SecondLeft,
                PlayMode.Instance.MainUnit.Life,
                GetRecord(),
				() =>
				{
					LogHelper.Info("游戏成绩提交成功");
					if (null != successCB)
					{
						successCB.Invoke();
					}
					//SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
					//if (PlayMode.Instance.SceneState.GameSucceed) {
					//    Messenger.Broadcast (EMessengerType.GameFinishSuccessShowUI);
					//} else {
					//    Messenger.Broadcast (EMessengerType.GameFinishFailedShowUI);
					//}
				}, errCode =>
				{
					//SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
					//CommonTools.ShowPopupDialog("游戏成绩提交失败", null,
					//new System.Collections.Generic.KeyValuePair<string, Action>("重试", ()=>{
					//  CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnGameFinishSuccess));
					//}), 
					//new System.Collections.Generic.KeyValuePair<string, Action>("跳过", ()=>{
					//  Messenger.Broadcast(EMessengerType.GameFinishSuccessShowUI);
					//}));
					if (null != failureCB)
					{
						failureCB.Invoke(errCode);
					}
				}
			);
		}

        private byte[] GetRecord ()
        {
            GM2DRecordData recordData = new GM2DRecordData ();
            recordData.Version = GM2DGame.Version;
            recordData.FrameCount = ConstDefineGM2D.FixedFrameCount;
            recordData.Data.AddRange(PlayMode.Instance.InputDatas);
            recordData.BoostItem = new BoostItemData ();
            recordData.BoostItem.ExtraLife = PlayMode.Instance.IsUsingBoostItem (EBoostItemType.BIT_AddLifeCount1) ? 1 : 0;
            recordData.BoostItem.InvinsibleOnDead = PlayMode.Instance.IsUsingBoostItem (EBoostItemType.BIT_DeadInvincibleCount1) ? 1 : 0;
            recordData.BoostItem.LongerInvinsible = PlayMode.Instance.IsUsingBoostItem (EBoostItemType.BIT_InvincibleTime2) ? 1 : 0;
            recordData.BoostItem.ScoreBonus = PlayMode.Instance.IsUsingBoostItem (EBoostItemType.BIT_ScoreAddPercent20) ? 1 : 0;
            recordData.BoostItem.TimeBonus = PlayMode.Instance.IsUsingBoostItem (EBoostItemType.BIT_TimeAddPercent20) ? 1 : 0;
            recordData.Avatar = new AvatarData ();
            if (null == LocalUser.Instance.UsingAvatarData || !LocalUser.Instance.UsingAvatarData.IsInited)
            {
                recordData.Avatar.Appendage = 1;
                recordData.Avatar.Head = 1;
                recordData.Avatar.Lower = 1;
                recordData.Avatar.Upper = 1;    
            } else
            {
                if (null != LocalUser.Instance.UsingAvatarData.Appendage && LocalUser.Instance.UsingAvatarData.Appendage.IsInited)
                {
                    recordData.Avatar.Appendage = (int)LocalUser.Instance.UsingAvatarData.Appendage.Id;
                } else 
                {
                    recordData.Avatar.Appendage = 1;
                }
                if (null != LocalUser.Instance.UsingAvatarData.Head && LocalUser.Instance.UsingAvatarData.Head.IsInited) {
                    recordData.Avatar.Head = (int)LocalUser.Instance.UsingAvatarData.Head.Id;
                } else {
                    recordData.Avatar.Head = 1;
                }
                if (null != LocalUser.Instance.UsingAvatarData.Lower && LocalUser.Instance.UsingAvatarData.Lower.IsInited) {
                    recordData.Avatar.Lower = (int)LocalUser.Instance.UsingAvatarData.Lower.Id;
                } else {
                    recordData.Avatar.Lower = 1;
                }
                if (null != LocalUser.Instance.UsingAvatarData.Upper && LocalUser.Instance.UsingAvatarData.Upper.IsInited) {
                    recordData.Avatar.Upper = (int)LocalUser.Instance.UsingAvatarData.Upper.Id;
                } else {
                    recordData.Avatar.Upper = 1;
                }
            }
            byte[] recordByte = GameMapDataSerializer.Instance.Serialize(recordData);
            if(recordByte == null)
            {
                LogHelper.Error("录像数据出错");
            }
            else
            {
                recordByte = MatrixProjectTools.CompressLZMA(recordByte);
            }
            return recordByte;
        }


    }
}
