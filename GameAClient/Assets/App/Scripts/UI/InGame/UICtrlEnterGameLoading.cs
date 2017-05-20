/********************************************************************
** Filename : UICtrlEnterGameLoading.cs
** Author : quan
** Date : 2015/7/11 20:35
** Summary : 进游戏的loading界面
***********************************************************************/

using SoyEngine;
using UnityEngine;
using System.Collections.Generic;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlEnterGameLoading : UICtrlGenericBase<UIViewEnterGameLoading>
    {
        #region 常量与字段
        private const string CompleteRateTemplate = "通过率 {0:0.0} %";
        private List<UMCtrlUser40> _recentCompleteUserList = new List<UMCtrlUser40>(5);
        #endregion

        #region 属性

        #endregion

        #region 继承方法
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.InGame;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnRequestStartGame, OnRequestStartGame);
            RegisterEvent(EMessengerType.OnGameStartComplete, OnGameStartComplete);
			RegisterEvent(EMessengerType.OnLoadingErrorCloseUI, OnLoadingErrorCloseUI);
        }

	    protected override void OnClose()
	    {
		    base.OnClose();
	    }

	    protected override void OnViewCreated()
        {
            base.OnViewCreated();
            for(int i = 0; i < 5; i++)
            {
                var item = new UMCtrlUser40();
                item.Init(_cachedView.GameWideRecentCompleteUserDock.GetComponent<RectTransform>());
                _recentCompleteUserList.Add(item);
            }
			Messenger<float>.AddListener(EMessengerType.OnEnterGameLoadingProcess, OnSetProcess);
        }

	    protected override void OnDestroy()
	    {
			Messenger<float>.RemoveListener(EMessengerType.OnEnterGameLoadingProcess, OnSetProcess);
			base.OnDestroy();
	    }

	    protected override void OnOpen(object parameter)
        {
            ScreenOrientation orientation = ScreenOrientation.Portrait;
            if(GameManager.Instance.CurrentGame != null)
            {
                orientation = GameManager.Instance.CurrentGame.ScreenOrientation;
            }
            else
            {
                LogHelper.Error("UICtrlEnterGameLoading OnOpen, Current Game is Null");
            }
            if(GameManager.Instance.CurrentGame.GameInitType == GameManager.EStartType.Play
                && GameManager.Instance.GameMode == EGameMode.Normal)
            {
                _cachedView.NormalDock.SetActive(false);
                _cachedView.GameDock.SetActive(true);
            }
            else
            {
                _cachedView.NormalDock.SetActive(true);
                _cachedView.GameDock.SetActive(false);
            }
            if(orientation == ScreenOrientation.Landscape
                || orientation == ScreenOrientation.LandscapeLeft
                || orientation == ScreenOrientation.LandscapeRight)
            {
                _cachedView.GameTall.SetActive(false);
                _cachedView.NormalTall.SetActive(false);
                _cachedView.GameWide.SetActive(true);
                _cachedView.NormalWide.SetActive(true);
            }
            else
            {
                _cachedView.GameTall.SetActive(true);
                _cachedView.NormalTall.SetActive(true);
                _cachedView.GameWide.SetActive(false);
                _cachedView.NormalWide.SetActive(false);
            }
            UpdateView();
            base.OnOpen(parameter);
        }
        #endregion 继承方法

        private void UpdateView()
        {
            UpdateProgress(0);
            if(_cachedView.GameDock.activeSelf)
            {
                if(_cachedView.GameWide.activeSelf)
                {
                    UpdateGameWideView();
                }
                if(_cachedView.GameTall.activeSelf)
                {
                    UpdateGameTallView();
                }
            }
            if(_cachedView.NormalDock.activeSelf)
            {
                if(_cachedView.NormalWide.activeSelf)
                {
                    UpdateNormalWideView();
                }
                if(_cachedView.NormalTall.activeSelf)
                {
                    UpdateNormalTallView();
                }
            }
        }

        private void UpdateGameWideView()
        {
            _cachedView.GameWideInfoText.text = string.Empty;
            _cachedView.GameWideProgressText.text = string.Empty;
            Project project = GameManager.Instance.CurrentGame.Project;
            DictionaryTools.SetContentText(_cachedView.GameWideTitle, project.Name);
//            DictionaryTools.SetContentText(_cachedView.GameWideAuthorName, project.UserLegacy.NickName);
            DictionaryTools.SetContentText(_cachedView.GameWideSummary, "简介：" + project.Summary);
            _cachedView.GameWideCompleteRate.Set(project.ExtendReady, project.CompleteCount, project.FailCount);
//            Matrix matrix = project.Matrix;
//            if(matrix != null)
//            {
//                MatrixUIParams param = null;
//                if(SocialGUIManager.Instance.GetUI<UICtrlMainCreate>().UiParamsDict.TryGetValue(matrix.MatrixGuid, out param))
//                {
//                    _cachedView.GameWideMatrixLabelBg.gameObject.SetActive(true);
//                    _cachedView.GameWideMatrixLabelBg.sprite = param.LabelBg;
//                    DictionaryTools.SetContentText(_cachedView.GameWideMatrixLabel, matrix.Name);
//                }
//                else
//                {
//                    _cachedView.GameWideMatrixLabelBg.gameObject.SetActive(false);
//                }
//            }
//            else
//            {
//                _cachedView.GameWideMatrixLabelBg.gameObject.SetActive(false);
//            }
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.GameWideCover, project.IconPath, _cachedView.DefaultCoverTexture);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.GameWideAuthorIcon, project.UserLegacy.HeadImgUrl, _cachedView.DefaultUserTexture);

            if(project.RecentPlayedUserList == null || project.RecentPlayedUserList.Count == 0)
            {
                for(var i=0; i<_recentCompleteUserList.Count; i++)
                {
                    _recentCompleteUserList[i].Hide();
                }
                _cachedView.GameWideRecentCompleteUserTip.gameObject.SetActive(true);
            }
            else
            {
                _cachedView.GameWideRecentCompleteUserTip.gameObject.SetActive(false);
                for(var i=0; i<_recentCompleteUserList.Count; i++)
                {
                    if(i<project.RecentPlayedUserList.Count)
                    {
                        _recentCompleteUserList[i].Show();
                        _recentCompleteUserList[i].Set(project.RecentPlayedUserList[i].User);
                    }
                    else
                    {
                        _recentCompleteUserList[i].Hide();
                    }
                }
            }
        }


        private void UpdateGameTallView()
        {

        }
        private void UpdateNormalWideView()
        {
            _cachedView.NormalWideInfoText.text = string.Empty;
            _cachedView.NormalWideProgressText.text = string.Empty;
            Project project = GameManager.Instance.CurrentGame.Project;
//            Matrix matrix = project.Matrix;
//            _cachedView.NormalWideLoadingImage.sprite = null;
//            if(matrix != null)
//            {
//                MatrixUIParams param = null;
//                if(SocialGUIManager.Instance.GetUI<UICtrlMainCreate>().UiParamsDict.TryGetValue(matrix.MatrixGuid, out param))
//                {
//                    _cachedView.NormalWideLoadingImage.sprite = param.LoadingImg;
//                }
//                           }
        }

		private void UpdateNormalTallView()
        {
			
        }

        #region 公开方法

		public void SetText (string str)
		{
			_cachedView.NormalWideProgressText.text = str;
			_cachedView.GameWideProgressText.text = str;
		}
        #endregion 公开方法

        #region 事件响应
        private void OnGameStartComplete()
        {
			if (SocialGUIManager.Instance.RunRecordInApp)
			{
				return;
			}
			SocialGUIManager.Instance.CloseUI<UICtrlEnterGameLoading>();
        }

        private void OnRequestStartGame()
        {
	        if (SocialGUIManager.Instance.RunRecordInApp)
	        {
		        return;
	        }
            SocialGUIManager.Instance.OpenUI<UICtrlEnterGameLoading>();
        }

	    private void OnLoadingErrorCloseUI()
	    {
			if (SocialGUIManager.Instance.RunRecordInApp)
			{
				return;
			}
			SocialGUIManager.Instance.CloseUI<UICtrlEnterGameLoading>();
		}

	    private void OnSetProcess(float process)
	    {
		    UpdateProgress(process);
	    }

		private void UpdateProgress(float rate)
		{
			rate = Mathf.Min(1, Mathf.Max(0, rate));
			string str = "" + Mathf.CeilToInt(rate * 1000) / 10f + " %";
			_cachedView.NormalWideProgressText.text = str;
			_cachedView.GameWideProgressText.text = str;
			float totalWidth = _cachedView.GameWideProgressImage.transform.parent.rectTransform().rect.width;
			_cachedView.GameWideProgressImage.rectTransform().sizeDelta = rate * new Vector2(totalWidth, 0);
			totalWidth = _cachedView.NormalWideProgressImage.transform.parent.rectTransform().rect.width;
			_cachedView.NormalWideProgressImage.rectTransform().sizeDelta = rate * new Vector2(totalWidth, 0);
		}

		#endregion 事件响应
	}
}