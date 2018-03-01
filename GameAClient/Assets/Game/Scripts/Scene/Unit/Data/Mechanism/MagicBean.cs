﻿using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 5023, Type = typeof(MagicBean))]
    public class MagicBean : CollectionBase
    {
        public const int MaxCarryCount = 5;

        public static Table_Equipment TableEquipment
        {
            get
            {
                return new Table_Equipment
                {
                    Id = -1,
                    InputType = 2,
                    Icon = "M1MagicBeanIcon"
                };
            }
        }

        private PlayerBase _putDownPlayer;
        private bool _justPutDown;
        private int _justPutDownTimer;

        protected override void OnTrigger(UnitBase other)
        {
            if (!_justPutDown && ((PlayerBase) other).PickUpMagicBean())
            {
                base.OnTrigger(other);
            }
        }

        public override void UpdateLogic()
        {
            if (_justPutDown && _putDownPlayer != null)
            {
                if (_justPutDownTimer > 0)
                {
                    _justPutDownTimer--;
                }
                else
                {
                    if (!_colliderGrid.Intersects(_putDownPlayer.ColliderGrid))
                    {
                        _justPutDown = false;
                    }
                }
            }

            base.UpdateLogic();
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            GetDownUnit();
        }

        protected override void Clear()
        {
            base.Clear();
            _justPutDown = false;
            _putDownPlayer = null;
        }

        public void PutDownByPlayer(PlayerBase playerBase)
        {
            _putDownPlayer = playerBase;
            _justPutDown = true;
            _justPutDownTimer = 10;
        }

        public void OnBulletHit(Bullet bullet)
        {
            if (bullet.Id != UnitDefine.WaterBulletId)
            {
                return;
            }

            if (_downUnit != null && UnitDefine.CanGrowCirrus(_downUnit))
            {
                CirrusManager.Instance.CreateCirrus(_curPos);
                PlayMode.Instance.DestroyUnit(this);
            }
        }

        private void GetDownUnit()
        {
            var grid = new Grid2D(_colliderGrid.XMin + 1, _colliderGrid.YMin - 1, _colliderGrid.XMax - 1,
                _colliderGrid.YMin - 1);
            var units = ColliderScene2D.GridCastAllReturnUnits(grid, EnvManager.ItemLayer);
            if (units.Count == 1)
            {
                _downUnit = units[0];
            }
        }
    }
}