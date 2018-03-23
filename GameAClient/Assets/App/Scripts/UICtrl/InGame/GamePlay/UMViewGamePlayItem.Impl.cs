/********************************************************************
** Filename : UMViewGamePlayItem  
** Author : ake
** Date : 4/28/2016 3:53:58 PM
** Summary : UMViewGamePlayItem  
***********************************************************************/

using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public partial class UMViewGamePlayItem
    {
        protected FinishCondition _refData;
        protected EWinCondition _condition;

        public EWinCondition WinCondition
        {
            get
            {
                return _condition;
            }
        }

        public virtual void InitItem(EWinCondition value,FinishCondition data)
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
	        if (_condition == EWinCondition.WC_TimeLimit)
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
                case EWinCondition.WC_Arrive:
                    {
                        return GM2DUIConstDefine.FinishConditionArrive;
                    }
                case EWinCondition.WC_TimeLimit:
                    {
                        return GM2DUIConstDefine.FinishConditionTimeLimit;
                    }
                case EWinCondition.WC_Collect:
                    {
                        return GM2DUIConstDefine.FinishConditionCollectTreasure;
                    }
                case EWinCondition.WC_Monster:
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
                case EWinCondition.WC_Arrive:
                    {
                        return EditMode.Instance.MapStatistics.FinalCount;
                    }
                case EWinCondition.WC_Collect:
                    {
                        return EditMode.Instance.MapStatistics.GemCount + Scene2DManager.Instance.GetGemCountInWoodCase();
                    }
                case EWinCondition.WC_Monster:
                    {
                        return EditMode.Instance.MapStatistics.MonsterCount + Scene2DManager.Instance.GetMonsterCountInCaves();
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
