using SoyEngine;

namespace GameA
{
    public class USCtrlFriendItem : USCtrlBase<USViewFriendItem>
    {
        public void SetSelected(bool value)
        {
            _cachedView.SelectedObj.SetActive(value);
        }
    }
}