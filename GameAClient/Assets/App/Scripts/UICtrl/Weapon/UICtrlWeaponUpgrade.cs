using SoyEngine;
using SoyEngine.Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameA.Game;
using SoyEngine;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlWeaponUpgrade : UICtrlGenericBase<UIViewWeaponUpgrade>
    {

        #region Fields
        private int _weaponID;
        private int _weaponLv;
        private int _weaponlevelID;
        #endregion

        #region Properties
        #endregion

        #region Methods
        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            int[] _weaponIDlv = parameter as int[];
            _weaponID = _weaponIDlv[0];
            _weaponLv = _weaponIDlv[1];
            _weaponlevelID = _weaponIDlv[2];
            InitData();


        }

        protected override void OnClose()
        {

            base.OnClose();

        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
           

        }

        public override void OnUpdate()
        {
            base.OnUpdate();

        }

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlWeaponUpgrade>();
        }
        private void InitData()
        {
            Debug.Log(_weaponID);
            _cachedView.WeaponName.text = TableManager.Instance.GetEquipment(_weaponID).Name;
           
            _cachedView.HpAdd.text = TableManager.Instance.GetEquipmentLevel(_weaponlevelID).HpAdd.ToString() + "---" + TableManager.Instance.GetEquipmentLevel(_weaponlevelID + 1).HpAdd.ToString();
            _cachedView.AttackAdd.text = TableManager.Instance.GetEquipmentLevel(_weaponlevelID).AttackAdd.ToString() + "---" + TableManager.Instance.GetEquipmentLevel(_weaponlevelID + 1).AttackAdd.ToString();
            _cachedView.SkillEffect.text = TableManager.Instance.GetEquipmentLevel(_weaponlevelID).SkillEffect.ToString() + "---" + TableManager.Instance.GetEquipmentLevel(_weaponlevelID + 1).SkillEffect.ToString();
        }

        #endregion
    }
}
