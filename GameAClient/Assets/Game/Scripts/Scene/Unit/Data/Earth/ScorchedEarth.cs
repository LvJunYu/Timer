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
        protected int _state;
        protected int _timer;

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _animation.Init("Run");
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _state = 0;
            _timer = 0;
            PlayMode.Instance.UnFreeze(this);
            if (_view != null)
            {
                _view.SetRendererEnabled(true);
            }
            _canLazerCross = false;
            _canMagicCross = false;
            _canBridgeCross = false;
            _canFanCross = false;
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (_state == 2)
            {
                return false;
            }
            OnTrigger(other, checkOnly);
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (_state == 2)
            {
                return false;
            }
            OnTrigger(other, checkOnly);
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (_state == 2)
            {
                return false;
            }
            OnTrigger(other, checkOnly);
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (_state == 2)
            {
                return false;
            }
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
            if (_state == 0)
            {
                _state = 1;
                if (UseMagic())
                {
                    _run = false;
                }
            }
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_state != 0)
            {
                _timer++;
                if (_timer == 30)
                {
                    //开始爆炸
                    _state = 2;
                    _canLazerCross = true;
                    _canMagicCross = true;
                    _canBridgeCross = true;
                    _canFanCross = true;
                    if (_view != null)
                    {
                        _view.SetRendererEnabled(false);
                        GameAudioManager.Instance.PlaySoundsEffects(_tableUnit.DestroyAudioName);
                        GameParticleManager.Instance.Emit(_tableUnit.DestroyEffectName, _trans.position);
                    }
                    //查收周边的让其爆炸
                    SendMsgToAround();
                }
                else if (_timer >= 200)
                {
                    var units = ColliderScene2D.GridCastAllReturnUnits(_colliderGrid);
                    if (units.Count > 0)
                    {
                        for (int i = 0; i < units.Count; i++)
                        {
                            UnitBase unit = units[i];
                            if (IsBlockedBy(unit))
                            {
                                return;
                            }
                        }
                    }
                    //复原
                    _state = 0;
                    _canLazerCross = false;
                    _canMagicCross = false;
                    _canBridgeCross = false;
                    _canFanCross = false;
                    if (UseMagic())
                    {
                        _run = true;
                    }
                    _timer = 0;
                    if (_view != null)
                    {
                        _view.SetRendererEnabled(true);
                    }
                }
            }
        }

        private void SendMsgToAround()
        {
            CheckGrid(GetYGrid(30));
            CheckGrid(GetYGrid(-30));
            CheckGrid(GetXGrid(-30));
            CheckGrid(GetXGrid(30));
        }

        private void CheckGrid(Grid2D grid)
        {
            var units = ColliderScene2D.GridCastAllReturnUnits(grid);
            if (units.Count > 0)
            {
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (unit != null && unit.IsAlive && unit != this)
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
    }
}
