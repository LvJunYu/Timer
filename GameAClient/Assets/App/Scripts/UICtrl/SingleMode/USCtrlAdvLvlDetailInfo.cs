/********************************************************************
** Filename : UICtrlModifyMatchMain
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using GameA.Game;
using SoyEngine;

namespace GameA
{
    public class USCtrlAdvLvlDetailInfo : USCtrlBase<USViewAdvLvlDetailInfo>
    {
        #region 常量与字段
        #endregion

        #region 属性


        #endregion

        #region 方法

        public void Open (Project project, Table_StandaloneLevel table) {
            _cachedView.gameObject.SetActive (true);
            if (null == project) return;

            if (table.StarConditions.Length < 3) {
                LogHelper.Error ("standalonelevel table error, starCond cnt < 3, id: {0}", table.Id);
                return;
            }
            var tableStarRequire1 = TableManager.Instance.GetStarRequire (table.StarConditions [0]);
            if (null == tableStarRequire1) {
                LogHelper.Error ("Cant find table starrequire of id: {0}", table.StarConditions [0]);
                return;
            }
            _cachedView.Star1Desc.text = string.Format (tableStarRequire1.Desc, table.Star1Value);
            var tableStarRequire2 = TableManager.Instance.GetStarRequire (table.StarConditions [1]);
            if (null == tableStarRequire2) {
                LogHelper.Error ("Cant find table starrequire of id: {0}", table.StarConditions [1]);
                return;
            }
            _cachedView.Star2Desc.text = string.Format (tableStarRequire2.Desc, table.Star2Value);
            var tableStarRequire3 = TableManager.Instance.GetStarRequire (table.StarConditions [2]);
            if (null == tableStarRequire3) {
                LogHelper.Error ("Cant find table starrequire of id: {0}", table.StarConditions [2]);
                return;
            }
            _cachedView.Star3Desc.text = string.Format (tableStarRequire3.Desc, table.Star3Value);

            if (table.WinConditions.Length == 1 && table.WinConditions [0] == (int)EWinCondition.TimeLimit) {
                string winCondition = string.Format ("坚持存活 {0} 秒", table.TimeLimit);
                _cachedView.Pass1Desc.text = winCondition;
                _cachedView.Pass1Desc.SetActiveEx(true);
                _cachedView.Pass2Desc.SetActiveEx(false);
                _cachedView.Pass3Desc.SetActiveEx(false);

            } else
            {
                // 给的效果图 最多只能放3个条件
                int conditionCounts = table.WinConditions.Length;
                if (conditionCounts > 3)
                {
                    LogHelper.Error("The Pass Conditions is larger than 3, Now is : {0}", conditionCounts);
                }
                int idx = 1;
                for (int i = 0; i < conditionCounts; i++) {
                    string newCondition = string.Empty;
                    if ((int)EWinCondition.KillMonster == table.WinConditions[i]) {
                        newCondition = "杀死所有怪物";
                    }
                    else if ((int)EWinCondition.Arrived == table.WinConditions [i]) {
                        newCondition = "到达终点";
                    } else if ((int)EWinCondition.CollectTreasure == table.WinConditions [i]) {
                        newCondition = "收集所有钻石";
                    }
                    if (string.Empty != newCondition) {
                        switch (idx)
                        {
                            case 1:
                                _cachedView.Pass1Desc.text = newCondition;
                                idx++;
                                break;
                            case 2:
                                _cachedView.Pass2Desc.text = newCondition;
                                idx++;
                                break;
                            case 3:
                                _cachedView.Pass3Desc.text = newCondition;
                                idx++;
                                break;

                        }
                    }
                }
                // idx 如果为4说明 有三个条件。如果小于4 说明需要把最后的条件设置为不可见
                if (idx < 4)
                {
                    switch (idx)
                    {
                        case 1:
                            _cachedView.Pass1Desc.SetActiveEx(false);
                            _cachedView.Pass2Desc.SetActiveEx(false);
                            _cachedView.Pass3Desc.SetActiveEx(false);
                            break;
                        case 2:
                            _cachedView.Pass1Desc.SetActiveEx(true);
                            _cachedView.Pass2Desc.SetActiveEx(false);
                            _cachedView.Pass3Desc.SetActiveEx(false);
                            break;
                        case 3:
                            _cachedView.Pass1Desc.SetActiveEx(true);
                            _cachedView.Pass2Desc.SetActiveEx(true);
                            _cachedView.Pass3Desc.SetActiveEx(false);
                            break;
                        case 4:
                            _cachedView.Pass1Desc.SetActiveEx(true);
                            _cachedView.Pass2Desc.SetActiveEx(true);
                            _cachedView.Pass3Desc.SetActiveEx(true);
                            break;
                    }
                }
            }
        }
        public void Close() {
            _cachedView.gameObject.SetActive (false);
        }

        #endregion

    }
}