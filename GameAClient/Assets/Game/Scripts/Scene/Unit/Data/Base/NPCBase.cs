using UnityEngine;

namespace GameA.Game
{
    public class NPCBase : ActorBase
    {
        private bool _trigger;
        private UnitBase _unit;
        private int _time;
        private UMCtrlNpcDiaPop _diaPop;
        private int _showIntervalTime;
        private int _timeIntervalDynamic;
        private int _showTime = 150;

        protected override bool IsCheckClimb()
        {
            return false;
        }

        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        public override void OnIntersect(UnitBase other)
        {
            if (other.IsMain)
            {
                OnTrigger(other);
            }
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            _timeIntervalDynamic++;
            if (_trigger)
            {
                _time++;
                if (!_colliderGrid.Intersects(_unit.ColliderGrid) && _time >= 100)
                {
                    _trigger = false;
                    _unit = null;
                }
            }

            if (GetUnitExtra().NpcType == (byte) ENpcType.Dialog)
            {
                if (_diaPop == null)
                {
                    _diaPop = SocialGUIManager.Instance.GetUI<UICtrlGameScreenEffect>()
                        .GetNpcDialog(GetUnitExtra().NpcDialog, _trans.position);
                }
                if (GetUnitExtra().NpcShowType == (ushort) ENpcTriggerType.Close)
                {
                    if (CheckPlayPos())
                    {
                        _diaPop.Show();
                    }
                    else
                    {
                        _diaPop.Hide();
                    }
                }
                if (GetUnitExtra().NpcShowType == (ushort) ENpcTriggerType.Interval)
                {
                    _showIntervalTime = GetUnitExtra().NpcShowInterval * 30;
                    if (_timeIntervalDynamic > _showIntervalTime)
                    {
                        _diaPop.Show();
                    }
                    if (_timeIntervalDynamic > _showTime + _showIntervalTime)
                    {
                        _timeIntervalDynamic = 0;
                        _diaPop.Hide();
                    }
                }
            }
            if (GetUnitExtra().NpcType == (byte) ENpcType.Task)
            {
            }
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _input = _input ?? new InputBase();
            _input.Clear();
            _time = 0;
            if (_diaPop != null)
            {
                UMPoolManager.Instance.Free(_diaPop);
            }
            _trigger = false;
            _diaPop = null;
            _showIntervalTime = 0;
            _timeIntervalDynamic = 0;

            _unit = null;
        }

        protected virtual void OnTrigger(UnitBase other)
        {
            if (!_trigger)
            {
                _trigger = true;
                _unit = other;
                _time = 0;
            }
        }

        private bool CheckPlayPos()
        {
            float x = Mathf.Abs((PlayerManager.Instance.MainPlayer.Trans.position - _trans.position).x);
            return x <= 50;
        }
    }
}