using UnityEngine;
using System.Collections;
using SoyEngine;

namespace GameA {
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlScreenRotate : UICtrlGenericBase<UIViewScreenRotate>
    {
        #region 常量与字段
        private const int ShowFrameCount = 5;
        private const int FadeFrameCount = 10;
        private int _frameCount;
        private ScreenOrientation _screenOrientation;
        #endregion

        #region 属性

        #endregion

        #region 继承方法
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.ScreenRotateMask;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }


        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _frameCount=0;
            _cachedView.Bg.color = Color.white;
        }
        #endregion 继承方法

		[System.Obsolete]
        public void ChangeScreenOrientation(ScreenOrientation orientation)
        {
			return;
            LogHelper.Info("Begin ChangeScreenOrientation: " + orientation);
            if(!_isOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlScreenRotate>();
            }
            _screenOrientation = orientation;
            Screen.orientation = orientation;
            LogHelper.Info("UpdateEnd ChangeScreenOrientation: " + orientation);
        }

        public override void OnUpdate()
        {
            if(!_isOpen)
            {
                return;
            }
            base.OnUpdate();
            if(_frameCount < ShowFrameCount)
            {
                Screen.orientation = ScreenOrientation.AutoRotation;
                Screen.orientation = _screenOrientation;
            }
            else if(_frameCount < ShowFrameCount+FadeFrameCount)
            {
                Color c = Color.white;
                c.a =1f * ( ShowFrameCount + FadeFrameCount - _frameCount ) / FadeFrameCount;
                _cachedView.Bg.color = c;
            }
            else
            {
                SocialGUIManager.Instance.CloseUI<UICtrlScreenRotate>();
            }
            _frameCount++;
        }
    }
}