/********************************************************************
** Filename : JetBase
** Author : Dong
** Date : 2017/5/16 星期二 上午 10:52:35
** Summary : JetBase
***********************************************************************/

using System;
using System.Collections;
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

        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _shootAngle = (_unitDesc.Rotation) * 90;
            _timeScale = 1;
            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            InitAssetRotation();
            SetWeapon(_weaponId);
            return true;
        }
        
        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            FreeEffect(_efffectWeapon);
            _efffectWeapon = null;
        }

        public override void UpdateExtraData()
        {
            _weaponId = DataScene2D.Instance.GetUnitExtra(_guid).UnitValue;
            SetWeapon(_weaponId);
            base.UpdateExtraData();
        }
        
        public override bool SetWeapon(int id)
        {
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
                    switch ((EDirectionType) Rotation)
                    {
                        case EDirectionType.Up:
                            _efffectWeapon.Trans.localPosition = new Vector3(0f,0.4f,-10f);
                            break;
                        case EDirectionType.Down:
                            _efffectWeapon.Trans.localPosition = new Vector3(0f,0.8f,-10f);
                            break;
                        case EDirectionType.Left:
                            _efffectWeapon.Trans.localPosition = new Vector3(0.23f,0.6f,-10f);
                            break;
                        case EDirectionType.Right:
                            _efffectWeapon.Trans.localPosition = new Vector3(-0.23f,0.6f,-10f);
                            break;
                    }
                    _efffectWeapon.Play();
                }
            }
            _skillCtrl = _skillCtrl ?? new SkillCtrl(this);
            _skillCtrl.SetSkill(tableEquipment.SkillIds[0]);
            SetValue();
            return true;
        }

        protected virtual void SetValue()
        {
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (!_ctrlBySwitch)
            {
                if (_skillCtrl != null)
                {
                    _skillCtrl.UpdateLogic();
                    if (_skillCtrl.Fire(0))
                    {
                        if (_animation != null)
                        {
                            _animation.PlayOnce(((EDirectionType)_unitDesc.Rotation).ToString(), _timeScale);
                        }
                    }
                }
            }
        }
    }
}
