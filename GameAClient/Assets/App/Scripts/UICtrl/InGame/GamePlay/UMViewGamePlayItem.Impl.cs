/********************************************************************
** Filename : UMViewGamePlayItem  
** Author : ake
** Date : 4/28/2016 3:53:58 PM
** Summary : UMViewGamePlayItem  
***********************************************************************/



using SoyEngine;

namespace GameA
{
    public partial class UMViewGamePlayItem: UMViewBase
    {
        protected FinishCondition _refData;
        protected Game.EWinCondition _condition;

        public Game.EWinCondition WinCondition
        {
            get
            {
                return _condition;
            }
        }

        public virtual void InitItem(Game.EWinCondition value,FinishCondition data)
        {
            _condition = value;
            _refData = data;
            ItemButton.onClick.AddListener(OnClickButton);
        }

        public virtual void UpdateShow()
        {
            bool value = _refData.SettingValue[(int)_condition];
            BaseImage.SetActiveEx(value);
            DisableImage.SetActiveEx(!value);
            Name.text = GetName();
            Des.text = string.Format(GM2DUIConstDefine.HasBuild, GetBuildCount());
        }


        #region event

        private void OnClickButton()
        {
	        if (_condition == Game.EWinCondition.TimeLimit)
	        {
		        return;
	        }
            bool value = _refData.SettingValue[(int) _condition];
            _refData.SettingValue[(int) _condition] = !value;
            UpdateShow();
        }

        #endregion

        #region

        protected string GetName()
        {
            switch (_condition)
            {
                case Game.EWinCondition.Arrived:
                    {
                        return GM2DUIConstDefine.FinishConditionArrive;
                    }
                case Game.EWinCondition.TimeLimit:
                    {
                        return GM2DUIConstDefine.FinishConditionTimeLimit;
                    }
                case Game.EWinCondition.CollectTreasure:
                    {
                        return GM2DUIConstDefine.FinishConditionCollectTreasure;
                    }
                case Game.EWinCondition.KillMonster:
                    {
                        return GM2DUIConstDefine.FinishConditionKillMonster;
                    }
                //case Game.EWinCondition.RescueHero:
                    //{
                    //    return GM2DUIConstDefine.FinishConditionRescueHero;
                    //}
                default:
                    {
                        return "Error??";
                    }
            }
        }

        protected int GetBuildCount()
        {
            switch (_condition)
            {
                case Game.EWinCondition.Arrived:
                    {
                        return Game.EditMode.Instance.MapStatistics.FinalCount;
                    }
                case Game.EWinCondition.CollectTreasure:
                    {
                        return Game.EditMode.Instance.MapStatistics.GemCount;
                    }
                case Game.EWinCondition.KillMonster:
                    {
                        return Game.EditMode.Instance.MapStatistics.MonsterCount;
                    }
                //case Game.EWinCondition.RescueHero:
                    //{
                    //    return Game.EditMode.Instance.MapStatistics.HeroCageCount;
                    //}
                default:
                    {
                        return 0;
                    }
            }
        }

        #endregion
    }
}
