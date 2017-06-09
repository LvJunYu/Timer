
/********************************************************************
** Filename : UICtrlFashionShop
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GameA.Game;
using Spine;
using Spine.Unity;


namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlLottery : UISocialCtrlBase<UIViewLottery>
    {
        #region 常量与字段
        private int _selectedTicketNum = 0;
        //初始旋转速度
        private float _initSpeed = 0;
        private float _initialSpeed = 2000;
        //速度变化值
        private float _delta = 0f;
        //转盘是否暂停
        private bool _isPause = true;
        private int _TurnsNumber =3;
        private float _stopTime = 5f;
        private float _stopRotation = 0f;
        private float _currentEulerAngles = 0f;
        private bool _bright=false;
        private float RotationEulerAngles = 0;
        private int BrightNum = 0;
        private string GlodImageName= "icon_coins_2";
        private string DiamondImageName= "icon_diam";
        private string ExpImageName= "icon_gift_1";
        private string CreatorExpImageName = "icon_gift_3";
        private string RaffleTicket = "icon_star";
        private string FashionCoupon = "icon_star";
        private string BoostItem = "icon_star";
        //private string NoneImageName = "icon_star";
        //private Reward[] _reward;


        #endregion
        #region 属性
        #endregion
        #region 方法
        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            LocalUser.Instance.RaffleTicket.Request(LocalUser.Instance.UserGuid, () => {
                RefreshRaffleCount();
            }, code => {
                LogHelper.Error("Network error when get RaffleCount, {0}", code);
            });
            
        }
        public void RefreshRaffleCount()
        {
            _cachedView.NumberOfTicketlvl1.text = MakePositiveNumber(LocalUser.Instance.RaffleTicket.GetCountInRaffleDictionary(1));//查看数量
            _cachedView.NumberOfTicketlvl2.text = MakePositiveNumber(LocalUser.Instance.RaffleTicket.GetCountInRaffleDictionary(2));
            _cachedView.NumberOfTicketlvl3.text = MakePositiveNumber(LocalUser.Instance.RaffleTicket.GetCountInRaffleDictionary(3));
            _cachedView.NumberOfTicketlvl4.text = MakePositiveNumber(LocalUser.Instance.RaffleTicket.GetCountInRaffleDictionary(4));
            _cachedView.NumberOfTicketlvl5.text = MakePositiveNumber(LocalUser.Instance.RaffleTicket.GetCountInRaffleDictionary(5));
        }

        private string MakePositiveNumber(int lotteryNum)
        {
            if (lotteryNum < 0)
            {
                return "0";
            }
            else return lotteryNum.ToString();
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
        protected override void InitEventListener()
        {
            base.InitEventListener();
            //			RegisterEvent(EMessengerType.OnChangeToAppMode, OnReturnToApp);
            //            RegisterEvent(EMessengerType.OnAccountLoginStateChanged, OnEvent);
        }
        protected override void OnViewCreated()
        {
            base.OnViewCreated();

            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtnClick);

            //_cachedView.RaffleTicketlvl1.onClick.AddListener(()=>OnSelectedRaffleBtn(1));
            _cachedView.RaffleTicketlvl1Btn.onClick.AddListener(OnSelectedRaffleBtn1); //注册每个按钮点击时会显示当前选中
            _cachedView.RaffleTicketlvl2Btn.onClick.AddListener(OnSelectedRaffleBtn2);
            _cachedView.RaffleTicketlvl3Btn.onClick.AddListener(OnSelectedRaffleBtn3);
            _cachedView.RaffleTicketlvl4Btn.onClick.AddListener(OnSelectedRaffleBtn4);
            _cachedView.RaffleTicketlvl5Btn.onClick.AddListener(OnSelectedRaffleBtn5);
            _cachedView.RaffleBtn.onClick.AddListener(() => UseRaffleTicket(this._selectedTicketNum));
            _cachedView.CatBtn.onClick.AddListener(OnCatBtnClick);

        }

        private void OnCatBtnClick()
        {
            UseRaffleTicket(this._selectedTicketNum);
        }

        private void ShowNoneTicketTips()
        {
            _cachedView.Mask.SetEnableEx(false);
            if (_selectedTicketNum==0)
            {
                SocialGUIManager.ShowPopupDialog("请选择一种抽奖券", null,
                    new KeyValuePair<string, Action>("确定", () => {

                    })
                    );
            }
            else
            {
                SocialGUIManager.ShowPopupDialog("抽奖券数量不够", null,
                    new KeyValuePair<string, Action>("确定", () => {

                    })
                    );
            }
           
        }

        private void UseRaffleTicket(int selectedTicketNum)
        {
            _cachedView.Mask.SetEnableEx(true);
            if (LocalUser.Instance.RaffleTicket.GetCountInRaffleDictionary(selectedTicketNum) > 0)
            {
                _cachedView.SpineCat.AnimationState.SetAnimation(0, "Run", false);
                _isPause = false;
                this._initSpeed = _initialSpeed;
                LocalUser.Instance.RaffleTicket.UseRaffleTicket(selectedTicketNum, DecelerateThePanel
                    , null);
                RefreshRaffleCount();
                // RotateThePanel(LocalUser.Instance.RaffleTicket.CurrentRewardId);
            }
            else
            { ShowNoneTicketTips();}
        }
        private void DecelerateThePanel(long rewardid)
        {
            //this._initSpeed = 0;
            //_cachedView.RoolPanel.rotation = Quaternion.Euler(0, 0, rewardid * 360 / 8);
            //this._isPause = true;
            _cachedView.RewardExhibition.text = rewardid.ToString();
            _currentEulerAngles = _cachedView.RoolPanel.rotation.eulerAngles.z % 360;
            //if (_currentEulerAngles - rewardid*360/8 > 180)
            //{
            //    _stopRotation = Mathf.Abs(_currentEulerAngles - rewardid * 360 / 8);
            //}
             if (0 < _currentEulerAngles - rewardid*360/8)
            {
                _stopRotation = 360 - (_currentEulerAngles - rewardid * 360 / 8);
            }
            else if (_currentEulerAngles - rewardid*360/8 < 0)
            {
                _stopRotation = Mathf.Abs( rewardid * 360 / 8- _currentEulerAngles);
            }
            //_circleNum = (int)_stopRotation / 360;
            // _delta = _initSpeed*_initSpeed / (2 * _stopRotation);
            _delta = _initSpeed * _initSpeed / (2 * (_stopRotation + 360 * 15-17.0f));
            //Debug.Log("______________速度" + _initSpeed);
            //Debug.Log("______________加速度"+ _delta);
            //Debug.Log("______________当前位置" + _currentEulerAngles);
            //Debug.Log("______________目标位置" + rewardid * 360 / 8);
            //Debug.Log("______________停止距离" + _stopRotation);
        }

        private void LightEntireBright()
        {
            _bright = !_bright;
            for (int i = 0; i < _cachedView.BrightLamp.Length; i++)
            {
                _cachedView.BrightLamp[i].SetActiveEx(_bright);
            }
        }

        private void LightRotateBright()
        {
            _cachedView.BrightLamp[BrightNum].SetActiveEx(true);
            if (BrightNum >= _cachedView.BrightLamp.Length-1)
            {
                BrightNum = 0;
                ShutDownLight();
            }
            else
            {
                ++BrightNum;
            }
        }

        private void ShutDownLight()
        {
            for (int i = 0; i < _cachedView.BrightLamp.Length; i++)
            {
                _cachedView.BrightLamp[i].SetActiveEx(false);
            }

        }

        private void WhenShutDown()
        {
            _cachedView.Mask.SetEnableEx(false);
            ShutDownLight();
            _cachedView.BrightLamp[4].SetActiveEx(true);
            SocialGUIManager.ShowReward(LocalUser.Instance.RaffleTicket.GetReward);

        }

        public override void OnUpdate()
        {
            if (!_isPause)
            {
                //转动转盘(-1为顺时针,1为逆时针)
                //_initSpeed = Mathf.MoveTowardsAngle((float)_cachedView.RoolPanel.rotation, 10, 0.1f*Time.deltaTime);
                RotationEulerAngles += _initSpeed*Time.deltaTime;
                _cachedView.RoolPanel.Rotate(new Vector3(0, 0, _initSpeed) *Time.deltaTime);
                if (RotationEulerAngles>=45)
                {
                    RotationEulerAngles = 0;
                    LightRotateBright();
                }
                //让转动的速度缓缓降低
                _initSpeed -= _delta*Time.deltaTime;
                //当转动的速度为0时转盘停止转动
                if (_initSpeed <= 0)
                {
                    //转动停止
                    _isPause = true;
                    WhenShutDown();
                }
            }
        }

        private string JudgeRewardType(int RewardType)
        {
            string Type;
            switch (RewardType)
            {
                case 1:
                    return Type = "金币";
                case 2:
                    return Type = "钻石";
                case 3:
                    return Type = "冒险经验";
                case 4:
                    return Type = "工匠经验";
                case 5:
                    return Type = "时装券";
                case 6:
                    return Type = "低级抽奖券";
                case 7:
                    return Type = "道具";
                default:
                    return Type = "None";
            }
        }

        private string JudgeRewardImageName(int RewardType)
        {
            string Type;
            switch (RewardType)
            {
                case 1:
                    return Type = GlodImageName;
                case 2:
                    return Type = DiamondImageName;
                case 3:
                    return Type = ExpImageName;
                case 4:
                    return Type = CreatorExpImageName;
                case 5:
                    return Type = FashionCoupon;
                case 6:
                    return Type = RaffleTicket;
                case 7:
                    return Type = BoostItem;
                default:
                    return Type = GlodImageName;
            }
        }

        private Sprite FindImage(string ImageName)
        {
            Sprite fashion;
            if (GameResourceManager.Instance.TryGetSpriteByName(ImageName, out fashion))
            {
                //Debug.Log("____________时装" + fashion.name);
                //_cachedView.FashionPreview.sprite = fashion;
                return fashion;
            }
            else return null;

        }

        private void RefreshReward(int rewardNUm)//1 2 3
        {
            var TurntableUnit = TableManager.Instance.Table_TurntableDic[rewardNUm];
            
            _cachedView.RewardType1.text = JudgeRewardType(TableManager.Instance.Table_RewardDic[TurntableUnit.Reward1].Type1)
            +TableManager.Instance.Table_RewardDic[TurntableUnit.Reward1].Value1;
            _cachedView.Reward1.sprite =
                FindImage(JudgeRewardImageName(TableManager.Instance.Table_RewardDic[TurntableUnit.Reward1].Type1));

            ;_cachedView.RewardType2.text = JudgeRewardType(TableManager.Instance.Table_RewardDic[TurntableUnit.Reward2].Type1)
            +TableManager.Instance.Table_RewardDic[TurntableUnit.Reward2].Value1;
            _cachedView.Reward2.sprite =
                FindImage(JudgeRewardImageName(TableManager.Instance.Table_RewardDic[TurntableUnit.Reward2].Type1));

            _cachedView.RewardType3.text = JudgeRewardType(TableManager.Instance.Table_RewardDic[TurntableUnit.Reward3].Type1)
            +TableManager.Instance.Table_RewardDic[TurntableUnit.Reward3].Value1;
            _cachedView.Reward3.sprite =
                FindImage(JudgeRewardImageName(TableManager.Instance.Table_RewardDic[TurntableUnit.Reward3].Type1));

            _cachedView.RewardType4.text = JudgeRewardType(TableManager.Instance.Table_RewardDic[TurntableUnit.Reward4].Type1)
            +TableManager.Instance.Table_RewardDic[TurntableUnit.Reward4].Value1;
            _cachedView.Reward4.sprite =
                FindImage(JudgeRewardImageName(TableManager.Instance.Table_RewardDic[TurntableUnit.Reward4].Type1));

            _cachedView.RewardType5.text = JudgeRewardType(TableManager.Instance.Table_RewardDic[TurntableUnit.Reward5].Type1)
            +TableManager.Instance.Table_RewardDic[TurntableUnit.Reward5].Value1;
            _cachedView.Reward5.sprite =
                FindImage(JudgeRewardImageName(TableManager.Instance.Table_RewardDic[TurntableUnit.Reward5].Type1));

            _cachedView.RewardType6.text = JudgeRewardType(TableManager.Instance.Table_RewardDic[TurntableUnit.Reward6].Type1)
            +TableManager.Instance.Table_RewardDic[TurntableUnit.Reward6].Value1;
            _cachedView.Reward6.sprite =
                FindImage(JudgeRewardImageName(TableManager.Instance.Table_RewardDic[TurntableUnit.Reward6].Type1));

            _cachedView.RewardType7.text = JudgeRewardType(TableManager.Instance.Table_RewardDic[TurntableUnit.Reward7].Type1)
            +TableManager.Instance.Table_RewardDic[TurntableUnit.Reward7].Value1;
            _cachedView.Reward7.sprite =
                FindImage(JudgeRewardImageName(TableManager.Instance.Table_RewardDic[TurntableUnit.Reward7].Type1));

            _cachedView.RewardType8.text = JudgeRewardType(TableManager.Instance.Table_RewardDic[TurntableUnit.Reward8].Type1)
            +TableManager.Instance.Table_RewardDic[TurntableUnit.Reward8].Value1;
            _cachedView.Reward8.sprite =
                FindImage(JudgeRewardImageName(TableManager.Instance.Table_RewardDic[TurntableUnit.Reward8].Type1));



    //[ProtoContract(Name = "ERewardType")]
    //    public enum ERewardType
    //    {
    //        RT_None = 0,
    //        RT_Gold = 1,
    //        RT_Diamond = 2,
    //        RT_PlayerExp = 3,
    //        RT_CreatorExp = 4,
    //        RT_FashionCoupon = 5,
    //        RT_RaffleTicket = 6,
    //        RT_BoostItem = 7
    //    }
    }


        #region 接口
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }
        private void OnCloseBtnClick()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlLottery>();
        }

        private void OnSelectedRaffleBtn1()
        {
            this._selectedTicketNum = 1;
            _cachedView.SelectedTicketType.text = "<color=#ffb400>壹</color>";
            RefreshReward(1);
        }

        private void OnSelectedRaffleBtn2()
        {
            this._selectedTicketNum = 2;
            _cachedView.SelectedTicketType.text = "<color=#ededeb>贰</color>"; 
            RefreshReward(2);
        }
        private void OnSelectedRaffleBtn3()
        {
            this._selectedTicketNum = 3;
            _cachedView.SelectedTicketType.text = "<color=#fbf11a>叁</color>";
            RefreshReward(3);
        }
        private void OnSelectedRaffleBtn4()
        {
            this._selectedTicketNum = 4;
            _cachedView.SelectedTicketType.text = "<color=#00ffff>匠</color>";
            RefreshReward(4);

        }
        private void OnSelectedRaffleBtn5()
        {
            this._selectedTicketNum = 5;
            _cachedView.SelectedTicketType.text = "<color=#f65656>亲</color>";
            RefreshReward(5);
        }
        #endregion 接口
        #endregion
    }
}

