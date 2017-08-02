/********************************************************************
** Filename : JetBase
** Author : Dong
** Date : 2017/5/16 星期二 上午 10:52:35
** Summary : JetBase
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    public class JetBase : Magic
    {
        protected SkillCtrl _skillCtrl;
        protected int _timeScale;
        protected int _weaponId;

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
            return true;
        }

        public override void UpdateExtraData()
        {
            _weaponId = DataScene2D.Instance.GetUnitExtra(_guid).UnitValue;
            ChangeWeapon(_weaponId);
            base.UpdateExtraData();
        }
        
        public override bool ChangeWeapon(int id)
        {
            var tableEquipment = TableManager.Instance.GetEquipment(id);
            if (tableEquipment == null)
            {
                LogHelper.Error("GetEquipment Failed : {0}", id);
                return false;
            }
            _skillCtrl = _skillCtrl ?? new SkillCtrl(this, 1);
            _skillCtrl.Clear();
            _skillCtrl.ChangeSkill(tableEquipment.SkillIds[0]);
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
                    if (_skillCtrl.FireForever(0))
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
