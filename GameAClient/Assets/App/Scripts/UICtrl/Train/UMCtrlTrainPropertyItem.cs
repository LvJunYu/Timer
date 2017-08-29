using UnityEngine;

namespace GameA
{
    public class UMCtrlTrainPropertyItem : UMCtrlBase<UMViewTrainPropertyItem>
    {
        private TrainProperty _trainProperty;

        public UMCtrlTrainPropertyItem(TrainProperty trainProperty)
        {
            _trainProperty = trainProperty;
        }

        public void InitView(Sprite icon,string name)
        {
            _cachedView.Icon.sprite = icon;
            _cachedView.Icon.SetNativeSize();
            _cachedView.NameTxt.text = name;
        }

        public void Refresh()
        {
            _cachedView.CostTxt.text = _trainProperty.Cost.ToString();
            _cachedView.TimeTxt.text = GetTimeContent(_trainProperty.Time);
            _cachedView.MaxLvTxt.text = GameATools.GetLevelString(_trainProperty.MaxLv);
            _cachedView.CurLvTxt.text = GameATools.GetLevelString(_trainProperty.Level);
            _cachedView.NextLvTxt.text = GameATools.GetLevelString(_trainProperty.Level + 1);
            _cachedView.DisableObj.SetActive(_trainProperty.MaxLv == _trainProperty.Level);
            
            _cachedView.MaxLvTxt.gameObject.SetActive(_trainProperty.MaxLv == _trainProperty.Level);
            _cachedView.CurLvTxt.gameObject.SetActive(_trainProperty.MaxLv > _trainProperty.Level);
            _cachedView.CostTxt.gameObject.SetActive(_trainProperty.MaxLv > _trainProperty.Level);
            _cachedView.TimeTxt.gameObject.SetActive(_trainProperty.MaxLv > _trainProperty.Level);
            _cachedView.StartBtn.gameObject.SetActive(_trainProperty.MaxLv > _trainProperty.Level);
        }

        private string GetTimeContent(int second)
        {
            if (second < 60)
                return string.Format("{0}second", second);
            else if (second >= 60 && second < 3600)
                return string.Format("{0}min", second / 60);
            else if (second >= 3600 && second < 3600 * 24)
                return string.Format("{0}hour", second / 3600);
            else
                return string.Format("{0}day", second / 3600 / 24);
        }
    }
}