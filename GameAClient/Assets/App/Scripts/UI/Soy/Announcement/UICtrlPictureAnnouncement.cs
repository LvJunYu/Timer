  /********************************************************************
  ** Filename : UICtrlPictureAnnouncement.cs
  ** Author : quan
  ** Date : 2016/6/12 18:19
  ** Summary : UICtrlPictureAnnouncement.cs
  ***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlPictureAnnouncement : UISocialContentCtrlBase<UIViewPictureAnnouncement>, IUIWithTitle
    {
        #region 常量与字段
        private string _currentUrl;
        private List<ImageItem> _imageList = new List<ImageItem>();
        #endregion

        #region 属性

        #endregion

        #region 方法
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
        }


        protected override void OnOpen(object parameter)
        {
            string path = parameter as string;
            if(path == null)
            {
                LogHelper.Error("UICtrlPictureAnnouncement Open error, parameter is null");
                return;
            }
            base.OnOpen(parameter);
            if(_currentUrl != path)
            {
                _currentUrl = path;
                ScrollToTop();
                RefreshView();
            }
        }

        private void RefreshView()
        {
            if(string.IsNullOrEmpty(_currentUrl))
            {
                SetImageList(0);
                return;
            }
            string[] imageAry = _currentUrl.Split('|');
            SetImageList(imageAry.Length);
            for(int i=0; i<imageAry.Length; i++)
            {
                ImageItem item = _imageList[i];
                ImageResourceManager.Instance.SetDynamicImage(item.RawImage, imageAry[i], t=>{
                    float tWidth = Mathf.Max(t.width, 1);
                    float tHeight = Mathf.Max(t.height, 2);
                    float factor = UIConstDefine.UINormalScreenWidth/tWidth;
                    float height = factor * tHeight;
                    item.LayoutElement.preferredHeight = item.LayoutElement.minHeight = height;
                });
            }
        }

        private void SetImageList(int count)
        {
            for(int i=0, len = Mathf.Min(count, _imageList.Count); i<len; i++)
            {
                _imageList[i].RawImage.gameObject.SetActive(true);
            }
            for(int i=count, len = _imageList.Count; i<len; i++)
            {
                _imageList[i].RawImage.gameObject.SetActive(false);
            }
            for(int i=_imageList.Count, len=count; i<len; i++)
            {
                ImageItem item = new ImageItem();
                item.RawImage = UGUITools.AddUIChildTo<RawImage>(_cachedView.Dock);
                GameObject go = item.RawImage.gameObject;
                go.name = ""+i;
                item.LayoutElement = go.AddComponent<LayoutElement>();
                item.LayoutElement.minWidth = item.LayoutElement.preferredWidth = UIConstDefine.UINormalScreenWidth;
                _imageList.Add(item);
            }
        }

        protected override void OnDestroy()
        {
        }

        #endregion

        #region 事件处理

        #endregion 事件处理

        #region 接口
        public object GetTitle()
        {
            return "公告";
        }
        #endregion

        private struct ImageItem
        {
            public RawImage RawImage;
            public LayoutElement LayoutElement;
        }
    }
}

