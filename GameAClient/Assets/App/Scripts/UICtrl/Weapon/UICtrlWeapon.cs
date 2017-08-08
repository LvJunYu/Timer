using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using NewResourceSolution;
using GameA.Game;
namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlWeapon : UICtrlGenericBase<UIViewWeapon>
    {
        #region Fields
        private Sprite _weaponEffect = null;
        private string _weaponEffectSpriteName;
        private int _weaponID = 101;
        private int _skillID;
        private int _weaponlevelID ;
        private int _weaponLv = 2;
        private int _multiple = 100;
        private Color _weaponColor;
        private string _colorName;
        private int[] _idColltions = new int[] { 101,102,103,201,202,203};
        private string[] _weaponName = new string[] { "weapon1", "weapon2", "weapon3", "weapon4", "weapon5", "weapon6", };
        private int index = 0;
        private Action _closeCB;
        #endregion

        #region Properties
        #endregion

        #region Methods
        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
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
            _cachedView.CloseButton.onClick.AddListener(OnCloseBtn);
            _cachedView.LeftWeapon.onClick.AddListener(OnLeftButton);
            _cachedView.RightWeapon.onClick.AddListener(OnRightButton);
            _cachedView.UpGrade.onClick.AddListener(OnUpgrade);
            RefershWeaponShow();


        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }
        private void OnLeftButton()
        {
            //  _weaponID = TableManager.Instance.GetEquipment.
            if (index > 0)
            {
                _weaponID = _idColltions[--index ];
                RefershWeaponShow();
            }
         
       
        }
        private void OnRightButton()
        {
            if (index < _idColltions.Length - 1)
            {
                _weaponID = _idColltions[++index];
                RefershWeaponShow();

            }
        }
        private void OnUpgrade()
        {
            int[] _weaponIDlv = new int[] { _weaponID,_weaponLv , _weaponlevelID };
            SocialGUIManager.Instance.OpenUI<UICtrlWeaponUpgrade>(_weaponIDlv);
        }
        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlWeapon>();
        }
        private void RefershWeaponShow()
        {
            // _weaponEffectSpriteName = TableManager.Instance.GetEquipment(_weaponID).Icon; 

            ResourcesManager.Instance.TryGetSprite(_weaponName[index], out _weaponEffect);
            Debug.Log(_weaponEffect);
            _cachedView.EffectShow.sprite = _weaponEffect;
            _skillID = TableManager.Instance.GetEquipment(_weaponID).SkillId;
            _weaponlevelID = _skillID * _multiple + _weaponLv;
            _cachedView.SkillDescription.text = TableManager.Instance.GetSkill(_skillID).Summary;
            _cachedView.HpAddNum.text = TableManager.Instance.GetEquipmentLevel(_weaponlevelID).HpAdd.ToString();
            _cachedView.AttackAddNum.text = TableManager.Instance.GetEquipmentLevel(_weaponlevelID).AttackAdd.ToString();
            _cachedView.WeaponName.text = TableManager.Instance.GetEquipment(_weaponID).Name;
           // _colorName = TableManager.Instance.GetEquipment(_weaponID).Color;
            ColorUtility.TryParseHtmlString("#F20000FF", out _weaponColor);
            _cachedView.WeaponName.color = _weaponColor;
            _cachedView.WeaponLv.text = _weaponLv.ToString();
            }

        #endregion
    }
}
