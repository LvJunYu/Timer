/********************************************************************
** Filename : UICtrlWorkShop.cs
** Author : quan
** Date : 6/7/2017 10:54 AM
** Summary : UICtrlWorkShop.cs
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlWorkShop : UICtrlGenericBase<UIViewWorkShop>
    {
        #region 常量与字段
        /// <summary>
        /// 当前的状态
        /// </summary>
        private EWorkShopState _state = EWorkShopState.None;

        private CardDataRendererWrapper<Project> _curSelectedPrivateProject;
        private CardDataRendererWrapper<Project> _curSelectedPublicProject;
        private List<CardDataRendererWrapper<Project>> _privateContents = new List<CardDataRendererWrapper<Project>>();
        private List<CardDataRendererWrapper<Project>> _publicContents = new List<CardDataRendererWrapper<Project>>();
        private Dictionary<long, CardDataRendererWrapper<Project>> _privateDict = new Dictionary<long, CardDataRendererWrapper<Project>>();
        private Dictionary<long, CardDataRendererWrapper<Project>> _publicDict = new Dictionary<long, CardDataRendererWrapper<Project>>();
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
            base.OnOpen (parameter);
            SetMode(EWorkShopState.PersonalProject);
            if (!LocalUser.Instance.PersonalProjectList.IsInited)
            {
                LocalUser.Instance.PersonalProjectList.Request();
            }
            LocalUser.Instance.UserPublishedWorldProjectList.Request(
                LocalUser.Instance.UserGuid,
                0, int.MaxValue,
                EPublishedProjectOrderBy.PPOB_PublishTime,
                EOrderType.OT_Asc,
                null,
                null
            );

            if (null == _curSelectedPrivateProject) {
                AutoSelectFirstProject ();
            }
            RefreshView();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnWorkShopProjectListChanged, OnPersonalProjectListChanged);
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
                    WeakReference projectWR = new WeakReference (_curSelectedPrivateProject.Content);
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
//            }
        }

        private void RefreshView()
        {
//            RefreshView();
            if (_state == EWorkShopState.PersonalProject) {
                RefreshWorkShopProjectList ();
                RefreshProjectDetailInfoPanel ();
            } else if (_state == EWorkShopState.PublishList) {
                RefreshPublishedProjectList ();
                RefreshPublishedInfoPanel();

            }
        }

        private void AutoSelectFirstProject () {
            _autoSelectFirstProject = true;
        }

        private void RefreshWorkShopProjectList () {
            long preSelectPRojectId = 0;
            if (null != _curSelectedPrivateProject) {
                preSelectPRojectId = _curSelectedPrivateProject.Content.ProjectId;
            }
            if (LocalUser.Instance.PersonalProjectList.IsInited) {
                List<Project> list = LocalUser.Instance.PersonalProjectList.ProjectList;
                _privateContents.Clear();
                _privateContents.Capacity = Mathf.Max(_privateContents.Count, list.Capacity);
                _privateDict.Clear();
                for(int i=0; i < list.Count; i++)
                {
                    var wrapper = new CardDataRendererWrapper<Project>(list[i], OnPrivateProjectCardClick);
                    wrapper.IsSelected = list [i].ProjectId == preSelectPRojectId;
                    _curSelectedPrivateProject = wrapper.IsSelected ? wrapper : _curSelectedPrivateProject;
                    _privateContents.Add(wrapper);
                    _privateDict.Add(wrapper.Content.ProjectId, wrapper);
                }
                _cachedView.PrivateProjectsGridScroller.SetItemCount(_privateContents.Count);
                if (_autoSelectFirstProject && null == _curSelectedPrivateProject) {
                    _autoSelectFirstProject = false;
                    if (_privateContents.Count > 0) {
                        _privateContents [0].IsSelected = true;
                        _curSelectedPrivateProject = _privateContents [0];
                    }
                }
                for (int i = 0; i < _cachedView.ObjectsShowWhenEmpty.Length; i++) {
                    _cachedView.ObjectsShowWhenEmpty [i].SetActive (list.Count == 0);
                }
            }
        }

        private void RefreshProjectDetailInfoPanel () {
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


            if (null != _curSelectedPrivateProject && null != _curSelectedPrivateProject.Content) {
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, _curSelectedPrivateProject.Content.IconPath, _cachedView.DefaultCoverTexture);
                DictionaryTools.SetContentText(_cachedView.Title, _curSelectedPrivateProject.Content.Name);
                DictionaryTools.SetContentText(_cachedView.Desc, _curSelectedPrivateProject.Content.Summary);
            } else {
                ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCoverTexture);
                DictionaryTools.SetContentText(_cachedView.Title, "关卡标题");
                DictionaryTools.SetContentText(_cachedView.Desc, "关卡简介");
            }
        }

        private void RefreshPublishedInfoPanel() {

            //int curLv = LocalUser.Instance.User.UserInfoSimple.LevelData.CreatorLevel;
            //int nexLv = curLv + 1;
            //long curExp = LocalUser.Instance.User.UserInfoSimple.LevelData.CreatorExp;
            //Game.Table_PlayerLvToExp curLvTable = Game.TableManager.Instance.GetPlayerLvToExp (curLv);
            //Game.Table_PlayerLvToExp nextLvTable = Game.TableManager.Instance.GetPlayerLvToExp (nexLv);
            //long curLvExpBase = curLvTable == null ? curExp : curLvTable.AdvExp;
            //long nextLvExp = nextLvTable == null ? int.MaxValue : nextLvTable.AdvExp;

            //_cachedView.MakerLvl.text = curLv.ToString ();
            //_cachedView.MakerExpText.text = string.Format ("{0} / {1}", curExp, nextLvExp);
            //_cachedView.MakerExpFillImg.fillAmount = Mathf.Clamp01((float)(curExp - curLvExpBase) / Mathf.Max(1, (nextLvExp - curLvExpBase)));
            //            _cachedView.PublishedProjectCnt.text = LocalUser.Instance.User.UserInfoSimple.
            _cachedView.EditDescBtn.gameObject.SetActive(false);
            _cachedView.EditTitleBtn.gameObject.SetActive(false);
            _cachedView.Data.gameObject.SetActive(true);
            _cachedView.HummerIcon.gameObject.SetActive(false);
            if (null != _curSelectedPublicProject && null != _curSelectedPublicProject.Content)
            {
                Debug.Log("_Cover___" + _cachedView.Cover + "_curSelectedPublicProject" +
                          _curSelectedPublicProject.Content.IconPath + "DefaultCoverTexture" +
                          _cachedView.DefaultCoverTexture);
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, _curSelectedPublicProject.Content.IconPath, _cachedView.DefaultCoverTexture);
                DictionaryTools.SetContentText(_cachedView.Title, _curSelectedPublicProject.Content.Name);
                DictionaryTools.SetContentText(_cachedView.Desc, _curSelectedPublicProject.Content.Summary);
            }
            else
            {
                ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCoverTexture);
                DictionaryTools.SetContentText(_cachedView.Title, "关卡标题");
                DictionaryTools.SetContentText(_cachedView.Desc, "关卡简介");
            }

        }

        private void RefreshPublishedProjectList () {
            long preSelectPRojectId = 0;
            if (null != _curSelectedPublicProject)
            {
                preSelectPRojectId = _curSelectedPublicProject.Content.ProjectId;
            }
            //            LocalUser.Instance.per
            if (LocalUser.Instance.UserPublishedWorldProjectList.IsInited) {
                List<Project> list = LocalUser.Instance.UserPublishedWorldProjectList.ProjectList;
                _publicContents.Clear();
                _publicContents.Capacity = Mathf.Max(_publicContents.Count, list.Capacity);
                for(int i=0; i < list.Count; i++)
                {
                    var wrapper = new CardDataRendererWrapper<Project>(list[i], OnPublicProjectCardClick);
                    wrapper.IsSelected = list[i].ProjectId == preSelectPRojectId;
                    _curSelectedPublicProject = wrapper.IsSelected ? wrapper : _curSelectedPublicProject;
                    _publicContents.Add(wrapper);
                    _publicDict.Add(wrapper.Content.ProjectId, wrapper);

                    //                    if(_mode == EMode.Edit)
                    //                    {
                    //                        wrapper.CardMode = ECardMode.Selectable;
                    //                        wrapper.IsSelected = false;
                    //                    }
                    //                    else
                    {
//                        _curSelectedPrivateProject = wrapper.IsSelected ? wrapper : _curSelectedPrivateProject;
                    }
                }
                _cachedView.PublicProjectsGridScroller.SetItemCount(_publicContents.Count);
                for (int i = 0; i < _cachedView.ObjectsShowWhenEmpty.Length; i++) {
                    _cachedView.ObjectsShowWhenEmpty [i].SetActive (list.Count == 0);
                }
                
//                if (_autoSelectFirstProject && null == _curSelectedPrivateProject) {
//                    _autoSelectFirstProject = false;
//                    if (_privateContents.Count > 0) {
//                        _privateContents [0].IsSelected = true;
//                        _curSelectedPrivateProject = _privateContents [0];
//                    }
//                }
                //                _currentSelectedCount = 0;
            }
        }

        private void OnPrivateProjectCardClick(CardDataRendererWrapper<Project> item) {
            if (null != _curSelectedPrivateProject) {
                _curSelectedPrivateProject.IsSelected = false;
            }
            item.IsSelected = true;
            _curSelectedPrivateProject = item;
            _cachedView.PrivateProjectsGridScroller.RefreshCurrent ();
            RefreshProjectDetailInfoPanel ();
        }

        private void OnPublicProjectCardClick (CardDataRendererWrapper<Project> item) {
            //return;
            //if (null != item && null != item.Content)
            //{
            //    SocialGUIManager.Instance.OpenUI<UICtrlAdvLvlDetail>(item.Content);
            //}
            if (null != _curSelectedPublicProject)
            {
                _curSelectedPrivateProject.IsSelected = false;
            }
            item.IsSelected = true;
            _curSelectedPublicProject = item;
            _cachedView.PublicProjectsGridScroller.RefreshCurrent();
            RefreshPublishedInfoPanel();

        }

        private void OnPrivateItemRefresh(IDataItemRenderer item, int inx)
        {
            if(inx >= _privateContents.Count)
            {
                LogHelper.Error("OnPrivateItemRefresh Error Inx > count");
                return;
            }
            item.Set(_privateContents[inx]);
        }

        private IDataItemRenderer GetPrivateItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlWorkShopProjectCard();
            item.Init(parent, Vector3.zero);
            return item;
        }

        private void OnPublicItemRefresh(IDataItemRenderer item, int inx)
        {
            if(inx >= _publicContents.Count)
            {
                LogHelper.Error("OnPublicItemRefresh Error Inx > count");
                return;
            }
            item.Set(_publicContents[inx]);
        }

        private IDataItemRenderer GetPublicItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlPublishedProjectCard();
            item.Init(parent, Vector3.zero);
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


        private void ProcessDelete () {
            if (null == _curSelectedPrivateProject || null == _curSelectedPrivateProject.Content)
                return;
            CommonTools.ShowPopupDialog(string.Format("删除之后将无法恢复，确定要删除《{0}》吗？", _curSelectedPrivateProject.Content.Name), null,
                new KeyValuePair<string, Action>("确定",()=>{
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在删除");
                    var projList = new List<long>();
                    projList.Add(_curSelectedPrivateProject.Content.ProjectId);
                    RemoteCommands.DeleteProject(projList, msg => {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        LocalUser.Instance.PersonalProjectList.ProjectList.Remove(_curSelectedPrivateProject.Content);
                        _curSelectedPrivateProject = null;
                        AutoSelectFirstProject ();
                        RefreshView ();
                        LocalUser.Instance.PersonalProjectList.Request (
                            () => {
                            },
                            code => {
                                // todo error handle
                            }
                        );
                    },
                    code => {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        CommonTools.ShowPopupDialog("删除失败");
                    });
                }), new KeyValuePair<string, Action>("取消", ()=>{
                    LogHelper.Info("Cancel Delete");
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
                DictionaryTools.SetContentText(_cachedView.ChangeModeBtnText, "创作中的关卡");
            } else if (_state == EWorkShopState.PublishList) {
                LocalUser.Instance.UserPublishedWorldProjectList.Request(
                    LocalUser.Instance.UserGuid,
                    0, int.MaxValue,
                    EPublishedProjectOrderBy.PPOB_PublishTime,
                    EOrderType.OT_Asc,
                    () => {
                        if (_isOpen && _state == EWorkShopState.PublishList) {
                            RefreshPublishedProjectList();
                        }
                    },
                    null
                );
                _cachedView.Private.SetActive (false);
                _cachedView.Public.SetActive (true);
                DictionaryTools.SetContentText(_cachedView.ChangeModeBtnText, "已发布的关卡");
            }
            RefreshView ();
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
                SocialGUIManager.Instance.OpenUI<UICtrlAdvLvlDetail>(_curSelectedPublicProject.Content);
            }
        }

		private void OnReturnBtn () {
            if (_waitReturnToAppTimer > 0)
                return;
            if (_wait2RequestUpdateProjects.Count == 0) {
                SocialGUIManager.Instance.CloseUI<UICtrlWorkShop>();
            } else {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在提交关卡修改...");
                _waitReturnToAppTimer = _waitReturnToAppTimeOut;
            }
		}
        #region 接口
        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.UIAdvDetail;
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
            RefreshWorkShopProjectList ();
            RefreshProjectDetailInfoPanel ();
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
                RefreshProjectDetailInfoPanel();
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
