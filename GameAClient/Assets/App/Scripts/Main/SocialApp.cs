/********************************************************************
** Filename : SocialApp
** Author : Dong
** Date : 2015/5/30 18:18:18
** Summary : SocialApp
***********************************************************************/

using System;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using NewResourceSolution;
using EMessengerType = SoyEngine.EMessengerType;
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
        [SerializeField] private EEnvironment _env;
		[SerializeField] private bool _clearCache;
        [SerializeField] private AddressConfig[] _appServerAddress;

		internal static SocialApp Instance;

        public EEnvironment Env
        {
            get { return _env; }
            set { _env = value; }
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

            GlobalVar.Instance.Env = this._env;
            GlobalVar.Instance.AppVersion = RuntimeConfig.Instance.Version.ToString();
            var addressConfig = GetAppServerAddress();
            NetworkManager.AppHttpClient.BaseUrl = addressConfig.AppServerApiRoot;

            gameObject.AddComponent<SocialGUIManager>();

            CoroutineManager.Instance.Init(this);
            ResourcesManager.Instance.Init ();
            LocalizationManager.Instance.Init();
//            TableManager.Instance.Init();

            ResourcesManager.Instance.CheckApplicationAndResourcesVersion();
        }

        public void LoginAfterUpdateResComplete()
        {
            gameObject.AddComponent<TableManager>();
            TableManager.Instance.Init();
            LocalUser.Instance.Init();
            Account.Instance.ApiPath = SoyHttpApiPath.LoginByToken;
            LocalUser.Instance.Account.LoginByToken(()=>{
                SocialApp.Instance.LoginSucceed();
            }, code=>{
                if(code == ELoginByTokenCode.LBTC_None)
                {
                    //                    CommonTools.ShowPopupDialog("服务器连接失败，检查网络后重试", null,
                    //                        new System.Collections.Generic.KeyValuePair<string, System.Action>("重试", ()=>{
                    //                            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(()=>LoginByToken()));
                    //                        }));
                }
                else
                {
                    LogHelper.Error("登录失败, Code: " + code);
                }
            });
            SocialGUIManager.Instance.OpenUI<UICtrlLogin>();
		}

        public void LoginSucceed ()
        {
            AppData.Instance.Init();

            //            LoginLogicUtil.Init();
            ShareUtil.Init();
            RoomManager.Instance.Init();

            GetUserData ();
        }

        private void GetUserData ()
        {
            ParallelTaskHelper<ENetResultCode> helper = new ParallelTaskHelper<ENetResultCode>(()=>{
                GameProcessManager.Instance.Init ();
                SocialGUIManager.Instance.CloseUI<UICtrlLogin>();
                SocialGUIManager.Instance.ShowAppView ();
            }, code=>{
                SocialGUIManager.ShowPopupDialog("服务器连接失败，请检查网络后重试，错误代码："+code.ToString(), null,
                    new System.Collections.Generic.KeyValuePair<string, System.Action>("重试", ()=>{
                        CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(()=>GetUserData()));
                    }));
            });
            helper.AddTask(AppData.Instance.LoadAppData);
            helper.AddTask(LocalUser.Instance.LoadUserData);
            helper.AddTask(AppData.Instance.AdventureData.PrepareAllData);
            helper.AddTask (LocalUser.Instance.LoadPropData);
        }

//	    private void InitLocalResource()
//	    {
//			LocalResourceManager.Instance.Init();
//			LocaleManager.Instance.Init();
//		}

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
            RoomManager.Instance.Update();
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