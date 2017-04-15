/********************************************************************
** Filename : LineCommand
** Author : Dong
** Date : 2017/4/5 星期三 上午 11:27:07
** Summary : LineCommand 开关连线模式
***********************************************************************/

using System;
using System.Collections;
using UnityEngine;

namespace GameA.Game
{
    public class LineCommand : CommandBase, ICommand
    {
        public bool Execute(Vector2 mousePos)
        {
            return true;
        }

        public bool Redo()
        {
            return true;
        }

        public bool Undo()
        {
            return true;
        }

        public void Exit()
        {
        }
    }
}
