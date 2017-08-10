/********************************************************************
** Filename : StoneUnit
** Author : Dong
** Date : 2017/3/14 星期二 下午 10:06:41
** Summary : StoneUnit
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 4007, Type = typeof(Stone))]
    public class Stone : PaintBlock
    {
        public override void DoPaint(int start, int end, EDirectionType direction, EPaintType ePaintType, int maskRandom, bool draw = true)
        {
            if (!_isAlive)
            {
                return;
            }
            //如果是火的话干掉自己生成焦土
            if (ePaintType == EPaintType.Fire)
            {
                OnChanged();
            }
        }

        public void OnChanged()
        {
            PlayMode.Instance.CreateRuntimeUnit(4013, _curPos);
            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitFrames(20, SendMsgToAround));
            PlayMode.Instance.DestroyUnit(this);
        }

        private void SendMsgToAround()
        {
            if (!GameRun.Instance.IsPlay)
            {
                return;
            }
            CheckGrid(GetYGrid(30));
            CheckGrid(GetYGrid(-30));
            CheckGrid(GetXGrid(-30));
            CheckGrid(GetXGrid(30));
        }
        
        private void CheckGrid(Grid2D grid)
        {
            var units = ColliderScene2D.GridCastAllReturnUnits(grid);
            if (units.Count > 0)
            {
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (unit != null && unit.IsAlive && unit != this)
                    {
                        var stone = unit as Stone;
                        if (stone != null)
                        {
                            stone.OnChanged();
                        }
                    }
                }
            }
        }
    }
}
