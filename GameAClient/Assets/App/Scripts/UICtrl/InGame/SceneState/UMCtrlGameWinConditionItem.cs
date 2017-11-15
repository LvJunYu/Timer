using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public class UMCtrlGameWinConditionItem : UMCtrlBase<UMViewGameWinConditionItem>
    {
        public EWinCondition WinCondition { get; set; }

        public void Show()
        {
            _cachedView.SetActiveEx(true);
        }

        public void Hide()
        {
            _cachedView.SetActiveEx(false);
        }
        
        public void SetText(string str)
        {
            DictionaryTools.SetContentText(_cachedView.WinConditionText, str);
        }

        public void SetComplete(bool c)
        {
            _cachedView.WinConditionMark.SetActiveEx(c);
            _cachedView.WinConditionEmptyMark.SetActiveEx(!c);
        }
    }
}
