using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class USCtrlAdvLvlDetailInfo : USCtrlBase<USViewAdvLvlDetailInfo>
    {
        private const string _icon_star_detail = "icon_star_detail";
        private const string _icon_star_detail_grey = "icon_star_detail_grey";

        public void Open(Project project, Table_StandaloneLevel table, AdventureUserLevelDataDetail userLevelDataDetail)
        {
            _cachedView.gameObject.SetActive(true);
            if (null == project) return;

            if (table.StarConditions.Length < 3)
            {
                LogHelper.Error("standalonelevel table error, starCond cnt < 3, id: {0}", table.Id);
                return;
            }
            int[] starValueAry = {table.Star1Value, table.Star2Value, table.Star3Value};
            bool[] starAry =
            {
                userLevelDataDetail.SimpleData.Star1Flag,
                userLevelDataDetail.SimpleData.Star2Flag,
                userLevelDataDetail.SimpleData.Star3Flag
            };

            for (int i = 0; i < table.StarConditions.Length; i++)
            {
                var tableStarRequire = TableManager.Instance.GetStarRequire(table.StarConditions[i]);
                if (null == tableStarRequire)
                {
                    LogHelper.Error("Cant find table starrequire of id: {0}", table.StarConditions[i]);
                    break;
                }
                _cachedView.StarDescAry[i].text = string.Format(tableStarRequire.Desc, starValueAry[i]);
                _cachedView.StarImageAry[i].sprite =
                    starAry[i] ? GetSprite(_icon_star_detail) : GetSprite(_icon_star_detail_grey);
            }

            for (int i = 0; i < _cachedView.PassDescAry.Length; i++)
            {
                if (i < table.WinConditions.Length)
                {
                    _cachedView.PassDescAry[i].SetActiveEx(true);
                    var winCondition = string.Empty;
                    switch ((EWinCondition) table.WinConditions[i])
                    {
                        case EWinCondition.WC_TimeLimit:
                            if (table.WinConditions.Length == 1)
                            {
                                winCondition = string.Format("坚持存活 {0}",
                                    GameATools.SecondToHour(table.TimeLimit, true));
                            }
                            else
                            {
                                winCondition = string.Format("{0} 内过关", GameATools.SecondToHour(table.TimeLimit, true));
                            }
                            break;
                        case EWinCondition.WC_Arrive:
                            winCondition = "到达终点";
                            break;
                        case EWinCondition.WC_Collect:
                            winCondition = "收集所有兽牙";
                            break;
                        case EWinCondition.WC_Monster:
                            winCondition = "杀死所有怪物";
                            break;
                    }
                    DictionaryTools.SetContentText(_cachedView.PassDescAry[i], winCondition);
                }
                else
                {
                    _cachedView.PassDescAry[i].SetActiveEx(false);
                }
            }
        }

        public override void Close()
        {
            _cachedView.gameObject.SetActive(false);
        }

        private Sprite GetSprite(string spriteName)
        {
            return JoyResManager.Instance.GetSprite(spriteName);
        }
    }
}