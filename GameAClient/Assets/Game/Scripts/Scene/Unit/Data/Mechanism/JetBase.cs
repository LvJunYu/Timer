/********************************************************************
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
        protected ERotateType _eRotateType;
        protected float _endAngle;
        protected float _curAngle;
        protected int _timeDelay;
        protected int _timeInterval;
        
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
            return true;
        }
        
        public override void UpdateExtraData()
        {
            var unitExtra = DataScene2D.Instance.GetUnitExtra(_guid);
            _weaponId = unitExtra.ChildId;
            _eRotateType = (ERotateType) unitExtra.RotateMode;
            _endAngle = GM2DTools.GetAngle(unitExtra.RotateValue);
            _timeDelay = TableConvert.GetTime(unitExtra.TimeDelay);
            _timeInterval = TableConvert.GetTime(unitExtra.TimeInterval);
            _timeInterval = Math.Max(25, _timeInterval);
            base.UpdateExtraData();
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            SetWeapon(_weaponId);
            return true;
        }

        protected override void Clear()
        {
            if (_skillCtrl != null)
            {
                _skillCtrl.Clear();
            }
            _curAngle = _angle;
            base.Clear();
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            FreeEffect(_efffectWeapon);
            _efffectWeapon = null;
        }
        
        public override bool SetWeapon(int id)
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
                    _efffectWeapon.Trans.localPosition = new Vector3(0f,0.4f,-10f);
                    _efffectWeapon.Play();
                }
            }
            _skillCtrl = _skillCtrl ?? new SkillCtrl(this);
            _skillCtrl.SetSkill(tableEquipment.SkillId);
            SetValue();
            return true;
        }

        protected virtual void SetValue()
        {
        }

        public override void UpdateLogic()
        {
            if (!_activeState)
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
            if (_eRotateType != ERotateType.None)
            {
                switch (_eRotateType)
                {
                    case ERotateType.Clockwise:
                        _curAngle += 1;
                        break;
                    case ERotateType.Anticlockwise:
                        _curAngle += -1;
                        break;
                }
                Util.CorrectAngle360(ref _curAngle);
                if (Util.IsFloatEqual(_curAngle, _angle) || Util.IsFloatEqual(_curAngle, _endAngle))
                {
                    _eRotateType = _eRotateType == ERotateType.Clockwise ? ERotateType.Anticlockwise : ERotateType.Clockwise;
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
