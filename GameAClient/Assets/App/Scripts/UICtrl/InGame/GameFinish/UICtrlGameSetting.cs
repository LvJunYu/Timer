/********************************************************************
** Filename : UICtrlGameSetting  
** Author : ake
** Date : 6/8/2016 8:15:38 PM
** Summary : UICtrlGameSetting  
***********************************************************************/


using System.Collections;
using SoyEngine;
using UnityEngine;
using System;
using SoyEngine.Proto;
using SoyEngine;
using GameA.Game;

namespace GameA
{
	[UIAutoSetup(EUIAutoSetupType.Add)]
	public class UICtrlGameSetting:UICtrlInGameBase<UIViewGameSetting>
	{

		private GameSettingData _setting;
		protected override void InitGroupId()
		{
			_groupId = (int)EUIGroupType.AppGameUI;
		}

		protected override void InitEventListener()
		{
			base.InitEventListener();
			RegisterEvent(EMessengerType.OpenGameSetting, ReceiveOpenSettingCommand);
		}

		protected override void OnViewCreated()
		{
			base.OnViewCreated();
			_setting = GM2DGame.Instance.Settings;

			_cachedView.MusicItem.InitItem(OnClickMusicButton);
			_cachedView.SoundsEffects.InitItem(OnClickSoundsEffectsButton);
			_cachedView.ShowRunTimeShadowItem.InitItem(OnClickShowRuntimeShadow);
			_cachedView.ShowEditShadowItem.InitItem(OnClickShowEditShadow);

			_cachedView.ButtonExit.onClick.AddListener(OnClickExitButton);
			_cachedView.ButtonReturnToGame.onClick.AddListener(OnClickReturnToGameButton);
			_cachedView.ButtonRestart.onClick.AddListener(OnClickRestartButton);
			_cachedView.ButtonClose.onClick.AddListener(OnClickReturnToGameButton);

			InitLocale();
		}

		protected override void OnOpen(object parameter)
		{
			base.OnOpen(parameter);
			UpdateSettingItem();
			UpdateShowState();

            Game.PlayMode.Instance.Pause ();
		}

		protected override void OnClose()
		{
			if (_setting != null)
			{
				_setting.Save();
			}
			if (Game.PlayMode.Instance == null)
			{
				return;
			}
            Game.PlayMode.Instance.Continue ();
			Messenger.Broadcast(EMessengerType.OnCloseGameSetting);
			base.OnClose();
		}

		#region private 

		private void InitLocale()
		{
			_cachedView.Title.text = LocaleManager.GameLocale("ui_setting_title");
			_cachedView.ButtonReturnToGameText.text = LocaleManager.GameLocale("ui_setting_btn_return_game");
			_cachedView.ButtonExitText.text = LocaleManager.GameLocale("ui_setting_btn_exit_game");
			_cachedView.ButtonRestartText.text = LocaleManager.GameLocale("ui_setting_btn_restart_game");
			_cachedView.MusicItem.Title.text = LocaleManager.GameLocale("ui_setting_item_title_muisc");
			_cachedView.SoundsEffects.Title.text = LocaleManager.GameLocale("ui_setting_item_title_sounds");
			_cachedView.ShowRunTimeShadowItem.Title.text = LocaleManager.GameLocale("ui_setting_item_show_runtime_shadow");
			_cachedView.ShowEditShadowItem.Title.text = LocaleManager.GameLocale("ui_setting_item_show_edit_shadow");
		}

		private void UpdateShowState()
		{
			if (GM2DGame.Instance.CurrentMode == EMode.Edit)
			{
				UIRectTransformStatus.SetStatus(_cachedView.UIStateController.gameObject,"1");
			}
			else
			{
				UIRectTransformStatus.SetStatus(_cachedView.UIStateController.gameObject,"2");
			}
		}

		private void UpdateSettingItem()
		{
			if (_setting != null)
			{
				_cachedView.MusicItem.UpdateShow(_setting.PlayMusic);
				_cachedView.SoundsEffects.UpdateShow(_setting.PlaySoundsEffects);
				_cachedView.ShowRunTimeShadowItem.UpdateShow(_setting.ShowPlayModeShadow);
				_cachedView.ShowEditShadowItem.UpdateShow(_setting.ShowEidtModeShadow);
			}
		}

		private void OnClickMusicButton()
		{
			_setting.PlayMusic = !_setting.PlayMusic;
			GameAudioManager.Instance.OnSettingChanged();
			UpdateSettingItem();
		}


		private void OnClickSoundsEffectsButton()
		{
			_setting.PlaySoundsEffects = !_setting.PlaySoundsEffects;
			GameAudioManager.Instance.OnSettingChanged();
			UpdateSettingItem();
		}


		private void OnClickShowRuntimeShadow()
		{
			_setting.ShowPlayModeShadow = !_setting.ShowPlayModeShadow;
			UpdateSettingItem();
		}

		private void OnClickShowEditShadow()
		{
			_setting.ShowEidtModeShadow = !_setting.ShowEidtModeShadow;
			UpdateSettingItem();
		}

		private void OnClickExitButton()
        {
            //if (GM2DGame.Instance.GameInitType == GameManager.EStartType.Create
            //|| GM2DGame.Instance.GameInitType == GameManager.EStartType.Edit) {
            //    if (GM2DGame.Instance.NeedSave) {
            //        CommonTools.ShowPopupDialog ("关卡做出的修改还未保存，是否退出", null,
            //            new System.Collections.Generic.KeyValuePair<string, Action> ("取消", () => {
            //            }),
            //            new System.Collections.Generic.KeyValuePair<string, Action> ("保存", () => {
            //                if (GM2DGame.Instance.CurrentMode == EMode.EditTest) {
            //                    Messenger<ECommandType>.Broadcast (EMessengerType.OnCommandChanged, ECommandType.Pause);
            //                    GM2DGame.Instance.ChangeToMode (EMode.Edit);
            //                }
            //                Project project = GM2DGame.Instance.Project;
            //                if (project.LocalDataState == ELocalDataState.LDS_UnCreated) {
            //                    CoroutineProxy.Instance.StartCoroutine (CoroutineProxy.RunNextFrame (() => {
            //                    SocialGUIManager.Instance.CloseUI<UICtrlGameSetting> ();
            //                        SocialGUIManager.Instance.OpenUI<UICtrlPublish> ();
            //                    }));
            //                } else {
            //                    UICtrlPublish.SaveProject (project.Name, project.Summary,
            //                        project.DownloadPrice, project.PublishRecordFlag, () => {
            //                        SocialGUIManager.Instance.CloseUI<UICtrlGameSetting> ();
            //                        SocialApp.Instance.ReturnToApp ();
            //                    }, () => {
            //                        //保存失败
            //                    });
            //                }
            //            }),
            //            new System.Collections.Generic.KeyValuePair<string, Action> ("退出", () => {
            //                SocialGUIManager.Instance.CloseUI<UICtrlGameSetting> ();
            //                SocialApp.Instance.ReturnToApp ();
            //            }));
            //        return;
            //    }
            //} else if (GM2DGame.Instance.GameInitType == GameManager.EStartType.ModifyEdit) {
            //    if (GM2DGame.Instance.NeedSave) {
            //        // 保存改造关卡
            //        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "正在保存改造关卡");
            //        GM2DGame.Instance.SaveModifyProject (() => {
            //            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
            //            SocialGUIManager.Instance.CloseUI<UICtrlGameSetting> ();
            //            SocialApp.Instance.ReturnToApp ();
            //        },
            //            code => {
            //                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
            //                SocialGUIManager.Instance.CloseUI<UICtrlGameSetting> ();
            //                SocialApp.Instance.ReturnToApp ();
            //                // todo error handle
            //                LogHelper.Error ("SaveModifyProject failed");
            //            }
            //        );
            //        return;
            //    }
            //}
			SocialGUIManager.Instance.CloseUI<UICtrlGameSetting>();
            //SocialApp.Instance.ReturnToApp();
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "...");
            GM2DGame.Instance.QuitGame (
                () => {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                },
                code => {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                },
                true
            );
		}

		private void OnClickReturnToGameButton()
		{
			SocialGUIManager.Instance.CloseUI<UICtrlGameSetting>();
		}

		private void OnClickRestartButton()
		{
			if (GM2DGame.Instance.CurrentMode == EMode.Edit)
			{
				return;
			}

            Project p = GameManager.Instance.CurrentGame.Project;
            if(p.ProjectStatus == SoyEngine.Proto.EProjectStatus.PS_Public
                && GameManager.Instance.GameMode == EGameMode.Normal)
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "");
                p.BeginPlay(false, ()=>{
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    Game.PlayMode.Instance.RePlay();
                    SocialGUIManager.Instance.CloseUI<UICtrlGameSetting>();
                }, code=>{
//                    string tip = null;
//                    if(code == EPlayProjectRetCode.PPRC_ProjectHasBeenDeleted)
//                    {
//                        tip = "作品已被删除，启动失败";
//                    }
//                    else if(code == EPlayProjectRetCode.PPRC_FrequencyTooHigh)
//                    {
//                        tip = "启动过于频繁，启动失败";
//                    }
//                    else
//                    {
//                        if(Application.internetReachability == NetworkReachability.NotReachable)
//                        {
//                            tip = "启动失败，请检查网络环境";
//                        }
//                        else
//                        {
//                            tip = "启动失败，未知错误";
//                        }
//                    }
//                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                    CommonTools.ShowPopupDialog(tip, null, 
//                        new System.Collections.Generic.KeyValuePair<string, Action>("重试", ()=>{
//                            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnClickRestartButton));
//                        }),
//                        new System.Collections.Generic.KeyValuePair<string, Action>("取消", ()=>{
//                        }));
                });
            }
            else
            {
                Game.PlayMode.Instance.RePlay();
                SocialGUIManager.Instance.CloseUI<UICtrlGameSetting>();
            }
		}

		private void ReceiveOpenSettingCommand()
		{
			if (_isOpen)
			{
				LogHelper.Error("ReceiveOpenSettingCommand called but isopen is true");
				return;
			}
			SocialGUIManager.Instance.OpenUI<UICtrlGameSetting>();
		}
		#endregion
	}
}

