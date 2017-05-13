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
    public class UICtrlReward : UISocialCtrlBase<UIViewReward>
    {
        #region Fields
        /// <summary>
        /// 奖励界面关闭锁定的时间，打开界面后，这个时间内不能关闭
        /// </summary>
        private const float _closeTime = 1f;
        private float _closeTimer;
        #endregion

        #region Properties
        #endregion

        #region Methods
        protected override void OnOpen (object parameter)
        {
            base.OnOpen(parameter);
            _closeTimer = 1f;
            _cachedView.Tip.gameObject.SetActive (false);

//            RefreshView ();
        }
        
        protected override void OnClose() {
            
            base.OnClose ();
        }
        
        protected override void InitEventListener() {
            base.InitEventListener ();
        }
        
        protected override void OnViewCreated() {
            base.OnViewCreated ();

            _cachedView.BGBtn.onClick.AddListener (OnBGBtn);
//            _cachedView.PlayBtn.onClick.AddListener (OnPlayBtn);
//            _cachedView.AddTagBtn.onClick.AddListener (OnAddTagBtn);
        }

        public override void OnUpdate ()
        {
            base.OnUpdate ();
            _closeTimer -= Time.deltaTime;
            if (_closeTimer < 0 && _cachedView.Tip.gameObject.activeSelf == false) {
                _cachedView.Tip.gameObject.SetActive(true);
            }

            _cachedView.BgLight.transform.localRotation = Quaternion.Euler(0,0,Time.realtimeSinceStartup * 20f);
        }
        
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }

        private void RefreshView () {
//            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, _project.IconPath, _cachedView.DefaultCover);
//            _cachedView.ProjectTitle.text = _project.Name;
//            _cachedView.ProjectDesc.text = _project.Summary;
//            _cachedView.AuthorName.text = _project.UserInfo.NickName;
//            ImageResourceManager.Instance.SetDynamicImage(_cachedView.AuthorImg, 
//                _project.UserInfo.HeadImgUrl,
//                _cachedView.DefaultHeadImg);
//
//            _cachedView.ServedPlayerCnt.text = _project.ExtendData.PlayCount.ToString();
//            _cachedView.LikedPlayerCnt.text = _project.ExtendData.LikeCount.ToString();
//            _cachedView.PassRate.text = string.Format ("{0:F1}%", _project.PassRate);
        }


        private void OnBGBtn () {
            if (_closeTimer < 0) {
                SocialGUIManager.Instance.CloseUI<UICtrlReward> ();
            }
        }


        private void OnAddTagBtn () {
        }
        #endregion
    }
}
