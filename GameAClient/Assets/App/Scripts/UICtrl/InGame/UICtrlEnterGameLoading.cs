/********************************************************************
** Filename : UICtrlEnterGameLoading.cs
** Author : quan
** Date : 2015/7/11 20:35
** Summary : 进游戏的loading界面
***********************************************************************/

using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlEnterGameLoading : UICtrlGenericBase<UIViewEnterGameLoading>
    {
        #region 常量与字段
        #endregion

        #region 属性

        #endregion

        #region 继承方法
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.Purchase;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnRequestStartGame, OnRequestStartGame);
            RegisterEvent(EMessengerType.OnGameStartComplete, OnGameStartComplete);
			RegisterEvent(EMessengerType.OnLoadingErrorCloseUI, OnLoadingErrorCloseUI);
        }

	    protected override void OnViewCreated()
        {
            base.OnViewCreated();
			Messenger<float>.AddListener(EMessengerType.OnEnterGameLoadingProcess, OnSetProcess);
        }

	    protected override void OnDestroy()
	    {
			Messenger<float>.RemoveListener(EMessengerType.OnEnterGameLoadingProcess, OnSetProcess);
			base.OnDestroy();
	    }

	    protected override void OnOpen(object parameter)
        {
            UpdateView();
            base.OnOpen(parameter);
        }
        #endregion 继承方法

        private void UpdateView()
        {
            UpdateProgress(0);
        }

        #region 公开方法

        #endregion 公开方法

        #region 事件响应
        private void OnGameStartComplete()
        {
	        if (!_isViewCreated)
	        {
		        return;
	        }
	        if (_isOpen)
	        {
		        SocialGUIManager.Instance.CloseUI<UICtrlEnterGameLoading>();
	        }
        }

        private void OnRequestStartGame()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlEnterGameLoading>();
        }

	    private void OnLoadingErrorCloseUI()
	    {
			SocialGUIManager.Instance.CloseUI<UICtrlEnterGameLoading>();
		}

	    private void OnSetProcess(float process)
	    {
		    if (!_isViewCreated)
		    {
			    return;
		    }
		    UpdateProgress(process);
	    }

		private void UpdateProgress(float rate)
		{
            rate = Mathf.Clamp01(rate);
            _cachedView.ProcessText.text = string.Format ("{0:F1} %", rate * 100);
			SetProgress(rate);
		}
	    
	    private void SetProgress(float progress)
	    {
		    _cachedView.ProcessImg.fillAmount = progress;
		    _cachedView.ProcessMark.rectTransform.anchoredPosition =
			    Vector2.right * _cachedView.ProcessImg.rectTransform.GetWidth() * progress;
	    }

		#endregion 事件响应
	}
}