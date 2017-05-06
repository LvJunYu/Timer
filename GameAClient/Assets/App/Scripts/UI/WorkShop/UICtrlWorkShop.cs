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
    public class UICtrlWorkShop : UISocialCtrlBase<UIViewWorkShop>
    {
        #region 常量与字段
        /// <summary>
        /// 当前的状态
        /// </summary>
        private EWorkShopState _state = EWorkShopState.None;
        private int _personalProjectCount;

        private CardDataRendererWrapper<Project> _curSelectedProject;
        private List<CardDataRendererWrapper<Project>> _content = new List<CardDataRendererWrapper<Project>>();
        private bool _autoSelectFirstProject = false;

        #endregion

        #region 属性

        #endregion

        #region 方法
        protected override void OnOpen (object parameter)
        {
            _personalProjectCount = 0;
            base.OnOpen (parameter);
            SetMode(EWorkShopState.Edit);
            LocalUser.Instance.PersonalProjectList.Request (0, long.MaxValue, int.MaxValue, 0,
                () => {
                    Refresh();
                },
                code => {
                    // todo error handle
                }
            );
            if (null == _curSelectedProject) {
                AotoSelectFirstProject ();
            }
            Refresh();
        }

        protected override void OnClose()
        {
            base.OnClose();
//            if(LocalUser.Instance.Account.HasLogin)
//            {
//                _cachedView.SoyPersonalProjectList.OnClose();
//            }
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnChangeToAppMode, OnReturnToApp);
//            RegisterEvent(SoyEngine.EMessengerType.OnAccountLoginStateChanged, OnEvent);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
//            _cachedView.LoginBtn.onClick.AddListener(OnLoginBtnClick);
//            _cachedView.RegisterBtn.onClick.AddListener(OnRegisterBtnClick);
            _cachedView.NewProjectBtn.onClick.AddListener(OnNewProjectBtn);
            _cachedView.DeleteBtn.onClick.AddListener (OnDeleteBtn);
            _cachedView.EditBtn.onClick.AddListener (OnEditBtn);
//            _cachedView.CreateJieMiBtn.onClick.AddListener (OnRunJieMiBtnClick);
//            _cachedView.CreateJiXianBtn.onClick.AddListener (OnRunJiXianBtnClick);
//            _cachedView.CloseCategoryMaskBtn.onClick.AddListener (OnCloseCatogeryMaskClick);
//            _cachedView.CloseCategoryMaskBtnBigger.onClick.AddListener (OnCloseCatogeryMaskClick);
//            _cachedView.SoyPersonalProjectList.SetUICtrl(this);


			_cachedView.ReturnBtn.onClick.AddListener (OnReturnBtn);

            _cachedView.GridScroller.SetCallback(OnItemRefresh, GetItemRenderer);
        }

        private void Refresh()
        {
//            RefreshView();
            RefreshWorkShopProjectList ();
            RefreshProjectDetailInfoPanel ();
        }

        private void AotoSelectFirstProject () {
            _autoSelectFirstProject = true;
        }

        private void RefreshView()
        {
//            if(LocalUser.Instance.Account.HasLogin)
//            {
//                _cachedView.SoyPersonalProjectList.gameObject.SetActive(true);
//                _cachedView.UnloginDock.SetActive(false);
//                CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitFrames(2, _cachedView.SoyPersonalProjectList.Refresh));
//            }
//            else
//            {
//                _cachedView.UnloginDock.SetActive(true);
//                _cachedView.SoyPersonalProjectList.gameObject.SetActive(false);
//            }
        }

        private void RefreshWorkShopProjectList () {
            long preSelectPRojectId = 0;
            if (null != _curSelectedProject) {
                preSelectPRojectId = _curSelectedProject.Content.ProjectId;
            }
//            LocalUser.Instance.per
            if (LocalUser.Instance.PersonalProjectList.IsInited) {
                List<Project> list = LocalUser.Instance.PersonalProjectList.ProjectList;
                _content.Clear();
                _content.Capacity = list.Capacity;
                for(int i=0; i < list.Count; i++)
                {
                    var wrapper = new CardDataRendererWrapper<Project>(list[i], OnProjectCardClick);
//                    if(_mode == EMode.Edit)
//                    {
//                        wrapper.CardMode = ECardMode.Selectable;
//                        wrapper.IsSelected = false;
//                    }
//                    else
                    {
                        wrapper.CardMode = ECardMode.Normal;
                        wrapper.IsSelected = list [i].ProjectId == preSelectPRojectId;
                        _curSelectedProject = wrapper.IsSelected ? wrapper : _curSelectedProject;
                    }
                    _content.Add(wrapper);
                }
                _cachedView.GridScroller.SetItemCount(_content.Count);
                if (_autoSelectFirstProject && null == _curSelectedProject) {
                    _autoSelectFirstProject = false;
                    if (_content.Count > 0) {
                        _content [0].IsSelected = true;
                        _curSelectedProject = _content [0];
                    }
                }
//                _currentSelectedCount = 0;
            }
        }

        private void RefreshProjectDetailInfoPanel () {
            if (null != _curSelectedProject && null != _curSelectedProject.Content) {
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, _curSelectedProject.Content.IconPath, _cachedView.DefaultCoverTexture);
                DictionaryTools.SetContentText(_cachedView.Title, _curSelectedProject.Content.Name);
                DictionaryTools.SetContentText(_cachedView.Desc, _curSelectedProject.Content.Summary);
            } else {
                ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCoverTexture);
                DictionaryTools.SetContentText(_cachedView.Title, "----");
                DictionaryTools.SetContentText(_cachedView.Desc, "--------");
            }
        }

        private void OnProjectCardClick(CardDataRendererWrapper<Project> item) {
            if (null != _curSelectedProject) {
                _curSelectedProject.IsSelected = false;
            }
            item.IsSelected = true;
            _curSelectedProject = item;
            _cachedView.GridScroller.RefreshCurrent ();
            RefreshProjectDetailInfoPanel ();
        }

        private void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if(inx >= _content.Count)
            {
                LogHelper.Error("OnItemRefresh Error Inx > count");
                return;
            }
            item.Set(_content[inx]);
//            if(!_isEnd && _mode != EMode.Edit)
//            {
//                if(inx > _content.Count - 2)
//                {
//                    RequestData(true);
//                }
//            }
        }

        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlWorkShopProjectCard();
            item.Init(parent, Vector3.zero);
            return item;
        }

        private void OnCloseCatogeryMaskClick ()
        {
//            HideCreateCategoryMask ();
        }
        private void OnRunXiuXianBtnClick ()
        {
//            HideCreateCategoryMask ();
//            OnRunBtnClick ();
        }
        private void OnDeleteBtn ()
        {
            ProcessDelete ();
        }
        private void OnNewProjectBtn ()
        {
            ProcessCreate ();
        }

        private void OnRunBtnClick()
        {
            if(!AppLogicUtil.CheckAndRequiredLogin())
            {
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "...");
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
//            int localCount = LocalUser.Instance.User.GetSavedProjectCount();
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

        private void ProcessDelete () {
            if (null == _curSelectedProject || null == _curSelectedProject.Content)
                return;
            CommonTools.ShowPopupDialog(string.Format("确定要删除作品《{0}》吗？", _curSelectedProject.Content.Name), null,
                new KeyValuePair<string, Action>("确定",()=>{
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在删除");
                    var projList = new List<long>();
                    projList.Add(_curSelectedProject.Content.ProjectId);
                    RemoteCommands.DeleteProject(
                        projList,
                        msg => {
                            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                            LocalUser.Instance.PersonalProjectList.ProjectList.Remove(_curSelectedProject.Content);
                            _curSelectedProject = null;
                            AotoSelectFirstProject ();
                            Refresh ();
                            LocalUser.Instance.PersonalProjectList.Request (0, long.MaxValue, int.MaxValue, 0,
                                () => {
                                    Refresh();
                                },
                                code => {
                                    // todo error handle
                                }
                            );
                        },
                        code => {
                            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                            CommonTools.ShowPopupDialog("删除失败");
                        }
                    );
                }), new KeyValuePair<string, Action>("取消", ()=>{
                    LogHelper.Info("Cancel Delete");
            }));
        }

//        public void ShowCreateCategoryMask ()
//        {
//            if (_cachedView.CategaryMask.activeInHierarchy == false) {
//                _cachedView.CategaryMask.SetActive (true);
//                ScrollToTop ();
//                LockScroll ();
//            }
//        }

//        private void HideCreateCategoryMask ()
//        {
//            if (_cachedView.CategaryMask.activeInHierarchy == true) {
//                _cachedView.CategaryMask.SetActive (false);
//                UnLockScroll ();
//            }
//        }

        private void InternalCreate()
        {
            Project project = Project.CreateWorkShopProject();
            MatrixProjectTools.SetProjectVersion(project);
//            project.ProjectCategory = category;
            GameManager.Instance.GameMode = EGameMode.Normal;
            GameManager.Instance.RequestCreate (project);
            SocialGUIManager.Instance.ChangeToGameMode();
        }


//        private void OnEvent()
//        {
//            if(_isViewCreated && _isOpen)
//            {
//                Refresh();
//            }
//        }

        private void SetMode(EWorkShopState mode)
        {
            _state = mode;
//            _cachedView.SoyPersonalProjectList.SetCardMode(mode);
//            _uiStack.Titlebar.RefreshRightButton();
        }


//        public void SetPersonalProjectCount(int count)
//        {
//            _personalProjectCount = count;
////            _uiStack.Titlebar.RefreshRightButton();
//        }

//        private void OnLoginBtnClick()
//        {
//            SocialGUIManager.Instance.OpenPopupUI<UICtrlLogin>();
//        }
//
        private void OnEditBtn()
        {
            if (null != _curSelectedProject && null != _curSelectedProject.Content) {
                AppLogicUtil.EditPersonalProject(_curSelectedProject.Content);
            }
        }

		private void OnReturnBtn () {
			SocialGUIManager.Instance.ReturnToHome ();
		}
        #region 接口
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }
//        public object GetTitle()
//        {
//            return "创作";
//        }

//        public Button GetRightButton()
//        {
//            if(_state == EWorkShopState.None || _personalProjectCount == 0)
//            {
//                return null;
//            }
//            else if(_state == EWorkShopState.Normal)
//            {
//                return _cachedView.EditButtonRightResource;
//            }
//            else if(_state == EWorkShopState.Edit)
//            {
//                return _cachedView.CancelButtonRightResource;
//            }
//            return null;
//        }

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


        private void OnReturnToApp () {
            LocalUser.Instance.PersonalProjectList.Request (0, long.MaxValue, int.MaxValue, 0,
                () => {
                    Refresh();
                },
                code => {
                    // todo error handle
                }
            );
        }

        #endregion 接口
        #endregion

        private enum EWorkShopState
        {
            None,
            Edit,
            PublishList,
        }
    }
}
