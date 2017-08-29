using System;
using SoyEngine;
using System.Collections.Generic;
using NewResourceSolution;
using SoyEngine.Proto;
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
            _cachedView.ValueTxt.text = GetValueString(_trainProperty.Value);
        }

        private string GetValueString(float value)
        {
            switch (_trainProperty.Property)
            {
                case ETrainPropertyType.TPT_HPRegeneration:
                    return string.Format("{0}HP/s",value);
                case ETrainPropertyType.TPT_RunSpeed:
                    return string.Format("{0}m/s",value);
                case ETrainPropertyType.TPT_JumpHeight:
                    return string.Format("{0}m",value);
                case ETrainPropertyType.TPT_AntiStrike:
                    return value.ToString();
                case ETrainPropertyType.TPT_Magnet:
                    return string.Format("{0}m",value);
                default:
                    return null;
            }
        }
    }
}