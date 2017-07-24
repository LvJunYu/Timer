/********************************************************************
** Filename : UICtrlModifyMatchMain
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using GameA.Game;

namespace GameA
{
    public class USCtrlAdvLvlDetailInfo : USCtrlBase<USViewAdvLvlDetailInfo>
    {
        #region 常量与字段
        #endregion

        #region 属性


        #endregion

        #region 方法
        public override void Init (USViewAdvLvlDetailInfo view)
        {
            base.Init (view);
        }

        protected override void OnViewCreated ()
        {
            base.OnViewCreated ();
//            _cachedView.SelectBtn.onClick.AddListener (OnSelectBtn);
        }

        public void Open (Project project, Table_StandaloneLevel table) {
            _cachedView.gameObject.SetActive (true);
            if (null == project) return;
            //ImageResourceManager.Instance.SetDynamicImage (
            //    _cachedView.Cover1,
            //    project.IconPath,
            //    _cachedView.DefaultCover);
            //ImageResourceManager.Instance.SetDynamicImage (
            //    _cachedView.Cover2,
            //    project.IconPath,
            //    _cachedView.DefaultCover);
            //ImageResourceManager.Instance.SetDynamicImage (
            //    _cachedView.Cover3,
            //    project.IconPath,
            //    _cachedView.DefaultCover);

            if (table.StarConditions.Length < 3) {
                LogHelper.Error ("standalonelevel table error, starCond cnt < 3, id: {0}", table.Id);
                return;
            }
            var tableStarRequire1 = Game.TableManager.Instance.GetStarRequire (table.StarConditions [0]);
            if (null == tableStarRequire1) {
                LogHelper.Error ("Cant find table starrequire of id: {0}", table.StarConditions [0]);
                return;
            }
            _cachedView.Star1Desc.text = string.Format (tableStarRequire1.Desc, table.Star1Value);
            var tableStarRequire2 = Game.TableManager.Instance.GetStarRequire (table.StarConditions [1]);
            if (null == tableStarRequire2) {
                LogHelper.Error ("Cant find table starrequire of id: {0}", table.StarConditions [1]);
                return;
            }
            _cachedView.Star2Desc.text = string.Format (tableStarRequire2.Desc, table.Star2Value);
            var tableStarRequire3 = Game.TableManager.Instance.GetStarRequire (table.StarConditions [2]);
            if (null == tableStarRequire3) {
                LogHelper.Error ("Cant find table starrequire of id: {0}", table.StarConditions [2]);
                return;
            }
            _cachedView.Star3Desc.text = string.Format (tableStarRequire3.Desc, table.Star3Value);

            string winCondition = string.Empty;
            if (table.WinConditions.Length == 1 && table.WinConditions [0] == (int)EWinCondition.TimeLimit) {
                winCondition = string.Format ("坚持存活 {0} 秒", table.TimeLimit);
            } else {
                for (int i = 0; i < table.WinConditions.Length; i++) {
                    string newCondition = string.Empty;
                    if ((int)EWinCondition.KillMonster == table.WinConditions[i]) {
                        newCondition = "杀死所有怪物";
                    } else if ((int)EWinCondition.Arrived == table.WinConditions [i]) {
                        newCondition = "到达终点";
                    } else if ((int)EWinCondition.CollectTreasure == table.WinConditions [i]) {
                        newCondition = "收集所有钻石";
                    }
                    if (string.Empty != newCondition) {
                        if (i == 0) {
                            winCondition = newCondition;
                        } else {
                            winCondition = string.Format ("{0} . {1}", winCondition, newCondition);
                        }
                    }
                }
            }
            _cachedView.WinCondition.text = winCondition;
        }
        public void Close() {
            _cachedView.gameObject.SetActive (false);
        }

        #endregion

    }
}