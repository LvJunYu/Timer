/********************************************************************
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
    public class Final : BlockBase
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
                return PlayMode.Instance.MainPlayer.Trans.position;
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

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            if (_withEffect != null)
            {
                SetRelativeEffectPos(_withEffect.Trans, EDirectionType.Up, UnitDefine.ZOffsetBackground);
            }
            return true;
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly && other.IsPlayer)
            {
                if (IntersectX(other, _colliderGrid.Shrink(319)))
                {
                    if (!PlayMode.Instance.SceneState.Arrived)
                    {
                        Messenger<UnitBase>.Broadcast(EMessengerType.OnPlayerArrive, other);
                    }
                    PlayMode.Instance.SceneState.Arrived = true;
                }
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }
    }
}