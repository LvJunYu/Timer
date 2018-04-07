using UnityEngine;

namespace GameA
{
    public class UMCtrlTrainPropertyInfo : UMCtrlBase<UMViewTrainPropertyInfo>
    {
        private TrainProperty _trainProperty;

        public UMCtrlTrainPropertyInfo(TrainProperty trainProperty)
        {
            _trainProperty = trainProperty;
        }

        public void InitView(Sprite sprite)
        {
            _cachedView.Icon.sprite = sprite;
            _cachedView.Icon.SetNativeSize();
        }

        public void Refresh()
        {
            _cachedView.CurLvTxt.text = GameATools.GetLevelString(_trainProperty.Level);
            _cachedView.ValueTxt.text = _trainProperty.ValueDesc;
        }

        
    }
}