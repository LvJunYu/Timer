using UnityEngine;
using System.Collections;
using System;
using SoyEngine.Proto;

namespace GameA.Game
{
    public class GameModeWorldPlayRecord : GameModePlayRecord
    {
        public override bool Init(Project project, object param, GameManager.EStartType startType)
        {
            if (!base.Init(project, param, startType))
            {
                return false;
            }
            _gameSituation = EGameSituation.World;
            return true;
		}
    }
}
