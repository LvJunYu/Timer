 /********************************************************************
 ** Filename : UIViewTitlebar.cs
 ** Author : quansiwei
 ** Date : 2015/5/6 21:05
 ** Summary : 标题栏
 ***********************************************************************/


using System;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;
using UnityEngine;

namespace GameA
{
    public class UIViewGMTool : UIViewResManagedBase
    {
        public Button SwitchBtn;
        public Button EnterBtn;
        public InputField InputField;
        public Image SucceedImg;
        public Image FailedImg;
        public GameObject InputObj;
        public Text Version;

        protected override void Start ()
        {
            Version.text = SocialApp.Instance.Env.ToString ().Substring (0, 3);
        }
    }
}
