namespace GameA.Game
{
    [Unit(Id = 5028, Type = typeof(PasswordDoor))]
    public class PasswordDoor : BlockBase
    {
        private bool _opened; //门已经打开
        private int _timer;
        private int _uiTimer;
        private int _password;
        private PlayerBase _player;
        private bool _uiOpen;
        private EDirectionType _directionRelativeMain;
        private bool _hasCorrected; //已经输入正确密码

        public int UiTimer
        {
            get { return _uiTimer; }
            set { _uiTimer = value; }
        }

        public bool UiOpen
        {
            get { return _uiOpen; }
            set { _uiOpen = value; }
        }

        public bool HasCorrected
        {
            get { return _hasCorrected; }
        }

        public EDirectionType DirectionRelativeMain
        {
            get { return _directionRelativeMain; }
            set { _directionRelativeMain = value; }
        }

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

            if (_opened || _timer > 0)
            {
                if (_animation != null)
                {
                    var entry = _animation.PlayOnce("Open3");
                    entry.time = entry.endTime;
                }

                SetOpen(true);
            }

            return true;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_uiTimer > 0)
            {
                _uiTimer--;
            }

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

        protected override void Clear()
        {
            base.Clear();
            _timer = 0;
            _uiTimer = 0;
            _opened = false;
            _hasCorrected = false;
            _player = null;
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

        public bool CheckOpen(ushort value)
        {
            return _password == value;
        }

        public void OnPasswordDoorOpen()
        {
            if (_player != null && _player.IsMain)
            {
                ((MainPlayer) _player).SetKeyDown(EInputType.PasswordDoorOpen);
            }

            _hasCorrected = true;
        }

        public void ShowOpen()
        {
            SetEnabled(false);
            _timer = 50;
            if (_animation != null)
            {
                _animation.PlayOnce("Open3");
            }

            _player = null;
        }

        public void OnUiOpen(bool value)
        {
            if (_player != null && _player.IsMain)
            {
                ((MainPlayer) _player).SetInputValid(!value);
            }

            if (!value && _uiOpen && !_hasCorrected)
            {
                if (_player != null && _player.IsMain)
                {
                    ((MainPlayer) _player).SetKeyDown(EInputType.PasswordDoorUIClose);
                }
            }
        }

        public void OnPlayerDead(PlayerBase player)
        {
            if (_player == player)
            {
                if (_uiOpen)
                {
                    SocialGUIManager.Instance.CloseUI<UICtrlPasswordDoorInGame>();
                }

                _player = null;
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

        public void SetPlayer(PlayerBase player)
        {
            _player = player;
        }
    }
}