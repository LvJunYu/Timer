/********************************************************************
** Filename : SocialApp
** Author : Dong
** Date : 2015/5/30 18:18:18
** Summary : SocialApp
***********************************************************************/

using System;
using System.Collections.Generic;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using FileTools = SoyEngine.FileTools;

namespace GameA
{
    /// <summary>
    ///     每个工程都必须包含一个Main脚本目录，这里只存放驱动工程运行的代码
    /// </summary>
    [Serializable]
    public class SocialApp : App
    {
        [SerializeField] private EEnvironment _env;
        [SerializeField] private LogHelper.ELogLevel _logLevel = LogHelper.ELogLevel.All;
        [SerializeField] private PublishChannel.EType _publishChannel = PublishChannel.EType.None;
        [SerializeField] private bool _clearCache;
        [SerializeField] private string _roomServerAddress;
        [SerializeField] private AddressConfig[] _appServerAddress;
        private float _startTime;
        private const float MinLoadingTime = 2f;

        internal static SocialApp Instance;

        public EEnvironment Env
        {
            get { return _env; }
            set { _env = value; }
        }

        public string RoomServerAddress
        {
            get { return _roomServerAddress; }
        }

        public static AddressConfig GetAppServerAddress()
        {
            if (GlobalVar.Instance.Env == EEnvironment.Production)
            {
                return new AddressConfig
                {
                    Enable = true,
                    AppServerApiRoot = "http://joygame-logic.gz.1255339982.clb.myqcloud.com/gameaapi",
                    GameResoureRoot = "http://joygameres-1255339982.file.myqcloud.com/gamea"
                };
            }
            if (GlobalVar.Instance.Env == EEnvironment.Staging)
            {
                return new AddressConfig
                {
                    Enable = true,
                    AppServerApiRoot = "http://joygame-logic.gz.1255339982.clb.myqcloud.com/gameaapi",
                    GameResoureRoot = "http://joygameres-1255339982.file.myqcloud.com/gamea"
                };
            }
            if (GlobalVar.Instance.Env == EEnvironment.Test)
            {
                return new AddressConfig
                {
                    Enable = true,
                    AppServerApiRoot = "http://devtest.joy-you.com/gameaapi",
                    GameResoureRoot = "http://devtest.joy-you.com/res/gamea"
                };
            }
            if (GlobalVar.Instance.Env == EEnvironment.Development)
            {
                return new AddressConfig
                {
                    Enable = true,
//                    AppServerApiRoot = "http://dev.joy-you.com/gameaapi",
//                    GameResoureRoot = "http://dev.joy-you.com/res/gamea",
                    AppServerApiRoot = "http://139.129.229.229/gameaapi",
                    GameResoureRoot = "http://139.129.229.229/res/gamea"
                };
            }
            for (int i = 0; i < Instance._appServerAddress.Length; i++)
            {
                if (Instance._appServerAddress[i].Enable)
                {
                    return Instance._appServerAddress[i];
                }
            }
            Debug.LogError("AddressConfig not found");
            return new AddressConfig
            {
                Enable = true,
                AppServerApiRoot = "http://localhost:8080/GameAServer/gameaapi",
                GameResoureRoot = "http://dev.joy-you.com/res/gamea"
            };
        }

        protected override void OnStart()
        {
            Instance = this;
            LogHelper.LogLevel = _logLevel;
            if (_clearCache)
            {
                ClearCache();
            }
            var channel = PublishChannel.EType.None;
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                channel = _publishChannel;
            }
            PublishChannel.Init(channel);
			RegisterGameTypeVersion();
            JoyNativeTool.Instance.Init();
            JoySceneManager.Instance.Init();
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
            GlobalVar.Instance.Env = _env;
            GlobalVar.Instance.AppVersion = RuntimeConfig.Instance.Version;
            var addressConfig = GetAppServerAddress();
            NetworkManager.AppHttpClient.BaseUrl = addressConfig.AppServerApiRoot;
            NetworkManager.AppHttpClient.SendInspector = Account.AppHttpClientAccountInspector;
            gameObject.AddComponent<SocialGUIManager>();
            CoroutineManager.Instance.Init(this);
            JoyResManager.Instance.Init();
            JoyResManager.Instance.SetDefaultResScenary(EResScenary.Default);
            LocalizationManager.Instance.Init();
            SocialGUIManager.Instance.OpenUI<UICtrlUpdateResource>();
            _startTime = Time.realtimeSinceStartup;
            JoyResManager.Instance.CheckApplicationAndResourcesVersion();
        }

        public void LoginAfterUpdateResComplete()
        {
            JoyResManager.Instance.SetDefaultResScenary(EResScenary.Home);
            gameObject.AddComponent<TableManager>();
            TableManager.Instance.Init();
            LocalUser.Instance.Init();
            CompassManager.Instance.Login();
            GameParticleManager.Instance.Init();
            GameAudioManager.Instance.Init();
            PublishChannel.Instance.Login();
        }

        public void LoginSucceed()
        {
            AppData.Instance.Init();
//            ShareUtil.Init();
//            RoomManager.Instance.Init();
#if !UNITY_EDITOR_OSX && !UNITY_STANDALONE_OSX
//            YIMManager.Instance.Login();
#endif
            GetUserData();
        }

        private void GetUserData()
        {
            if (SocialGUIManager.Instance.GetUI<UICtrlUpdateResource>().IsOpen)
            {
                SocialGUIManager.Instance.GetUI<UICtrlUpdateResource>().ShowInfo("正在加载用户数据");
            }
            else
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在加载用户数据");
            }
            ParallelTaskHelper<ENetResultCode> helper = new ParallelTaskHelper<ENetResultCode>(() =>
            {
                GameProcessManager.Instance.Init();
                if (SocialGUIManager.Instance.GetUI<UICtrlUpdateResource>().IsOpen)
                {
                    SocialGUIManager.Instance.CloseUI<UICtrlUpdateResource>();
                }
                else
                {
                    SocialGUIManager.Instance.CloseUI<UICtrlLogin>();
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                }
                SocialGUIManager.Instance.ShowAppView();
            }, code =>
            {
                if (GlobalVar.Instance.Env != EEnvironment.Production)
                {
                    if (SocialGUIManager.Instance.GetUI<UICtrlUpdateResource>().IsOpen)
                    {
                        SocialGUIManager.Instance.OpenUI<UICtrlLogin>();
                        SocialGUIManager.Instance.CloseUI<UICtrlUpdateResource>();
                    }
                    else
                    {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    }
                }
                SocialGUIManager.ShowPopupDialog("服务器连接失败，请检查网络后重试，错误代码：" + code.ToString(), null,
                    new KeyValuePair<string, Action>("重试",
                        () => { CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(GetUserData)); }));
            });
            //最少loading时间MinLoadingTime
            helper.AddTask((successCb, failedCb) =>
            {
                var leftTime = _startTime + MinLoadingTime - Time.realtimeSinceStartup;
                if (leftTime > 0)
                {
                    CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitForSeconds(leftTime, successCb));
                }
                else
                {
                    successCb.Invoke();
                }
            });
            helper.AddTask(AppData.Instance.LoadAppData);
            helper.AddTask(LocalUser.Instance.LoadUserData);
            helper.AddTask(AppData.Instance.AdventureData.PrepareAllData);
            helper.AddTask(LocalUser.Instance.LoadPropData);
            helper.AddTask(LocalUser.Instance.LoadWorkshopUnitData);
        }

        public void ReturnToApp()
        {
            GameManager.Instance.RequestStopGame();
            //JoySceneManager.Instance.LoadEmptyScene();
            JoyResManager.Instance.SetDefaultResScenary(EResScenary.Home);
            SocialGUIManager.Instance.ChangeToAppMode();
            GameParticleManager.Instance.OnChangeScene();
        }

        public void Exit()
        {
            if (Application.isEditor)
            {
                LogHelper.Info("App Exit");
            }
            else
            {
                Application.Quit();
            }
        }

        public void ChangeToGame()
        {
            SocialGUIManager.Instance.ChangeToGameMode();
            GameParticleManager.Instance.OnChangeScene();
        }

        public void ClearCache()
        {
            try
            {
                PlayerPrefs.DeleteAll();
                FileTools.ClearFolder(SoyPath.Instance.RootPath);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
        }

        private void RegisterGameTypeVersion()
        {
            GameManager.Instance.Init(typeof(GM2DGame));
        }

        protected override void OnDestroy()
        {
            Messenger.Broadcast(EMessengerType.OnApplicationQuit);
            if (PublishChannel.Instance != null)
            {
                PublishChannel.Instance.OnDestroy();
            }
            base.OnDestroy();
            CompassManager.Instance.Quit(((int) Time.realtimeSinceStartup).ToString());
        }

        protected override void Update()
        {
            base.Update();
            GameManager.Instance.Update();
            CompassManager.Instance.Update();
//            RoomManager.Instance.Update();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Messenger.Broadcast(EMessengerType.OnEscapeClick);
            }
            PublishChannel.Instance.Update();
        }

        [Serializable]
        public class AddressConfig
        {
            public bool Enable;
            public string AppServerApiRoot;
            public string GameResoureRoot;
        }
    }
}