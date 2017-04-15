/********************************************************************
** Filename : ICommandManager
** Author : Dong
** Date : 2015/7/8 星期三 上午 2:43:38
** Summary : ICommandManager
***********************************************************************/

using UnityEngine;

namespace GameA.Game
{
    public interface ICommandManager
    {
        bool Execute(ICommand command,Vector2 mousePos);

        void ClearAllCommands();

        void Undo();

        void Redo();

        bool CanUndo();

        bool CanRedo();
    }
}