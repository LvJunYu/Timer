/********************************************************************
** Filename : SocialApp
** Author : Dong
** Date : 2015/5/30 18:18:18
** Summary : SocialApp
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using Debug = UnityEngine.Debug;
using FileTools = SoyEngine.FileTools;
#if UNITY_EDITOR
using UnityEditor;

#endif

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
        [SerializeField] private string _masterServerAddress;
        [SerializeField] private AddressConfig[] _appServerAddress;
        [SerializeField] private GameObject _logoObj;
        private float _startTime;
        private const float MinLoadingTime = 2f;
        private bool _banEsc;

        internal static SocialApp Instance;

        public EEnvironment Env
        {
            get { return _env; }
            set { _env = value; }
        }

        public PublishChannel.EType Channel
        {
            set { _publishChannel = value; }
        }

        public LogHelper.ELogLevel LogLevel
        {
            set { _logLevel = value; }
        }

        public string MasterServerAddress
        {
            get { return _masterServerAddress; }
        }

        public bool BanEsc
        {
            set { _banEsc = value; }
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

        protected override void OnAwake()
        {
            base.OnAwake();
            LogHelper.LogLevel = _logLevel;
        }

        protected override void OnStart()
        {
            Instance = this;
            Loom.Init();
            ScreenResolutionManager.Instance.Init();
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
            RoomManager.Instance.MasterServerAddress = _masterServerAddress;
            gameObject.AddComponent<SocialGUIManager>();
            CoroutineManager.Instance.Init(this);
            JoyResManager.Instance.Init();
            JoyResManager.Instance.SetDefaultResScenary(EResScenary.Default);
            LocalizationManager.Instance.Init();
            SocialGUIManager.Instance.OpenUI<UICtrlUpdateResource>();
            _startTime = Time.realtimeSinceStartup;
            if (_env == EEnvironment.Production && Application.isEditor)
            {
                SocialGUIManager.ShowPopupDialog("当前环境为正式服，你真的确定要继续吗？", null,
                    new KeyValuePair<string, Action>("确定",
                        () => { JoyResManager.Instance.CheckApplicationAndResourcesVersion(); }),
                    new KeyValuePair<string, Action>("取消", () =>
                    {
#if UNITY_EDITOR
                        EditorApplication.isPlaying = false;
#endif
                    }));
            }
            else
            {
                JoyResManager.Instance.CheckApplicationAndResourcesVersion();
            }

            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(() => Destroy(_logoObj)));
        }

        public void LoginAfterUpdateResComplete()
        {
            JoyResManager.Instance.SetDefaultResScenary(EResScenary.Home);
            gameObject.AddComponent<TableManager>();
            TableManager.Instance.Init();
            LocalUser.Instance.Init();
            GameParticleManager.Instance.Init();
            GameAudioManager.Instance.Init();
            PublishChannel.Instance.Login();
            BadWordManger.Instance.Init();
        }

        public void LoginSucceed()
        {
            AppData.Instance.Init();
//            ShareUtil.Init();
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
                ReYunManager.Instance.Init();
                if (LocalUser.Instance.User.LoginCount == 1)
                {
                    ReYunManager.Instance.Register();
                }

                ReYunManager.Instance.Login();
                RoomManager.Instance.Init();
                InfoNotificationManager.Instance.Init();
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
            ReYunManager.Instance.Quit((int) Time.realtimeSinceStartup);
            if (!Application.isEditor)
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        protected override void Update()
        {
            base.Update();
            InputManager.Instance.Update();
            GameManager.Instance.Update();
            ReYunManager.Instance.Update();
            RoomManager.Instance.Update();
            InfoNotificationManager.Instance.Update();
            if (Input.GetKeyDown(KeyCode.Escape) && !_banEsc)
            {
                Messenger.Broadcast(EMessengerType.OnEscapeClick);
            }

            if (PublishChannel.Instance != null)
            {
                PublishChannel.Instance.Update();
            }
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