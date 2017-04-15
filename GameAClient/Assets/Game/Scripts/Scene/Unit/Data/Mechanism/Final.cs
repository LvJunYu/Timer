﻿/********************************************************************
** Filename : Final
** Author : Dong
** Date : 2016/10/20 星期四 下午 1:59:03
** Summary : Final
***********************************************************************/

using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 5001, Type = typeof(Final))]
    public class Final : Magic
    {
        protected static Final _instance;

        public static Vector3 Position
        {
            get
            {
                if (_instance != null)
                {
                    return _instance.Trans.position;
                }
                return PlayMode.Instance.MainUnit.Trans.position;
            }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _instance = this;
            return true;
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.IsMain)
            {
                //播放动画
                if (PlayMode.Instance.SceneState.CheckWinWithoutConditionArrived())
                {
                    PlayMode.Instance.SceneState.Arrived = true;
                }
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }
    }
}