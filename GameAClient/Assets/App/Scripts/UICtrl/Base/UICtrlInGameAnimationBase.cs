﻿using SoyEngine;

namespace GameA
{
    public abstract class UICtrlInGameAnimationBase<T> : UICtrlAnimationBase<T> where T : UIViewBase
    {
        protected bool _reEnterGame = true;
        
        protected override void InitEventListener ()
        {
            base.InitEventListener ();
            RegisterEvent (EMessengerType.OnGameRestart, OnGameRestart);
            RegisterEvent (EMessengerType.OnChangeToAppMode, OnChangeToAppMode);
        }

        protected virtual void EnterGameInit()
        {
            
        }
        
        protected virtual void ExitGame()
        {
            
        }

        protected override void OnOpen(object parameter)
        {
            if (_reEnterGame)
            {
                EnterGameInit();
                _reEnterGame = false;
            }
            base.OnOpen(parameter);
        }

        private void OnChangeToAppMode ()
        {
            ExitGame();
            _reEnterGame = true;
            if (_isOpen)
            {
                SocialGUIManager.Instance.CloseUI (GetType ());
            }
        }
        
        private void OnGameRestart ()
        {
            ExitGame();
            _reEnterGame = true;
            if (_isOpen)
            {
                SocialGUIManager.Instance.CloseUI(GetType ());
            }
        }
    }
}