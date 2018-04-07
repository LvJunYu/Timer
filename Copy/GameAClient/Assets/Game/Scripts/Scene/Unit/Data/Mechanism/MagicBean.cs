using DG.Tweening;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 5023, Type = typeof(MagicBean))]
    public class MagicBean : CollectionBase, ICanBulletHit
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

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }

            _tweener = _trans.DOMoveY(_trans.position.y + 0.1f, 0.6f);
            _tweener.Play();
            _tweener.SetLoops(-1, LoopType.Yoyo);

            return true;
        }

        protected override void OnTrigger(UnitBase other)
        {
            if (!_justPutDown && ((PlayerBase) other).PickUpMagicBean())
            {
                base.OnTrigger(other);
                PlayMode.Instance.DestroyUnit(this);
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
                    if (!ColliderGrid.Intersects(_putDownPlayer.ColliderGrid))
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

        protected override void OnLastTrigger()
        {
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
                OnDead();
                PlayMode.Instance.DestroyUnit(this);
            }
        }

        private void GetDownUnit()
        {
            var grid = new Grid2D(ColliderGrid.XMin + 1, ColliderGrid.YMin - 1, ColliderGrid.XMax - 1,
                ColliderGrid.YMin - 1);
            using (var units = ColliderScene2D.GridCastAllReturnUnits(grid, EnvManager.ItemLayer))
            {
                if (units.Count == 1)
                {
                    _downUnit = units[0];
                }
            }
        }
    }
}