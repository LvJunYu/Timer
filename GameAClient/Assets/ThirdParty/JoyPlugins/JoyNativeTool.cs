using UnityEngine;
using System.Collections;
using System;
using System.IO;
using SoyEngine;
using GameA;

public class JoyNativeTool:IJoyNativeTool
{
    public static Action<Texture2D, string> OnImagePicked;
    private static IJoyNativeTool _instance;
    private static long _userGuid;
    private TextEditor _textEditor = new TextEditor();

    public static IJoyNativeTool Instance
    {
        get
        {
            if (_instance == null)
            {
                if (Application.isEditor)
                {
                    _instance = new JoyNativeTool();
                }
                else
                {
#if UNITY_IOS
                    _instance = JoyNativeTooliOS.Instance;
#elif UNITY_ANDROID
                    _instance = JoyNativeToolAndroid.Instance;
#else
                    _instance = new JoyNativeTool();
#endif
                }
                if(LocalUser.Instance.Account.HasLogin)
                {
                    _userGuid = LocalUser.Instance.UserGuid;
                }
                Messenger.AddListener(SoyEngine.EMessengerType.OnAccountLoginStateChanged, OnAccountStateChanged);
            }
            return _instance;
        }
    }

    public void Init()
    {
    }

    public void AddPushAlias(string alias, string type)
    {
    }

    public void SetPushAlias(string alias, string type)
    {
    }

    public void RemovePushAlias(string alias, string type)
    {
    }

    public void SetStatusBarShow(bool show)
    {
    }

    public void PickImage()
    {
    }

    public string GetCustomNotificationField()
    {
        return string.Empty;
    }

    public void CopyTextToClipboard(string text)
                    {
                    _textEditor.text = text;
                    _textEditor.SelectAll();
                    _textEditor.Copy();
    }

    public string GetTextFromClipboard()
                    {
                    _textEditor.text = string.Empty;
                    _textEditor.Paste();
                    return _textEditor.text;
    }


    public static Texture2D LoadImageFromFile(string path)
    {
        if (path == "Cancelled")
            return null;
        byte[] bytes;
        var texture = new Texture2D(128, 128, TextureFormat.RGB24, false);

        bytes = File.ReadAllBytes(path);
        texture.LoadImage(bytes);

        return texture;
    }

    private static void OnAccountStateChanged()
    {
        if(LocalUser.Instance.Account.HasLogin)
        {
            _userGuid = LocalUser.Instance.UserGuid;
            Instance.SetPushAlias(_userGuid.ToString(), SoyConstDefine.PushAliasAccountGuid);
        }
        else
        {
            if(_userGuid != 0)
            {
                Instance.RemovePushAlias(_userGuid.ToString(), SoyConstDefine.PushAliasAccountGuid);
            }
            else
            {
                LogHelper.Warning("OnLogout, userGuid is 0");
            }
        }
    }
}


public interface IJoyNativeTool
{
    void Init();
    void AddPushAlias(string alias, string type);
    void SetPushAlias(string alias, string type);
    void RemovePushAlias(string alias, string type);
    void SetStatusBarShow(bool show);
    void PickImage();
    string GetCustomNotificationField();
    void CopyTextToClipboard(string text);
    string GetTextFromClipboard();
}