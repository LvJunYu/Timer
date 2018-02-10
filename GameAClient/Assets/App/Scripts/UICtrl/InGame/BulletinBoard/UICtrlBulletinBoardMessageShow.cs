/********************************************************************
** Filename : UICtrlBulletinBoardMessageShow  
** Author : ake
** Date : 9/27/2016 4:06:23 PM
** Summary : UICtrlBulletinBoardMessageShow  
***********************************************************************/


using DG.Tweening;
using SoyEngine;
using UnityEngine;
using GameA.Game;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlBulletinBoardMessageShow : UICtrlInGameBase<UIViewBulletinBoardMessageShow>
    {
        private int _curShowCount = 0;
        private IntVec3 _curUnitInx = IntVec3.zero;
        private int _time;
        private bool _exit;
        private IntVec3 _tempVec3;

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameMainUI;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (_exit)
            {
                _time++;
                if (_time > 100)
                {
                    OnTriggerBulletinBoardExit(_tempVec3);
                }
            }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();

//			MessageOperator(true); 
        }

        protected override void OnDestroy()
        {
//			MessageOperator(false);
            base.OnDestroy();
        }

        public override void Open(object parameter)
        {
            base.Open(parameter);
            _curShowCount = 0;
            _curUnitInx = IntVec3.zero;
            _exit = false;
            _time = 0;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnEdit, OnEditMode);
            RegisterEvent(EMessengerType.OnPlay, OnPlayMode);
            Messenger<IntVec3>.AddListener(EMessengerType.OnTriggerBulletinBoardEnter, OnTriggerBulletinBoardEnter);
            Messenger<IntVec3>.AddListener(EMessengerType.OnTriggerBulletinBoardExit, OnTriggerBulletinBoardExitStart);
            Messenger.AddListener(EMessengerType.OnSceneChange, Close);
        }

        #region event

        private void OnEditMode()
        {
            if (!_isViewCreated)
            {
                return;
            }

            ClearAndClose();
        }

        private void OnPlayMode()
        {
            if (!_isViewCreated)
            {
                return;
            }

            ClearAndClose();
        }

//		private void MessageOperator(bool value)
//		{
//			if (value)
//			{
//				Messenger<IntVec3>.AddListener(EMessengerType.OnTriggerBulletinBoardEnter, OnTriggerBulletinBoardEnter);
//				Messenger.AddListener(EMessengerType.OnTriggerBulletinBoardExit, OnTriggerBulletinBoardExit);
//			}
//			else
//			{
//				Messenger<IntVec3>.RemoveListener(EMessengerType.OnTriggerBulletinBoardEnter, OnTriggerBulletinBoardEnter);
//				Messenger.RemoveListener(EMessengerType.OnTriggerBulletinBoardExit, OnTriggerBulletinBoardExit);
//			}
//		}

        private void OnTriggerBulletinBoardEnter(IntVec3 index)
        {
            if (!IsOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlBulletinBoardMessageShow>();
            }

            if (_curUnitInx != index)
            {
                _curShowCount = 1;
                _curUnitInx = index;
                string value = DataScene2D.CurScene.GetUnitExtra(index).Msg;
                _cachedView.ShowText.text = string.IsNullOrEmpty(value)
                    ? GM2DUIConstDefine.GameBulletinBoardMessageDefaultMessage
                    : value;
                UpdateUIShow();
            }
            else
            {
                _curShowCount++;
            }

            _exit = false;
            _time = 0;
        }

        private void OnTriggerBulletinBoardExitStart(IntVec3 index)
        {
            _exit = true;
            _time = 0;
            _tempVec3 = index;
        }

        private void OnTriggerBulletinBoardExit(IntVec3 index)
        {
            if (_curUnitInx != index)
            {
                return;
            }

            _curShowCount--;
            if (_curShowCount == 0)
            {
                _curUnitInx = IntVec3.zero;
                UpdateUIShow();
            }
        }

        #endregion

        #region private

        private void ClearAndClose()
        {
            _curShowCount = 0;
            _curUnitInx = IntVec3.zero;
            SocialGUIManager.Instance.CloseUI<UICtrlBulletinBoardMessageShow>();
        }

        private void UpdateUIShow()
        {
            if (_curShowCount == 0)
            {
                _cachedView.BgImage.DOComplete();
                _cachedView.ShowText.DOComplete();
                _cachedView.BgImage.CrossFadeAlpha(0, 0.3f, true);
                _cachedView.ShowText.CrossFadeAlpha(0, 0.3f, true);
            }
            else
            {
                _cachedView.BgImage.DOComplete();
                _cachedView.ShowText.DOComplete();
                _cachedView.BgImage.CrossFadeAlpha(1, 0.5f, true);
                _cachedView.ShowText.CrossFadeAlpha(1, 0.5f, true);
            }
        }

        #endregion
    }
}