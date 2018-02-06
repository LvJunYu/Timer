﻿using System;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using Object = UnityEngine.Object;

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
        private int _closeDis = 1;
        private NpcStateBar _stateBar;
        private Action _oldState;

        public Action OldState
        {
            get { return _oldState; }
        }

        public Action NowState
        {
            get { return _nowState; }
        }

        private Action _nowState;

        public NpcStateBar StateBar
        {
            get { return _stateBar; }
            set { _stateBar = value; }
        }

        protected override bool OnInit()
        {
            return base.OnInit();
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            if (GetUnitExtra().MoveDirection == EMoveDirection.None)
            {
                ChangeState(EMonsterState.Idle);
            }
            SetNpcName();
            SetNoShow();
        }

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
            SetDia();

//            if (GetUnitExtra().NpcType == (byte) ENpcType.Task)
//            {
//                foreach (var VARIABLE in _hitUnits)
//                {
//                    Debug.Log(VARIABLE.Id);
//                }
//            }
        }

        private void SetDia()
        {
            if (GetUnitExtra().NpcType == (byte) ENpcType.Dialog)
            {
                if (GetUnitExtra().NpcDialog == null)
                {
                    return;
                }
                if (GetUnitExtra().NpcDialog.Length == 0)
                {
                    return;
                }
                if (_diaPop == null)
                {
                    _diaPop = SocialGUIManager.Instance.GetUI<UICtrlGameScreenEffect>()
                        .GetNpcDialog(GetUnitExtra().NpcDialog, _trans.position);
                }
                if (GetUnitExtra().NpcShowType == (ushort) ENpcTriggerType.Close)
                {
                    if (CheckPlayerPos())
                    {
                        _diaPop.Show();
                        SocialGUIManager.Instance.GetUI<UICtrlGameScreenEffect>().SetDymicPos(_diaPop, _trans.position);
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
                        SocialGUIManager.Instance.GetUI<UICtrlGameScreenEffect>().SetDymicPos(_diaPop, _trans.position);
                    }

                    if (_timeIntervalDynamic > _showTime + _showIntervalTime)
                    {
                        _timeIntervalDynamic = 0;
                        _diaPop.Hide();
                    }
                }
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
            if (_stateBar != null)
            {
                SetNpcNum();
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
            return x <= _closeDis;
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

        public void SetReady()
        {
            _stateBar.SetReady();
            _oldState = null;
            _nowState = SetReady;
        }

        public void SetInTask()
        {
            _stateBar.SetInTask();
            _oldState = null;
            _nowState = SetInTask;
        }

        public void SetFinishTask()
        {
            _stateBar.AllTaskFinish();
            _oldState = null;
            _nowState = SetFinishTask;
        }

        public void SetNpcNum()
        {
            _stateBar.SetNpcNum(GetUnitExtra().NpcSerialNumber);
        }

        public void SetShowTip()
        {
            _stateBar.ShowTip();
            _oldState = _nowState;
        }

        public void SetNoShow()
        {
            _stateBar.SetNoShow();
            _oldState = null;
            _nowState = SetFinishTask;
        }

        public void SetNpcName()
        {
            _stateBar.NameTextMesh.text = GetUnitExtra().NpcName;
        }

        internal override bool InstantiateView()
        {
            return base.InstantiateView();
        }

        protected override void CreateStatusBar()
        {
            if (null != _statusBar)
            {
                return;
            }
            GameObject statusBarObj =
                Object.Instantiate(JoyResManager.Instance.GetPrefab(EResType.ParticlePrefab, "NpcSerNumber")) as
                    GameObject;
            if (null != statusBarObj)
            {
                _stateBar = statusBarObj.GetComponent<NpcStateBar>();
                CommonTools.SetParent(statusBarObj.transform, _trans);
                SetNpcNum();
                SetNpcName();
            }
        }

        protected override void UpdateAttackTarget(UnitBase lastTarget = null)
        {
            if (_attactTarget == null)
            {
                _attactTarget = PlayMode.Instance.MainPlayer;
            }
        }

        internal override void OnObjectDestroy()
        {
            if (_stateBar != null)
            {
                Object.Destroy(_stateBar.gameObject);
                _stateBar = null;
            }
            base.OnObjectDestroy();
        }

        protected override void CheckAssist()
        {
            base.CheckAssist();
        }
    }
}