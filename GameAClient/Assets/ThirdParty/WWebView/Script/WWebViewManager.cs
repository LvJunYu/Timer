using System;
using System.Collections;
using GameA;
using GameA.Game;
using UnityEngine;

public sealed class WWebViewManager : MonoBehaviour
{
    private static WWebViewManager _instance;

    public static WWebViewManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("WWebViewDemo").AddComponent<WWebViewManager>();
//                _instance.Initialize();
            }
            return _instance;
        }
    }

    public int Width = 690;
    public int Height = 600;
    private int _top = 30;
    private int _bottom = 30;
    private int _left = 30;
    private int _right = 30;
    private string _urlFormat = "http://minigame.qq.com/plat/social_hall/app_frame/?appid=1106419259&param={0}";
    private string userAgent = "User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0)";

    private string script =
        @"function Test()
        {
            var msg = 'Hello JavaScript';
            alert(msg);
            return msg;
        } Test();";

    private string html =
        @"<html><body>
        <p><font size='5'>Hello WWebView - This is simple code string</font></p><br>
        <p><a href='wwebview://openfile/'>wwebview://openfile/ - open local html file</a></p>
        <p><a href='wwebview://close/'>wwebview://close/</a></p>
        <p><a href='javascript:window.close();'>window.close() - works only on Win32 Webview</a></p>
        <p><a href='http://www.icodes.studio/'>http://www.icodes.studio/</a></p>
        <p><a href='https://www.microsoft.com/store/apps/9ph2smnbphnq'>Windows 10 sample</a></p>
        <p><a href='https://www.assetstore.unity3d.com/#!/content/97395'>Unity AssetStore</a></p>
        </body></html>";

    private string localHTML = "/demo.html";

    private bool showFlag = true;
    private bool initialize;

    public void Initialize()
    {
        if (initialize == false)
        {
#if UNIWEBVIEW3_SUPPORTED
            UniWebViewInterface.Init(gameObject.name, 0, margine, Screen.width, Screen.height - (margine * 2));
#elif UNIWEBVIEW2_SUPPORTED
            UniWebViewPlugin.Init(gameObject.name, margine, 0, margine, 0);
#else
            _top = _bottom = (ScreenResolutionManager.Instance.CurRealResolution.height - Height) / 2;
            _left = _right = (ScreenResolutionManager.Instance.CurRealResolution.width - Width) / 2;
//            _top = 30;
            WWebViewPlugin.Init(gameObject.name, _top, _left, _bottom, _right);
#endif
            initialize = true;
        }
    }

//    public Vector2 GetRect()
//    {
//        
//    }

    public void Open(ERequestType eRequestType, int itemId = 0, int itemCount = 0)
    {
        string param = string.Empty;
        if (eRequestType == ERequestType.OpenBlueVip)
        {
            param = ((int) ERequestType.OpenBlueVip).ToString();
        }
        else if (eRequestType == ERequestType.BuyItem)
        {
            param = string.Format("{0}|{1}|{2}", (int) eRequestType, itemId, itemCount);
        }
        Navigate(string.Format(_urlFormat, param));
        SocialGUIManager.Instance.OpenUI<UICtrlWWebView>();
    }

    public void Navigate(string url)
    {
        Initialize();

#if UNIWEBVIEW3_SUPPORTED
        UniWebViewInterface.Load(gameObject.name, url, false);
#elif UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.Load(gameObject.name, url);
#else
        WWebViewPlugin.Load(gameObject.name, url);
#endif
        Show();
    }

    public void LoadHTML()
    {
        Initialize();

#if UNIWEBVIEW3_SUPPORTED
        UniWebViewInterface.LoadHTMLString(gameObject.name, html, string.Empty, false);
#elif UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.LoadHTMLString(gameObject.name, html, string.Empty);
#else
        WWebViewPlugin.LoadHTMLString(gameObject.name, html, string.Empty);
#endif
        Show();
    }

    public void NavigateLocalFile()
    {
        Initialize();

        string localFile =
#if UNITY_EDITOR
            Application.streamingAssetsPath + localHTML;
#elif UNITY_STANDALONE_WIN
        Application.streamingAssetsPath + localHTML;
#elif UNITY_WSA
        "ms-appx-web:///Data/StreamingAssets" + localHTML;
#elif UNITY_IOS
        Application.streamingAssetsPath + localHTML;
#elif UNITY_ANDROID
        "file:///android_asset" + localHTML;
#else
        string.Empty;
#endif

#if UNIWEBVIEW3_SUPPORTED
        UniWebViewInterface.Load(gameObject.name, localFile, false);
#elif UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.Load(gameObject.name, localFile);
#else
        WWebViewPlugin.Load(gameObject.name, localFile);
#endif
        Show();
    }

    public void Refresh()
    {
#if UNIWEBVIEW3_SUPPORTED
        UniWebViewInterface.Reload(gameObject.name);
#elif UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.Reload(gameObject.name);
#else
        WWebViewPlugin.Reload(gameObject.name);
#endif
        Show();
    }

    public void JavaScript()
    {
#if UNIWEBVIEW3_SUPPORTED
        UniWebViewInterface.EvaluateJavaScript(gameObject.name, script, string.Empty);
#elif UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.EvaluatingJavaScript(gameObject.name, script);
#else
        WWebViewPlugin.EvaluatingJavaScript(gameObject.name, script);
#endif
    }

    public void Back()
    {
#if UNIWEBVIEW3_SUPPORTED
        UniWebViewInterface.GoBack(gameObject.name);
#elif UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.GoBack(gameObject.name);
#else
        WWebViewPlugin.GoBack(gameObject.name);
#endif
        Show();
    }

    public void Forward()
    {
#if UNIWEBVIEW3_SUPPORTED
        UniWebViewInterface.GoForward(gameObject.name);
#elif UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.GoForward(gameObject.name);
#else
        WWebViewPlugin.GoForward(gameObject.name);
#endif
        Show();
    }

    public void Stop()
    {
#if UNIWEBVIEW3_SUPPORTED
        UniWebViewInterface.Stop(gameObject.name);
#elif UNIWEBVIEW2_SUPPORTED
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
#if UNIWEBVIEW3_SUPPORTED
        UniWebViewInterface.Show(gameObject.name, false, 0, 0f, string.Empty);
#elif UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.Show(gameObject.name, false, 0, 0f);
#else
        WWebViewPlugin.Show(gameObject.name, false, 0, 0f);
#endif
        showFlag = true;
//        showText.text = "Hide";
    }

    public void Hide()
    {
#if UNIWEBVIEW3_SUPPORTED
        UniWebViewInterface.Hide(gameObject.name, false, 0, 0f, string.Empty);
#elif UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.Hide(gameObject.name, false, 0, 0f);
#else
        WWebViewPlugin.Hide(gameObject.name, false, 0, 0f);
#endif
        showFlag = false;
//        showText.text = "Show";
    }

    public void Destroy()
    {
        WWebViewPlugin.Destroy(gameObject.name);
        initialize = false;
    }

    public void UserAgent()
    {
        Initialize();

#if UNIWEBVIEW3_SUPPORTED
        UniWebViewInterface.SetUserAgent(gameObject.name, userAgent);
        UniWebViewInterface.Load(gameObject.name, "http://whatsmyuseragent.com", false);
#elif UNIWEBVIEW2_SUPPORTED
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

#if UNIWEBVIEW3_SUPPORTED
        #if UNITY_EDITOR_WIN || ((UNITY_STANDALONE_WIN || UNITY_WSA) && !UNITY_EDITOR)
        Texture2D texture = new Texture2D(
            UniWebViewInterface.GetActualWidth(gameObject.name),
            UniWebViewInterface.GetActualHeight(gameObject.name),
            TextureFormat.ARGB32, false);

        texture.filterMode = FilterMode.Trilinear; texture.Apply();
        background.texture = texture;

        UniWebViewInterface.Load(gameObject.name, "https://youtu.be/ctNF6QlLBWo", false);
        UniWebViewInterface.SetTexture(gameObject.name, texture);
        StartCoroutine("OnRenderTexture", UniWebViewInterface.GetRenderEventFunc());
        #else
        Debug.Assert(false);
        #endif
#elif UNIWEBVIEW2_SUPPORTED
        #if UNITY_EDITOR_WIN || ((UNITY_STANDALONE_WIN || UNITY_WSA) && !UNITY_EDITOR)
        Texture2D texture = new Texture2D(
            UniWebViewPlugin.GetActualWidth(gameObject.name),
            UniWebViewPlugin.GetActualHeight(gameObject.name),
            TextureFormat.ARGB32, false);

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
            TextureFormat.ARGB32, false);

        texture.filterMode = FilterMode.Trilinear;
        texture.Apply();
//        background.texture = texture;

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
#if UNIWEBVIEW3_SUPPORTED
        UniWebViewInterface.SetWebViewAlpha(gameObject.name, 0.5f);
#elif UNIWEBVIEW2_SUPPORTED
        UniWebViewPlugin.SetAlpha(gameObject.name, 0.5f);
#else
        WWebViewPlugin.SetAlpha(gameObject.name, 0.5f);
#endif
        Show();
    }

    public void UniWebView2()
    {
        Navigate("http://www.icodes.studio/2017/10/how-to-integrate-with-uniwebview2.html");
    }

    public void UniWebView3()
    {
        Navigate("http://www.icodes.studio/2017/10/how-to-integrate-with-uniwebview3.html");
    }

    public void ReleaseNotes()
    {
        Navigate("http://www.icodes.studio/2017/10/wwebview-release-notes.html");
    }

    public void BugReporting()
    {
        Navigate("http://www.icodes.studio/2017/10/wwebview-bug-reporting.html");
    }

#if UNIWEBVIEW3_SUPPORTED
    public void PageStarted(string url)
#else
    public void LoadBegin(string url)
#endif
    {
//        status.text = "LoadBegin : " + url;
    }

#if UNIWEBVIEW3_SUPPORTED
    public void PageFinished(string url)
#else
    public void LoadComplete(string url)
#endif
    {
#if UNIWEBVIEW3_SUPPORTED
        goBack.interactable = UniWebViewInterface.CanGoBack(gameObject.name);
        goForward.interactable = UniWebViewInterface.CanGoForward(gameObject.name);
#elif UNIWEBVIEW2_SUPPORTED
        goBack.interactable = UniWebViewPlugin.CanGoBack(gameObject.name);
        goForward.interactable = UniWebViewPlugin.CanGoForward(gameObject.name);
#else
#endif
//        status.text = "LoadComplete : " + url;
    }

#if UNIWEBVIEW3_SUPPORTED
    public void MessageReceived(string url)
#else
    public void ReceivedMessage(string url)
#endif
    {
        if (url == "wwebview://openfile/")
            NavigateLocalFile();

        else if (url == "wwebview://loadstring/")
            LoadHTML();

//        status.text = "ReceivedMessage : " + url;
    }

    public void EvalJavaScriptFinished(string result)
    {
//        status.text = "EvalJavaScriptFinished : " + result;
    }

    public void WebViewDone(string url)
    {
#if UNITY_EDITOR
//        status.text = "WebViewDone";
        initialize = false;
#else
//        Application.Quit();
#endif
        SocialGUIManager.Instance.CloseUI<UICtrlWWebView>();
    }

#if UNIWEBVIEW2_SUPPORTED
    public void ShowTransitionFinished(string message)
    {
        // N/A
    }
#endif
}

public enum ERequestType
{
    OpenBlueVip,
    BuyItem
}