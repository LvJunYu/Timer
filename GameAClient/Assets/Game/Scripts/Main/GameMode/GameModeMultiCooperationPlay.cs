using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using System.Collections;

namespace GameA.Game
{
    public class GameModeMultiCooperationPlay : GameModeNetPlay
    {
        public override bool Init(Project project, object param, GameManager.EStartType startType,
            MonoBehaviour corountineProxy)
        {
            if (!base.Init(project, param, startType, corountineProxy))
            {
                return false;
            }

            _gameSituation = EGameSituation.Adventure;
            return true;
        }

        public override void OnGameFailed()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlSettlePlayersData>(TeamManager.Instance.GetSettlePlayerDatas());
            SocialGUIManager.Instance.GetUI<UICtrlSettlePlayersData>().setProject(_project);
        }

        public override void OnGameSuccess()
        {
//            SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.Win);
            SocialGUIManager.Instance.OpenUI<UICtrlSettlePlayersData>(TeamManager.Instance.GetSettlePlayerDatas());
            SocialGUIManager.Instance.GetUI<UICtrlSettlePlayersData>().setProject(_project);
        }

        public override bool Restart(Action<bool> successCb, Action failedCb)
        {
            _project.RequestPlay(() =>
            {
                GameRun.Instance.RePlay();
                OnGameStart();
                if (successCb != null)
                {
                    successCb.Invoke(true);
                }
            }, code => failedCb());
            return true;
        }
    }
}