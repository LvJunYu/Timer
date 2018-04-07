// WWebViewSystem.cs : WWebViewSystem implementation file
//
// Description      : WWebViewSystem
// Author           : icodes (icodes.studio@gmail.com)
// Maintainer       : icodes
// License          : 
// Created          : 2017/07/22
// Last Update      : 2017/10/08
// Repository       : https://github.com/icodes-studio/WWebView
// Official         : http://www.icodes.studio/
//
// (C) ICODES STUDIO. All rights reserved. 
//

#if UNITY_STANDALONE_WIN || UNITY_WSA
using UnityEngine;
using System;
using System.Collections;

#if UNITY_STANDALONE_WIN
using Win32;
#endif

#if UNITY_WSA
using WSA.ICODES;
using WSA.ICODES.Controls;
#endif

public sealed class WWebViewSystem : MonoBehaviour
{
    private static WWebViewSystem instance = null;

    public static WWebViewSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(WWebViewSystem)) as WWebViewSystem;

                if (instance == null)
                {
                    GameObject go = new GameObject("WWebViewSystem");
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<WWebViewSystem>();
                }
            }
            return instance;
        }
    }

    public void Initialize()
    {
#if UNITY_EDITOR
        StartCoroutine("OnPumpMessage");
#elif UNITY_WSA
        Dispatcher.InvokeOnAppThread += InvokeOnAppThread;
        Dispatcher.InvokeOnUIThread += InvokeOnUIThread;
#elif UNITY_STANDALONE_WIN
        WWebViewWin32.ModifyStyle(WWebViewWin32.FindUnityPlayerWindow(), 0, PARAMS.WS_CLIPCHILDREN, 0);
        #if WINPROC_SUBCLASS
        WWebViewWin32.SubclassWindow();
        #endif
#endif
    }

    private void OnDestroy()
    {
        instance = null;
    }

#if UNITY_WSA
    private static void InvokeOnAppThread(Action callback)
    {
        UnityEngine.WSA.Application.InvokeOnAppThread(() =>
        {
            callback();
        },
        false);
    }

    private static void InvokeOnUIThread(Action callback)
    {
        UnityEngine.WSA.Application.InvokeOnUIThread(() =>
        {
            callback();
        },
        false);
    }
#endif

    private IEnumerator OnPumpMessage()
    {
        // Whenever Win32 window has run in editor mode, the window message loop is gradually frozen.
        // I'm not exactly sure how this works. I can only guess the Unity3D is trying to consume more resources 
        // on the renderer in editor mode. It seems to be a sort of optimization. 
        // so, I just tried to dispatch window messages by force.

#if UNITY_EDITOR
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                WWebViewWin32.PumpMessage();
            }
        }
#else
        yield break;
#endif
    }

    private void OnApplicationQuit()
    {
#if UNIWEBVIEW2_SUPPORTED
    #if UNITY_EDITOR
        WWebViewPlugin.Release();
    #elif UNITY_STANDALONE_WIN || UNITY_WSA
        UniWebViewPlugin.Release();
    #else
        Debug.Assert(false);
    #endif
#else
        WWebViewPlugin.Release();
#endif
    }
}
#endif