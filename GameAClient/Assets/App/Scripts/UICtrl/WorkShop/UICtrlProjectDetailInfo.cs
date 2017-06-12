using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlProjectDetailInfo : UISocialCtrlBase<UIViewProjectDetailInfo>
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
                SocialGUIManager.Instance.CloseUI<UICtrlProjectDetailInfo> ();
            }
            _project = parameter as Project;
            if (null == _project) {
                SocialGUIManager.Instance.CloseUI<UICtrlProjectDetailInfo> ();
            }

            RefreshView ();
        }
        
        protected override void OnClose() {
            
            base.OnClose ();
        }
        
        protected override void InitEventListener() {
            base.InitEventListener ();
        }
        
        protected override void OnViewCreated() {
            base.OnViewCreated ();

            _cachedView.CloseBtn.onClick.AddListener (OnCloseBtn);
            _cachedView.PlayBtn.onClick.AddListener (OnPlayBtn);
            _cachedView.AddTagBtn.onClick.AddListener (OnAddTagBtn);
        }
        
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }

        private void RefreshView () {
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, _project.IconPath, _cachedView.DefaultCover);
            _cachedView.ProjectTitle.text = _project.Name;
            _cachedView.ProjectDesc.text = _project.Summary;
            _cachedView.AuthorName.text = _project.UserInfo.NickName;
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.AuthorImg, 
                _project.UserInfo.HeadImgUrl,
                _cachedView.DefaultHeadImg);

            _cachedView.ServedPlayerCnt.text = _project.PlayCount.ToString();
            _cachedView.LikedPlayerCnt.text = _project.LikeCount.ToString();
            _cachedView.PassRate.text = string.Format ("{0:F1}%", _project.CompleteRate);
        }

        private void OnPlayBtn () {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "作品加载中");
            _project.PrepareRes(()=>{
                _project.RequestPlay(()=>{
                    MatrixProjectTools.OnProjectBeginPlaySuccess();
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    GameManager.Instance.RequestPlay(_project);
                    SocialGUIManager.Instance.ChangeToGameMode();
                }, code=>{
                    //                    if(code == EPlayProjectRetCode.PPRC_ProjectHasBeenDeleted)
                    //                    {
                    //                        CommonTools.ShowPopupDialog("作品已被删除，启动失败");
                    //                    }
                    //                    else if(code == EPlayProjectRetCode.PPRC_FrequencyTooHigh)
                    //                    {
                    //                        CommonTools.ShowPopupDialog("启动过于频繁，启动失败");
                    //                    }
                    //                    else
                    //                    {
                    //                        if(Application.internetReachability == NetworkReachability.NotReachable)
                    //                        {
                    //                            CommonTools.ShowPopupDialog("启动失败，请检查网络环境");
                    //                        }
                    //                        else
                    //                        {
                    //                            CommonTools.ShowPopupDialog("启动失败，未知错误");
                    //                        }
                    //                    } 
                    //                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(SocialGUIManager.Instance.GetUI<UICtrlProjectDetail>());
                });
            }, ()=>{
                LogHelper.Error("Project OnPlayClick, Project GetRes Error");
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                CommonTools.ShowPopupDialog("作品加载失败，请检查网络");
            });
        }

        private void OnCloseBtn () {
            SocialGUIManager.Instance.CloseUI<UICtrlProjectDetailInfo> ();
        }


        private void OnAddTagBtn () {
        }
        #endregion
    }
}
