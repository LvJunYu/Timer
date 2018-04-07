// WWebViewPluginWSA.cs : WWebViewPluginWSA implementation file
//
// Description      : WWebViewPluginWSA
// Author           : icodes (icodes.studio@gmail.com)
// Maintainer       : icodes
// License          : 
// Created          : 2017/07/12
// Last Update      : 2017/10/08
// Repository       : https://github.com/icodes-studio/WWebView
// Official         : http://www.icodes.studio/
//
// (C) ICODES STUDIO. All rights reserved. 
//

#if UNITY_WSA && !UNITY_EDITOR
using UnityEngine;
using System;
using WSA.ICODES;
using WSA.ICODES.Controls;

#if UNIWEBVIEW2_SUPPORTED
public sealed class UniWebViewPlugin
#else
public sealed class WWebViewPlugin
#endif
{
    private static bool initialize = false;

    public static void Init(string name, int top, int left, int bottom, int right)
    {
        Init(name, top, left, bottom, right, 0, 0);
    }

    public static void Init(string name, int top, int left, int bottom, int right, int width, int height) 
    {
        if (initialize == false)
        {
            WWebViewSystem.Instance.Initialize();
            initialize = true;
        }

        WebViewSystem.Instance.Create(
            name,
            OnNavigationStarting,
            OnNavigationCompleted,
            OnNavigationFailed,
            OnScriptNotify,
            OnEvaluatingJavaScriptFinished, 
            left, top, right, bottom, width, height);

#if UNIWEBVIEW2_SUPPORTED
        AddUrlScheme(name, "uniwebview");
#else
        AddUrlScheme(name, "wwebview");
#endif
    }

    public static void AddUrlScheme(string name, string scheme) 
    {
        WebViewSystem.Instance.AddUrlScheme(name, scheme);
    }

    public static void RemoveUrlScheme(string name, string scheme) 
    {
        WebViewSystem.Instance.RemoveUrlScheme(name, scheme);
    }

    public static void ChangeInsets(string name, int top, int left, int bottom, int right)
    {
        ChangeInsets(name, left, top, right, bottom, 0, 0);
    }

    public static void ChangeInsets(string name, int top, int left, int bottom, int right, int width, int height)
    {
        WebViewSystem.Instance.ChangeInsets(name, left, top, right, bottom, width, height);
    }

    public static void Load(string name, string url) 
    {
        WebViewSystem.Instance.Navigate(name, url);
    }

    public static void LoadHTMLString(string name, string html, string baseUrl) 
    {
        WebViewSystem.Instance.NavigateToString(name, html);
    }

    public static void Reload(string name) 
    {
        WebViewSystem.Instance.Refresh(name);
    }

    public static void AddJavaScript(string name, string script) 
    {
        WebViewSystem.Instance.AddJavaScript(name, script);
    }

    public static void EvaluatingJavaScript(string name, string script) 
    {
        WebViewSystem.Instance.EvaluateJavaScript(name, script);
    }

    public static void EnableContextMenu(string name, bool enable)
    {
        WebViewSystem.Instance.EnableContextMenu(name, enable);
    }

    public static void CleanCache(string url) 
    {
        WebViewSystem.Instance.CleanCache(url);
    }

    public static void CleanCookie(string url, string key)
    {
        WebViewSystem.Instance.CleanCookie(url);
    }

    public static string GetCookie(string url, string key) 
    {
        return WebViewSystem.Instance.GetCookie(url, key);
    }

    public static void SetCookie(string url, string cookie) 
    {
        WebViewSystem.Instance.SetCookie(url, cookie);
    }

    public static void Destroy(string name) 
    {
        WebViewSystem.Instance.Destroy(name);
    }

    public static void GoBack(string name) 
    {
        WebViewSystem.Instance.GoBack(name);
    }

    public static void GoForward(string name) 
    {
        WebViewSystem.Instance.GoForward(name);
    }

    public static void Stop(string name)
    {
        WebViewSystem.Instance.Stop(name);
    }

    public static string GetCurrentUrl(string name) 
    {
        return WebViewSystem.Instance.CurrentUrl(name);
    }

    public static void Show(string name, bool fade, int direction, float duration) 
    {
        WebViewSystem.Instance.Show(name, true);
    }

    public static void Hide(string name, bool fade, int direction, float duration) 
    {
        WebViewSystem.Instance.Show(name, false);
    }

    public static void SetZoom(string name, int factor) 
    {
        // TODO: ...
    }

    public static void SetZoomEnable(string name, bool enable) 
    {
        // TODO: ...
    }

    public static string GetUserAgent(string name)
    {
        return WebViewSystem.Instance.GetUserAgent(name);
    }

    public static void SetUserAgent(string userAgent)
    {
        WebViewSystem.Instance.SetUserAgent(null, userAgent);
    }

    public static void SetTexture(string name, Texture texture)
    {
        WebViewSystem.Instance.SetTexture(name, texture.GetNativeTexturePtr(), texture.width, texture.height);
    }

    public static IntPtr GetRenderEventFunc()
    {
        return WebViewSystem.Instance.GetRenderEventFunc();
    }

    public static void TransparentBackground(string name, bool transparent) 
    {
        WebViewSystem.Instance.Transparent(name, transparent);
    }

    public static float GetAlpha(string name)
    {
        return WebViewSystem.Instance.GetAlpha(name);
    }

    public static void SetAlpha(string name, float alpha)
    {
        WebViewSystem.Instance.SetAlpha(name, alpha);
    }

    public static int GetActualWidth(string name)
    {
        return WebViewSystem.Instance.GetActualWidth(name);
    }

    public static int GetActualHeight(string name)
    {
        return WebViewSystem.Instance.GetActualHeight(name);
    }

    public static void ShowScroll(string name, bool show)
    {
        WebViewSystem.Instance.ShowScroll(name, show);
    }

    public static void ShowScrollX(string name, bool show)
    {
        WebViewSystem.Instance.ShowScrollX(name, show);
    }

    public static void ShowScrollY(string name, bool show)
    {
        WebViewSystem.Instance.ShowScrollY(name, show);
    }

    public static void SetHorizontalScrollBarShow(string name, bool show)
    {
        ShowScrollX(name, show);
    }

    public static void SetVerticalScrollBarShow(string name, bool show)
    {
        ShowScrollY(name, show);
    }

    public static bool CanGoBack(string name) 
    {
        return WebViewSystem.Instance.CanGoBack(name);
    }

    public static bool CanGoForward(string name)
    {
        return WebViewSystem.Instance.CanGoForward(name);
    }

    public static void Release()
    {
    }

    public static void SetSpinnerShowWhenLoading(string name, bool show) 
    {
    }

    public static void SetSpinnerText(string name, string text) 
    {
    }

    public static void SetBounces(string name, bool enable) 
    {
    }

    public static bool GetOpenLinksInExternalBrowser(string name) 
    {
        return false;
    }

    public static void SetOpenLinksInExternalBrowser(string name, bool value) 
    {
    }

    public static void SetBackgroundColor(string name, float r, float g, float b, float a) 
    {
    }

    public static void SetHeaderField(string name, string key, string value) 
    {
    }

    public static void SetAllowAutoPlay(string name, bool value) 
    {
    }

    public static void SetAllowThirdPartyCookies(bool allowed)
    {
    }

    private static void OnNavigationStarting(WebViewClient sender, WebViewNavigationStartingEventArgs args)
    {
        GameObject webView = GameObject.Find(sender.Name);
        if (webView != null)
        {
            if (string.IsNullOrEmpty(args.CustomMessage) == false)
            {
                webView.SendMessage(
                    "ReceivedMessage", args.Uri.ToString(), SendMessageOptions.DontRequireReceiver);

                if (args.CustomMessage == "close" || args.CustomMessage == "close/")
                {
#if !UNIWEBVIEW2_SUPPORTED
                    Destroy(sender.Name);
#endif
                    webView.SendMessage("WebViewDone", string.Empty, SendMessageOptions.DontRequireReceiver);
                }
            }
            else
            {
                webView.SendMessage("LoadBegin", args.Uri.ToString(), SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private static void OnNavigationCompleted(WebViewClient sender, WebViewNavigationCompletedEventArgs args)
    {
        GameObject webView = GameObject.Find(sender.Name);
        if (webView != null)
        {
            webView.SendMessage(
                "LoadComplete", 
#if UNIWEBVIEW2SUPPORTED
                (args.IsSuccess == true) ? string.Empty : args.WebErrorStatus.ToString(), 
#else
                (args.IsSuccess == true) ? args.Uri.ToString() : args.WebErrorStatus.ToString(), 
#endif
                SendMessageOptions.DontRequireReceiver);
        }
    }

    private static void OnNavigationFailed(WebViewClient sender, WebViewNavigationFailedEventArgs args)
    {
        GameObject webView = GameObject.Find(sender.Name);
        if (webView != null)
        {
            webView.SendMessage(
                "LoadComplete", 
                args.WebErrorStatus.ToString(), 
                SendMessageOptions.DontRequireReceiver);
        }
    }

    private static void OnScriptNotify(WebViewClient sender, NotifyEventArgs args)
    {
        // To enable an external web page to fire the ScriptNotify event when calling window.external.notify, 
        // You must include the page's URI in the ApplicationContentUriRules section of the app manifest. 
        // You can do this in Visual Studio on the Content URIs tab of the Package.appxmanifest designer. 
        // The URIs in this list must use HTTPS and may contain subdomain wildcards (for example, "https://*.microsoft.com"), 
        // but they can't contain domain wildcards (for example, "https://*.com" and "https://*.*"). 
        // The manifest requirement does not apply to content that originates from the app package, 
        // uses an ms-local-stream:// URI, or is loaded using NavigateToString.

        // REF: https://social.msdn.microsoft.com/Forums/vstudio/en-US/f322a505-87af-42a1-b196-1143011ba327/uwpc-script-notify-webview-not-working?forum=wpdevelop
        // REF: https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.controls.notifyeventhandler
        // REF: https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.WebView#Windows_UI_Xaml_Controls_WebView_AllowedScriptNotifyUris
    }

    private static void OnEvaluatingJavaScriptFinished(WebViewClient sender, string result)
    {
        GameObject webView = GameObject.Find(sender.Name);
        if (webView != null)
            webView.SendMessage("EvalJavaScriptFinished", result, SendMessageOptions.DontRequireReceiver);
    }
}
#endif
