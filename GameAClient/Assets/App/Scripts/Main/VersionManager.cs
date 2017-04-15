  /********************************************************************
  ** Filename : VersionManager.cs
  ** Author : quan
  ** Date : 9/28/2016 4:34 PM
  ** Summary : 
  *版本规则：1: a.b.c 主版本号.次版本号.修订版本号
  *                 主版本号在功能重大升级时变化
  *                 次版本号在添加小功能时变化，通常这些功能和上一版本不兼容
  *                 修订版本号在修bug时变化，仅修改修订版本号，互相应该是兼容的
  *               2: 资源版本号 int整型
  *                 如果软件放出时资源有变化可以简单由abbccdd生成整型版本号
  *                 abc为软件版本号，dd为软件不变化但资源变化时的资源修订版本
  *                 如果软件更新资源没变化，资源继承上一版本号
  *               3: 创作工具程序版本号 int整型
  *                 在新程序做地图不兼容旧的程序时必须变化，同时软件至少次版本号需要变化
  *               4: 创作工具地图版本号 int整型
  *                 仅用于辅助地图解析
  ***********************************************************************/
using System;
using UnityEngine;
using SoyEngine;

namespace GameA
{
    public class VersionManager
    {
        public readonly static VersionManager Instance = new VersionManager();
        private bool _hasInit = false;

        public bool HasNewDownload()
        {
            if(!_hasInit)
            {
                return false;
            }
            string curVersion = GlobalVar.Instance.AppVersion;
            string newVersion = AppData.Instance.NewestAppVersion;
            return curVersion != newVersion;
        }

        public void Init()
        {
            if(_hasInit)
            {
                return;
            }
            _hasInit = true;
            Messenger.AddListener(EMessengerType.OnAppDataChanged, OnAppDataChanged);
        }


        private void OnAppDataChanged()
        {
            CheckUpdate();
        }

        private void CheckUpdate()
        {
            if(!AppData.Instance.HasInit)
            {
                return;
            }
            if(string.IsNullOrEmpty(AppData.Instance.NewestAppVersion))
            {
                return;
            }

            string curVersion = GlobalVar.Instance.AppVersion;
            string newVersion = AppData.Instance.NewestAppVersion;
            LogHelper.Info("CheckUpdate, CurVersion: {0}, NewestVersion: {1}", curVersion, newVersion);
            if(newVersion != curVersion)
            {
                ShowUpdateTip();
            }
        }

        public void ShowUpdateTip()
        {
            CommonTools.ShowPopupDialog("匠游新版本已经发布啦，请更新最新版本以获得更好的体验", "更新提示",
                new System.Collections.Generic.KeyValuePair<string, Action>("更新", ()=>{
                    GoToUpdate();
                }), 
                new System.Collections.Generic.KeyValuePair<string, Action>("取消", ()=>{

                }));
        }


        public void GoToUpdate()
        {
            Application.OpenURL("http://a.app.qq.com/o/simple.jsp?pkgname=com.GameLife.Soy");
        }
    }
}

