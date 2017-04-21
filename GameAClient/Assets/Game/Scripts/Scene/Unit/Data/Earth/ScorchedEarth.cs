/********************************************************************
** Filename : ScorchedEarth
** Author : Dong
** Date : 2017/3/14 星期二 下午 10:07:39
** Summary : ScorchedEarth
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 4013, Type = typeof(ScorchedEarth))]
    public class ScorchedEarth : BlockBase
    {
        protected bool _trigger;
        protected int _timer;

        protected override void Clear()
        {
            base.Clear();
            _trigger = false;
            _timer = 0;
            PlayMode.Instance.UnFreeze(this);
            if (_view != null)
            {
                _view.SetRendererEnabled(true);
            }
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            OnTrigger(other, checkOnly);
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            OnTrigger(other, checkOnly);
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            OnTrigger(other, checkOnly);
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            OnTrigger(other, checkOnly);
            return base.OnRightHit(other, ref x, checkOnly);
        }

        private void OnTrigger(UnitBase other, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                if (other.IsMain)
                {
                    OnExplode();
                }
            }
        }

        public void OnExplode()
        {
            if (!_trigger)
            {
                _trigger = true;
            }
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_trigger)
            {
                _timer++;
                if (_timer == 10)
                {
                    //开始爆炸
                    PlayMode.Instance.Freeze(this);
                    if (_view != null)
                    {
                        _view.SetRendererEnabled(false);
                        GameAudioManager.Instance.PlaySoundsEffects(_tableUnit.DestroyAudioName);
                        GameParticleManager.Instance.Emit(_tableUnit.DestroyEffectName, _trans.position, Vector3.one);
                    }
                }
                else if (_timer == 30)
                {
                    //查收周边的让其爆炸
                    SendMsgToAround();
                }
                else if (_timer == 200)
                {
                    //复原
                    _trigger = false;
                    _timer = 0;
                    PlayMode.Instance.UnFreeze(this);
                    if (_view != null)
                    {
                        _view.SetRendererEnabled(true);
                    }
                }
            }
        }

        private void SendMsgToAround()
        {
            var scale = ConstDefineGM2D.ServerTileScale;
            Check(new IntVec3(_guid.x, _guid.y + scale, _guid.z));
            Check(new IntVec3(_guid.x, _guid.y - scale, _guid.z));
            Check(new IntVec3(_guid.x - scale, _guid.y, _guid.z));
            Check(new IntVec3(_guid.x + scale, _guid.y, _guid.z));
        }

        private void Check(IntVec3 guid)
        {
            UnitBase unit;
            if (ColliderScene2D.Instance.TryGetUnit(guid, out unit))
            {
                var scorchedEarth = unit as ScorchedEarth;
                if (scorchedEarth != null)
                {
                    scorchedEarth.OnExplode();
                }
            }
        }
    }
}
