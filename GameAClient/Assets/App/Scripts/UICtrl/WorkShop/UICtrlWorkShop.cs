/********************************************************************
** Filename : UICtrlWorkShop.cs
** Author : quan
** Date : 6/7/2017 10:54 AM
** Summary : UICtrlWorkShop.cs
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UISingleMode, EUIAutoSetupType.Create)]
    public class UICtrlWorkShop : UICtrlAnimationBase<UIViewWorkShop>
    {
        #region 常量与字段
        /// <summary>
        /// 当前的状态
        /// </summary>
        private EWorkShopState _state = EWorkShopState.PersonalProject;
        private CardDataRendererWrapper<Project> _curSelectedPrivateProject;
        private CardDataRendererWrapper<Project> _curSelectedPublicProject;
        private readonly List<CardDataRendererWrapper<Project>> _privateContents = new List<CardDataRendererWrapper<Project>>();
        private readonly List<CardDataRendererWrapper<Project>> _publicContents = new List<CardDataRendererWrapper<Project>>();
        private readonly Dictionary<long, CardDataRendererWrapper<Project>> _privateDict = new Dictionary<long, CardDataRendererWrapper<Project>>();
        private readonly Dictionary<long, CardDataRendererWrapper<Project>> _publicDict = new Dictionary<long, CardDataRendererWrapper<Project>>();
        private const int PublishedProjectPageSize = 20;
        /// <summary>
        /// 本地信息改变了等待update上传的关卡列表
        /// </summary>
        private readonly List<WeakReference> _wait2RequestUpdateProjects = new List<WeakReference>();
        /// <summary>
        /// 返回时如果等待更新列表为空，则进入等待返回家园状态，当所有等待更新的关卡更新成功后，返回家园
        /// </summary>
        private int _waitReturnToAppTimer;
        private const int WaitReturnToAppTimeOut = 4000;
        private bool _pushGoldEnergyStyle;
        private bool _unload;
        #endregion

        #region 属性

        #endregion

        #region 方法
        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
            if (!LocalUser.Instance.PersonalProjectList.IsInited
                || LocalUser.Instance.PersonalProjectList.IsDirty)
            {
                LocalUser.Instance.PersonalProjectList.Request();
            }

            _unload = false;
            var mode = _state;
            _state = EWorkShopState.None;
            SetMode(mode);
            if (!_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PushStyle(UICtrlGoldEnergy.EStyle.GoldDiamond);
                _pushGoldEnergyStyle = true;
            }
        }

        protected override void OnClose()
        {
            if (_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PopStyle();
                _pushGoldEnergyStyle = false;
            }
            _unload = true;
            _cachedView.PrivateProjectsGridScroller.RefreshCurrent();
            _cachedView.PublicProjectsGridScroller.RefreshCurrent();
            base.OnClose();
        }

        protected override void SetAnimationType()
        {
            base.SetAnimationType();
            _animationType = EAnimationType.None;
            _firstDelayFrames = 5;
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.TitleRtf,EAnimationType.MoveFromUp,new Vector3(0,100,0),0.1f);
            SetPart(_cachedView.LeftPartRtf, EAnimationType.MoveFromLeft,new Vector3(-700,0,0));
            SetPart(_cachedView.RightPartRtf, EAnimationType.MoveFromRight,new Vector3(700,0,0));
            SetPart(_cachedView.BGRtf,EAnimationType.Fade);
        }

        protected override void OnOpenAnimationUpdate()
        {
            base.OnOpenAnimationUpdate();
            _cachedView.PrivateProjectsScrollbar.value = 1;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnWorkShopProjectListChanged, OnPersonalProjectListChanged);
            RegisterEvent(EMessengerType.OnUserPublishedProjectChanged, OnUserPublishedProjectChanged);
            RegisterEvent<Project>(EMessengerType.OnWorkShopProjectDataChanged, OnPersonalProjectDataChanged);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.PublishedBtn.onClick.AddListener(OnChangeModeBtn);
            _cachedView.WorkingOnBtn.onClick.AddListener(OnChangeModeBtn);
            _cachedView.NewProjectBtn.onClick.AddListener(OnNewProjectBtn);
            _cachedView.PublishBtn.onClick.AddListener (OnPublishBtn);
            _cachedView.DeleteBtn.onClick.AddListener (OnDeleteBtn);
            _cachedView.EditBtn.onClick.AddListener (OnEditBtn);
            _cachedView.EditTitleBtn.onClick.AddListener (OnEditTitleBtn);
            _cachedView.ConfirmTitleBtn.onClick.AddListener (OnConfirmTitleBtn);
            _cachedView.EditDescBtn.onClick.AddListener (OnEditDescBtn);
            _cachedView.ConfirmDescBtn.onClick.AddListener (OnConfirmDescBtn);
			_cachedView.ReturnBtn.onClick.AddListener (OnReturnBtn);
            _cachedView.PrivateProjectsGridScroller.SetCallback(OnPrivateItemRefresh, GetPrivateItemRenderer);
            _cachedView.PublicProjectsGridScroller.SetCallback (OnPublicItemRefresh, GetPublicItemRenderer);
        }
 
        public override void OnUpdate ()
        {
            base.OnUpdate ();
            if (!_isOpen)
                return;
            if (SocialGUIManager.Instance.CurrentMode == SocialGUIManager.EMode.Game)
                return;
//            if (null != _curSelectedProject) {
            if (!RemoteCommands.IsRequstingUpdateProject) {
                // 优先更新当前选择关卡
                if (null != _curSelectedPrivateProject && 
                    null != _curSelectedPrivateProject.Content &&
                    _curSelectedPrivateProject.Content.IsDirty) {
//                    Debug.Log ("_________Request update current select project");
                    _curSelectedPrivateProject.Content.Save(
                        _curSelectedPrivateProject.Content.Name,
                        _curSelectedPrivateProject.Content.Summary,
                        null,
                        null,
                        _curSelectedPrivateProject.Content.PassFlag,
                        _curSelectedPrivateProject.Content.PassFlag,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0,
                        null,
                        _curSelectedPrivateProject.Content.TimeLimit,
                        _curSelectedPrivateProject.Content.WinCondition,
                        null,
                        null
                    );
                } else {
                    // 其次，更新等待更新的关卡
                    if (_wait2RequestUpdateProjects.Count > 0) {
                        WeakReference wr = _wait2RequestUpdateProjects [0];
                        _wait2RequestUpdateProjects.RemoveAt (0);
                        Project project = wr.Target as Project;
                        if (null != project && project.IsDirty) {
//                                Debug.Log ("_________Request update project");
                            project.Save(
                                project.Name,
                                project.Summary,
                                null,
                                null,
                                project.PassFlag,
                                project.PassFlag,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                null,
                                project.TimeLimit,
                                project.WinCondition,
                                null,
                                null
                            );
                        }
                    }
                }
            }
            if (_waitReturnToAppTimer > 0) {
                _waitReturnToAppTimer -= (int)(Time.deltaTime * 1000);
                // 所有等待更新关卡更新成功了，或超时了
                if (_wait2RequestUpdateProjects.Count == 0 || _waitReturnToAppTimer <= 0) {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.Instance.CloseUI<UICtrlWorkShop>();
                }
            }
        }

        private void RefreshWorkShopProjectList () {
            long preSelectPRojectId = 0;
            if (null != _curSelectedPrivateProject) {
                preSelectPRojectId = _curSelectedPrivateProject.Content.ProjectId;
                _curSelectedPrivateProject = null;
            }
            if (LocalUser.Instance.PersonalProjectList.IsInited) {
                List<Project> list = LocalUser.Instance.PersonalProjectList.ProjectList;
                _privateContents.Clear();
                _privateContents.Capacity = Mathf.Max(_privateContents.Capacity, list.Count);
                _privateDict.Clear();
                for(int i=0; i < list.Count; i++)
                {
                    var wrapper = new CardDataRendererWrapper<Project>(list[i], OnPrivateProjectCardClick);
                    if (list[i].ProjectId == preSelectPRojectId)
                    {
                        wrapper.IsSelected = true;
                        _curSelectedPrivateProject = wrapper;
                    }
                    else
                    {
                        wrapper.IsSelected = false;
                    }
                    _privateContents.Add(wrapper);
                    _privateDict.Add(wrapper.Content.ProjectId, wrapper);
                }
                if (null == _curSelectedPrivateProject) {
                    if (_privateContents.Count > 0) {
                        _privateContents [0].IsSelected = true;
                        _curSelectedPrivateProject = _privateContents [0];
                    }
                }
                _cachedView.PrivateProjectsGridScroller.SetItemCount(_privateContents.Count);
                for (int i = 0; i < _cachedView.ObjectsShowWhenEmpty.Length; i++) {
                    _cachedView.ObjectsShowWhenEmpty [i].SetActive (list.Count == 0);
                }
            }
            RefreshPersonalProjectDetailPanel();
        }

        private void RefreshPersonalProjectDetailPanel () {
            _cachedView.Title.gameObject.SetActive (true);
            _cachedView.TitleInput.gameObject.SetActive (false);
            _cachedView.EditTitleBtn.gameObject.SetActive (true);
            _cachedView.ConfirmTitleBtn.gameObject.SetActive (false);
            _cachedView.Desc.gameObject.SetActive (true);
            _cachedView.DescInput.gameObject.SetActive (false);
            _cachedView.EditDescBtn.gameObject.SetActive (true);
            _cachedView.ConfirmDescBtn.gameObject.SetActive (false);
            _cachedView.Data.gameObject.SetActive(false);
            _cachedView.HummerIcon.gameObject.SetActive(true);
            _cachedView.PlayIcon.gameObject.SetActive(false);


            if (null != _curSelectedPrivateProject && null != _curSelectedPrivateProject.Content) {
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, _curSelectedPrivateProject.Content.IconPath, _cachedView.DefaultCoverTexture);
                DictionaryTools.SetContentText(_cachedView.Title, _curSelectedPrivateProject.Content.Name);
                DictionaryTools.SetContentText(_cachedView.Desc, _curSelectedPrivateProject.Content.Summary);
            } else {
                ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCoverTexture);
                DictionaryTools.SetContentText(_cachedView.Title, "");
                DictionaryTools.SetContentText(_cachedView.Desc, "");
            }
        }

        private void RefreshPublishedProjectDetailPanel() {
            _cachedView.EditDescBtn.gameObject.SetActive(false);
            _cachedView.TitleInput.gameObject.SetActive (false);
            _cachedView.EditTitleBtn.gameObject.SetActive(false);
            _cachedView.ConfirmTitleBtn.gameObject.SetActive (false);
            _cachedView.DescInput.gameObject.SetActive (false);
            _cachedView.ConfirmDescBtn.gameObject.SetActive (false);
            _cachedView.Data.gameObject.SetActive(true);
            _cachedView.HummerIcon.gameObject.SetActive(false);
            _cachedView.PlayIcon.gameObject.SetActive(true);
            if (null != _curSelectedPublicProject && null != _curSelectedPublicProject.Content)
            {
                var p = _curSelectedPublicProject.Content;
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover,
                    _curSelectedPublicProject.Content.IconPath, _cachedView.DefaultCoverTexture);
                DictionaryTools.SetContentText(_cachedView.Title, _curSelectedPublicProject.Content.Name);
                DictionaryTools.SetContentText(_cachedView.Desc, _curSelectedPublicProject.Content.Summary);
                if (p.ExtendReady)
                {
                    _cachedView.Data.SetActiveEx(true);
                    DictionaryTools.SetContentText(_cachedView.PlayCount, p.PlayCount.ToString());
                    DictionaryTools.SetContentText(_cachedView.LikedCnt, p.LikeCount.ToString());
                    DictionaryTools.SetContentText(_cachedView.CompleteRate, GameATools.GetCompleteRateString(p.CompleteRate));
                }
                else
                {
                    _cachedView.Data.SetActiveEx(false);
                }
            }
            else
            {
                ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCoverTexture);
                DictionaryTools.SetContentText(_cachedView.Title, "");
                DictionaryTools.SetContentText(_cachedView.Desc, "");
                _cachedView.Data.SetActiveEx(false);
            }

        }

        private void RefreshPublishedProjectList () {
            long preSelectPRojectId = 0;
            if (null != _curSelectedPublicProject)
            {
                preSelectPRojectId = _curSelectedPublicProject.Content.ProjectId;
                _curSelectedPublicProject = null;
            }
            if (LocalUser.Instance.UserPublishedWorldProjectList.IsInited) {
                List<Project> list = LocalUser.Instance.UserPublishedWorldProjectList.ProjectList;
                _publicContents.Clear();
                _publicContents.Capacity = Mathf.Max(_publicContents.Capacity, list.Count);
                for(int i=0; i < list.Count; i++)
                {
                    var wrapper = new CardDataRendererWrapper<Project>(list[i], OnPublicProjectCardClick);
                    if (list[i].ProjectId == preSelectPRojectId)
                    {
                        wrapper.IsSelected = true;
                        _curSelectedPublicProject = wrapper;
                    }
                    else
                    {
                        wrapper.IsSelected = false;
                    }
                    _publicContents.Add(wrapper);
                    if(!_publicDict.ContainsKey(wrapper.Content.ProjectId))
                    _publicDict.Add(wrapper.Content.ProjectId, wrapper);
                }
                if (null == _curSelectedPublicProject)
                {
                    if (_publicContents.Count > 0)
                    {
                        _publicContents[0].IsSelected = true;
                        _curSelectedPublicProject = _publicContents[0];
                    }
                }
                _cachedView.PublicProjectsGridScroller.SetItemCount(_publicContents.Count);

                for (int i = 0; i < _cachedView.ObjectsShowWhenEmpty.Length; i++) {
                    _cachedView.ObjectsShowWhenEmpty [i].SetActive (list.Count == 0);
                }
            }
            RefreshPublishedProjectDetailPanel();
        }

        private void OnPrivateProjectCardClick(CardDataRendererWrapper<Project> item) {
            if (null != _curSelectedPrivateProject) {
                _curSelectedPrivateProject.IsSelected = false;
            }
            item.IsSelected = true;
            _curSelectedPrivateProject = item;
            _cachedView.PrivateProjectsGridScroller.RefreshCurrent ();
            RefreshPersonalProjectDetailPanel ();
        }

        private void OnPublicProjectCardClick (CardDataRendererWrapper<Project> item) {
            if (null != _curSelectedPublicProject)
            {
                _curSelectedPublicProject.IsSelected = false;
            }
            item.IsSelected = true;
            _curSelectedPublicProject = item;
            _cachedView.PublicProjectsGridScroller.RefreshCurrent();
            RefreshPublishedProjectDetailPanel();
        }

        private void OnPrivateItemRefresh(IDataItemRenderer item, int inx)
        {
            if (_unload)
            {
                item.Set(null);
            }
            else
            {
                if(inx >= _privateContents.Count)
                {
                    LogHelper.Error("OnPrivateItemRefresh Error Inx > count");
                    return;
                }
                item.Set(_privateContents[inx]);
            }
        }

        private IDataItemRenderer GetPrivateItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlWorkShopProjectCard();
            item.Init(parent, ResScenary);
            return item;
        }

        private void OnPublicItemRefresh(IDataItemRenderer item, int inx)
        {
            if (_unload)
            {
                item.Set(null);
            }
            else
            {
                if (inx >= _publicContents.Count)
                {
                    LogHelper.Error("OnPublicItemRefresh Error Inx > count");
                    return;
                }
                item.Set(_publicContents[inx]);
                var pList = LocalUser.Instance.UserPublishedWorldProjectList;
                if (!pList.IsEnd && inx > pList.AllList.Count - 2)
                {
                    RequestPublishedProject(true);
                }
            }
        }

        private IDataItemRenderer GetPublicItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlPublishedProjectCard();
            item.Init(parent, ResScenary);
            return item;
        }

        private void OnPublishBtn () {
            if (null == _curSelectedPrivateProject || null == _curSelectedPrivateProject.Content)
                return;
            if (_curSelectedPrivateProject.Content.PassFlag == false) {
                SocialGUIManager.ShowPopupDialog("关卡还未通过，无法发布，请先在关卡编辑中测试过关");
                return;
            }
            SocialGUIManager.Instance.OpenUI<UICtrlPublishProject> (_curSelectedPrivateProject.Content);
        }

        private void OnDeleteBtn ()
        {
            ProcessDelete ();
        }
        private void OnNewProjectBtn ()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlSetProjectSize> ();
        }


        private void ProcessDelete()
        {
            if (null == _curSelectedPrivateProject || null == _curSelectedPrivateProject.Content)
                return;
            CommonTools.ShowPopupDialog(
                string.Format("删除之后将无法恢复，确定要删除《{0}》吗？", _curSelectedPrivateProject.Content.Name), "删除提示",
                new KeyValuePair<string, Action>("取消", () => { LogHelper.Info("Cancel Delete"); }),
                new KeyValuePair<string, Action>("确定", () =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在删除");
                    var projList = new List<long>();
                    projList.Add(_curSelectedPrivateProject.Content.ProjectId);
                    RemoteCommands.DeleteProject(projList, msg =>
                    {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        LocalUser.Instance.PersonalProjectList.ProjectList.Remove(_curSelectedPrivateProject.Content);
                        _curSelectedPrivateProject.Content.Delete();
                        _curSelectedPrivateProject = null;
                        if (_isOpen && _state == EWorkShopState.PersonalProject)
                        {
                            RefreshWorkShopProjectList();
                        }
                        LocalUser.Instance.PersonalProjectList.Request(
                            () => { },
                            code =>
                            {
                                // todo error handle
                            }
                            );
                    },
                        code =>
                        {
                            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                            CommonTools.ShowPopupDialog("删除失败");
                        });
                }));
        }

        private void SetMode(EWorkShopState mode)
        {
            if (mode == _state)
                return;
            _state = mode;
            if (_state == EWorkShopState.PersonalProject) {
                _cachedView.Private.SetActive (true);
                _cachedView.Public.SetActive (false);
                RefreshWorkShopProjectList ();
                _cachedView.PublishBtn.SetActiveEx(true);
                _cachedView.DeleteBtn.SetActiveEx(true);
            } else if (_state == EWorkShopState.PublishList) {
                _cachedView.Private.SetActive (false);
                _cachedView.Public.SetActive (true);
                RequestPublishedProject();
                RefreshPublishedProjectList();
                _cachedView.PublishBtn.SetActiveEx(false);
                _cachedView.DeleteBtn.SetActiveEx(false);
            }
        }

        private void RequestPublishedProject(bool append = false)
        {
            int startInx = 0;
            if (append)
            {
                startInx = LocalUser.Instance.UserPublishedWorldProjectList.AllList.Count;
            }
            LocalUser.Instance.UserPublishedWorldProjectList.Requset(
                startInx,
                PublishedProjectPageSize, () => {
                    if (_isOpen && _state == EWorkShopState.PublishList) {
                        RefreshPublishedProjectList();
                    }
                }, null);
        }

        private void OnEditBtn()
        {
            if (_state == EWorkShopState.PersonalProject)
            {
                if (null != _curSelectedPrivateProject && null != _curSelectedPrivateProject.Content)
                {
                    AppLogicUtil.EditPersonalProject(_curSelectedPrivateProject.Content);
                }
            }
            else
            {
                if (_curSelectedPublicProject != null)
                {
                   SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(_curSelectedPublicProject.Content);
                }
            }
        }

		private void OnReturnBtn () {
            if (_waitReturnToAppTimer > 0)
                return;
            if (_wait2RequestUpdateProjects.Count == 0) {
                SocialGUIManager.Instance.CloseUI<UICtrlWorkShop>();
            } else {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在提交关卡修改...");
                _waitReturnToAppTimer = WaitReturnToAppTimeOut;
            }
		}
        #region 接口
        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
        }

        private void OnChangeModeBtn () {
            if (_state == EWorkShopState.PersonalProject) {
                SetMode (EWorkShopState.PublishList);
            } else if (_state == EWorkShopState.PublishList) {
                SetMode (EWorkShopState.PersonalProject);
            }
        }

        private void OnEditTitleBtn () {
            if (null == _curSelectedPrivateProject || null == _curSelectedPrivateProject.Content)
                return;
            _cachedView.Title.gameObject.SetActive (false);
            _cachedView.TitleInput.text = _curSelectedPrivateProject.Content.Name;
            _cachedView.TitleInput.gameObject.SetActive (true);
            _cachedView.EditTitleBtn.gameObject.SetActive (false);
            _cachedView.ConfirmTitleBtn.gameObject.SetActive (true);
            EventSystem.current.SetSelectedGameObject(_cachedView.TitleInput.gameObject);
        }
//TODO 副标题
        //private void OnEditSubTitleBtn()
        //{
        //    if (null == _curSelectedPrivateProject || null == _curSelectedPrivateProject.Content)
        //        return;
        //    _cachedView.SubTitle.gameObject.SetActive(false);
        //    _cachedView.SubTitleInput.text = _curSelectedPrivateProject.Content.Name;
        //    _cachedView.TitleInput.gameObject.SetActive(true);
        //    _cachedView.EditTitleBtn.gameObject.SetActive(false);
        //    _cachedView.ConfirmTitleBtn.gameObject.SetActive(true);
        //    EventSystem.current.SetSelectedGameObject(_cachedView.TitleInput.gameObject);
        //}

        private void OnConfirmTitleBtn () {
            string newTitle = _cachedView.TitleInput.text;
            newTitle = CheckProjectTitleValid (newTitle);
            if (!string.IsNullOrEmpty(newTitle) &&
                newTitle != _cachedView.Title.text) {
                _curSelectedPrivateProject.Content.Name = newTitle;
                _cachedView.Title.text = newTitle;
                AddWaitUpdateProject (_curSelectedPrivateProject.Content);
                Messenger<Project>.Broadcast(EMessengerType.OnWorkShopProjectDataChanged, _curSelectedPrivateProject.Content);
            }
            _cachedView.Title.gameObject.SetActive (true);
            _cachedView.TitleInput.gameObject.SetActive (false);
            _cachedView.EditTitleBtn.gameObject.SetActive (true);
            _cachedView.ConfirmTitleBtn.gameObject.SetActive (false);
        }
        private void OnEditDescBtn () {
            if (null == _curSelectedPrivateProject || null == _curSelectedPrivateProject.Content)
                return;
            _cachedView.Desc.gameObject.SetActive (false);
            _cachedView.DescInput.text = _curSelectedPrivateProject.Content.Summary;
            _cachedView.DescInput.gameObject.SetActive (true);
            _cachedView.EditDescBtn.gameObject.SetActive (false);
            _cachedView.ConfirmDescBtn.gameObject.SetActive (true);
            EventSystem.current.SetSelectedGameObject(_cachedView.DescInput.gameObject);
        }
        private void OnConfirmDescBtn () {
            string newDesc = _cachedView.DescInput.text;
            newDesc = CheckProjectDescValid (newDesc);
            if (!string.IsNullOrEmpty(newDesc) &&
                newDesc != _cachedView.Desc.text) {
                _curSelectedPrivateProject.Content.Summary = newDesc;
                _cachedView.Desc.text = newDesc;
                AddWaitUpdateProject (_curSelectedPrivateProject.Content);
                Messenger<Project>.Broadcast(EMessengerType.OnWorkShopProjectDataChanged, _curSelectedPrivateProject.Content);
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

        private void OnPersonalProjectListChanged()
        {
            if (!_isViewCreated)
            {
                return;
            }
            if (!_isOpen)
            {
                return;
            }
            RefreshWorkShopProjectList ();
        }
        
        private void OnUserPublishedProjectChanged()
        {
            if (!_isViewCreated)
            {
                return;
            }
            if (!_isOpen)
            {
                return;
            }
            RefreshPublishedProjectList();
        }

        private void OnPersonalProjectDataChanged(Project p)
        {
            if (!_isViewCreated)
            {
                return;
            }
            CardDataRendererWrapper<Project> personalProject;
            if (_privateDict.TryGetValue(p.ProjectId, out personalProject))
            {
                personalProject.BroadcastDataChanged();
            }
            if (_curSelectedPrivateProject != null && _curSelectedPrivateProject.Content.ProjectId == p.ProjectId)
            {
                RefreshPersonalProjectDetailPanel();
            }
        }
        #endregion

        private enum EWorkShopState
        {
            None,
            PersonalProject,
            PublishList,
        }
    }
}
