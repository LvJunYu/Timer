
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlGameStarItem: UMCtrlBase<UMViewGameStarItem>
    {
        public void SetText(string str)
        {
            DictionaryTools.SetContentText(_cachedView.StarConditionText, str);
        }

        public void SetComplete(bool c)
        {
            _cachedView.StarConditionStar.SetActiveEx(c);
            _cachedView.StarConditionEmptyStar.SetActiveEx(!c);
        }
    }
}