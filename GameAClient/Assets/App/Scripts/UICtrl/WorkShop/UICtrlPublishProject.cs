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
    public class UICtrlPublishProject : UISocialCtrlBase<UIViewPublishProject>
    {
        #region Fields
        private Project _project;
        #endregion

        #region Properties
        #endregion

        #region Methods
        protected override void OnOpen (object parameter)
        {
            base.OnOpen(parameter);

            if (null == parameter) {
                SocialGUIManager.Instance.CloseUI<UICtrlPublishProject> ();
            }
            _project = parameter as Project;
            if (null == _project) {
                SocialGUIManager.Instance.CloseUI<UICtrlPublishProject> ();
            }

            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, _project.IconPath, _cachedView.DefaultCover);
            _cachedView.ProjectTitle.text = _project.Name;
            _cachedView.ProjectDesc.text = _project.Summary;
        }
        
        protected override void OnClose() {
            
            base.OnClose ();
        }
        
        protected override void InitEventListener() {
            base.InitEventListener ();
        }
        
        protected override void OnViewCreated() {
            base.OnViewCreated ();

            _cachedView.OKBtn.onClick.AddListener (OnOKBtn);
            _cachedView.CancelBtn.onClick.AddListener (OnCancelBtn);
        }
        
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }

        private void OnOKBtn () {
            SocialGUIManager.Instance.CloseUI<UICtrlPublishProject> ();
            if (null == _project)
                return;
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在发布");
            RemoteCommands.PublishWorldProject (
                _project.ProjectId,
                _project.Name,
                _project.Summary,
                _project.ProgramVersion,
                _project.ResourcesVersion,
                _project.RecordUsedTime,
                _project.TimeLimit,
                _project.WinCondition,
                msg => {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    CommonTools.ShowPopupDialog("发布关卡成功");
                },
                code => {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    CommonTools.ShowPopupDialog("发布关卡失败，错误代码 " + code.ToString());
                }, null
            );
        }

        private void OnCancelBtn () {
            SocialGUIManager.Instance.CloseUI<UICtrlPublishProject> ();
        }
        #endregion
    }
}
