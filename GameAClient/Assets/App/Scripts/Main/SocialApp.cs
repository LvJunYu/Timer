/********************************************************************
** Filename : SocialApp
** Author : Dong
** Date : 2015/5/30 18:18:18
** Summary : SocialApp
***********************************************************************/

using System;
using GameA.Game;
using SoyEngine;
using UnityEngine;
using EMessengerType = SoyEngine.EMessengerType;

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
        public string AppVersion = "1.0";
	    public int PackageAppResourceVersion = 1;
	    public bool UseLocalDebugRes = false;

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
	        InitLocalResource();
			RegisterGameTypeVersion();
            VersionManager.Instance.Init();
            JoyNativeTool.Instance.Init();
            JoySceneManager.Instance.Init();
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
            gameObject.AddComponent<SocialGUIManager>();
            GameResourceManager rm = gameObject.AddComponent<GameResourceManager> ();
            if (!rm.Init ("GameMaker2D")) {
                LogHelper.Error ("GameResourceManager initFailed");
            }
        }

        public void Init()
        {
            GlobalVar.Instance.Env = this._env;
            GlobalVar.Instance.AppVersion = this.AppVersion;
            var addressConfig = GetAppServerAddress();
            NetworkManager.AppHttpClient.BaseUrl = addressConfig.AppServerApiRoot;
            GameResourcePathManager.Instance.WebServerRoot = addressConfig.GameResoureRoot;
            LocalUser.Instance.Init();
            AppData.Instance.Init();
            LocalResourceManager.Instance.transform.parent = transform;
            MatrixProjectTools.InitAndCheckOnStart();
            LoginLogicUtil.Init();
            ShareUtil.Init();
        }

        public void InitAfterUpdateResComplete()
        {
            gameObject.AddComponent<TableManager>();
            TableManager.Instance.Init();
            SocialGUIManager.Instance.ShowAppView();

            GameProcessManager.Instance.Init ();
		}

	    private void InitLocalResource()
	    {
			LocalResourceManager.Instance.Init();
			LocaleManager.Instance.Init();
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