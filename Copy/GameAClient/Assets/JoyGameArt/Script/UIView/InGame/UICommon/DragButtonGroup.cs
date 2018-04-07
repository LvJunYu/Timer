/********************************************************************
** Filename : DragButtonGroup  
** Author : ake
** Date : 4/29/2016 10:56:58 AM
** Summary : DragButtonGroup  
***********************************************************************/


using UnityEngine.EventSystems;

namespace SoyEngine
{
    public class DragButtonGroup:UIBehaviour
    {
        private DragButton _curButton;

        public void SetCurSelectButton(DragButton button)
        {
            if (_curButton!=null)
            {
                _curButton.ReleaseButton();
            }
            _curButton = button;
        }

        public bool CheckCanClick()
        {
            return _curButton == null;
        }

        public bool CheckCanEnter(DragButton button)
        {
            return _curButton != null && _curButton!=button;
        }

        public void ReleaseButton()
        {
            if (_curButton != null)
            {
                _curButton.ReleaseButton();
                _curButton = null;
            }
            else
            {
                LogHelper.Error("ReleaseButton called but _curButton is null!");
            }
        }

        public void ResetState ()
        {
            if (_curButton != null) {
                _curButton = null;
            }
        }
    }
}