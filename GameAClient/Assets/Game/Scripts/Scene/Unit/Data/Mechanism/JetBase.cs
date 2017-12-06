﻿/********************************************************************
** Filename : JetBase
** Author : Dong
** Date : 2017/5/16 星期二 上午 10:52:35
** Summary : JetBase
***********************************************************************/

using System;
using System.Collections;
using System.Web;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class JetBase : Magic
    {
        protected SkillCtrl _skillCtrl;
        protected int _timeScale;
        protected int _weaponId;
        protected UnityNativeParticleItem _efffectWeapon;
        protected ERotateMode _eRotateType;
        protected float _endAngle;
        protected float _curAngle;
        protected int _timeDelay;
        protected int _attackInterval;

        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        public override float Angle
        {
            get { return _curAngle; }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _timeScale = 1;
            SetSortingOrderBackground();
            return true;
        }

        public override UnitExtra UpdateExtraData()
        {
            var unitExtra = base.UpdateExtraData();
            _weaponId = unitExtra.ChildId;
            _eRotateType = (ERotateMode) unitExtra.RotateMode;
            _endAngle = GM2DTools.GetAngle(unitExtra.RotateValue);
            _timeDelay = TableConvert.GetTime(unitExtra.TimeDelay);
            _attackInterval = TableConvert.GetTime(unitExtra.TimeInterval);
            _attackInterval = Math.Max(25, _attackInterval);
            return unitExtra;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _trans.localEulerAngles = new Vector3(0, 0, -_angle);
            return true;
        }

        protected override void Clear()
        {
            _curAngle = _angle;
            if (_trans != null)
            {
                _trans.localEulerAngles = new Vector3(0, 0, -_angle);
            }
            _skillCtrl = null;
            base.Clear();
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            SetWeapon(_weaponId);
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            FreeEffect(_efffectWeapon);
            _efffectWeapon = null;
        }

        public override bool SetWeapon(int id, UnitExtra unitExtra = default(UnitExtra))
        {
            if (id == 0)
            {
                return false;
            }
            var tableEquipment = TableManager.Instance.GetEquipment(id);
            if (tableEquipment == null)
            {
                LogHelper.Error("GetEquipment Failed : {0}", id);
                return false;
            }
            FreeEffect(_efffectWeapon);
            _efffectWeapon = null;
            if (!string.IsNullOrEmpty(tableEquipment.Model))
            {
                _efffectWeapon = GameParticleManager.Instance.GetUnityNativeParticleItem(tableEquipment.Model, _trans);
                if (_efffectWeapon != null)
                {
                    _efffectWeapon.Trans.localPosition = new Vector3(0f, -0.22f, -10f);
                    _efffectWeapon.Play();
                }
            }
            _skillCtrl = _skillCtrl ?? new SkillCtrl(this);
            _skillCtrl.SetSkill(tableEquipment.SkillId);
            SetSkillValue();
            return true;
        }

        public override void UpdateLogic()
        {
            if (_eActiveState != EActiveState.Active)
            {
                return;
            }
            //timeDelay
            if (_timeDelay > 0)
            {
                _timeDelay--;
                return;
            }
            //MoveDirection
            base.UpdateLogic();
            //Rotate
            if (_eRotateType != ERotateMode.None)
            {
                switch (_eRotateType)
                {
                    case ERotateMode.Clockwise:
                        _curAngle += 1;
                        break;
                    case ERotateMode.Anticlockwise:
                        _curAngle += -1;
                        break;
                }
                Util.CorrectAngle360(ref _curAngle);
                if (!Util.IsFloatEqual(_angle, _endAngle))
                {
                    if (Util.IsFloatEqual(_curAngle, _angle) || Util.IsFloatEqual(_curAngle, _endAngle))
                    {
                        _eRotateType = _eRotateType == ERotateMode.Clockwise
                            ? ERotateMode.Anticlockwise
                            : ERotateMode.Clockwise;
                    }
                }
                if (_trans != null)
                {
                    _trans.localEulerAngles = new Vector3(0, 0, -_curAngle);
                }
            }
            if (_skillCtrl != null)
            {
                _skillCtrl.UpdateLogic();
                if (_skillCtrl.Fire(0))
                {
                    if (_animation != null)
                    {
                        _animation.PlayOnce("Start", _timeScale);
                    }
                }
            }
        }
    }
}