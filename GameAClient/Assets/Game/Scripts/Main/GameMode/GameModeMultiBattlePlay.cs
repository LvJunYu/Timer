using System;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using System.Collections;

namespace GameA.Game
{
    public class GameModeMultiBattlePlay : GameModePlay
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

        public override void OnGameStart()
        {
            base.OnGameStart();
            _coroutineProxy.StopAllCoroutines();
            _coroutineProxy.StartCoroutine(GameFlow());
        }

        public override void OnGameFailed()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.Lose);
        }

        public override void OnGameSuccess()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.Win);
        }

        public override bool Restart(Action successCb, Action failedCb)
        {
            _project.RequestPlay(() => {
                GameRun.Instance.RePlay();
                OnGameStart();
                if (successCb != null)
                {
                    successCb.Invoke();
                }
            }, code => failedCb());
            return true;
        }

        private IEnumerator GameFlow()
        {
            UICtrlCountDown uictrlCountDown = SocialGUIManager.Instance.OpenUI<UICtrlCountDown>();
            yield return new WaitUntil(()=>uictrlCountDown.ShowComplete);

            UICtrlSceneState uictrlSceneState = SocialGUIManager.Instance.GetUI<UICtrlSceneState>();
            uictrlSceneState.ShowHelpPage3Seconds();
            yield return new WaitUntil(()=>uictrlSceneState.ShowHelpPage3SecondsComplete);
            GameRun.Instance.Playing();
        }
    }
}
