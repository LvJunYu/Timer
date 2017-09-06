using GameA.Game;
using SoyEngine;

namespace GameA
{
    public class UMCtrlGameWinConditionItem : UMCtrlBase<UMViewGameWinConditionItem>
    {
        public EWinCondition WinCondition { get; set; }
        
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
