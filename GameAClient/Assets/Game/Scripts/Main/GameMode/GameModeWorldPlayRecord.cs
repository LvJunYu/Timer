using UnityEngine;
using System.Collections;
using System;
using SoyEngine.Proto;

namespace GameA.Game
{
    public class GameModeWorldPlayRecord : GameModePlayRecord
    {
        public override bool Init(Project project, object param, GameManager.EStartType startType, MonoBehaviour corountineProxy)
        {
            if (!base.Init(project, param, startType, corountineProxy))
            {
                return false;
            }
            _gameSituation = EGameSituation.World;
            _record = param as Record;
            InitRecord();
            return true;
        }

        public override void OnGameStart()
        {
            base.OnGameStart();
            _coroutineProxy.StopAllCoroutines();
            _coroutineProxy.StartCoroutine(GameFlow());
        }

        private IEnumerator GameFlow()
        {
//            UICtrlCountDown uictrlCountDown = SocialGUIManager.Instance.OpenUI<UICtrlCountDown>();
//            yield return new WaitUntil(()=>uictrlCountDown.ShowComplete);
            yield return null;
            GameRun.Instance.Playing();
        }
    }
}
