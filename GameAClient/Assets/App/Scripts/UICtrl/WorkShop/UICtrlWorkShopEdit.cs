using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome, EUIAutoSetupType.Create)]
    public class UICtrlWorkShopEdit : UICtrlAnimationBase<UIViewWorkShopEdit>
    {
        private CardDataRendererWrapper<Project> _curSelectedPrivateProject;

        private readonly List<CardDataRendererWrapper<Project>> _privateContents =
            new List<CardDataRendererWrapper<Project>>();

        private readonly Dictionary<long, CardDataRendererWrapper<Project>> _privateDict =
            new Dictionary<long, CardDataRendererWrapper<Project>>();

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<Project>(EMessengerType.OnWorkShopProjectDataChanged, OnPersonalProjectDataChanged);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ReturnBtn.onClick.AddListener(OnReturnBtn);

            _cachedView.PublishBtn.onClick.AddListener(OnPublishBtn);
            _cachedView.DeleteBtn.onClick.AddListener(OnDeleteBtn);

            _cachedView.EditBtn.onClick.AddListener(OnEditBtn);
            _cachedView.EditTitleBtn.onClick.AddListener(OnEditTitleBtn);
            _cachedView.ConfirmTitleBtn.onClick.AddListener(OnConfirmTitleBtn);
            _cachedView.EditDescBtn.onClick.AddListener(OnEditDescBtn);
            _cachedView.ConfirmDescBtn.onClick.AddListener(OnConfirmDescBtn);
        }

        private void RefreshWorkShopProjectList()
        {
            long preSelectPRojectId = 0;
            if (null != _curSelectedPrivateProject)
            {
                preSelectPRojectId = _curSelectedPrivateProject.Content.ProjectId;
                _curSelectedPrivateProject = null;
            }
            if (LocalUser.Instance.PersonalProjectList.IsInited)
            {
                List<Project> list = LocalUser.Instance.PersonalProjectList.ProjectList;
                _privateContents.Clear();
                _privateContents.Capacity = Mathf.Max(_privateContents.Capacity, list.Count);
                _privateDict.Clear();
                for (int i = 0; i < list.Count; i++)
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
                if (null == _curSelectedPrivateProject)
                {
                    if (_privateContents.Count > 0)
                    {
                        _privateContents[0].IsSelected = true;
                        _curSelectedPrivateProject = _privateContents[0];
                    }
                }
                _cachedView.PrivateProjectsGridScroller.SetItemCount(_privateContents.Count);
                for (int i = 0; i < _cachedView.ObjectsShowWhenEmpty.Length; i++)
                {
                    _cachedView.ObjectsShowWhenEmpty[i].SetActive(list.Count == 0);
                }
            }
            RefreshPersonalProjectDetailPanel();
        }

        private void RefreshPersonalProjectDetailPanel()
        {
            _cachedView.Title.gameObject.SetActive(true);
            _cachedView.TitleInput.gameObject.SetActive(false);
            _cachedView.EditTitleBtn.gameObject.SetActive(true);
            _cachedView.ConfirmTitleBtn.gameObject.SetActive(false);
            _cachedView.Desc.gameObject.SetActive(true);
            _cachedView.DescInput.gameObject.SetActive(false);
            _cachedView.EditDescBtn.gameObject.SetActive(true);
            _cachedView.ConfirmDescBtn.gameObject.SetActive(false);
            _cachedView.Data.gameObject.SetActive(false);
            _cachedView.HummerIcon.gameObject.SetActive(true);
            _cachedView.PlayIcon.gameObject.SetActive(false);


            if (null != _curSelectedPrivateProject && null != _curSelectedPrivateProject.Content)
            {
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover,
                    _curSelectedPrivateProject.Content.IconPath, _cachedView.DefaultCoverTexture);
                DictionaryTools.SetContentText(_cachedView.Title, _curSelectedPrivateProject.Content.Name);
                DictionaryTools.SetContentText(_cachedView.Desc, _curSelectedPrivateProject.Content.Summary);
            }
            else
            {
                ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover,
                    _cachedView.DefaultCoverTexture);
                DictionaryTools.SetContentText(_cachedView.Title, "");
                DictionaryTools.SetContentText(_cachedView.Desc, "");
            }
        }

        private void OnPrivateProjectCardClick(CardDataRendererWrapper<Project> item)
        {
            if (null != _curSelectedPrivateProject)
            {
                _curSelectedPrivateProject.IsSelected = false;
            }
            item.IsSelected = true;
            _curSelectedPrivateProject = item;
            _cachedView.PrivateProjectsGridScroller.RefreshCurrent();
            RefreshPersonalProjectDetailPanel();
        }

        private void OnPublishBtn()
        {
            if (null == _curSelectedPrivateProject || null == _curSelectedPrivateProject.Content)
                return;
            if (_curSelectedPrivateProject.Content.PassFlag == false)
            {
                SocialGUIManager.ShowPopupDialog("关卡还未通过，无法发布，请先在关卡编辑中测试过关");
                return;
            }
            SocialGUIManager.Instance.OpenUI<UICtrlPublishProject>(_curSelectedPrivateProject.Content);
        }

        private void OnDeleteBtn()
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
                            LocalUser.Instance.PersonalProjectList.ProjectList.Remove(
                                _curSelectedPrivateProject.Content);
                            _curSelectedPrivateProject.Content.Delete();
                            _curSelectedPrivateProject = null;
                            if (_isOpen)
                            {
                                RefreshWorkShopProjectList();
                            }
                            LocalUser.Instance.PersonalProjectList.Request();
                        },
                        code =>
                        {
                            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                            CommonTools.ShowPopupDialog("删除失败");
                        });
                }));
        }

        private void OnEditBtn()
        {
            if (null != _curSelectedPrivateProject && null != _curSelectedPrivateProject.Content)
            {
                AppLogicUtil.EditPersonalProject(_curSelectedPrivateProject.Content);
            }
        }

        private void OnReturnBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlWorkShopEdit>();
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainPopUpUI;
        }

        private void OnEditTitleBtn()
        {
            if (null == _curSelectedPrivateProject || null == _curSelectedPrivateProject.Content)
                return;
            _cachedView.Title.gameObject.SetActive(false);
            _cachedView.TitleInput.text = _curSelectedPrivateProject.Content.Name;
            _cachedView.TitleInput.gameObject.SetActive(true);
            _cachedView.EditTitleBtn.gameObject.SetActive(false);
            _cachedView.ConfirmTitleBtn.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_cachedView.TitleInput.gameObject);
        }

        private void OnConfirmTitleBtn()
        {
            string newTitle = _cachedView.TitleInput.text;
            if (string.IsNullOrEmpty(newTitle) || newTitle != _curSelectedPrivateProject.Content.Name) return;
            if (CheckTools.CheckProjectName(_cachedView.TitleInput.text) == CheckTools.ECheckProjectNameResult.Success)
            {
                _curSelectedPrivateProject.Content.Name = newTitle;
                _cachedView.Title.text = newTitle;
                Messenger<Project>.Broadcast(EMessengerType.OnWorkShopProjectDataChanged,
                    _curSelectedPrivateProject.Content);
            }
            _cachedView.Title.gameObject.SetActive(true);
            _cachedView.TitleInput.gameObject.SetActive(false);
            _cachedView.EditTitleBtn.gameObject.SetActive(true);
            _cachedView.ConfirmTitleBtn.gameObject.SetActive(false);
        }

        private void OnEditDescBtn()
        {
            if (null == _curSelectedPrivateProject || null == _curSelectedPrivateProject.Content)
                return;
            _cachedView.Desc.gameObject.SetActive(false);
            _cachedView.DescInput.text = _curSelectedPrivateProject.Content.Summary;
            _cachedView.DescInput.gameObject.SetActive(true);
            _cachedView.EditDescBtn.gameObject.SetActive(false);
            _cachedView.ConfirmDescBtn.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_cachedView.DescInput.gameObject);
        }

        private void OnConfirmDescBtn()
        {
            string newDesc = _cachedView.DescInput.text;
//            newDesc = CheckProjectDescValid(newDesc);
            if (!string.IsNullOrEmpty(newDesc) &&
                newDesc != _cachedView.Desc.text)
            {
                _curSelectedPrivateProject.Content.Summary = newDesc;
                _cachedView.Desc.text = newDesc;
//                AddWaitUpdateProject(_curSelectedPrivateProject.Content);
                Messenger<Project>.Broadcast(EMessengerType.OnWorkShopProjectDataChanged,
                    _curSelectedPrivateProject.Content);
            }
            _cachedView.Desc.gameObject.SetActive(true);
            _cachedView.DescInput.gameObject.SetActive(false);
            _cachedView.EditDescBtn.gameObject.SetActive(true);
            _cachedView.ConfirmDescBtn.gameObject.SetActive(false);
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
    }
}