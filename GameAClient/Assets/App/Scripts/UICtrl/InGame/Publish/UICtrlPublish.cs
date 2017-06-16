///********************************************************************
//** Filename : UICtrlPublish
//** Author : quan
//** Date : 2015/7/2 16:30:13
//** Summary : UICtrlPublish
//***********************************************************************/
//
//
//using SoyEngine;
//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using SoyEngine.Proto;
//using UnityEngine.UI;
//using GameA.Game;
//
//namespace GameA
//{
//    [UIAutoSetup(EUIAutoSetupType.Add)]
//    public class UICtrlPublish : UICtrlInGameBase<UIViewPublish>
//    {
//	    public const string TagButtonSpriteNormalState = "ButtonBG_12";
//	    public const string TagButtonSpriteSelectedState = "ButtonBG_13";
//
//		#region 常量与字段
//		private Project _project;
//        private Texture2D _coverTexture;
//        private int _downloadPrice;
//
//	    private bool _uploadRecord = false;
//        private GameModeEdit _gameModeEdit;
//
//		#endregion
//
//		#region 属性
//        #endregion
//
//        #region 方法
//
//	    private void UpdatePublicTagState()
//	    {
////		    var enumerator = _cachedButtons.GetEnumerator();
////
////		    while (enumerator.MoveNext())
////		    {
////			    var curItem = enumerator.Current;
////				curItem.Value.image.SetSpriteEx(_curSelectType == curItem.Key? TagButtonSpriteSelectedState: TagButtonSpriteNormalState);
////			}
//	    }
//
//	    private void UpdateUploadRecordState()
//	    {
//		    _cachedView.UploadSelectedFlag.SetActiveEx(_uploadRecord);
//	    }
//
//        protected override void InitGroupId()
//        {
//            _groupId = (int) EUIGroupType.InGamePopup;
//        }
//
//        protected override void OnOpen(object parameter)
//        {
//            base.OnOpen(parameter);
//
//            _project = GM2DGame.Instance.Project;
//            _gameModeEdit = GM2DGame.Instance.GameMode as GameModeEdit;
////            _curSelectType = _project.ProjectCategory;
////            if(_curSelectType == EProjectCategory.PC_None)
////            {
////                _curSelectType = EProjectCategory.PC_Relaxation;
////            }
//            _downloadPrice = _project.DownloadPrice;
//            _cachedView.NameInputField.text = string.IsNullOrEmpty(_project.Name)? "我的匠游大作" : _project.Name;
//            _cachedView.DesInputField.text = _project.Summary;
//            RefreshDownloadPriceView();
//            RefreshPublishCountView();
//	        UpdatePublicTagState();
//	        UpdateUploadRecordState();
//            if (_gameModeEdit.IconBytes == null)
//            {
//                OnCoverButtonClick();
//            }
//            RefreshCover();
//        }
//
//        private void RefreshDownloadPriceView()
//        {
//            if(_downloadPrice < 0)
//            {
//                DictionaryTools.SetContentText(_cachedView.DownloadPriceText, "∞");
//            }
//            else
//            {
//                DictionaryTools.SetContentText(_cachedView.DownloadPriceText, "" + _downloadPrice);
//            }
//        }
//
//        private void RefreshPublishCountView()
//        {
//            if(_cachedView.PublishCountText == null)
//            {
//                return;
//            }
//
////            UserMatrixData.Item userMatrixData = AppData.Instance.UserMatrixData.GetData(GM2DGame.Instance.Project.MatrixGuid);
////            if(userMatrixData == null)
////            {
////                DictionaryTools.SetContentText(_cachedView.PublishCountText, string.Format("今日发布: {0}/{1}", 0, 0));
////            }
////            else
////            {
////                DictionaryTools.SetContentText(_cachedView.PublishCountText, string.Format("今日发布: {0}/{1}", userMatrixData.TodayPublishCount, userMatrixData.PublishCountEachDay));
////            }
//        }
//
//        protected override void OnViewCreated()
//        {
//            base.OnViewCreated();
//            _cachedView.NameInputField.characterLimit = SoyConstDefine.MaxProjectNameLength;
//            _cachedView.DesInputField.characterLimit = SoyConstDefine.MaxProjectSumaryLength;
//            _cachedView.CoverBtn.onClick.AddListener(OnCoverButtonClick);
//            _cachedView.DownloadPriceBtn.onClick.AddListener(OnDownloadPriceSettingButtonClick);
//            _cachedView.SaveBtn.onClick.AddListener(OnSaveButtonClick);
//            _cachedView.PublishBtn.onClick.AddListener(OnPublishButtonClick);
//            _cachedView.CancleBtn.onClick.AddListener(OnCancleButtonClick);
//            _cachedView.NameInputField.characterLimit = SoyConstDefine.MaxProjectNameLength;
//            _cachedView.DesInputField.characterLimit = SoyConstDefine.MaxProjectSumaryLength;
//
//	        _cachedView.NameHeadingText.text = LocaleManager.GameLocale("ui_publish_name_heading");
//	        //_cachedView.DesHeadingText.text = LocaleManager.GameLocale("ui_publish_des_heading");
//
//	        _cachedView.PublishBtnText.text = LocaleManager.GameLocale("ui_publish_btn_publish");
//			_cachedView.SaveBtnText.text = LocaleManager.GameLocale("ui_publish_btn_save");
//            _coverTexture = new Texture2D(1,1);
//            _downloadPrice = 0;
//
////            _cachedButtons.Add(EProjectCategory.PC_Relaxation, _cachedView.ButtonTagArder);
////            _cachedButtons.Add(EProjectCategory.PC_Puzzle, _cachedView.ButtonTagRiddle);
////            _cachedButtons.Add(EProjectCategory.PC_Challenge, _cachedView.ButtonTagHighpoint);
//
//			_cachedView.ButtonTagArder.onClick.AddListener(OnClickTagCasualGame);
//			_cachedView.ButtonTagRiddle.onClick.AddListener(OnClickTagPuzzleGame);
//			_cachedView.ButtonTagHighpoint.onClick.AddListener(OnClickTagXGame);
//			_cachedView.ButtonUploadRecord.onClick.AddListener(OnClickUploadRecord);
//		}
//
//
//		private void RefreshCover()
//        {
//            if(_gameModeEdit.IconBytes != null)
//            {
//                _coverTexture.LoadImage(_gameModeEdit.IconBytes);
//                _cachedView.CoverImage.texture = _coverTexture;
//                _cachedView.CoverImage.SetAllDirty();
//            }
//        }
//
//		private void OnCoverButtonClick()
//        {
//            var iconBytes = _gameModeEdit.CaptureLevel();
//            _gameModeEdit.IconBytes = iconBytes;
//            RefreshCover();
//        }
//
//        private void OnDownloadPriceSettingButtonClick()
//        {
//            SocialGUIManager.Instance.OpenUI<UICtrlGameDownloadPriceSetting>(_downloadPrice);
//        }
//
//	    private void OnClickTagCasualGame()
//	    {
////            if (_curSelectType == EProjectCategory.PC_Relaxation)
////		    {
////			    return;
////		    }
////            _curSelectType = EProjectCategory.PC_Relaxation;
////			UpdatePublicTagState();
//	    }
//
//		private void OnClickTagPuzzleGame()
//		{
////            if (_curSelectType == EProjectCategory.PC_Puzzle)
////			{
////				return;
////			}
////            _curSelectType = EProjectCategory.PC_Puzzle;
////			UpdatePublicTagState();
////            _uploadRecord = true;
////            UpdateUploadRecordState();
//		}
//
//	    private void OnClickTagXGame()
//	    {
////            if (_curSelectType == EProjectCategory.PC_Challenge)
////			{
////				return;
////			}
////            _curSelectType = EProjectCategory.PC_Challenge;
//			UpdatePublicTagState();
//		}
//
//	    private void OnClickUploadRecord()
//	    {
////            if(_curSelectType == EProjectCategory.PC_Puzzle)
////            {
////                _uploadRecord = true;
////				Messenger<string>.Broadcast(EMessengerType.GameErrorLog,LocaleManager.GameLocale("ui_error_upload_record_tips_puzzle"));
////            }
////            else
////            {
////                _uploadRecord = !_uploadRecord;
////            }
//		    UpdateUploadRecordState();
//	    }
//
//
//		private void OnSaveButtonClick()
//        {
//            SaveProject(_cachedView.NameInputField.text, _cachedView.DesInputField.text, _downloadPrice,
//                _uploadRecord, ()=>{
//                Close();
//            }, ()=>{
//                
//            });
//        }
//
//        private void OnPublishButtonClick()
//        {
//            if (!_gameModeEdit.CheckCanPublish(true))
//			{
//				return;
//			}
//            PublishProject(_cachedView.NameInputField.text, _cachedView.DesInputField.text, ()=>{
//                RefreshPublishCountView();
//                Close();
//                CommonTools.ShowPopupDialog("发布成功，请在我的作品里查看，是否立即退出", null,
//                    new System.Collections.Generic.KeyValuePair<string, Action>("继续", ()=>{
//                        
//                    }), new System.Collections.Generic.KeyValuePair<string, Action>("退出", ()=>{
//                        SocialApp.Instance.ReturnToApp();
//                    }));
//            }, ()=>{
//                
//            });
//        }
//
//        private bool CheckUserInput()
//        {
//            string inputName = _cachedView.NameInputField.text;
//            string inputSummary = _cachedView.DesInputField.text;
//            if (string.IsNullOrEmpty(inputName))
//            {
//                CommonTools.ShowPopupDialog("请输入作品名称~", null);
//                return false;
//            }
//            return true;
//        }
//
//
//        private void OnCancleButtonClick()
//        {
//            SocialGUIManager.Instance.CloseUI<UICtrlPublish>();
//        }
//
//        protected override void OnDestroy()
//        {
//            _cachedView.SaveBtn.onClick.RemoveAllListeners();
//            _cachedView.CancleBtn.onClick.RemoveAllListeners();
//            if(_coverTexture != null)
//            {
//                UnityEngine.Object.Destroy(_coverTexture);
//                _coverTexture = null;
//            }
//        }
//        #endregion
//        #region 静态方法
//        public static void SaveProject(string name, string summary, int downloadPrice,
//            bool publishRecordFlag, Action successCallback, Action failedCallback)
//        {
//            if (string.IsNullOrEmpty(name))
//            {
//                CommonTools.ShowPopupDialog("请输入作品名称");
//                if(failedCallback != null)
//                {
//                    failedCallback.Invoke();
//                }
//                return;
//            }
////            if(projectCategory == EProjectCategory.PC_None)
////            {
////                projectCategory = EProjectCategory.PC_Relaxation;
////            }
////            else if(projectCategory == EProjectCategory.PC_Puzzle)
////            {
////                publishRecordFlag = true;
////            }
//
//            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(SocialGUIManager.Instance.GetUI<UICtrlPublish>(), "作品正在保存中");
//            GameModeEdit gameModeEdit = GM2DGame.Instance.GameMode as GameModeEdit;
//            gameModeEdit.Save(()=>{
//                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(SocialGUIManager.Instance.GetUI<UICtrlPublish>());
//                CommonTools.ShowPopupDialog("保存成功");
//                if(successCallback != null)
//                {
//                    successCallback.Invoke();
//                }
//            }, (code)=>{
//                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(SocialGUIManager.Instance.GetUI<UICtrlPublish>());
//                if(code == EProjectOperateResult.POR_None)
//                {
//                }
//                else if(code == EProjectOperateResult.POR_WorkshopSizeNotEnough)
//                {
//                    CommonTools.ShowPopupDialog("工坊已满，保存失败");
//                }
//                else
//                {
//                    CommonTools.ShowPopupDialog("保存失败");
//                }
//                if(failedCallback != null)
//                {
//                    failedCallback.Invoke();
//                }
//            });
//        }
//
//        public static void PublishProject(string name, string summary, Action successCallback, Action failedCallback)
//        {
//            if (string.IsNullOrEmpty(name))
//            {
//                CommonTools.ShowPopupDialog("请输入作品名称");
//                if(failedCallback != null)
//                {
//                    failedCallback.Invoke();
//                }
//                return;
//            }
////            UserMatrixData.Item userMatrixData = AppData.Instance.UserMatrixData.GetData(GM2DGame.Instance.Project.MatrixGuid);
////            if(userMatrixData != null)
////            {
////                if(DateTimeUtil.GetServerTimeNowTimestampMillis() - userMatrixData.LastPublishTime < 30*GameTimer.Second2Ms)
////                {
////                    ShowPublishErrorTip(EProjectOperateResult.POR_FrequencyTooHigh);
////                    return;
////                }
////                if(userMatrixData.TodayPublishCount >= userMatrixData.PublishCountEachDay)
////                {
////                    ShowPublishErrorTip(EProjectOperateResult.POR_CountLimitExceeded);
////                    return;
////                }
////            }
//
////            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(GM2DGUIManager.Instance.GetUI<UICtrlPublish>(), "作品正在努力发布中");
////            GM2DGame.Instance.Publish(name, summary, ()=>{
////                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(GM2DGUIManager.Instance.GetUI<UICtrlPublish>());
////                if(successCallback != null)
////                {
////                    successCallback.Invoke();
////                }
////            }, (code)=>{
////                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(GM2DGUIManager.Instance.GetUI<UICtrlPublish>());
////                ShowPublishErrorTip(code);
////                if(failedCallback != null)
////                {
////                    failedCallback.Invoke();
////                }
////            });
//        }
//
//        private static void ShowPublishErrorTip(EProjectOperateResult code)
//        {
//            if(code == EProjectOperateResult.POR_None)
//            {
//            }
//            else if(code == EProjectOperateResult.POR_WorkshopSizeNotEnough)
//            {
//                CommonTools.ShowPopupDialog("工坊已满，发布失败");
//            }
//            else if(code == EProjectOperateResult.POR_CountLimitExceeded)
//            {
//                CommonTools.ShowPopupDialog("今日发布数量到达上限，发布失败，先去挑战别人的作品吧~");
//            }
//            else if(code == EProjectOperateResult.POR_FrequencyTooHigh)
//            {
//                CommonTools.ShowPopupDialog("你刚刚发布才没过多久哦，发布失败，请稍后再试~");
//            }
//            else
//            {
//                CommonTools.ShowPopupDialog("发布失败");
//            }
//        }
//        #endregion
//    }
//}