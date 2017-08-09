using SoyEngine;
using SoyEngine.Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameA.Game;
using NewResourceSolution;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlWeaponUpgrade : UICtrlGenericBase<UIViewWeaponUpgrade>
    {

        #region Fields
        private int _weaponID;
        private int _weaponLv;
        private int _weaponlevelID;
        private int _isCompoudAddNum;
        private int _needGoldCoinNum;
        private int _needWeaponPartNum;
        private int _needUniversalNum;
        private string _universalSpriteName = "universalpart";
        private Sprite _universalSprie;
        private string _weaponPartSpriteName;
        private Sprite _weaponPartSprite;
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
            _isCompoudAddNum = _weaponIDlv[3];
            _needGoldCoinNum = _weaponIDlv[4];
            _needWeaponPartNum = _weaponIDlv[5];
            _needUniversalNum = _weaponIDlv[6];
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
            if (_isCompoudAddNum == 0)
            {
                _cachedView.TipUpgrade.text = "合成";
                _cachedView.HpAdd.text = TableManager.Instance.GetEquipmentLevel(_weaponlevelID + _isCompoudAddNum).HpAdd.ToString();
                _cachedView.AttackAdd.text =  TableManager.Instance.GetEquipmentLevel(_weaponlevelID + _isCompoudAddNum).AttackAdd.ToString();
                _cachedView.SkillEffect.text =  TableManager.Instance.GetEquipmentLevel(_weaponlevelID + _isCompoudAddNum).SkillEffect.ToString();
            }
            else
            {
                _cachedView.TipUpgrade.text = "升级";
                _cachedView.HpAdd.text = TableManager.Instance.GetEquipmentLevel(_weaponlevelID).HpAdd.ToString() + "---" + TableManager.Instance.GetEquipmentLevel(_weaponlevelID + _isCompoudAddNum).HpAdd.ToString();
                _cachedView.AttackAdd.text = TableManager.Instance.GetEquipmentLevel(_weaponlevelID).AttackAdd.ToString() + "---" + TableManager.Instance.GetEquipmentLevel(_weaponlevelID + _isCompoudAddNum).AttackAdd.ToString();
                _cachedView.SkillEffect.text = TableManager.Instance.GetEquipmentLevel(_weaponlevelID).SkillEffect.ToString() + "---" + TableManager.Instance.GetEquipmentLevel(_weaponlevelID + _isCompoudAddNum).SkillEffect.ToString();
        }
            _cachedView.WeaponName.text = TableManager.Instance.GetEquipment(_weaponID).Name;
            //武器碎片的图标
             _weaponPartSpriteName = TableManager.Instance.GetEquipment(_weaponID).WeaponPartIcon;
            ResourcesManager.Instance.TryGetSprite(_weaponPartSpriteName, out _weaponPartSprite);
            _cachedView.WeaponFragmentIcon.sprite = _weaponPartSprite;

            //万能碎片的图片

            ResourcesManager.Instance.TryGetSprite(_universalSpriteName, out _universalSprie);
            _cachedView.UniversalFragmentsIcon.sprite = _universalSprie;
            //金币的数目
            _cachedView.CoinNum.text = _needGoldCoinNum.ToString();
            //武器碎片的名字
            _cachedView.WeaponFragmentNum.text = _needWeaponPartNum.ToString();
            //万能图标数量
            if (_needUniversalNum > 0)
            {
                _cachedView.UniversalPart.SetActive(true);
                _cachedView.UniversalFragmentsNum.text = _needUniversalNum.ToString();
            }
            else
            {
                _cachedView.UniversalPart.SetActive(false);
            }
        }

        private void OnConfirm()
        {

        }
        #endregion
    }
}
