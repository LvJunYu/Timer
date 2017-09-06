using SoyEngine;

namespace GameA
{
    public abstract class UICtrlInGameAnimationBase<T> : UICtrlAnimationBase<T> where T : UIViewBase
    {
        protected bool _reEnterGame = true;
        
        protected override void InitEventListener ()
        {
            base.InitEventListener ();
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
            SocialGUIManager.Instance.CloseUI (GetType ());
            _reEnterGame = true;
        }
    }
}