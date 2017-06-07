﻿/********************************************************************
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
            var allUnits = ColliderScene2D.Instance.AllUnits;
            for (int i = 0; i < allUnits.Count; i++)
            {
                allUnits[i].CheckStart();
            }
            for (int i = 0; i < allUnits.Count; i++)
            {
                allUnits[i].UpdateLogic();
            }
            for (int i = 0; i < allUnits.Count; i++)
            {
                allUnits[i].CalculateExtraDeltaPos();
            }
            var mainUnit = PlayMode.Instance.MainUnit;
            var boxOperateType = mainUnit.GetBoxOperateType();
            switch (boxOperateType)
            {
                case EBoxOperateType.None:
                case EBoxOperateType.Push:
                    for (int i = 0; i < allUnits.Count; i++)
                    {
                        if (allUnits[i].IsMain)
                        {
                            continue;
                        }
                        allUnits[i].UpdateView(deltaTime);
                    }
                    PlayMode.Instance.MainUnit.UpdateView(deltaTime);
                    break;
                case EBoxOperateType.Pull:
                    PlayMode.Instance.MainUnit.UpdateView(deltaTime);
                    for (int i = 0; i < allUnits.Count; i++)
                    {
                        if (allUnits[i].IsMain)
                        {
                            continue;
                        }
                        allUnits[i].UpdateView(deltaTime);
                    }
                    break;
            }
        }

        public void UpdateRenderer(float deltaTime)
        {
            var allUnits = ColliderScene2D.Instance.AllUnits;
            for (int i = 0; i < allUnits.Count; i++)
            {
                allUnits[i].UpdateRenderer(deltaTime);
            }
        }
    }
}