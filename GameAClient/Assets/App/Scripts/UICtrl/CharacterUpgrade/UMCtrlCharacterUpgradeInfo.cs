using SoyEngine;
using System.Collections.Generic;
using NewResourceSolution;
using UnityEngine;

namespace GameA
{
    public class UMCtrlCharacterUpgradeInfo : UMCtrlBase<UMViewCharacterUpgradeInfo>
    {
        private TrainProperty _trainProperty;

        

        public UMCtrlCharacterUpgradeInfo(TrainProperty trainProperty)
        {
            _trainProperty = trainProperty;
        }

        public void InitView(Sprite sprite)
        {
            _cachedView.Icon.sprite = sprite;
        }

        public void Refresh()
        {
            _cachedView.CurLvTxt.text = GameATools.GetLevelString(_trainProperty.CurLv);
        }

    }
}