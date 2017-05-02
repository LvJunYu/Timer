
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

        private int SelectedTicketNum = 0;

        //初始旋转速度
        private float mInitSpeed = 0;

        //速度变化值
        private float mDelta = 0.5f;

        //转盘是否暂停
        private bool isPause = true;




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
            _cachedView.Ticketlvl1.text = LocalUser.Instance.RaffleTicket.RaffleDictionary(1).ToString();//查看数量
            _cachedView.Ticketlvl2.text = LocalUser.Instance.RaffleTicket.RaffleDictionary(2).ToString();
            _cachedView.Ticketlvl3.text = LocalUser.Instance.RaffleTicket.RaffleDictionary(3).ToString();
            _cachedView.Ticketlvl4.text = LocalUser.Instance.RaffleTicket.RaffleDictionary(4).ToString();
            _cachedView.Ticketlvl5.text = LocalUser.Instance.RaffleTicket.RaffleDictionary(5).ToString();
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
            _cachedView.RaffleTicketlvl1.onClick.AddListener(OnSelectedRaffleBtn1); //注册每个按钮点击时会显示当前选中
            _cachedView.RaffleTicketlvl2.onClick.AddListener(OnSelectedRaffleBtn2);
            _cachedView.RaffleTicketlvl3.onClick.AddListener(OnSelectedRaffleBtn3);
            _cachedView.RaffleTicketlvl4.onClick.AddListener(OnSelectedRaffleBtn4);
            _cachedView.RaffleTicketlvl5.onClick.AddListener(OnSelectedRaffleBtn5);
            _cachedView.RaffleButton.onClick.AddListener(() => UseRaffleTicket(this.SelectedTicketNum));

        }

        private void UseRaffleTicket(int selectedTicketNum)
        {
            if (LocalUser.Instance.RaffleTicket.RaffleDictionary(selectedTicketNum) > 0)
            {

                this.mInitSpeed = 100;
                LocalUser.Instance.RaffleTicket.UseRaffleTicket(selectedTicketNum, this.DecelerateThePanel, null);

                RefreshRaffleCount();
                // RotateThePanel(LocalUser.Instance.RaffleTicket.CurrentRewardId);

            }
            else _cachedView.ShowSelectedTicket.text = "抽奖券数量不够";
        }



        private void DecelerateThePanel(long rewardid)
        {
            this.mInitSpeed = 0;
            _cachedView.mRoolPanel.rotation = Quaternion.Euler(0, 0, rewardid * 360 / 8);
            this.isPause = true;


        }

        public override void OnUpdate()
        {
            if (!isPause)
            {

                //转动转盘(-1为顺时针,1为逆时针)
                _cachedView.mRoolPanel.Rotate(new Vector3(0, 0, mInitSpeed) * mInitSpeed * Time.deltaTime);
                //让转动的速度缓缓降低
                mInitSpeed -= mDelta;
                //当转动的速度为0时转盘停止转动
                if (mInitSpeed <= 0)
                {
                    //转动停止
                    isPause = true;
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
            this.SelectedTicketNum = 1;
            _cachedView.ShowSelectedTicket.text = "新手抽奖券";
        }

        private void OnSelectedRaffleBtn2()
        {
            this.SelectedTicketNum = 2;
            _cachedView.ShowSelectedTicket.text = "初级抽奖券";
        }
        private void OnSelectedRaffleBtn3()
        {
            this.SelectedTicketNum = 3;
            _cachedView.ShowSelectedTicket.text = "中级抽奖券";
        }
        private void OnSelectedRaffleBtn4()
        {
            this.SelectedTicketNum = 4;
            _cachedView.ShowSelectedTicket.text = "高级抽奖券";
        }
        private void OnSelectedRaffleBtn5()
        {
            this.SelectedTicketNum = 5;
            _cachedView.ShowSelectedTicket.text = "大师抽奖券";
        }




        #endregion 接口
        #endregion

    }
}

