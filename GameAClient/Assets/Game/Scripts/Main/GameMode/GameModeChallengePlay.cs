using System;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA.Game
{
    public class GameModeChallengePlay : GameModePlay
    {
        public override bool Init(Project project, object param, GameManager.EStartType startType)
        {
            if (!base.Init(project, param, startType))
            {
                return false;
            }
            _gameSituation = EGameSituation.Match;
            return true;
		}

        public override void OnGameFailed()
		{
			SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "提交成绩中...");
			CommitGameResult(() => {
				SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.ChallengeLose);
			},
			code =>
			{
				SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
				SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.ChallengeLose);
			});
        }

        public override void OnGameSuccess()
		{
			SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "提交成绩中...");
            CommitGameResult(() => {
				SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.ChallengeWin);
			},
			code =>
			{
				SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.ChallengeWin);
			});
        }

        public override void QuitGame(Action successCB, Action<int> failureCB, bool forceQuitWhenFailed = false)
		{
			if (!LocalUser.Instance.MatchUserData.ChallengeResultCommitted)
			{
                CommitGameResult(
					() =>
					{
						if (null != successCB)
						{
							successCB.Invoke();
						}
						SocialApp.Instance.ReturnToApp();
					},
					code =>
					{
						if (null != failureCB)
						{
							failureCB.Invoke((int)code);
						}
						SocialApp.Instance.ReturnToApp();
					}
				);
			}
			else
			{
				if (null != successCB)
				{
					successCB.Invoke();
				}
				SocialApp.Instance.ReturnToApp();
			}
        }

		private void CommitGameResult(Action successCB, Action<ENetResultCode> failureCB)
		{
			LocalUser.Instance.MatchUserData.CommitChallengeResult(
				PlayMode.Instance.SceneState.GameSucceed,
				PlayMode.Instance.GameSuccessFrameCnt * ConstDefineGM2D.FixedDeltaTime,
				() =>
				{
					LogHelper.Info("——————————————————————提交挑战成功");
					if (null != successCB)
					{
						successCB.Invoke();
					}
				},
				code =>
				{
					LogHelper.Info("——————————————————————提交挑战失败");
					if (null != failureCB)
					{
						failureCB.Invoke(code);
					}
				}
			);
		}
    }
}
