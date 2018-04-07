 /********************************************************************
 ** Filename : SocialUIConfig.cs
 ** Author : quansiwei
 ** Date : 2015/5/8 21:41
 ** Summary : SocialUIConfig.cs
 ***********************************************************************/



using SoyEngine;
using UnityEngine.UI;
using UnityEngine;
/*meiyuoyong*/
/*社交界面的配置*/
namespace GameA
{
    public static class SocialUIConfig//todo
    {
        #region 变量
        private static bool _hasInit = false;
        private static int _systemStatusBarHeight = 40;
        private static int _titleHeight = 128;
        private static int _taskBarHeight = 98;

        public readonly static Color FollowBtnBgColor = new Color(255f/255, 126f/255, 126f/255);
        public readonly static Color FollowBtnLabelColor = new Color(255f/255, 126f/255, 126f/255);
        public readonly static Color UnfollowBtnBgColor = new Color(100f/255, 100f/255, 100f/255);
        public readonly static Color UnfollowBtnLabelColor = new Color(100f/255, 100f/255, 100f/255);

        #endregion

        #region 属性
        public static int SystemStatusBarHeight
        {
            get
            {
                return _systemStatusBarHeight;
            }
            set
            {
                _systemStatusBarHeight = value;
            }
        }

        public static int TitleHeight
        {
            get
            {
                return _titleHeight;
            }
            set
            {
                _titleHeight = value;
            }
        }

        public static int TaskBarHeight
        {
            get
            {
                return _taskBarHeight;
            }
            set
            {
                _taskBarHeight = value;
            }
        }
        #endregion

        #region 方法
        public static void Init()
        {
            if(_hasInit)
            {
                return;
            }
            LogHelper.Info("Screen Dpi: {0}, Width: {1}, Height: {2}", Screen.dpi, Screen.width, Screen.height);
            _hasInit = true;
            if(!Application.isEditor)
            {
                float screenScale = Screen.dpi/320;
                var defaultTaskBarHeight = _taskBarHeight;
                _taskBarHeight =(int) (_taskBarHeight  * screenScale * UIConstDefine.UINormalScreenWidth/Screen.width);
                _taskBarHeight = Mathf.Clamp(_taskBarHeight, defaultTaskBarHeight*3/4, defaultTaskBarHeight);
                var defaultTitleHeight = _titleHeight;
                _titleHeight = (int)(_titleHeight * screenScale * UIConstDefine.UINormalScreenWidth/Screen.width);
                _titleHeight = Mathf.Clamp(_titleHeight, defaultTitleHeight*3/4, defaultTitleHeight);
            }
        }
        #endregion
    }
}