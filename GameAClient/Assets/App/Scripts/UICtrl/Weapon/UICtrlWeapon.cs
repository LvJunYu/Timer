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
        private Sprite _weaponPartSprite;
        private string _weaponPartSpriteName;
        private Sprite _universalSprie;
        private string _universalSpriteName = "universalpart";
        private int _weaponID = 101;
        private int _skillID;
        private int _weaponLv;
        private int _weaponlevelID;
        private int _multiple = 100;
        private Color _weaponColor;
        private string _colorName;
        private int[] _idColltions;
        private int _universalCount ;
        private int index = 0;
        private int _isCompoudAddNum;
        private int _userGoldCoin= 10;
        private int _needCoin;
        private int _needPart;
        private int _userOwnWeaponPart;
        private UserWeaponData _userWeaponData  = new UserWeaponData();
        private UserWeaponPartData _userWeaponPartData = new UserWeaponPartData() ;
        private Dictionary<long, int> _userWeaponPartDataDic = new Dictionary<long, int>();
        private Dictionary<long, Weapon> _userWeaponDataDic = new Dictionary<long, Weapon>();
        #endregion
        #region Properties
        #endregion

        #region Methods
        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            RefershWeaponShow();
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
            SetUserWeaponData();
            SetUserWeaponPartData();
            InitData();
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
            if (index > 0)
            {
                _weaponID = _idColltions[--index];
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
                        if (_needPart > _userOwnWeaponPart + _universalCount)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGetCoin>();
                return;
            }
            if (_needCoin > _userGoldCoin)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGetCoin>();
                return;
            }
            if (_needPart <= _userOwnWeaponPart + _universalCount && _needCoin < _userGoldCoin)
            {
                int[] _weaponIDlv = new int[] { _weaponID, _weaponLv, _weaponlevelID , _isCompoudAddNum ,_needCoin ,
                    Math.Min(_userOwnWeaponPart,_needPart) ,Math.Max(0, _needPart - _userOwnWeaponPart) };
                SocialGUIManager.Instance.OpenUI<UICtrlWeaponUpgrade>(_weaponIDlv);

                SocialGUIManager.Instance.OpenUI<UICtrlWeaponUpgrade>(_weaponIDlv);
            }
        }
        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlWeapon>();
        }
        private void RefershWeaponShow()
        {
            //等级的判断合成或者升级
            if (_userWeaponDataDic.ContainsKey(_weaponID))
            {
                _weaponLv = _userWeaponDataDic[_weaponID].Level;
                _cachedView.UpGradeImage.gameObject.SetActive(true);
                _cachedView.Compound.gameObject.SetActive(false);
                _isCompoudAddNum = 1;
            }
            else
            {
                _weaponLv = 1;
                _cachedView.UpGradeImage.gameObject.SetActive(false);
                _cachedView.Compound.gameObject.SetActive(true);
                _isCompoudAddNum = 0;
            }
            //图片显示
            _weaponEffectSpriteName = TableManager.Instance.GetEquipment(_weaponID).Icon;  //加载图片
            ResourcesManager.Instance.TryGetSprite(_weaponEffectSpriteName, out _weaponEffect);
            _cachedView.EffectShow.sprite = _weaponEffect;
            //技能显示
            _skillID = TableManager.Instance.GetEquipment(_weaponID).SkillId;
            _weaponlevelID = _skillID * _multiple + _weaponLv;
            _cachedView.SkillDescription.text = TableManager.Instance.GetSkill(_skillID).Summary;//技能描述
            _cachedView.HpAddNum.text = TableManager.Instance.GetEquipmentLevel(_weaponlevelID).HpAdd.ToString();//血量增加
            _cachedView.AttackAddNum.text = TableManager.Instance.GetEquipmentLevel(_weaponlevelID).AttackAdd.ToString();//攻击力增加
            _cachedView.WeaponName.text = TableManager.Instance.GetEquipment(_weaponID).Name;//武器的名字

            //_colorName = TableManager.Instance.GetRarity(TableManager.Instance.GetEquipment(_weaponID).Rarity).Color;
            //ColorUtility.TryParseHtmlString(_colorName, out _weaponColor);
            _cachedView.WeaponName.color = GetRarityColor(TableManager.Instance.GetEquipment(_weaponID).Rarity);//颜色
            _cachedView.WeaponLv.text = _weaponLv.ToString();//武器的等级
            _cachedView.UnlockedWeaponNum.text = _userWeaponData.ItemDataList.Count.ToString();//解锁的武器数目
            _cachedView.OwnedUniversalFragmentsNum.text = _universalCount.ToString();//拥有的万能碎片的数目
            _weaponEffectSpriteName = TableManager.Instance.GetEquipment(_weaponID).Icon;  //加载图片
            //花费的金币数目
            _needCoin =  TableManager.Instance.GetEquipmentLevel(_weaponlevelID + _isCompoudAddNum).GoldCoinNum;
             _cachedView.CostGolCoinNum.text = _needCoin.ToString();
            _needPart = TableManager.Instance.GetEquipmentLevel(_weaponlevelID + _isCompoudAddNum).WeaponFragment;
            _cachedView.CostWeaponFragmentsNum.text  = _needPart.ToString();

            //武器碎片图片
            _weaponPartSpriteName = TableManager.Instance.GetEquipment(_weaponID).WeaponPartIcon;
            ResourcesManager.Instance.TryGetSprite(_weaponPartSpriteName, out _weaponPartSprite);
            _cachedView.OwnedWeaponFragmentsIcon.sprite = _weaponPartSprite;

            //万能碎片的图片
            ResourcesManager.Instance.TryGetSprite(_universalSpriteName, out _universalSprie);
            _cachedView.OwnedUniversalFragmentsIcon.sprite = _universalSprie;

            //武器碎片的数目
            if (_userWeaponPartDataDic.ContainsKey(_weaponID))
            {
                _userOwnWeaponPart = _userWeaponPartDataDic[_weaponID];
                _cachedView.OwnedWeaponFragmentsNum.text = _userOwnWeaponPart.ToString();
            }
            else
            {
                _userOwnWeaponPart = 0;
                _cachedView.OwnedWeaponFragmentsNum.text = _userOwnWeaponPart.ToString();
            }

        }
        private void LoadUserData()
        {
            Debug.Log("用户id" + LocalUser.Instance.UserGuid);
            _userWeaponData.Request(LocalUser.Instance.UserGuid, OnSucess, code => { });
        }
        private void OnSucess()
        {
            Debug.Log(_userWeaponData.ItemDataList.Count);
        }
        private void OnFaild()
        { }
        private void InitData()
        {
            //网络请求数据
            //LocalUser.Instance.UserWeaponData.Request(LocalUser.Instance.UserGuid, null,
            //    code => { LogHelper.Error("Network error when get UserWeaponData, {0}", code); });
            //LocalUser.Instance.UserWeaponPartData.Request(LocalUser.Instance.UserGuid,null,
            //    code => { LogHelper.Error("Network error when get UserWeaponPartData, {0}" ,code); });
            //_userWeaponData = LocalUser.Instance.UserWeaponData;
            //_userWeaponPartData = LocalUser.Instance.UserWeaponPartData;
            foreach (var item in _userWeaponData.ItemDataList)
            {
                _userWeaponDataDic.Add(item.Id, item);
            }
            foreach (var item in _userWeaponPartData.ItemDataList)
            {
                _userWeaponPartDataDic.Add(item.Id, item.TotalCount);
            }
            _idColltions = new int[TableManager.Instance.Table_EquipmentDic.Keys.Count];
            TableManager.Instance.Table_EquipmentDic.Keys.CopyTo(_idColltions, 0);
            _universalCount = _userWeaponPartDataDic[0];
        }
        private void SetUserWeaponData()
        {
            Weapon weapon1 = new Weapon();
            weapon1.Id = 101;
            weapon1.Level = 5;
            Weapon weapon2 = new Weapon();
            weapon2.Id = 102;
            weapon2.Level = 3;
            Weapon weapon3 = new Weapon();
            weapon3.Id = 103;
            weapon3.Level = 5;
            _userWeaponData.ItemDataList.Add(weapon1);
            _userWeaponData.ItemDataList.Add(weapon2);
            _userWeaponData.ItemDataList.Add(weapon3);
           
        }
        private void SetUserWeaponPartData()
        {
            WeaponPart weaponpart0 = new WeaponPart();
            weaponpart0.Id = 0;
            weaponpart0.TotalCount = 5;
            WeaponPart weaponpart1 = new WeaponPart();
            weaponpart1.Id = 201;
            weaponpart1.TotalCount = 10;
            WeaponPart weaponpart2 = new WeaponPart();
            weaponpart2.Id = 202;
            weaponpart2.TotalCount = 5;
            WeaponPart weaponpart3 = new WeaponPart();
            weaponpart3.Id = 203;
            weaponpart3.TotalCount = 0;
            WeaponPart weaponpart4 = new WeaponPart();
            weaponpart4.Id = 101;
            weaponpart4.TotalCount = 10;
            _userWeaponPartData.ItemDataList.Add(weaponpart0);
            _userWeaponPartData.ItemDataList.Add(weaponpart1);
            _userWeaponPartData.ItemDataList.Add(weaponpart2);
            _userWeaponPartData.ItemDataList.Add(weaponpart3);
            _userWeaponPartData.ItemDataList.Add(weaponpart4);
         
        }
        public Color GetRarityColor(int Rarity)
        {
            Color _color = new Color();
            switch ( Rarity)
            {
                case 1:
                    _color = Color.red;
                    break;
                case 2:
                    _color = Color.yellow;
                    break;
                case 3:
                    _color = Color.green;
                    break;
                case 4:
                    _color = Color.blue;
                    break;
            }
            return _color; 
        }
        #endregion
    }
}
