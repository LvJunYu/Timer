/********************************************************************
** Filename : UnitUpdateManager
** Author : Dong
** Date : 2016/10/20 星期四 下午 10:14:44
** Summary : UnitUpdateManager
***********************************************************************/

using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class UnitUpdateManager
    {
        public void UpdateLogic(float deltaTime)
        {
            var mainUnit = PlayMode.Instance.MainPlayer;

            var allSwitchUnits = ColliderScene2D.Instance.AllSwitchUnits;
            var allMagicUnits = ColliderScene2D.Instance.AllMagicUnits;
            var allBulletUnits = ColliderScene2D.Instance.AllBulletUnits;
            var allOtherUnits = ColliderScene2D.Instance.AllOtherUnits;
            
            for (int i = 0; i < allSwitchUnits.Count; i++)
            {
                allSwitchUnits[i].UpdateLogic();
            }
            for (int i = 0; i < allMagicUnits.Count; i++)
            {
                allMagicUnits[i].UpdateLogic();
            }
            
            for (int i = 0; i < allOtherUnits.Count; i++)
            {
                allOtherUnits[i].CheckStart();
            }
            //人先执行 AI怪物后执行
            mainUnit.UpdateLogic();
            for (int i = 0; i < allOtherUnits.Count; i++)
            {
                if (allOtherUnits[i].IsMain)
                {
                    continue;
                }
                allOtherUnits[i].UpdateLogic();
            }
            for (int i = 0; i < allOtherUnits.Count; i++)
            {
                allOtherUnits[i].CalculateExtraDeltaPos();
            }

            for (int i = 0; i < allBulletUnits.Count; i++)
            {
                allBulletUnits[i].UpdateLogic();
            }
            for (int i = 0; i < allMagicUnits.Count; i++)
            {
                allMagicUnits[i].UpdateView(deltaTime);
            }

            var boxOperateType = mainUnit.GetBoxOperateType();
            switch (boxOperateType)
            {
                case EBoxOperateType.None:
                case EBoxOperateType.Push:
                    for (int i = 0; i < allOtherUnits.Count; i++)
                    {
                        if (allOtherUnits[i].IsMain)
                        {
                            continue;
                        }
                        allOtherUnits[i].UpdateView(deltaTime);
                    }
                    mainUnit.UpdateView(deltaTime);
                    break;
                case EBoxOperateType.Pull:
                    mainUnit.UpdateView(deltaTime);
                    for (int i = 0; i < allOtherUnits.Count; i++)
                    {
                        if (allOtherUnits[i].IsMain)
                        {
                            continue;
                        }
                        allOtherUnits[i].UpdateView(deltaTime);
                    }
                    break;
            }
            for (int i = 0; i < allBulletUnits.Count; i++)
            {
                allBulletUnits[i].UpdateView(deltaTime);
            }
        }
    }
}