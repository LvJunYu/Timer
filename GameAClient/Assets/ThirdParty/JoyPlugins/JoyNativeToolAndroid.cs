  /********************************************************************
  ** Filename : JoyNativeToolAndroid.cs
  ** Author : quan
  ** Date : 2016/6/23 16:42
  ** Summary : JoyNativeToolAndroid.cs
  ***********************************************************************/
#if UNITY_ANDROID

using System;
using UnityEngine;
using SoyEngine;
using Umeng;


public class JoyNativeToolAndroid: MonoBehaviour, IJoyNativeTool
{
    private static AndroidJavaClass _androidObj;
    private static JoyNativeToolAndroid _instance;
    public static JoyNativeToolAndroid Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject {name = "JoyNativeTool"};
                _instance = go.AddComponent<JoyNativeToolAndroid>();
                _androidObj = new AndroidJavaClass("com.GameLife.Soy.JoyNativeTool.JoyUnityPlayerNativeActivity");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        LogHelper.Info("JoyNativeToolAndroid init");
    }

    public void Init()
    {
//        UMPushAndroid.enable();
//        UMPushAndroid.onAppStart();
//        UMPushAndroid.setDebugMode(GlobalVar.Instance.IsDebug);
        LogHelper.Info("Device Token:"+UMPushAndroid.getRegistrationId());
        string appkey = "5779c8d167e58e5d37003c26";  
        GA.StartWithAppKeyAndChannelId(appkey, "default");  
        GA.SetLogEnabled(GlobalVar.Instance.IsDebug); 
    }

    public void AddPushAlias(string alias, string type)
    {
//        bool result = UMPushAndroid.addAlias(alias, type);
//        LogHelper.Info("AddPushAlias, result: " + result);
    }

    public void SetPushAlias(string alias, string type)
    {
//        bool result = UMPushAndroid.addExclusiveAlias(alias, type);
//        LogHelper.Info("SetPushAlias, result: " + result);
    }

    public void RemovePushAlias(string alias, string type)
    {
//            bool result = UMPushAndroid.removeAlias(alias, type);
//            LogHelper.Info("RemovePushAlias, result: " + result);
    }

    public void SetStatusBarShow(bool show)
    {
        if(show)
        {
            ApplicationChrome.statusBarState = ApplicationChrome.States.TranslucentOverContent;
            ApplicationChrome.navigationBarState = ApplicationChrome.States.Hidden;
        }
        else 
        {
            ApplicationChrome.statusBarState = ApplicationChrome.States.Hidden;
            ApplicationChrome.navigationBarState = ApplicationChrome.States.Hidden;
        }
    }


    public void PickImage()
    {
//        _androidObj.CallStatic("pickImageFromGallery");
    }

    public void OnPickImage(string path)
    {
//        Texture2D texture = JoyNativeTool.LoadImageFromFile(path);
//        if (JoyNativeTool.OnImagePicked != null)
//        {
//            JoyNativeTool.OnImagePicked(texture, path);
//        }
    }
    public string GetCustomNotificationField()
    {
        return string.Empty;
    }

    public void CopyTextToClipboard(string text)
    {
//        _androidObj.CallStatic("copyTextToClipboard", text);
    }

    public string GetTextFromClipboard()
    {
        return string.Empty;
//        return _androidObj.CallStatic<String>("getTextFromClipboard");
    }
}
#endif