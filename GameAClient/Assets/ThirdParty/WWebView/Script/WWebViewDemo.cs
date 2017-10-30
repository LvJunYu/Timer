// WWebViewPluginWin32.cs : WWebViewPluginWin32 implementation file
//
// Description      : WWebViewPluginWin32
// Author           : icodes (icodes.studio@gmail.com)
// Maintainer       : icodes
// License          : 
// Created          : 2017/07/12
// Last Update      : 2017/10/10
// Repository       : https://github.com/icodes-studio/WWebView
// Official         : http://www.icodes.studio/
//
// (C) ICODES STUDIO. All rights reserved. 
//

#if UNITY_STANDALONE_WIN || UNITY_WSA

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class WWebViewDemo : MonoBehaviour
{
    public Text status = null;
    public Text showText = null;
    public RawImage background = null;
    public Button javaScript = null;
    public Button goBack = null;
    public Button goForward = null;
    public int margine = 30;
    public string url = "https://www.baidu.com";
    public string userAgent = "User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0)";

    public string script =
        @"function Test()
        {
            var msg = 'Hello JavaScript';
            alert(msg);
            return msg;
        } Test();";

    public string html =
        @"<html><body>
        <p><font size='5'>Hello WWebView - This is simple code string</font></p><br>
        <p><a href='wwebview://openfile/'>wwebview://openfile/ - open local html file</a></p>
        <p><a href='wwebview://close/'>wwebview://close/</a></p>
        <p><a href='javascript:window.close();'>window.close() - works only on Win32 Webview</a></p>
        <p><a href='http://www.icodes.studio/'>http://www.icodes.studio/</a></p>
        <p><a href='https://www.microsoft.com/store/apps/9ph2smnbphnq'>Windows 10 sample</a></p>
        <p><a href='https://www.assetstore.unity3d.com/#!/content/97395'>Unity AssetStore</a></p>
        </body></html>";

    public string localHTML = "/demo.html";

    private bool showFlag = true;
    private bool initialize = false;

    private void Start()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        javaScript.interactable = false;
#endif
        Initialize();
    }

    public void Initialize()
    {
        if (initialize == false)
        {
#if UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.Init(gameObject.name, margine, 0, margine, 0);
#else
            WWebViewPlugin.Init(gameObject.name, margine, 0, margine, 0);
#endif
            initialize = true;
        }
    }

    public void Navigate()
    {
        Initialize();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        javaScript.interactable = true;
#endif

#if UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.Load(gameObject.name, url);
#else
        WWebViewPlugin.Load(gameObject.name, url);
#endif
        Show();
    }

    public void LoadHTML()
    {
        Initialize();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        javaScript.interactable = false;
#endif

#if UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.LoadHTMLString(gameObject.name, html, string.Empty);
#else
        WWebViewPlugin.LoadHTMLString(gameObject.name, html, string.Empty);
#endif
        Show();
    }

    public void NavigateLocalFile()
    {
        Initialize();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        javaScript.interactable = false;
#endif

        string localFile =
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            Application.streamingAssetsPath + localHTML;
#elif UNITY_WSA
            "ms-appx-web:///Data/StreamingAssets" + localHTML;
#else
            Debug.Assert(false);
#endif

#if UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.Load(gameObject.name, localFile);
#else
        WWebViewPlugin.Load(gameObject.name, localFile);
#endif
        Show();
    }

    public void Refresh()
    {
#if UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.Reload(gameObject.name);
#else
        WWebViewPlugin.Reload(gameObject.name);
#endif
        Show();
    }

    public void JavaScript()
    {
#if UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.EvaluatingJavaScript(gameObject.name, script);
#else
        WWebViewPlugin.EvaluatingJavaScript(gameObject.name, script);
#endif
    }

    public void Back()
    {
#if UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.GoBack(gameObject.name);
#else
        WWebViewPlugin.GoBack(gameObject.name);
#endif
        Show();
    }

    public void Forward()
    {
#if UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.GoForward(gameObject.name);
#else
        WWebViewPlugin.GoForward(gameObject.name);
#endif
        Show();
    }

    public void Stop()
    {
#if UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.Stop(gameObject.name);
#else
        WWebViewPlugin.Stop(gameObject.name);
#endif
        Show();
    }

    public void ShowToggle()
    {
        if (showFlag)
            Hide();
        else
            Show();
    }

    public void Show()
    {
#if UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.Show(gameObject.name, false, 0, 0f);
#else
        WWebViewPlugin.Show(gameObject.name, false, 0, 0f);
#endif
        showFlag = true;
        showText.text = "Hide";
    }

    public void Hide()
    {
#if UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.Hide(gameObject.name, false, 0, 0f);
#else
        WWebViewPlugin.Hide(gameObject.name, false, 0, 0f);
#endif
        showFlag = false;
        showText.text = "Show";
    }

    public void UserAgent()
    {
        Initialize();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        javaScript.interactable = true;
#endif

#if UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.SetUserAgent(userAgent);
        UniWebViewPlugin.Load(gameObject.name, "http://whatsmyuseragent.com");
#else
        WWebViewPlugin.SetUserAgent(userAgent);
        WWebViewPlugin.Load(gameObject.name, "http://whatsmyuseragent.com");
#endif
        Show();
    }

    public void Texturing()
    {
        Initialize();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        javaScript.interactable = true;
#endif

#if UNIWEBVIEW2_SUPPORTED
    #if (UNITY_STANDALONE_WIN || UNITY_WSA) && !UNITY_EDITOR
        Texture2D texture = new Texture2D(
            UniWebViewPlugin.GetActualWidth(gameObject.name),
            UniWebViewPlugin.GetActualHeight(gameObject.name),
            TextureFormat.ARGB32,
            false);

        texture.filterMode = FilterMode.Trilinear; texture.Apply();
        background.texture = texture;

        UniWebViewPlugin.Load(gameObject.name, "https://youtu.be/ctNF6QlLBWo");
        UniWebViewPlugin.SetTexture(gameObject.name, texture);
        StartCoroutine("OnRenderTexture", UniWebViewPlugin.GetRenderEventFunc());
    #else
        Debug.Assert(false);
    #endif
#else
        Texture2D texture = new Texture2D(
            WWebViewPlugin.GetActualWidth(gameObject.name),
            WWebViewPlugin.GetActualHeight(gameObject.name),
            TextureFormat.ARGB32,
            false);

        texture.filterMode = FilterMode.Trilinear; texture.Apply();
        background.texture = texture;

        WWebViewPlugin.Load(gameObject.name, "https://youtu.be/ctNF6QlLBWo");
        WWebViewPlugin.SetTexture(gameObject.name, texture);
        StartCoroutine("OnRenderTexture", WWebViewPlugin.GetRenderEventFunc());
#endif
        Hide();
    }

    private IEnumerator OnRenderTexture(IntPtr handler)
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            GL.IssuePluginEvent(handler, 0);
        }
    }

    public void Transparent()
    {
#if UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.TransparentBackground(gameObject.name, true);
        UniWebViewPlugin.SetAlpha(gameObject.name, 0.5f);
#else
        WWebViewPlugin.TransparentBackground(gameObject.name, true);
        WWebViewPlugin.SetAlpha(gameObject.name, 0.5f);
#endif
        Show();
    }

    public void LoadBegin(string url)
    {
        status.text = "LoadBegin : " + url;
    }

    public void LoadComplete(string url)
    {
#if UNIWEBVIEW2_SUPPORTED
        goBack.interactable = UniWebViewPlugin.CanGoBack(gameObject.name);
        goForward.interactable = UniWebViewPlugin.CanGoForward(gameObject.name);
#else
        goBack.interactable = WWebViewPlugin.CanGoBack(gameObject.name);
        goForward.interactable = WWebViewPlugin.CanGoForward(gameObject.name);
#endif
        status.text = "LoadComplete : " + url;
    }

    public void ReceivedMessage(string url)
    {
        if (url == "wwebview://openfile/")
            NavigateLocalFile();

        else if (url == "wwebview://loadstring/")
            LoadHTML();

        status.text = "ReceivedMessage : " + url;
    }

    public void EvalJavaScriptFinished(string result)
    {
        status.text = "EvalJavaScriptFinished : " + result;
    }

    public void WebViewDone(string url)
    {
#if UNITY_EDITOR
        status.text = "WebViewDone";
        initialize = false;
#else
        Application.Quit();
#endif
    }
}
#endif
