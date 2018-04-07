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
    public class UICtrlWeaponUpgrade : UICtrlAnimationBase<UIViewWeaponUpgrade>
    {

        #region Fields
        private int _weaponID;
        private int _weaponLv;
        private int _weaponlevelID;
        private int _isCompoudAddNum;
        private int _needGoldCoinNum;
        private int _needWeaponPartNum;
        private int _needUniversalNum;
        private Sprite _universalSprie;
        private string _weaponPartSpriteName;
        private Sprite _weaponPartSprite;
        private string _weaponName;
        private string _upStr = "升级";
        private string _comStr = "合成";
        private string _tipStr = "确定使用以下资源{0}{1}吗？";
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
            _cachedView.Cancel.onClick.AddListener(OnCancelBtn);
            _cachedView.Confirm.onClick.AddListener(OnConfirmBtn);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

        }

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainPopUpUI;
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlWeaponUpgrade>();
        }
        private void InitData()
        {
            _weaponName = TableManager.Instance.GetEquipment(_weaponID).Name;
            if (_isCompoudAddNum == 0)
            {   
                _cachedView.TileText.text = _comStr;  
                _cachedView.TipText.text = string.Format(_tipStr,_comStr,_weaponName);      
                _cachedView.HpOld.text =  TableManager.Instance.GetEquipmentLevel(_weaponlevelID + _isCompoudAddNum).HpAdd.ToString();
                _cachedView.HpOld.transform.GetChild(0).gameObject.SetActive(false);
                _cachedView.AttactOld.text = TableManager.Instance.GetEquipmentLevel(_weaponlevelID + _isCompoudAddNum).AttackAdd.ToString();
                _cachedView.AttactOld.transform.GetChild(0).gameObject.SetActive(false);
                _cachedView.SkillOld.text =  TableManager.Instance.GetEquipmentLevel(_weaponlevelID + _isCompoudAddNum).SkillEffect.ToString();
                _cachedView.SkillOld.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                _cachedView.TipText.text = _upStr;
                _cachedView.TipText.text = string.Format(_tipStr,_upStr,_weaponName);
                _cachedView.HpOld.text = "+"+TableManager.Instance.GetEquipmentLevel(_weaponlevelID).HpAdd.ToString();
                _cachedView.HpAdd.text = "+"+ TableManager.Instance.GetEquipmentLevel(_weaponlevelID + _isCompoudAddNum).HpAdd.ToString();
                _cachedView.AttactOld.text = "+" + TableManager.Instance.GetEquipmentLevel(_weaponlevelID).AttackAdd.ToString();
                _cachedView.AttackAdd.text = "+" + TableManager.Instance.GetEquipmentLevel(_weaponlevelID + _isCompoudAddNum).AttackAdd.ToString();
                _cachedView.SkillOld.text = TableManager.Instance.GetEquipmentLevel(_weaponlevelID).SkillEffect.ToString();
                _cachedView.SkillEffect.text = TableManager.Instance.GetEquipmentLevel(_weaponlevelID + _isCompoudAddNum).SkillEffect.ToString();
            }

            //武器碎片的图标
            _weaponPartSpriteName = TableManager.Instance.GetSkill(TableManager.Instance.GetEquipment(_weaponID).SkillId).Icon ;
            JoyResManager.Instance.TryGetSprite(_weaponPartSpriteName, out _weaponPartSprite);
            _cachedView.WeaponFragmentIcon.sprite = _weaponPartSprite;

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

        private void OnConfirmBtn()
        {
            //if (_isCompoudAddNum == 0)
            //{
            //    CompoundWeapon();
            //}
            //else
            //{
            //    UpgradeWeapon();
            //}
            //SocialGUIManager.Instance.CloseUI<UICtrlWeaponUpgrade>();
            //SocialGUIManager.Instance.OpenUI<UICtrlGetCoin>();
            //测试
            Messenger.Broadcast(EMessengerType.OnWeaponDataChange);
        }
        private void OnCancelBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlWeaponUpgrade>();
        }
        private void CompoundWeapon()
        {
            RemoteCommands.CompoundWeapon(_weaponID, _needUniversalNum, null, 
                code => { LogHelper.Error("Network error when CompoundWeapon , {0}", code); });
            LocalUser.Instance.UserWeaponData.Request(LocalUser.Instance.UserGuid, null,
                code => { LogHelper.Error("Network error when get UserWeaponData, {0}", code); });
            LocalUser.Instance.UserWeaponPartData.Request(LocalUser.Instance.UserGuid, null,
                code => { LogHelper.Error("Network error when get UserWeaponPartData, {0}", code); });
        }
        private void UpgradeWeapon()
        {
            RemoteCommands.UpgradeWeapon(_weaponID, _needUniversalNum, _weaponLv + 1, null,
                code => { LogHelper.Error("Network error when UpgradeWeapon , {0}", code); });
            LocalUser.Instance.UserWeaponData.Request(LocalUser.Instance.UserGuid, null,
                  code => { LogHelper.Error("Network error when get UserWeaponData, {0}", code); });
            LocalUser.Instance.UserWeaponPartData.Request(LocalUser.Instance.UserGuid, null,
                code => { LogHelper.Error("Network error when get UserWeaponPartData, {0}", code); });
        }
        //public void UseRaffleTicket(long selectedTicketNum, Action<long> successCallback, Action<ENetResultCode> failedCallback)
        //{
        //    this._RaffleTicketDict[selectedTicketNum]--;
        //    RemoteCommands.Raffle(
        //        selectedTicketNum,
        //        (re) =>
        //        {
        //            ERaffleCode resultCode = (ERaffleCode)re.ResultCode;
        //            List<Msg_RewardItem> RewardList = re.Reward.ItemList;

        //            if (resultCode == ERaffleCode.RC_Success)
        //            {
        //                //SuccessfullyUseRaffleTicket(re.RewardId, (int)selectedTicketNum);
        //                for (int i = 0; i < RewardList.Count; i++)
        //                {
        //                    RewardItem item = new RewardItem(RewardList[i]);
        //                    item.AddToLocal();
        //                }
        //                successCallback(ReturnRewardOnPanel(re.RewardId, (int)selectedTicketNum));
        //                _reward.OnSync(re.Reward);
        //            }
        //            else
        //            {
        //                UnsuccessfullyUseRaffleTicket();
        //            }
        //        },
        //        code => {
        //            LogHelper.Error("Network error when use Raffle, {0}", code);
        //            UnsuccessfullyUseRaffleTicket();
        //            this._RaffleTicketDict[selectedTicketNum]++;
        //        });
        //}
        #endregion
    }
}
