using SoyEngine;

namespace GameA
{
    public class USCtrlMultiScore : USCtrlBase<USViewMultiScore>
    {
        public void SetScore(int score)
        {
            _cachedView.MyNum.text = _cachedView.Num.text = score.ToString();
        }

        public void SetEnable(bool value)
        {
            _cachedView.SetActiveEx(value);
        }

        public void SetMyTeam(bool value)
        {
            _cachedView.MyNum.SetActiveEx(value);
            _cachedView.Num.SetActiveEx(!value);
        }

        public void SetIndex(int index)
        {
            _cachedView.Trans.SetSiblingIndex(index);
        }
    }
}