using SoyEngine;
using System;
using SoyEngine.Proto;

namespace GameA.Game
{
    public class GameModeAdventurePlay : GameModePlay
    {
        public override bool Init(Project project, object param, GameManager.EStartType startType)
        {
            if (!base.Init(project, param, startType))
            {
                return false;
			}
			_gameSituation = EGameSituation.Adventure;
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

		private void CommitGameResult(Action successCB, Action<ENetResultCode> failureCB)
		{
			float usedTime = PlayMode.Instance.GameSuccessFrameCnt * ConstDefineGM2D.FixedDeltaTime;

			//SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "提交成绩中...");
			AppData.Instance.AdventureData.CommitLevelResult(
				PlayMode.Instance.SceneState.GameSucceed,
				usedTime,
				UnityEngine.Random.value > 0.5f,
				UnityEngine.Random.value > 0.5f,
				UnityEngine.Random.value > 0.5f,
				PlayMode.Instance.SceneState.CurScore,
				(int)(10 * UnityEngine.Random.value),
				(int)(10 * UnityEngine.Random.value),
				(int)(10 * UnityEngine.Random.value),
				(int)(10 * UnityEngine.Random.value),
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
    }
}
