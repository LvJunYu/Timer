
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



namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Create)]
    public class UICtrlLottery : UISocialCtrlBase<UIViewLottery>
    {
        #region 常量与字段
        private int _selectedTicketNum = 0;
        //初始旋转速度
        private float _initSpeed = 0;
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
        private void RefreshRaffleCount()
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
       }
        private void UseRaffleTicket(int selectedTicketNum)
        {
       

            if (LocalUser.Instance.RaffleTicket.GetCountInRaffleDictionary(selectedTicketNum) > 0)
            {
                _isPause = false;
                this._initSpeed = 2000;
                LocalUser.Instance.RaffleTicket.UseRaffleTicket(selectedTicketNum, DecelerateThePanel
                    , null);
                RefreshRaffleCount();
                // RotateThePanel(LocalUser.Instance.RaffleTicket.CurrentRewardId);
            }
            else _cachedView.SelectedTicketType.text = "抽奖券数量不够";
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
            _delta = _initSpeed * _initSpeed / (2 * (_stopRotation + 360 * 12-20.5f));
            Debug.Log("______________速度" + _initSpeed);
            Debug.Log("______________加速度"+ _delta);
            Debug.Log("______________当前位置" + _currentEulerAngles);
            Debug.Log("______________目标位置" + rewardid * 360 / 8);
            Debug.Log("______________停止距离" + _stopRotation);
        }

        public override void OnUpdate()
        {
            if (!_isPause)
            {
                //转动转盘(-1为顺时针,1为逆时针)
                //_initSpeed = Mathf.MoveTowardsAngle((float)_cachedView.RoolPanel.rotation, 10, 0.1f*Time.deltaTime);
                RotationEulerAngles += _initSpeed*Time.deltaTime;
                _cachedView.RoolPanel.Rotate(new Vector3(0, 0, _initSpeed) *Time.deltaTime);
                if (RotationEulerAngles>=270)
                {
                    RotationEulerAngles = 0;

                 
                        _bright = !_bright;
                        for (int i = 0; i < _cachedView.BrightLamp.Length; i++)
                        {
                            _cachedView.BrightLamp[i].SetActiveEx(_bright);
                        }

                    

                }
                //让转动的速度缓缓降低
                _initSpeed -= _delta*Time.deltaTime;
                //当转动的速度为0时转盘停止转动
                if (_initSpeed <= 0)
                {
                    //转动停止
                    _isPause = true;
                }
            }
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
            _cachedView.SelectedTicketType.text = "新手抽奖券";
        }

        private void OnSelectedRaffleBtn2()
        {
            this._selectedTicketNum = 2;
            _cachedView.SelectedTicketType.text = "初级抽奖券";
        }
        private void OnSelectedRaffleBtn3()
        {
            this._selectedTicketNum = 3;
            _cachedView.SelectedTicketType.text = "中级抽奖券";
        }
        private void OnSelectedRaffleBtn4()
        {
            this._selectedTicketNum = 4;
            _cachedView.SelectedTicketType.text = "高级抽奖券";
        }
        private void OnSelectedRaffleBtn5()
        {
            this._selectedTicketNum = 5;
            _cachedView.SelectedTicketType.text = "大师抽奖券";
        }
        #endregion 接口
        #endregion
    }
}

