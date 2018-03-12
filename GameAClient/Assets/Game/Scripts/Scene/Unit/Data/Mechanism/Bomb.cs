using System;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Serializable]
    [Unit(Id = 5031, Type = typeof(Bomb))]
    public class Bomb : Box, ICanBulletHit
    {
        private const int MaxFallDestroyDis = 1 * ConstDefineGM2D.ServerTileScale;
        private SkillCtrl _skillCtrl;

        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            _skillCtrl = new SkillCtrl(this);
            _skillCtrl.SetSkill(_tableUnit.SkillId);
        }

        protected override void Clear()
        {
            base.Clear();
            _skillCtrl = null;
        }

        protected override void OnActiveStateChanged()
        {
            base.OnActiveStateChanged();
            if (GameRun.Instance.IsPlaying && _eActiveState == EActiveState.Deactive)
            {
                DestroyBomb();
            }
        }

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            base.Hit(unit, eDirectionType);
            if (eDirectionType == EDirectionType.Down && FallDistance > MaxFallDestroyDis)
            {
                DestroyBomb();
            }
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            Check(other, checkOnly);
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            Check(other, checkOnly);
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            Check(other, checkOnly);
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            Check(other, checkOnly);
            return base.OnRightHit(other, ref x, checkOnly);
        }

        protected void Check(UnitBase other, bool checkOnly)
        {
            if (!checkOnly)
            {
                if (UnitDefine.IsBullet(other.Id))
                {
                    DestroyBomb();
                }
                else if (UnitDefine.BoxId == other.Id)
                {
                    var box = other as Box;
                    if (box != null && box.FallDistance > MaxFallDestroyDis)
                    {
                        DestroyBomb();
                    }
                }
            }
        }

        private void DestroyBomb()
        {
            if (!_isAlive)
            {
                return;
            }

            OnDead();
            _skillCtrl.Fire(0);
        }

        public void OnBulletHit(Bullet bullet)
        {
            DestroyBomb();
        }

        public static int CalculateDamage(int dis, int maxPower, int maxRange)
        {
            int value = maxPower * (maxRange - dis) / maxRange;
            LogHelper.Debug("Bomb power = {0}", value);
            return value;
        }

        public static IntVec2 CalculateForce(int dis, IntVec2 direction, int maxPower, int maxRange)
        {
            var power = maxPower * 3 * (maxRange - dis) / maxRange * new Vector2(direction.x, direction.y).normalized;
            var force = new IntVec2((int) power.x, (int) power.y);
            if (force.y >= 0 && force.y < 40)
            {
                force.y = 40;
            }
            LogHelper.Debug("Bomb Force = {0}", power);
            return force;
        }
    }
}