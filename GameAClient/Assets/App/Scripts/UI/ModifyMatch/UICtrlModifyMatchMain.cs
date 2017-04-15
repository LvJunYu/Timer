/********************************************************************
** Filename : UICtrlModifyMatchMain
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using SoyEngine;
using GameA.Game;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
	public class UICtrlModifyMatchMain : UISocialCtrlBase<UIViewModifyMatchMain>
    {
        #region 常量与字段


        #endregion

        #region 属性


        #endregion

        #region 方法
        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);

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

			_cachedView.CloseBtn.onClick.AddListener (OnCloseBtn);
			_cachedView.ModifyBtn.onClick.AddListener (OnModifyBtn);
        }
			

//		private void RefreshEnergyInfo () {
//			AppData.Instance.AdventureData.UserData.UserEnergyData.LocalRefresh ();
//			int currentEnergy = AppData.Instance.AdventureData.UserData.UserEnergyData.Energy;
//			int energyCapacity = AppData.Instance.AdventureData.UserData.UserEnergyData.EnergyCapacity;
//			_cachedView.EnergyNumber.text = string.Format("{0} / {1}",
//				currentEnergy,
//				energyCapacity
//			);
//			_cachedView.EnergyBar.fillAmount = (float)currentEnergy / energyCapacity;
//			_nextEnergyGenerateTime = AppData.Instance.AdventureData.UserData.UserEnergyData.NextGenerateTime;
//		}


        #region 接口
        protected override void InitGroupId()
        {
			_groupId = (int)EUIGroupType.PopUpUI;
        }
			

		private void OnCloseBtn () {
			SocialGUIManager.Instance.CloseUI<UICtrlModifyMatchMain>();
		}

		private void OnModifyBtn () {
			List<Project> projectList =	AppData.Instance.AdventureData.ProjectList.SectionList [0].NormalProjectList;
			projectList[0].PrepareRes(
				() => {
					GameManager.Instance.RequestModify(projectList[0]);
					SocialGUIManager.Instance.ChangeToGameMode();
				}
			);
		}

        #endregion 接口
        #endregion

    }
}
