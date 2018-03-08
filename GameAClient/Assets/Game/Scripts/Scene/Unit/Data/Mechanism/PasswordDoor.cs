namespace GameA.Game
{
    [Unit(Id = 5028, Type = typeof(PasswordDoor))]
    public class PasswordDoor : BlockBase
    {
        private bool _opened;
        protected int _timer;
        private int _password;

        public override UnitExtraDynamic UpdateExtraData(UnitExtraDynamic unitExtraDynamic = null)
        {
            var unitExtra = base.UpdateExtraData(unitExtraDynamic);
            _password = unitExtra.CommonValue;
            return unitExtra;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }

            if (_opened)
            {
                if (_animation != null)
                {
                    var entry = _animation.PlayOnce("Open");
                    entry.time = entry.endTime;
                }

                SetOpen(true);
            }

            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _timer = 0;
            _opened = false;
            SetOpen(false);
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (_opened)
            {
                return false;
            }

            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (_opened)
            {
                return false;
            }

            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (_opened)
            {
                return false;
            }

            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (_opened)
            {
                return false;
            }

            return base.OnRightHit(other, ref x, checkOnly);
        }

        public bool CheckOpen(int value)
        {
            return _password == value;
        }

        public void ShowOpen()
        {
            SetEnabled(false);
            _timer = 50;
            if (_animation != null)
            {
                _animation.PlayOnce("Open3");
            }
        }

        private void SetOpen(bool value)
        {
            SetCross(value);
            if (value)
            {
                SetSortingOrderBackground();
            }
            else
            {
                SetSortingOrderNormal();
            }
            UpdateTransPos();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_timer > 0)
            {
                _timer--;
                if (_timer == 0)
                {
                    _opened = true;
                    SetOpen(true);
                }
            }
        }
    }
}