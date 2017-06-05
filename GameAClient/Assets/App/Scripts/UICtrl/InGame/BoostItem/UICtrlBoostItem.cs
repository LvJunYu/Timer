﻿using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using SoyEngine.Proto;
using GameA.Game;

namespace GameA
{
    [UIAutoSetup (EUIAutoSetupType.Add)]
    public class UICtrlBoostItem : UICtrlInGameBase<UIViewBoostItem>
    {
        private USCtrlBoostItem[] _boostItems;


        protected override void InitGroupId ()
        {
            _groupId = (int)EUIGroupType.InGamePopup;
        }

        protected override void InitEventListener ()
        {
            base.InitEventListener ();
            Messenger.AddListener (EMessengerType.OnReady2Play, OnReady2Play);
        }

        protected override void OnViewCreated ()
        {
            base.OnViewCreated ();
            _boostItems = new USCtrlBoostItem [_cachedView.BoostItems.Length];
            for (int i = 0; i < _boostItems.Length; i++)
            {
                _boostItems [i] = new USCtrlBoostItem ();
                _boostItems [i].Init (_cachedView.BoostItems [i]);
            }

            _cachedView.OKBtn.onClick.AddListener (OnOKBtn);
            _cachedView.CloseBtn.onClick.AddListener (OnCloseBtn);
        }

        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);

            if (!LocalUser.Instance.UserProp.IsInited) {
                SocialGUIManager.Instance.CloseUI<UICtrlBoostItem> ();
            }
            else
            {
                for (int j = 0; j < _boostItems.Length; j++) 
                {
                    _boostItems [j].SetEmpty ();
                }
                for (int i = 0; i < LocalUser.Instance.UserProp.ItemDataList.Count; i++)
                {
                    for (int j = 0; j < _boostItems.Length; j++)
                    {
                        if (LocalUser.Instance.UserProp.ItemDataList[i].Type == _boostItems[j].BoostItemType)
                        {
                            _boostItems [j].SetItem (LocalUser.Instance.UserProp.ItemDataList [i]);
                            break;
                        }
                    }
                }
            }
        }
        private void OnOKBtn ()
        {
            List<int> selectedItems = new List<int> ();
            int totalPrice = 0;
            for (int i = 0; i < _boostItems.Length; i++)
            {
                if (_boostItems[i].Checked)
                {
                    selectedItems.Add (_boostItems[i].BoostItemType);
                    totalPrice += _boostItems [i].Price;
                }
            }
            if (totalPrice > 0)
            {
                SocialGUIManager.ShowPopupDialog (
                    string.Format ("确定花费 {0} 钻石购买未拥有的道具吗？", totalPrice),
                    "道具购买",
                    new KeyValuePair<string, Action> ("确认", ()=>{
                        if (GameATools.CheckDiamond (totalPrice))
                        {
                            GameModePlay gameModePlay = GM2DGame.Instance.GameMode as GameModePlay;
                            if (null != gameModePlay)
                            {
                                gameModePlay.UseBoostItem(selectedItems);
                            }
                            SocialGUIManager.Instance.CloseUI<UICtrlBoostItem> ();
                            Messenger<List<int>>.Broadcast (EMessengerType.OnBoostItemSelectFinish, selectedItems);
                        }
                    }),
                    new KeyValuePair<string, Action> ("取消", null)
                );
            } else
			{
				GameModePlay gameModePlay = GM2DGame.Instance.GameMode as GameModePlay;
				if (null != gameModePlay)
				{
					gameModePlay.UseBoostItem(selectedItems);
				}
                SocialGUIManager.Instance.CloseUI<UICtrlBoostItem> ();
                Messenger<List<int>>.Broadcast (EMessengerType.OnBoostItemSelectFinish, selectedItems);
            }
        }

        private void OnCloseBtn ()
        {
            SocialGUIManager.Instance.CloseUI <UICtrlBoostItem>();
        }

        private void OnReady2Play ()
        {
            Debug.Log ("project type: " + Game.GM2DGame.Instance.Project.ProjectStatus);
            if (EProjectStatus.PS_AdvNormal == Game.GM2DGame.Instance.Project.ProjectStatus ||
                EProjectStatus.PS_Challenge == Game.GM2DGame.Instance.Project.ProjectStatus) {
                SocialGUIManager.Instance.OpenUI<UICtrlBoostItem> ();
            } else {
                Messenger<List<int>>.Broadcast (EMessengerType.OnBoostItemSelectFinish, null);
            }                
        }
    }
}