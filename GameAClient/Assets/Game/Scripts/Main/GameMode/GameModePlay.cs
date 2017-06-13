using System.Collections;
using System;
using SoyEngine;
using SoyEngine.Proto;
using System.Collections.Generic;
using UnityEngine;

namespace GameA.Game
{
    public abstract class GameModePlay : GameModeBase
    {
        public override bool Init(Project project, object param, GameManager.EStartType startType, MonoBehaviour corountineProxy)
        {
            if (!base.Init(project, param, startType, corountineProxy))
            {
                return false;
            }
            _gameRunMode = EGameRunMode.Play;
            return true;
		}

        public override void InitByStep()
		{
			InitUI();
			InitGame();
        }


        public virtual bool PlayNext(Action successCb, Action failedCb)
        {
            GameManager.Instance.PlayNext();
            if (successCb != null)
            {
                successCb.Invoke();
            }
            return true;
        }

        public virtual void UseBoostItem(List<int> itemTypes)
		{
			if (null == _project) return;
			long token = 0;
			if (EProjectStatus.PS_Challenge == _project.ProjectStatus)
			{
				token = LocalUser.Instance.MatchUserData.PlayChallengeToken;
			}
			else if (EProjectStatus.PS_AdvNormal == _project.ProjectStatus)
			{
				token = AppData.Instance.AdventureData.LastRequiredLevelToken;
			}

			if (0 != token)
			{
				RemoteCommands.UseProps(
					token,
					itemTypes,
					msg =>
					{
						if ((int)EUsePropsCode.UPC_Success == msg.ResultCode)
						{
							LogHelper.Info("Use boost item success");
						}
						else
						{
							// todo error handle
							// 失败了应该终止关卡退回主菜单重新请求一遍数据
							LogHelper.Error("Use boost item failed");
						}
					},
					code =>
					{
						// todo error handle
						LogHelper.Error("Use boost item failed");
					}
				);
			}
		}

        protected virtual void InitUI()
		{
			SocialGUIManager.Instance.OpenUI<UICtrlSceneState>();
			InputManager.Instance.ShowGameInput();
        }

        protected virtual void InitGame()
        {
            MainUnit mainPlayer = PlayMode.Instance.MainUnit;
            if (mainPlayer == null)
                return;
            // todo set data

            ChangePartsSpineView view = mainPlayer.View as ChangePartsSpineView;
            if (view == null)
                return;
            int headId = 1;
            int upperId = 1;
            int lowerId = 1;
            int appendageId = 1;

            if (LocalUser.Instance.UsingAvatarData.IsInited)
            {
                if (LocalUser.Instance.UsingAvatarData.Head != null &&
                    LocalUser.Instance.UsingAvatarData.Head.IsInited)
                {
                    headId = (int)LocalUser.Instance.UsingAvatarData.Head.Id;
                }
                if (LocalUser.Instance.UsingAvatarData.Upper != null &&
                    LocalUser.Instance.UsingAvatarData.Upper.IsInited)
                {
                    upperId = (int)LocalUser.Instance.UsingAvatarData.Upper.Id;
                }
                if (LocalUser.Instance.UsingAvatarData.Lower != null &&
                    LocalUser.Instance.UsingAvatarData.Lower.IsInited)
                {
                    lowerId = (int)LocalUser.Instance.UsingAvatarData.Lower.Id;
                }
                if (LocalUser.Instance.UsingAvatarData.Appendage != null &&
                    LocalUser.Instance.UsingAvatarData.Appendage.IsInited)
                {
                    appendageId = (int)LocalUser.Instance.UsingAvatarData.Appendage.Id;
                }
            }
            //          view.SetParts (2, SpinePartsDefine.ESpineParts.Head);
            if (LocalUser.Instance.UsingAvatarData.Head != null)
            {
                view.SetParts(headId, SpinePartsHelper.ESpineParts.Head);
            }
            if (LocalUser.Instance.UsingAvatarData.Upper != null)
            {
                view.SetParts(upperId, SpinePartsHelper.ESpineParts.Upper);
            }
            if (LocalUser.Instance.UsingAvatarData.Lower != null)
            {
                view.SetParts(lowerId, SpinePartsHelper.ESpineParts.Lower);
            }
            if (LocalUser.Instance.UsingAvatarData.Appendage != null)
            {
                view.SetParts(appendageId, SpinePartsHelper.ESpineParts.Appendage);
            }
        }
    }
}
