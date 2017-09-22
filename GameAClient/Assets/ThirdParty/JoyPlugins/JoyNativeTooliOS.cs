/********************************************************************
** Filename : JoyNativeTooliOS.cs
** Author : quan
** Date : 2016/8/11 20:23
** Summary : JoyNativeTooliOS.cs
***********************************************************************/


#if UNITY_IOS

using System;
using System.IO;
using System.Text;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using EMessengerType = GameA.EMessengerType;
using NotificationServices = UnityEngine.iOS.NotificationServices;
using RemoteNotification = UnityEngine.iOS.RemoteNotification;
//using Umeng;

public class JoyNativeTooliOS : MonoBehaviour, IJoyNativeTool
{
    private static JoyNativeTooliOS _instance;
    public static JoyNativeTooliOS Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject {name = "JoyNativeTool"};
                _instance = go.AddComponent<JoyNativeTooliOS>();
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
        LogHelper.Info("JoyNativeTooliOS init");
    }

    public void Init()
    {
//        UMPushiOS.setAutoAlert(false);
//        UMPushiOS.setBadgeClear(true);
//        UMPushiOS.setLogEnabled(GlobalVar.Instance.IsDebug);
//
//        string appkey = "5779c90d67e58e60150010c6";  
//        GA.StartWithAppKeyAndChannelId(appkey, "App Store"); 
//        GA.SetLogEnabled(GlobalVar.Instance.IsDebug);
    }

    private void AliasHandler(string response,string error)
    {
        LogHelper.Info("response:\n"+response+" result:\n"+ error!=null?"OK":"Failed!");
    }

    public void AddPushAlias(string alias, string type)
    {
//        UMPushiOS.addAlias(alias, type, AliasHandler);
    }

    public void SetPushAlias(string alias, string type)
    {
//        UMPushiOS.setAlias(alias, type, AliasHandler);
    }

    public void RemovePushAlias(string alias, string type)
    {
//        UMPushiOS.removeAlias(alias, type, AliasHandler);
    }

    public void SetStatusBarShow(bool show)
    {
//        _setStatusBarShow(show);
    }

    public void PickImage()
    {
//        NativeToolkit.OnImagePicked = JoyNativeTool.OnImagePicked;
//        NativeToolkit.PickImage();
    }

    public string GetCustomNotificationField()
    {
        LogHelper.Info("RemoteNotificationCount: " + NotificationServices.remoteNotificationCount);
        StringBuilder sb = new StringBuilder();
        for(int i=0; i<NotificationServices.remoteNotificationCount; i++)
        {
            RemoteNotification rn = NotificationServices.GetRemoteNotification(i);
            string custom = null;
            if(rn.userInfo != null)
            {
                custom = rn.userInfo["custom"].ToString();
            }
            if(!string.IsNullOrEmpty(custom))
            {
                sb.Append(custom+"|");
            }
            LogHelper.Info("RemoteNotification, id: {0}, content: {1}, custom: {2}", i, rn.alertBody, custom);
        }
        NotificationServices.ClearRemoteNotifications();
        if(sb.Length > 0)
        {
            sb.Length --;
        }
        return sb.ToString();;
    }

//    [DllImport("__Internal")]
//    private static extern void _setStatusBarShow(bool show);

    public void OnReceiveRemoteNotification(string param)
    {
        LogHelper.Debug("OnReceiveRemoteNotification");
        MessengerAsync.Broadcast(EMessengerType.OnReceiveRemoteNotification);
    }

//    [DllImport("__Internal")]
//    private static extern void _copyTextToClipboard(string text);

    public void CopyTextToClipboard(string text)
    {
//        _copyTextToClipboard(text);
    }

//    [DllImport("__Internal")]
//    private static extern string _getTextFromClipboard();

    public string GetTextFromClipboard()
    {
//        return _getTextFromClipboard();
        return string.Empty;
    }
    
    public bool TryGetFromStreamingAssets(string fileName, out byte[] bytes)
    {
        string fileFullName = Path.Combine(ResPath.StreamingAssetsPath, fileName);
        bytes = null;
        if (!File.Exists(fileFullName)) {
            return false;
        }

        bool success;
        FileStream fs = null;
        try
        {
            fs = new FileStream(fileFullName, FileMode.Open, FileAccess.Read);
            bytes = new byte[fs.Length];
            fs.Read (bytes, 0, (int)fs.Length);
            success = true;
        }
        catch (Exception e)
        {
            LogHelper.Error(e.ToString());
            success = false;
        }
        finally
        {
            if (fs != null)
            {
                fs.Close();
            }
        }
        return success;
    }
}
#endif