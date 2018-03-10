/********************************************************************
** Filename : Switch
** Author : Dong
** Date : 2017/3/16 星期四 下午 4:41:07
** Summary : Switch
***********************************************************************/

using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 8102, Type = typeof(SwitchMagic))]
    public class SwitchMagic : BlockBase, ICanBulletHit
    {
        protected List<UnitBase> _units;
        protected UnityNativeParticleItem _effectStart;

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }

            _effectStart = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectSwitchStart", _trans);
            return true;
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            FreeEffect(_effectStart);
            _effectStart = null;
        }

        protected override void Clear()
        {
            base.Clear();
            _units = null;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            _units = DataScene2D.CurScene.GetControlledUnits(_guid);
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (UnitDefine.IsBullet(other.Id))
            {
                if (!checkOnly)
                {
                    OnTrigger();
                }

                return base.OnUpHit(other, ref y, checkOnly);
            }

            return false;
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (UnitDefine.IsBullet(other.Id))
            {
                if (!checkOnly)
                {
                    OnTrigger();
                }

                return base.OnDownHit(other, ref y, checkOnly);
            }

            return false;
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (UnitDefine.IsBullet(other.Id))
            {
                if (!checkOnly)
                {
                    OnTrigger();
                }

                return base.OnLeftHit(other, ref x, checkOnly);
            }

            return false;
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (UnitDefine.IsBullet(other.Id))
            {
                if (!checkOnly)
                {
                    OnTrigger();
                }

                return base.OnRightHit(other, ref x, checkOnly);
            }

            return false;
        }

        public void OnBulletHit(Bullet bullet)
        {
            OnTrigger();
        }

        private void OnTrigger()
        {
            if (_effectStart != null && !_effectStart.IsPlaying)
            {
                _effectStart.Play(0.5f);
            }

            if (_units != null)
            {
                for (int i = 0; i < _units.Count; i++)
                {
                    var unit = _units[i];
                    if (unit != null && unit.IsAlive)
                    {
                        unit.OnCtrlBySwitch();
                    }
                }
            }

            Messenger.Broadcast(EMessengerType.OnSwitchTriggered);
        }
    }
}