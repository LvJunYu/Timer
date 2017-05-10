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


        /// <summary>
        /// 本地信息改变了等待update上传的关卡列表
        /// </summary>
        private List<WeakReference> _wait2RequestUpdateProjects = new List<WeakReference>();
        /// <summary>
        /// 返回时如果等待更新列表为空，则进入等待返回家园状态，当所有等待更新的关卡更新成功后，返回家园
        /// </summary>
        private int _waitReturnToAppTimer = 0;
        private const int _waitReturnToAppTimeOut = 4000;
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
                    RefreshView();
                },
                code => {
                    // todo error handle
                }
            );
            if (null == _curSelectedProject) {
                AotoSelectFirstProject ();
            }
            RefreshView();
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

            _cachedView.ChangeModeBtn.onClick.AddListener(OnChangeModeBtn);
//            _cachedView.RegisterBtn.onClick.AddListener(OnRegisterBtnClick);
            _cachedView.NewProjectBtn.onClick.AddListener(OnNewProjectBtn);
            _cachedView.DeleteBtn.onClick.AddListener (OnDeleteBtn);
            _cachedView.EditBtn.onClick.AddListener (OnEditBtn);

            _cachedView.EditTitleBtn.onClick.AddListener (OnEditTitleBtn);
            _cachedView.ConfirmTitleBtn.onClick.AddListener (OnConfirmTitleBtn);
            _cachedView.EditDescBtn.onClick.AddListener (OnEditDescBtn);
            _cachedView.ConfirmDescBtn.onClick.AddListener (OnConfirmDescBtn);
//            _cachedView.SoyPersonalProjectList.SetUICtrl(this);


			_cachedView.ReturnBtn.onClick.AddListener (OnReturnBtn);

            _cachedView.PrivateProjectsGridScroller.SetCallback(OnItemRefresh, GetItemRenderer);
        }

        public override void OnUpdate ()
        {
            base.OnUpdate ();

//            if (null != _curSelectedProject) {
            if (!RemoteCommands.IsRequstingUpdateProject) {
                // 优先更新当前选择关卡
                if (null != _curSelectedProject && 
                    null != _curSelectedProject.Content &&
                    _curSelectedProject.Content.IsDirty) {
                    WeakReference projectWR = new WeakReference (_curSelectedProject.Content);
                    Debug.Log ("_________Request update current select project");
                    RemoteCommands.UpdateProject (
                        _curSelectedProject.Content.ProjectId,
                        _curSelectedProject.Content.Name,
                        _curSelectedProject.Content.Summary,
                        _curSelectedProject.Content.ProgramVersion,
                        _curSelectedProject.Content.ResourcesVersion,
                        _curSelectedProject.Content.PassFlag,
                        _curSelectedProject.Content.RecordUsedTime,
                        msg => {
                            if (msg.ResultCode == (int)EProjectOperateResult.POR_Success) {
                                if (null != _curSelectedProject &&
                                    null != _curSelectedProject.Content &&
                                    msg.ProjectData.ProjectId == _curSelectedProject.Content.ProjectId) {
                                    RefreshProjectDetailInfoPanel ();
                                }
                                if (null != projectWR.Target) {
                                    Project project = projectWR.Target as Project;
                                    project.OnSyncFromParent (msg.ProjectData);
                                }
                            }
                        },
                        code => {
                            // todo err handle
                        }
                    );
                } else {
                    // 其次，更新等待更新的关卡
                    if (_wait2RequestUpdateProjects.Count > 0) {
                        WeakReference wr = _wait2RequestUpdateProjects [0];
                        _wait2RequestUpdateProjects.RemoveAt (0);
                        if (null != wr.Target) {
                            Project project = wr.Target as Project;
                            if (null != project && project.IsDirty) {
                                Debug.Log ("_________Request update project");
                                RemoteCommands.UpdateProject (
                                    project.ProjectId,
                                    project.Name,
                                    project.Summary,
                                    project.ProgramVersion,
                                    project.ResourcesVersion,
                                    project.PassFlag,
                                    project.RecordUsedTime,
                                    msg => {
                                        if (msg.ResultCode == (int)EProjectOperateResult.POR_Success) {
                                            if (msg.ProjectData.ProjectId == project.ProjectId) {
                                                project.OnSyncFromParent (msg.ProjectData);
                                            }
                                        }
                                    },
                                    code => {
                                        // todo err handle
                                    }
                                );
                            }
                        }
                    }
                }
            }
            if (_waitReturnToAppTimer > 0) {
                _waitReturnToAppTimer -= (int)(Time.deltaTime * 1000);
                // 所有等待更新关卡更新成功了，或超时了
                if (_wait2RequestUpdateProjects.Count == 0 || _waitReturnToAppTimer <= 0) {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.Instance.ReturnToHome ();
                }
            }
//            }
        }

        private void RefreshView()
        {
//            RefreshView();
            if (_state == EWorkShopState.Edit) {
                RefreshWorkShopProjectList ();
                RefreshProjectDetailInfoPanel ();
            } else if (_state == EWorkShopState.PublishList) {
                RefreshPlayerInfoPanel ();
                RefreshPublishedProjectList ();
            }
        }

        private void AotoSelectFirstProject () {
            _autoSelectFirstProject = true;
        }

//        private void RefreshView()
//        {
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
//        }

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
                _cachedView.PrivateProjectsGridScroller.SetItemCount(_content.Count);
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
            _cachedView.Title.gameObject.SetActive (true);
            _cachedView.TitleInput.gameObject.SetActive (false);
            _cachedView.EditBtn.gameObject.SetActive (true);
            _cachedView.ConfirmTitleBtn.gameObject.SetActive (false);
            _cachedView.Desc.gameObject.SetActive (true);
            _cachedView.DescInput.gameObject.SetActive (false);
            _cachedView.EditDescBtn.gameObject.SetActive (true);
            _cachedView.ConfirmDescBtn.gameObject.SetActive (false);


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

        private void RefreshPlayerInfoPanel () {
            _cachedView.MakerLvl.text = LocalUser.Instance.User.UserInfoSimple.LevelData.CreatorLevel.ToString ();
        }

        private void RefreshPublishedProjectList () {
        }

        private void OnProjectCardClick(CardDataRendererWrapper<Project> item) {
            if (null != _curSelectedProject) {
                _curSelectedProject.IsSelected = false;
            }
            item.IsSelected = true;
            _curSelectedProject = item;
            _cachedView.PrivateProjectsGridScroller.RefreshCurrent ();
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
                            RefreshView ();
                            LocalUser.Instance.PersonalProjectList.Request (0, long.MaxValue, int.MaxValue, 0,
                                () => {
                                    RefreshView();
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
            if (mode == _state)
                return;
            _state = mode;
            if (_state == EWorkShopState.Edit) {
                _cachedView.Private.SetActive (true);
                _cachedView.Public.SetActive (false);
            } else if (_state == EWorkShopState.PublishList) {
                _cachedView.Private.SetActive (false);
                _cachedView.Public.SetActive (true);
            }
            RefreshView ();
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
        private void OnEditBtn ()
        {
            if (null != _curSelectedProject && null != _curSelectedProject.Content) {
                AppLogicUtil.EditPersonalProject(_curSelectedProject.Content);
            }
        }

		private void OnReturnBtn () {
            if (_waitReturnToAppTimer > 0)
                return;
            if (_wait2RequestUpdateProjects.Count == 0) {
                SocialGUIManager.Instance.ReturnToHome ();
            } else {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在提交关卡修改...");
                _waitReturnToAppTimer = _waitReturnToAppTimeOut;
            }
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
                    RefreshView();
                },
                code => {
                    // todo error handle
                }
            );
        }

        private void OnChangeModeBtn () {
            if (_state == EWorkShopState.Edit) {
                SetMode (EWorkShopState.PublishList);
            } else if (_state == EWorkShopState.PublishList) {
                SetMode (EWorkShopState.Edit);
            }
        }

        private void OnEditTitleBtn () {
            if (null == _curSelectedProject || null == _curSelectedProject.Content)
                return;
            _cachedView.Title.gameObject.SetActive (false);
            _cachedView.TitleInput.text = _curSelectedProject.Content.Name;
            _cachedView.TitleInput.gameObject.SetActive (true);
            _cachedView.EditTitleBtn.gameObject.SetActive (false);
            _cachedView.ConfirmTitleBtn.gameObject.SetActive (true);
        }

        private void OnConfirmTitleBtn () {
            string newTitle = _cachedView.TitleInput.text;
            newTitle = CheckProjectTitleValid (newTitle);
            if (!string.IsNullOrEmpty(newTitle) &&
                newTitle != _cachedView.Title.text) {
                _curSelectedProject.Content.Name = newTitle;
                _cachedView.Title.text = newTitle;
                AddWaitUpdateProject (_curSelectedProject.Content);
            }
            _cachedView.Title.gameObject.SetActive (true);
            _cachedView.TitleInput.gameObject.SetActive (false);
            _cachedView.EditTitleBtn.gameObject.SetActive (true);
            _cachedView.ConfirmTitleBtn.gameObject.SetActive (false);
        }
        private void OnEditDescBtn () {
            if (null == _curSelectedProject || null == _curSelectedProject.Content)
                return;
            _cachedView.Desc.gameObject.SetActive (false);
            _cachedView.DescInput.text = _curSelectedProject.Content.Summary;
            _cachedView.DescInput.gameObject.SetActive (true);
            _cachedView.EditDescBtn.gameObject.SetActive (false);
            _cachedView.ConfirmDescBtn.gameObject.SetActive (true);
        }
        private void OnConfirmDescBtn () {
            string newDesc = _cachedView.DescInput.text;
            newDesc = CheckProjectDescValid (newDesc);
            if (!string.IsNullOrEmpty(newDesc) &&
                newDesc != _cachedView.Desc.text) {
                _curSelectedProject.Content.Summary = newDesc;
                _cachedView.Desc.text = newDesc;
                AddWaitUpdateProject (_curSelectedProject.Content);
            }
            _cachedView.Desc.gameObject.SetActive (true);
            _cachedView.DescInput.gameObject.SetActive (false);
            _cachedView.EditDescBtn.gameObject.SetActive (true);
            _cachedView.ConfirmDescBtn.gameObject.SetActive (false);
        }

        #endregion 接口
        private string CheckProjectTitleValid (string title) {
            // todo 检测合法性
            return title;
        }
        private string CheckProjectDescValid (string desc) {
            // todo 检测合法性
            return desc;
        }

        private void AddWaitUpdateProject (Project project) {
            _wait2RequestUpdateProjects.Add (new WeakReference(project));
        }
        #endregion

        private enum EWorkShopState
        {
            None,
            Edit,
            PublishList,
        }
    }
}
