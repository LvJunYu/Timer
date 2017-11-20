using System;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using System.Collections;

namespace GameA.Game
{
    public class GameModeMultiBattlePlay : GameModeNetPlay
    {
        public override bool Init(Project project, object param, GameManager.EStartType startType, MonoBehaviour corountineProxy)
        {
            if (!base.Init(project, param, startType, corountineProxy))
            {
                return false;
            }
            _gameSituation = EGameSituation.Battle;
            return true;
        }

        public override void OnGameFailed()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.Lose);
        }

        public override void OnGameSuccess()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.Win);
        }

        public override bool Restart(Action<bool> successCb, Action failedCb)
        {
            _project.RequestPlay(() => {
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
