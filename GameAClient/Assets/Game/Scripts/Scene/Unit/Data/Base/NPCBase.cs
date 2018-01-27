using UnityEngine;

namespace GameA.Game
{
    public class NPCBase : MonsterAI_2
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

        public override bool IsMonster
        {
            get { return false; }
        }

        public override bool IsInvincible
        {
            get { return true; }
        }

        public override bool CanControlledBySwitch
        {
            get { return false; }
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
                if (GetUnitExtra().NpcDialog == null)
                {
                    return;
                }
                if (_diaPop == null)
                {
                    _diaPop = SocialGUIManager.Instance.GetUI<UICtrlGameScreenEffect>()
                        .GetNpcDialog(GetUnitExtra().NpcDialog, _trans.position);
                }
                SocialGUIManager.Instance.GetUI<UICtrlGameScreenEffect>().SetDymicPos(_diaPop, _trans.position);
                if (GetUnitExtra().NpcShowType == (ushort) ENpcTriggerType.Close)
                {
                    if (CheckPlayerPos())
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

        protected override void Clear()
        {
            base.Clear();
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

        private bool CheckPlayerPos()
        {
            float x = Mathf.Abs((PlayerManager.Instance.MainPlayer.Trans.position - _trans.position).x);
            return x <= 50;
        }

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            if (eDirectionType == EDirectionType.Up || eDirectionType == EDirectionType.Down)
            {
                return;
            }

            if (unit.IsActor)
            {
                ChangeState(EMonsterState.Dialog);
            }
            else
            {
                ChangeState(EMonsterState.Bang);
            }

            switch (eDirectionType)
            {
                case EDirectionType.Left:
                    SetNextMoveDirection(EMoveDirection.Right);
                    break;
                case EDirectionType.Right:
                    SetNextMoveDirection(EMoveDirection.Left);
                    break;
            }
        }

        protected override void CreateStatusBar()
        {
        }

        protected override void UpdateAttackTarget(UnitBase lastTarget = null)
        {
            if (_attactTarget == null)
            {
                _attactTarget = PlayMode.Instance.MainPlayer;
            }
        }
    }
}