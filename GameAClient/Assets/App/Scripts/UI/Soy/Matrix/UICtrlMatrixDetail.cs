/********************************************************************
** Filename : UICtrlMatrixDetail
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlMatrixDetail
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlMatrixDetail : UISocialContentCtrlBase<UIViewMatrixDetail>, IUIWithTitle, IUIWithTaskBar, IUIWithRightCustomButton
    {
        #region 常量与字段
        private EMode _mode = EMode.None;
        private int _personalProjectCount;
        #endregion

        #region 属性

        #endregion

        #region 方法
        protected override void OnOpen (object parameter)
        {
            _personalProjectCount = 0;
            base.OnOpen (parameter);
            SetMode(EMode.Normal);
            Refresh();
        }

        protected override void OnClose()
        {
            base.OnClose();
            if(LocalUser.Instance.Account.HasLogin)
            {
                _cachedView.SoyPersonalProjectList.OnClose();
            }
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnChangeToAppMode, OnEvent);
            RegisterEvent(SoyEngine.EMessengerType.OnAccountLoginStateChanged, OnEvent);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.LoginBtn.onClick.AddListener(OnLoginBtnClick);
            _cachedView.RegisterBtn.onClick.AddListener(OnRegisterBtnClick);
            //_cachedView.CreateBtn.onClick.AddListener(OnRunBtnClick);
            _cachedView.CreateXiuXianBtn.onClick.AddListener (OnRunXiuXianBtnClick);
            _cachedView.CreateJieMiBtn.onClick.AddListener (OnRunJieMiBtnClick);
            _cachedView.CreateJiXianBtn.onClick.AddListener (OnRunJiXianBtnClick);
            _cachedView.CloseCategoryMaskBtn.onClick.AddListener (OnCloseCatogeryMaskClick);
            _cachedView.CloseCategoryMaskBtnBigger.onClick.AddListener (OnCloseCatogeryMaskClick);
            _cachedView.SoyPersonalProjectList.SetUICtrl(this);


			_cachedView.ReturnBtn.onClick.AddListener (OnReturnBtnClick);
        }

        private void Refresh()
        {
            RefreshView();
        }

        private void RefreshView()
        {
            if(LocalUser.Instance.Account.HasLogin)
            {
                _cachedView.SoyPersonalProjectList.gameObject.SetActive(true);
                _cachedView.UnloginDock.SetActive(false);
                CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitFrames(2, _cachedView.SoyPersonalProjectList.Refresh));
            }
            else
            {
                _cachedView.UnloginDock.SetActive(true);
                _cachedView.SoyPersonalProjectList.gameObject.SetActive(false);
            }
        }

        private void OnCloseCatogeryMaskClick ()
        {
            HideCreateCategoryMask ();
        }
        private void OnRunXiuXianBtnClick ()
        {
            HideCreateCategoryMask ();
            OnRunBtnClick ();
        }
        private void OnRunJieMiBtnClick ()
        {
            HideCreateCategoryMask ();
            OnRunBtnClick ();
        }
        private void OnRunJiXianBtnClick ()
        {
            HideCreateCategoryMask ();
            OnRunBtnClick ();
        }

        private void OnRunBtnClick()
        {
            if(!AppLogicUtil.CheckAndRequiredLogin())
            {
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "");
            MatrixProjectTools.PreparePersonalProjectData(()=>{
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                ProcessCreate();
            },()=>{
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                CommonTools.ShowPopupDialog("数据请求失败，请检查网络后重试");
            });
        }

        public void ProcessCreate()
        {
//            var userMatrixData = AppData.Instance.UserMatrixData.GetData(_content.MatrixGuid);
            int localCount = LocalUser.Instance.User.GetSavedProjectCount();
//            if(userMatrixData.PersonalProjectWorkshopSize <= localCount)
//            {
//                CommonTools.ShowPopupDialog("工坊已满，请升级匠人等级或者前去工坊整理");
//                return;
//            }

            EMatrixProjectResState resState = EMatrixProjectResState.None;
            if(!MatrixProjectTools.CheckMatrixStateForRun(out resState))
            {
                MatrixProjectTools.ShowMatrixProjectResCheckTip(resState);
                return;
            }
            float needDownloadSize = LocalResourceManager.Instance.GetNeedDownloadSizeMB("GameMaker2D");
            if(Application.internetReachability != NetworkReachability.NotReachable
                && !Util.IsFloatEqual(needDownloadSize, 0))
            {
                CommonTools.ShowPopupDialog(string.Format("本次进入游戏需要更新 {0:N2}MB 资源，可能会产生费用，是否继续？", Mathf.Max(needDownloadSize, 0.01f)),
                    null,
                    new System.Collections.Generic.KeyValuePair<string, Action>("继续", ()=>{
                        InternalCreate();
                    }),
                    new System.Collections.Generic.KeyValuePair<string, Action>("取消", ()=>{
                        LogHelper.Debug("Cancel RunCreate");
                    })
                );
            }
            else
            {
                InternalCreate();
            }
        }

        public void ShowCreateCategoryMask ()
        {
            if (_cachedView.CategaryMask.activeInHierarchy == false) {
                _cachedView.CategaryMask.SetActive (true);
                ScrollToTop ();
                LockScroll ();
            }
        }

        private void HideCreateCategoryMask ()
        {
            if (_cachedView.CategaryMask.activeInHierarchy == true) {
                _cachedView.CategaryMask.SetActive (false);
                UnLockScroll ();
            }
        }

        private void InternalCreate()
        {
            Project project = Project.CreateProject();
            MatrixProjectTools.SetProjectVersion(project);
//            project.ProjectCategory = category;
            GameManager.Instance.GameMode = EGameMode.Normal;
            project.BeginCreate();
            SocialGUIManager.Instance.ChangeToGameMode();
        }


        private void OnEvent()
        {
            if(_isViewCreated && _isOpen)
            {
                Refresh();
            }
        }

        public void SetMode(EMode mode)
        {
            _mode = mode;
            _cachedView.SoyPersonalProjectList.SetCardMode(mode);
//            _uiStack.Titlebar.RefreshRightButton();
        }

        public void ProcessDelete()
        {
            List<Project> projectList = _cachedView.SoyPersonalProjectList.GetSelectedProjectList();
            if(projectList.Count == 0)
            {
                return;
            }
            CommonTools.ShowPopupDialog(string.Format("确定要删除这 {0} 个作品吗？", projectList.Count), null,
                new KeyValuePair<string, Action>("确定",()=>{
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在删除");
                    LocalUser.Instance.User.DeleteUserSavedProject(projectList, ()=>{
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        SetMode(EMode.Normal);
                        RefreshView();
                    }, ()=>{
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        CommonTools.ShowPopupDialog("删除失败");
                    });
                }), new KeyValuePair<string, Action>("取消", ()=>{
                    LogHelper.Info("Cancel Delete");
                }));
        }

        public void SetPersonalProjectCount(int count)
        {
            _personalProjectCount = count;
//            _uiStack.Titlebar.RefreshRightButton();
        }

        private void OnLoginBtnClick()
        {
            SocialGUIManager.Instance.OpenPopupUI<UICtrlLogin>();
        }

        private void OnRegisterBtnClick()
        {
            SocialGUIManager.Instance.OpenPopupUI<UICtrlSignup>();
        }

		private void OnReturnBtnClick () {
			SocialGUIManager.Instance.ReturnToHome ();
		}
        #region 接口
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }
        public object GetTitle()
        {
            return "创作";
        }

        public Button GetRightButton()
        {
            if(_mode == EMode.None || _personalProjectCount == 0)
            {
                return null;
            }
            else if(_mode == EMode.Normal)
            {
                return _cachedView.EditButtonRightResource;
            }
            else if(_mode == EMode.Edit)
            {
                return _cachedView.CancelButtonRightResource;
            }
            return null;
        }

//        public void OnRightButtonClick(UICtrlTitlebar titleBar)
//        {
//            if(_mode == EMode.Edit)
//            {
//                SetMode(EMode.Normal);
//            }
//            else
//            {
//                SetMode(EMode.Edit);
//            }
//        }

        #endregion 接口
        #endregion

        public enum EMode
        {
            None,
            Normal,
            Edit,
        }
    }
}
