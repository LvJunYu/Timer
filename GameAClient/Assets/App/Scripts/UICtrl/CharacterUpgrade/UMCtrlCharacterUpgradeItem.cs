using SoyEngine;
using System.Collections.Generic;
using NewResourceSolution;
using UnityEngine;

namespace GameA
{
    public class UMCtrlCharacterUpgradeItem : UMCtrlBase<UMViewCharacterUpgradeItem>
    {
        private TrainProperty _trainProperty;

        private string[] _spriteNames =
        {
            "icon_train_heart",
            "icon_train_run",
            "icon_train_jump",
            "icon_train_nutrition",
            "icon_train_magnet"
        };

        private string[] _propertyNames =
        {
            "冥想训练",
            "长跑训练",
            "跳远训练",
            "拳击训练",
            "摩擦训练"
        };

        public UMCtrlCharacterUpgradeItem(TrainProperty trainProperty)
        {
            _trainProperty = trainProperty;
        }

        public void InitView()
        {
            _cachedView.Icon.sprite = ResourcesManager.Instance.GetSprite(_spriteNames[_trainProperty.Property - 1]);
            _cachedView.NameTxt.text = _propertyNames[_trainProperty.Property - 1];
        }

        public void Refresh()
        {
            _cachedView.CostTxt.text = _trainProperty.Cost.ToString();
            _cachedView.TimeTxt.text = GetTimeContent(_trainProperty.Time);
            _cachedView.MaxLvTxt.text = GameATools.GetLevelString(_trainProperty.MaxLv);
            _cachedView.CurLvTxt.text = GameATools.GetLevelString(_trainProperty.CurLv);
            _cachedView.NextLvTxt.text = GameATools.GetLevelString(_trainProperty.CurLv + 1);
            _cachedView.MaxLvTxt.gameObject.SetActive(_trainProperty.MaxLv == _trainProperty.CurLv);
            _cachedView.CurLvTxt.gameObject.SetActive(_trainProperty.MaxLv > _trainProperty.CurLv);
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