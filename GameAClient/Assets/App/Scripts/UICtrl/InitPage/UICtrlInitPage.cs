using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using System.IO;

namespace GameA {
    [UIAutoSetup(EUIAutoSetupType.Show)]
    public class UICtrlInitPage : UICtrlGenericBase<UIViewInitPage>
    {
		private enum EAppStartStep
		{
			None,
			PrepareServerConfig,
			CheckAppVersion,
			WaitToAllowUpdateRes,
			UpdateRes,
            LoginByToken,
            LoadUserData,
			AppStartComplete,
		}

		#region 常量与字段
		private const int FadeFrameCount = 20;
        private int _frameCount;
        private static string ConfigPath = "http://res.joy-you.com/config.ini";

	    private EAppStartStep _curStep = EAppStartStep.None;

	    public const string GameTypeName = "GameMaker2D";
		#endregion

		#region 属性

		#endregion

		#region 继承方法
		protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
			RegisterEvent(EMessengerType.CheckAppVersionComplete, OnCheckAppVersionComplete);
		}

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            SetCurState(EAppStartStep.PrepareServerConfig);
        }

	    public override void OnUpdate()
	    {
		    if (!IsOpen)
		    {
			    return;
            }
            UpdateState(_curStep);
	    }

		#endregion 继承方法

		#region state logic

        private void SetCurState(EAppStartStep state)
        {
            //退出状态
            ExitState(_curStep);
            _curStep = state;
            //进入状态
            EnterState(_curStep);
        }

        /// <summary>
        /// 退出状态
        /// </summary>
        /// <param name="state">State.</param>
        private void ExitState(EAppStartStep state)
        {
            switch (state)
            {
                case EAppStartStep.None:
                    {
                    }
                    break;
                case EAppStartStep.PrepareServerConfig:
                    {
                        InitApp();
                    }
                    break;
                case EAppStartStep.CheckAppVersion:
                    {
                    }
                    break;
                case EAppStartStep.WaitToAllowUpdateRes:
                    {
                    }
                    break;
                case EAppStartStep.UpdateRes:
                    {
                    }
                    break;
                case EAppStartStep.LoginByToken:
                    {
                    }
                    break;
                case EAppStartStep.LoadUserData:
                    {
                    }
                    break;
                case EAppStartStep.AppStartComplete:
                    {
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 进入状态
        /// </summary>
        /// <param name="state">State.</param>
        private void EnterState(EAppStartStep state)
        {
            switch (state)
            {
                case EAppStartStep.None:
                    {
                    }
                    break;
                case EAppStartStep.PrepareServerConfig:
                    {
                        ProcessPrepareConfig();
                    }
                    break;
                case EAppStartStep.CheckAppVersion:
                    {
                    }
                    break;
                case EAppStartStep.WaitToAllowUpdateRes:
                    {
                    }
                    break;
                case EAppStartStep.UpdateRes:
                    {
                        EnterUpdateResState();
                    }
                    break;
                case EAppStartStep.LoginByToken:
                    {
                    DictionaryTools.SetContentText(_cachedView.StateText, "正在登录...");
                        LoginByToken();
                    }
                    break;
                case EAppStartStep.LoadUserData:
                    {
                    DictionaryTools.SetContentText(_cachedView.StateText, "正在加载用户数据...");
                        LoadUserData();
                    }
                    break;
                case EAppStartStep.AppStartComplete:
                    {
                        EnterAppStartCompleteState();
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 状态Tick
        /// </summary>
        /// <param name="state">State.</param>
        private void UpdateState(EAppStartStep state)
        {
            switch (state)
            {
                case EAppStartStep.None:
                    {
                    }
                    break;
                case EAppStartStep.PrepareServerConfig:
                    {
                    }
                    break;
                case EAppStartStep.CheckAppVersion:
                    {
//                        if (LocalResourceManager.Instance.CheckAppVersionComplete)
//                        {
//                            ExitCheckAppVersionState();
//                        }
                    }
                    break;
                case EAppStartStep.WaitToAllowUpdateRes:
                    {
                    }
                    break;
                case EAppStartStep.UpdateRes:
                    {
                    }
                    break;
                case EAppStartStep.LoginByToken:
                    {
                    }
                    break;
                case EAppStartStep.LoadUserData:
                    {
                    }
                    break;
                case EAppStartStep.AppStartComplete:
                    {
                    }
                    break;
                default:
                    break;
            }
        }

	    private void ExitCheckAppVersionState()
	    {
//		    var state = LocalResourceManager.Instance.CheckGameLocalFile(GameTypeName);
//		    float size = LocalResourceManager.Instance.GetNeedDownloadSizeMB(GameTypeName);
//		    if (state!=EGameUpdateCheckResult.CanPlay)
//		    {
//
//				size = Mathf.Max(0.01f, size);
//				string msg = string.Format("检查到需要更新{0:f2}m资源,是否更新?", size);
//			    CommonTools.ShowPopupDialog(msg,"更新提示",new KeyValuePair<string, Action>[2]
//			    {
//				    new KeyValuePair<string, Action>("更新",OnClickUpdateResButton),
//					new KeyValuePair<string, Action>("退出游戏",OnClickCancalUpdateResButton)
//			    });
//                SetCurState(EAppStartStep.WaitToAllowUpdateRes);
//		    }
//		    else
//		    {
//                SetCurState(EAppStartStep.LoginByToken);
//		    }
	    }

	    private void OnClickUpdateResButton()
	    {
		    EnterUpdateResState();
	    }

	    private void OnClickCancalUpdateResButton()
	    {
		    DoExitGame();
	    }

	    private void DoExitGame()
	    {
			Application.Quit();
		}

	    private void EnterAppStartCompleteState()
	    {
			SocialApp.Instance.InitAfterUpdateResComplete();
            SocialGUIManager.Instance.CloseUI<UICtrlInitPage>();
//			CoroutineProxy.Instance.StartCoroutine(FinishStartApp());
		}

	    private void EnterUpdateResState()
	    {
			_cachedView.UpdateResProcessRoot.SetActiveEx(true);
//			LocalResourceManager.Instance.DoUpdateGame(GameTypeName, OnUpdateGameResComplete, OnUpdateGameResProcess, OnUpdateGameResFailed);
		}

        private void LoginByToken()
        {
            Account.Instance.ApiPath = SoyHttpApiPath.LoginByToken;
            LocalUser.Instance.Account.LoginByToken(()=>{
                SetCurState(EAppStartStep.LoadUserData);
            }, code=>{
                if(code == ELoginByTokenCode.LBTC_None)
                {
                    CommonTools.ShowPopupDialog("服务器连接失败，检查网络后重试", null,
                        new System.Collections.Generic.KeyValuePair<string, System.Action>("重试", ()=>{
                            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(()=>LoginByToken()));
                        }));
                }
                else
                {
                    LogHelper.Error("登录失败, Code: " + code);
                }
            });
        }

        private void LoadUserData()
        {
            ParallelTaskHelper<ENetResultCode> helper = new ParallelTaskHelper<ENetResultCode>(()=>{
                SetCurState(EAppStartStep.AppStartComplete);
            }, code=>{
                CommonTools.ShowPopupDialog("服务器连接失败，检查网络后重试", null,
                    new System.Collections.Generic.KeyValuePair<string, System.Action>("重试", ()=>{
                        CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(()=>LoadUserData()));
                    }));
            });
            helper.AddTask(AppData.Instance.LoadAppData);
            helper.AddTask(LocalUser.Instance.LoadUserData);
            helper.AddTask(AppData.Instance.AdventureData.PrepareAllData);
            helper.AddTask (LocalUser.Instance.LoadPropData);
        }

	    private void OnUpdateGameResComplete()
	    {
		    _cachedView.UpdateResProcessValue.text = "更新成功";
            SetCurState(EAppStartStep.LoginByToken);
	    }

		private void OnUpdateGameResFailed()
		{ 
			CommonTools.ShowPopupDialog("更新失败,点确定退出游戏", "更新失败", new KeyValuePair<string, Action>[1]
			{
					new KeyValuePair<string, Action>("更新",DoExitGame),
			});
		}

		private void OnUpdateGameResProcess(float v)
		{
			float tmp = Mathf.Clamp(v, 0, 1) *100;
			string showValue = string.Format("{0:f2}%", tmp);
			_cachedView.UpdateResProcessValue.text = showValue;
		}


		#endregion

		#region private 

		private void OnCheckAppVersionComplete()
	    {
		    Application.Quit();
	    }

		private void ProcessPrepareConfig()
        {
            if(SocialApp.Instance.Env == EEnvironment.Production)
            {
                SFile sFile = SFile.GetFileWithUrl(ConfigPath + "?t=" + DateTimeUtil.GetNowTicks());
                sFile.DownloadAsync(f=>{
                    ProcessConfig(f.InnerWWW.text);
                }, f=>{
                    CommonTools.ShowPopupDialog("服务器连接失败，检查网络后重试", null,
                        new System.Collections.Generic.KeyValuePair<string, System.Action>("重试", ()=>{
                            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(()=>ProcessPrepareConfig()));
                        }));
                });
            }
            else
            {
                SetCurState(EAppStartStep.CheckAppVersion);
            }
        }

        private void ProcessConfig(string configText)
        {
            using(StringReader sr = new StringReader(configText))
            {
                string line = null;
                while((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if(line.StartsWith("#"))
                    {
                        continue;
                    }
                    else if(line.StartsWith("ReviewVersion"))
                    {
                        string[] ss = line.Split('=');
                        if(ss.Length < 2)
                        {
                            continue;
                        }
                        if(ss[1].Trim() == SocialApp.Instance.AppVersion)
                        {
                            SocialApp.Instance.Env = EEnvironment.Development;
                            break;
                        }
                    }
                }
            }
            SetCurState(EAppStartStep.CheckAppVersion);
        }


	    private void InitApp()
	    {
			SocialApp.Instance.Init();
		}

//	    private IEnumerator FinishStartApp()
//		{
//			for (int i = 20; i > 0; i--)
//			{
//				yield return null;
//			}
////			_cachedView.LoadingImage.gameObject.SetActive(false);
//			for (int i = FadeFrameCount; i > 0; i--)
//			{
//				Color c = Color.white;
//				c.a = 1f * (FadeFrameCount - i) / FadeFrameCount;
//				_cachedView.BgImage.color = c;
//				yield return null;
//			}
//			SocialGUIManager.Instance.CloseUI<UICtrlInitPage>();
//		}

	    #endregion
    }
}