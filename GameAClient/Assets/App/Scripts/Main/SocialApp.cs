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
		public ELanguage Language = ELanguage.CN;
        [SerializeField] private bool _isMaster;
        [SerializeField] private EEnvironment _env;
        [SerializeField] private bool _clearCache;
        [SerializeField] private string _roomServerAddress;
        
        [SerializeField] private AddressConfig[] _appServerAddress;

		internal static SocialApp Instance;

        public EEnvironment Env
        {
            get { return _env; }
            set { _env = value; }
        }

        public bool IsMaster
        {
            get { return _isMaster; }
        }

        public string RoomServerAddress
        {
            get { return _roomServerAddress; }
        }

        public static AddressConfig GetAppServerAddress()
        {
            if(GlobalVar.Instance.Env == EEnvironment.Production)
            {
                return new AddressConfig()
                {
                    Enable = true,
                    AppServerApiRoot = "https://app.joy-you.com/gameaapi",
                    GameResoureRoot = "http://res.joy-you.com"
                };
            }
            else if (GlobalVar.Instance.Env == EEnvironment.Staging)
            {
                return new AddressConfig()
                {
                    Enable = true,
                    AppServerApiRoot = "http://dev.joy-you.com/gameaapi",
                    GameResoureRoot = "http://dev.joy-you.com/res"
                };
            }
            else if (GlobalVar.Instance.Env == EEnvironment.Test)
            {
                return new AddressConfig()
                {
                    Enable = true,
                    AppServerApiRoot = "http://dev.joy-you.com/gameaapi",
                    GameResoureRoot = "http://dev.joy-you.com/res"
                };
            }
            else if(GlobalVar.Instance.Env == EEnvironment.Development)
            {
                return new AddressConfig()
                {
                    Enable = true,
                    AppServerApiRoot = "http://dev.joy-you.com/gameaapi",
                    GameResoureRoot = "http://dev.joy-you.com/res"
                };
            }
            else
            {
                for (int i = 0; i < Instance._appServerAddress.Length; i++)
                {
                    if (Instance._appServerAddress[i].Enable)
                    {
                        return Instance._appServerAddress[i];
                    }
                }
            }
            Debug.LogError("AddressConfig not found");
            return new AddressConfig
            {
                Enable = true,
                AppServerApiRoot = "http://localhost:8080/GameAServer/gameaapi",
                GameResoureRoot = "http://dev.joy-you.com/res"
            };
        }

        protected override void OnStart()
        {
            Instance = this;
            if (_clearCache)
            {
                ClearCache();
            }
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
            ResourcesManager.Instance.Init ();
            LocalizationManager.Instance.Init();
            ResourcesManager.Instance.CheckApplicationAndResourcesVersion();
        }

        public void LoginAfterUpdateResComplete()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlLogin>();
            gameObject.AddComponent<TableManager>();
            TableManager.Instance.Init();
            LocalUser.Instance.Init();
            //预加载单人模式UI
            SocialGUIManager.Instance.CreateView<UICtrlSingleMode>();
            if (!string.IsNullOrEmpty(LocalUser.Instance.Account.Token))
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "自动登录中");
                LocalUser.Instance.Account.LoginByToken(()=>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    LoginSucceed();
                }, code =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.Instance.OpenUI<UICtrlLogin>();
                });
            }
		}

        public void LoginSucceed ()
        {
            AppData.Instance.Init();
//            ShareUtil.Init();
//            RoomManager.Instance.Init();

            GetUserData ();
        }

        private void GetUserData ()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在加载用户数据");
            ParallelTaskHelper<ENetResultCode> helper = new ParallelTaskHelper<ENetResultCode>(()=>{
                GameProcessManager.Instance.Init ();
                SocialGUIManager.Instance.CloseUI<UICtrlLogin>();
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                SocialGUIManager.Instance.ShowAppView ();
            }, code=>{
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                SocialGUIManager.ShowPopupDialog("服务器连接失败，请检查网络后重试，错误代码："+code.ToString(), null,
                    new KeyValuePair<string, Action>("重试", ()=>{
                        CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(GetUserData));
                    }));
            });
            helper.AddTask(AppData.Instance.LoadAppData);
            helper.AddTask(LocalUser.Instance.LoadUserData);
            helper.AddTask(AppData.Instance.AdventureData.PrepareAllData);
            helper.AddTask(LocalUser.Instance.LoadPropData);
            helper.AddTask(LocalUser.Instance.LoadWorkshopUnitData);
        }

        internal void ReturnToApp(bool withScreenEffect = true)
        {
            GameManager.Instance.RequestStopGame();
            //JoySceneManager.Instance.LoadEmptyScene();

            SocialGUIManager.Instance.ChangeToAppMode();
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


        protected override void Update()
        {
            base.Update();
            GameManager.Instance.Update();
//            RoomManager.Instance.Update();
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Messenger.Broadcast(EMessengerType.OnEscapeClick);
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