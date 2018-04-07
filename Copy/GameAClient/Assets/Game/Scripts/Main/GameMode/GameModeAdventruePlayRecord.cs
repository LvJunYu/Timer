using UnityEngine;
using System.Collections;
using System;
using SoyEngine.Proto;

namespace GameA.Game
{
    public class GameModeAdventruePlayRecord : GameModePlayRecord, ISituationAdventure
    {
        private SituationAdventureParam _adventureLevelInfo;

        public override bool Init(Project project, object param, GameManager.EStartType startType, MonoBehaviour corountineProxy)
        {
            if (!base.Init(project, param, startType, corountineProxy))
            {
                return false;
            }
            _gameSituation = EGameSituation.Adventure;
            _adventureLevelInfo = param as SituationAdventureParam;
            _record = _adventureLevelInfo.Record;
            InitRecord();
            return true;
		}

        public SituationAdventureParam GetLevelInfo()
        {
            return _adventureLevelInfo;
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
            _run = true;
            GameRun.Instance.Playing();
        }
    }
}
