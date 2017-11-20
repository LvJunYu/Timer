using UnityEngine;

namespace GameA.Game
{
    public class GameModeShadowBattlePlay : GameModeWorldPlay
    {
        public override bool PlayShadowData
        {
            get { return true; }
        }

        public override bool Init(Project project, object param, GameManager.EStartType startType,
            MonoBehaviour corountineProxy)
        {
            if (!base.Init(project, param, startType, corountineProxy))
            {
                return false;
            }
            _gameSituation = EGameSituation.ShadowBattle;
            _successType = UICtrlGameFinish.EShowState.ShadowBattleWin;
            _failType = UICtrlGameFinish.EShowState.ShadowBattleLose;
            //读取影子数据
            ShadowDataPlayed = null;
            _record = param as Record;
            if (InitRecord() && _gm2drecordData.ShadowData != null)
            {
                ShadowDataPlayed = new ShadowData(_gm2drecordData.ShadowData);
            }
            return true;
        }
    }
}