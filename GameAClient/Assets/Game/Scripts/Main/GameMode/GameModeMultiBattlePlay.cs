using System;
using UnityEngine;

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
            _successType = UICtrlGameFinish.EShowState.MultiWin;
            _failType = UICtrlGameFinish.EShowState.MultiLose;
            return true;
        }

        public override void OnGameFailed()
        {
            base.OnGameFailed();
            if (!PlayMode.Instance.SceneState.GameFailed) return;
            SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(_failType);
        }

        public override void OnGameSuccess()
        {
            if (!PlayMode.Instance.SceneState.GameFailed) return;
            SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(_successType);
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
