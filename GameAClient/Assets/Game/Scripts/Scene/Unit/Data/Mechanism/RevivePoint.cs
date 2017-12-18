/********************************************************************
** Filename : RevivePoint
** Author : Dong
** Date : 2016/10/29 星期六 上午 11:26:57
** Summary : RevivePoint
***********************************************************************/

using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 5002, Type = typeof(RevivePoint))]
    public class RevivePoint : BlockBase
    {
        private Dictionary<long, bool> _triggerDic = new Dictionary<long, bool>(PlayerManager.MaxTeamCount);

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            Messenger<IntVec3, PlayerBase>.AddListener(EMessengerType.OnRespawnPointTrigger, OnRespawnPointTrigger);
            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _animation.Init("Run");
            for (int i = 0; i < _viewExtras.Length; i++)
            {
                _viewExtras[i].Animation.Init("Run");
            }
            return true;
        }

        private void OnRespawnPointTrigger(IntVec3 guid, PlayerBase player)
        {
            if (_guid == guid)
            {
                return;
            }
            var playerId = player.RoomUser.Guid;
            if (!CheckTrigger(playerId))
            {
                return;
            }
            if (player.IsMain && _trans != null)
            {
                _animation.Reset();
                _animation.PlayLoop("Run");
                for (int i = 0; i < _viewExtras.Length; i++)
                {
                    _viewExtras[i].Animation.Reset();
                    _viewExtras[i].Animation.PlayLoop("Run");
                }
            }
            SetTrigger(playerId, false);
        }

        internal override void Reset()
        {
            base.Reset();
            _triggerDic.Clear();
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                if (other.IsPlayer)
                {
                    var player = other as PlayerBase;
                    var playerId = player.RoomUser.Guid;
                    if (!CheckTrigger(playerId))
                    {
                        SetTrigger(playerId, true);
                        other.OnRevivePos(new IntVec2(_curPos.x, _curPos.y + ConstDefineGM2D.ServerTileScale));
                        if (other.IsMain)
                        {
                            _animation.PlayLoop("Start");
                            for (int i = 0; i < _viewExtras.Length; i++)
                            {
                                _viewExtras[i].Animation.PlayLoop("Start");
                            }
                            Messenger<IntVec3, PlayerBase>.Broadcast(EMessengerType.OnRespawnPointTrigger, _guid, player);
                        }
                    }
                }
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        private void SetTrigger(long playerId, bool value)
        {
            _triggerDic.AddOrReplace(playerId, value);
        }

        private bool CheckTrigger(long playerId)
        {
            bool trigger;
            if (_triggerDic.TryGetValue(playerId, out trigger))
            {
                return trigger;
            }
            return false;
        }

        internal override void OnDispose()
        {
            Messenger<IntVec3, PlayerBase>.RemoveListener(EMessengerType.OnRespawnPointTrigger, OnRespawnPointTrigger);
            base.OnDispose();
        }
    }
}