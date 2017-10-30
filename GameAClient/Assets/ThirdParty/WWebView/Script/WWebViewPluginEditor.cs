// WWebViewPluginWin32.cs : WWebViewPluginWin32 implementation file
//
// Description      : WWebViewPluginWin32
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

#if UNITY_STANDALONE_WIN

using UnityEngine;
using System;
using System.Runtime.InteropServices;
using Win32;

public sealed class WWebViewPlugin
{
    private static WWebViewWin32.ActionDocumentComplete documentComplete = null;
    private static WWebViewWin32.ActionBeforeNavigate beforeNavigate = null;
    private static WWebViewWin32.ActionWindowClosing windowClosing = null;
    private static WWebViewWin32.ActionTitleChange titleChange = null;
    private static WWebViewWin32.ActionNewWindow newWindow = null;
    private static WWebViewWin32.ActionNavigateComplete navigateComplete = null;
    private static bool initialize = false;

    public static bool Init(string name, int top, int left, int bottom, int right)
    {
        return Init(name, top, left, bottom, right, 0, 0);
    }

    public static bool Init(string name, int top, int left, int bottom, int right, int width, int height)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (initialize == false)
            {
                IntPtr module = Kernel32.GetModuleHandle(null);
                if (module == IntPtr.Zero)
                {
                    Debug.LogError("Can't find process module.");
                    return false;
                }

                IntPtr window = WWebViewWin32.FindUnityEditorWindow();
                if (window == IntPtr.Zero)
                {
                    Debug.LogError("Can't find Unity editor window handle.");
                    return false;
                }

                // IE Version description.
                // 11001: IE11. Webpages are displayed in IE11 edge mode, regardless of the declared !DOCTYPE directive.
                // 11000: IE11. Webpages containing standards - based !DOCTYPE directives are displayed in IE11 edge mode.
                // 10001: IE10. Webpages are displayed in IE10 Standards mode, regardless of the !DOCTYPE directive.
                // 10000: IE10. Webpages containing standards - based !DOCTYPE directives are displayed in IE10 Standards mode.
                //  9999:  IE9. Webpages are displayed in IE9 Standards mode, regardless of the declared !DOCTYPE directive.
                //  9000:  IE9. Webpages containing standards - based !DOCTYPE directives are displayed in IE9 mode.
                //  8888:  IE8. Webpages are displayed in IE8 Standards mode, regardless of the declared !DOCTYPE directive.
                //  8000:  IE8. Webpages containing standards - based !DOCTYPE directives are displayed in IE8 mode.
                //  7000:  IE7. Webpages containing standards - based !DOCTYPE directives are displayed in IE7 Standards mode.
                // REFER: https://msdn.microsoft.com/en-us/library/ee330730(v=vs.85).aspx

                WWebViewWin32.Initialize(module, IntPtr.Zero, null, 0, window, Screen.width, Screen.height, 11000);
                WWebViewSystem.Instance.Initialize();

                documentComplete = new WWebViewWin32.ActionDocumentComplete(OnDocumentComplete);
                beforeNavigate = new WWebViewWin32.ActionBeforeNavigate(OnBeforeNavigate);
                windowClosing = new WWebViewWin32.ActionWindowClosing(OnWindowClosing);
                titleChange = new WWebViewWin32.ActionTitleChange(OnTitleChange);
                newWindow = new WWebViewWin32.ActionNewWindow(OnNewWindow);
                navigateComplete = new WWebViewWin32.ActionNavigateComplete(OnNavigateComplete);
                initialize = true;
            }

            WWebViewWin32.Create(
                name,
                documentComplete,
                beforeNavigate,
                windowClosing,
                titleChange,
                newWindow,
                navigateComplete,
                left, top, right, bottom, width, height, true);

            WWebViewWin32.SetTitleText(name, "WWebView - In the editor mode, it is displayed as a popup window.");

#if UNIWEBVIEW2_SUPPORTED
            WWebViewWin32.AddUrlScheme(name, "uniwebview");
#else
            WWebViewWin32.AddUrlScheme(name, "wwebview");
#endif
            return true;
        }
        else
        {
            Debug.LogWarning("Mac Editor is not supported yet in WWebView. Please build it to devices or use a Windows Editor.");
            return false;
        }
    }

    public static void Release()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.Release();
    }

    public static void AddUrlScheme(string name, string scheme)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.AddUrlScheme(name, scheme);
    }

    public static void RemoveUrlScheme(string name, string scheme)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.RemoveUrlScheme(name, scheme);
    }

    public static void ChangeInsets(string name, int top, int left, int bottom, int right)
    {
        ChangeInsets(name, left, top, right, bottom, 0, 0);
    }

    public static void ChangeInsets(string name, int top, int left, int bottom, int right, int width, int height)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.ChangeInsets(name, left, top, right, bottom, width, height);
    }

    public static void Load(string name, string url)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.Navigate(name, url);
    }

    public static void LoadHTMLString(string name, string html, string baseUrl)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.NavigateToString(name, html);
    }

    public static void Reload(string name)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.Refresh(name);
    }

    public static void AddJavaScript(string name, string script)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.AddJavaScript(name, script);
    }

    public static void EvaluatingJavaScript(string name, string script)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            GameObject webView = GameObject.Find(name);
            if (webView != null)
            {
                webView.SendMessage(
                    "EvalJavaScriptFinished",
                    Marshal.PtrToStringAuto(WWebViewWin32.EvaluateJavaScript(name, script)),
                    SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public static void EnableContextMenu(string name, bool enable)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.EnableContextMenu(name, enable);
    }

    public static void CleanCache(string url)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.CleanCache(url);
    }

    public static void CleanCookie(string url, string key)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.CleanCookie(url);
    }

    public static string GetCookie(string url, string key)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return Marshal.PtrToStringAuto(WWebViewWin32.GetCookie(url, key));

        return string.Empty;
    }

    public static void SetCookie(string url, string cookie)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.SetCookie(url, cookie);
    }

    public static void Destroy(string name)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.Destroy(name);
    }

    public static void GoBack(string name)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.GoBack(name);
    }

    public static void GoForward(string name)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.GoForward(name);
    }

    public static void Stop(string name)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.Stop(name);
    }

    public static string GetCurrentUrl(string name)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return Marshal.PtrToStringAuto(WWebViewWin32.CurrentUrl(name));

        return string.Empty;
    }

    public static void Show(string name, bool fade, int direction, float duration)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.Show(name, true);
    }

    public static void Hide(string name, bool fade, int direction, float duration)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.Show(name, false);
    }

    public static void SetZoom(string name, int factor)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.SetZoom(name, factor);
    }

    public static void SetZoomEnable(string name, bool enable)
    {
    }

    public static string GetUserAgent(string name)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return Marshal.PtrToStringAuto(WWebViewWin32.GetUserAgent(name));

        return string.Empty;
    }

    public static void SetUserAgent(string userAgent)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.SetUserAgent(null, userAgent);
    }

    public static void SetTexture(string name, Texture texture)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.SetTexture(name, texture.GetNativeTexturePtr(), texture.width, texture.height);
    }

    public static IntPtr GetRenderEventFunc()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return WWebViewWin32.GetRenderEventFunc();

        return IntPtr.Zero;
    }

    public static void TransparentBackground(string name, bool transparent)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.Transparent(name, transparent);
    }

    public static float GetAlpha(string name)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return WWebViewWin32.GetAlpha(name);

        return 1f;
    }

    public static void SetAlpha(string name, float alpha)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.SetAlpha(name, alpha);
    }

    public static int GetActualWidth(string name)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return WWebViewWin32.GetActualWidth(name);

        return 0;
    }

    public static int GetActualHeight(string name)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return WWebViewWin32.GetActualHeight(name);

        return 0;
    }

    public static void ShowScroll(string name, bool show)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.ShowScroll(name, show);
    }

    public static void ShowScrollX(string name, bool show)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.ShowScrollX(name, show);
    }

    public static void ShowScrollY(string name, bool show)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            WWebViewWin32.ShowScrollY(name, show);
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
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return WWebViewWin32.CanGoBack(name);

        return false;
    }

    public static bool CanGoForward(string name)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return WWebViewWin32.CanGoForward(name);

        return false;
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

    public static int ScreenScale()
    {
        return 1;
    }

    public static int GetId(string name)
    {
        return 0;
    }

    public static void InputEvent(
        string name, int x, int y, float deltaY,
        bool buttonDown, bool buttonPress, bool buttonRelease,
        bool keyPress, short keyCode, string keyChars, int textureId)
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

    private static void OnBeforeNavigate(IntPtr name, IntPtr url, IntPtr message, ref bool cancel)
    {
        GameObject webView = GameObject.Find(Marshal.PtrToStringAuto(name));
        if (webView != null)
        {
            string navigateUrl = Marshal.PtrToStringAuto(url);
            string customMessage = Marshal.PtrToStringAuto(message);

            if (string.IsNullOrEmpty(customMessage) == false)
            {
                webView.SendMessage("ReceivedMessage", navigateUrl, SendMessageOptions.DontRequireReceiver);

                if (customMessage == "close" || customMessage == "close/")
                {
#if !UNIWEBVIEW2_SUPPORTED
                    Destroy(webView.name);
#endif
                    webView.SendMessage("WebViewDone", string.Empty, SendMessageOptions.DontRequireReceiver);
                }
            }
            else
            {
                webView.SendMessage("LoadBegin", navigateUrl, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private static void OnDocumentComplete(IntPtr name, IntPtr url)
    {
        GameObject webView = GameObject.Find(Marshal.PtrToStringAuto(name));
        if (webView != null)
        {
            webView.SendMessage(
                "LoadComplete",
#if UNIWEBVIEW2_SUPPORTED
                string.Empty,
#else
                Marshal.PtrToStringAuto(url),
#endif
                SendMessageOptions.DontRequireReceiver);
        }
    }

    private static void OnWindowClosing(IntPtr name, bool childWindow, ref bool cancel)
    {
        GameObject webView = GameObject.Find(Marshal.PtrToStringAuto(name));
        if (webView != null)
        {
            if (childWindow == false)
            {
                Destroy(webView.name);
                webView.SendMessage("WebViewDone", string.Empty, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private static void OnTitleChange(IntPtr name, IntPtr title)
    {
    }

    private static void OnNewWindow(IntPtr name, ref bool cancel)
    {
    }

    private static void OnNavigateComplete(IntPtr name, IntPtr url)
    {
    }
}

#endif
