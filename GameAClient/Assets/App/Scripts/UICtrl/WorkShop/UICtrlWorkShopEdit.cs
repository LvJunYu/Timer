using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine.EventSystems;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome, EUIAutoSetupType.Create)]
    public class UICtrlWorkShopEdit : UICtrlAnimationBase<UIViewWorkShopEdit>
    {
        private Project _project;
        private bool _isEditTitle;
        private bool _isEditDesc;
        private bool _needSave;
        private EEditState _editState;
        private const string _hasPublishedStr = "已发布";
        private const string _publishStr = "发 布";
        private const string _editStr = "编 辑";

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ReturnBtn.onClick.AddListener(OnReturnBtn);
            _cachedView.OKBtn.onClick.AddListener(OnOKBtn);
            _cachedView.DeleteBtn.onClick.AddListener(OnDeleteBtn);
            _cachedView.EditBtn.onClick.AddListener(OnEditBtn);
            _cachedView.EditTitleBtn.onClick.AddListener(OnEditTitleBtn);
            _cachedView.ConfirmTitleBtn.onClick.AddListener(OnConfirmTitleBtn);
            _cachedView.EditDescBtn.onClick.AddListener(OnEditDescBtn);
            _cachedView.ConfirmDescBtn.onClick.AddListener(OnConfirmDescBtn);
            _cachedView.TitleInput.onEndEdit.AddListener(msg => OnConfirmTitleBtn());
            _cachedView.DescInput.onEndEdit.AddListener(msg => OnConfirmDescBtn());
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<Project>(EMessengerType.OnWorkShopProjectDataChanged, OnPersonalProjectDataChanged);
            RegisterEvent<long>(EMessengerType.OnWorkShopProjectPublished, OnWorkShopProjectPublished);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _project = parameter as Project;
            if (_project == null)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlWorkShopEdit>();
                return;
            }
            RefreshView();
        }

        protected override void OnClose()
        {
            _project = null;
            _needSave = _isEditTitle = _isEditDesc = false;
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover,
                _cachedView.DefaultCoverTexture);
            _cachedView.Title.text = _cachedView.Desc.text = String.Empty;
            base.OnClose();
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PannelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.transform, EAnimationType.Fade);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainPopUpUI;
        }

        private void RefreshView()
        {
            if (_project == null)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlWorkShopEdit>();
                return;
            }
            RefreshEditStateAndBtnName();
            RefreshTitleDock();
            RefreshDescDock();
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, _project.IconPath,
                _cachedView.DefaultCoverTexture);
        }

        private void RefreshEditStateAndBtnName()
        {
            if (_project.PublishTime >= _project.UpdateTime)
            {
                _editState = EEditState.HasPublished;
                _cachedView.OKBtnTxt.text = _hasPublishedStr;
            }
            else
            {
                if (_project.PassFlag)
                {
                    _editState = EEditState.HasPassed;
                    _cachedView.OKBtnTxt.text = _publishStr;
                }
                else
                {
                    _editState = EEditState.Editing;
                    _cachedView.OKBtnTxt.text = _editStr;
                }
            }
        }

        private void RefreshTitleDock()
        {
            DictionaryTools.SetContentText(_cachedView.Title, _project.Name);
            _cachedView.Title.SetActiveEx(!_isEditTitle);
            _cachedView.EditTitleBtn.SetActiveEx(!_isEditTitle);
            _cachedView.TitleInput.SetActiveEx(_isEditTitle);
            _cachedView.ConfirmTitleBtn.SetActiveEx(_isEditTitle);
        }

        private void RefreshDescDock()
        {
            DictionaryTools.SetContentText(_cachedView.Desc, _project.Summary);
            _cachedView.Desc.SetActiveEx(!_isEditDesc);
            _cachedView.EditDescBtn.SetActiveEx(!_isEditDesc);
            _cachedView.DescInput.SetActiveEx(_isEditDesc);
            _cachedView.ConfirmDescBtn.SetActiveEx(_isEditDesc);
        }

        private void SaveProject(Action successAction = null, Action failAction = null)
        {
            if (!_needSave) return;
            _project.Save(
                _project.Name,
                _project.Summary,
                null,
                null,
                _project.PassFlag,
                _project.PassFlag,
                0,
                0,
                0,
                0,
                0,
                0,
                null,
                _project.TimeLimit,
                _project.WinCondition,
                () =>
                {
                    _needSave = false;
                    Messenger<Project>.Broadcast(EMessengerType.OnWorkShopProjectDataChanged, _project);
                    if (successAction != null)
                    {
                        successAction.Invoke();
                    }
                },
                code =>
                {
                    if (failAction != null)
                    {
                        failAction.Invoke();
                    }
                }
            );
        }

        private void OnWorkShopProjectPublished(long projectId)
        {
            if (_project != null && projectId == _project.ProjectId)
            {
                if (_isOpen)
                {
                    RefreshEditStateAndBtnName();
                }
            }
        }

        private void OnOKBtn()
        {
            if (_editState == EEditState.HasPassed)
            {
                OnPublish();
            }
//            else if (_editState == EEditState.HasPublished)
//            {
//                SocialGUIManager.ShowPopupDialog("关卡已经发布。继续编辑");
//            }
            else
            {
                OnEditBtn();
            }
        }

        private void OnPublish()
        {
            if (null == _project) return;
            if (!_project.PassFlag)
            {
                SocialGUIManager.ShowPopupDialog("关卡还未通过，无法发布，请先在关卡编辑中测试过关", null,
                    new KeyValuePair<string, Action>("取消", null), new KeyValuePair<string, Action>("进入", OnEditBtn));
                return;
            }
            if (_needSave)
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在保存修改");
                SaveProject(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.Instance.OpenUI<UICtrlPublishProject>(_project);
                }, () =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.ShowPopupDialog("保存数据失败。");
                });
            }
            else
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPublishProject>(_project);
            }
        }

        private void OnDeleteBtn()
        {
            if (null == _project) return;
            CommonTools.ShowPopupDialog(
                string.Format("删除之后将无法恢复，确定要删除《{0}》吗？", _project.Name), "删除提示",
                new KeyValuePair<string, Action>("取消", () => { }),
                new KeyValuePair<string, Action>("确定", () =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在删除");
                    var projList = new List<long>();
                    projList.Add(_project.ProjectId);
                    RemoteCommands.DeleteProject(projList, msg =>
                        {
                            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                            LocalUser.Instance.PersonalProjectList.ProjectList.Remove(_project);
                            _project.Delete();
                            _project = null;
                            LocalUser.Instance.PersonalProjectList.Request();
                            SocialGUIManager.Instance.CloseUI<UICtrlWorkShopEdit>();
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
            if (_project != null)
            {
                AppLogicUtil.EditPersonalProject(_project);
            }
        }

        private void OnReturnBtn()
        {
            if (_needSave)
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在保存修改");
                SaveProject(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.Instance.CloseUI<UICtrlWorkShopEdit>();
                }, () =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.Instance.CloseUI<UICtrlWorkShopEdit>();
                    SocialGUIManager.ShowPopupDialog("保存数据失败。");
                });
            }
            else
            {
                SocialGUIManager.Instance.CloseUI<UICtrlWorkShopEdit>();
            }
        }

        private void OnEditTitleBtn()
        {
            if (null == _project) return;
            _isEditTitle = true;
            RefreshTitleDock();
            _cachedView.TitleInput.text = _project.Name;
            EventSystem.current.SetSelectedGameObject(_cachedView.TitleInput.gameObject);
        }

        private void OnConfirmTitleBtn()
        {
            string newTitle = _cachedView.TitleInput.text;
            if (string.IsNullOrEmpty(newTitle) || newTitle == _project.Name)
            {
                _isEditTitle = false;
                RefreshTitleDock();
                return;
            }
            var testRes = CheckTools.CheckProjectName(newTitle);
            if (testRes == CheckTools.ECheckProjectNameResult.Success)
            {
                _project.Name = newTitle;
                _needSave = true;
                _isEditTitle = false;
                RefreshTitleDock();
            }
            else
            {
                SocialGUIManager.ShowCheckProjectNameRes(testRes);
            }
        }

        private void OnEditDescBtn()
        {
            if (null == _project) return;
            _isEditDesc = true;
            RefreshDescDock();
            _cachedView.DescInput.text = _project.Summary;
            EventSystem.current.SetSelectedGameObject(_cachedView.DescInput.gameObject);
        }

        private void OnConfirmDescBtn()
        {
            string newDesc = _cachedView.DescInput.text;
            if (string.IsNullOrEmpty(newDesc) || newDesc == _project.Summary)
            {
                _isEditDesc = false;
                RefreshDescDock();
                return;
            }
            var testRes = CheckTools.CheckProjectDesc(newDesc);
            if (testRes == CheckTools.ECheckProjectSumaryResult.Success)
            {
                _project.Summary = newDesc;
                _needSave = true;
                _isEditDesc = false;
                RefreshDescDock();
            }
            else
            {
                SocialGUIManager.ShowCheckProjectDescRes(testRes);
            }
        }

        private void OnPersonalProjectDataChanged(Project p)
        {
            if (_isOpen && _project != null && p.ProjectId == _project.ProjectId)
            {
                RefreshView();
            }
        }
    }

    public enum EEditState
    {
        Editing,
        HasPassed,
        HasPublished
    }
}