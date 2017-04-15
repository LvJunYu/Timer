/********************************************************************
** Filename : ICommand
** Author : Dong
** Date : 2015/7/8 星期三 上午 2:39:25
** Summary : ICommand
***********************************************************************/

using System;
using UnityEngine;

namespace GameA.Game
{
    public interface ICommand
    {
        bool Execute(Vector2 mousePos);

        bool Redo();

        bool Undo();

	    void Exit();
    }
}