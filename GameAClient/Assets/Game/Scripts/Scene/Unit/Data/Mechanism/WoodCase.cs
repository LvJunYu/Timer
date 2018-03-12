using System;

namespace GameA.Game
{
    [Serializable]
    [Unit(Id = 5030, Type = typeof(WoodCase))]
    public class WoodCase : Box, ICanBulletHit, ICanBombHit
    {
        private const int MaxFallDestroyDis = 2 * ConstDefineGM2D.ServerTileScale;
        private int _itemId;

        public override UnitExtraDynamic UpdateExtraData(UnitExtraDynamic unitExtraDynamic = null)
        {
            var unitExtra = base.UpdateExtraData(unitExtraDynamic);
            _itemId = unitExtra.CommonValue;
            return unitExtra;
        }

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            base.Hit(unit, eDirectionType);
            if (eDirectionType == EDirectionType.Down && FallDistance > MaxFallDestroyDis)
            {
                DestroyWoodCase();
            }
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                if (other.IsPlayer || UnitDefine.IsBullet(other.Id))
                {
                    DestroyWoodCase();
                }
            }

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

        public void OnBulletHit(Bullet bullet)
        {
            DestroyWoodCase();
        }

        protected void Check(UnitBase other, bool checkOnly)
        {
            if (!checkOnly)
            {
                if (UnitDefine.IsBullet(other.Id))
                {
                    DestroyWoodCase();
                }
                else if (other is Box)
                {
                    if (((Box)other).FallDistance > MaxFallDestroyDis)
                    {
                        DestroyWoodCase();
                    }
                }
            }
        }

        private void DestroyWoodCase()
        {
            if (!_isAlive)
            {
                return;
            }

            OnDead();
            CreateItem();
        }

        private void CreateItem()
        {
            if (_itemId == 0)
            {
                return;
            }

            var item = PlayMode.Instance.CreateRuntimeUnit(_itemId, _curPos);
            if (item != null)
            {
                item.OnPlay();
            }
        }

        public void OnBombHit()
        {
            DestroyWoodCase();
        }
    }
}