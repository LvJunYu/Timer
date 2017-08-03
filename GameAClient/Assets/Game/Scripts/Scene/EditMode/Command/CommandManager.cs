/********************************************************************
** Filename : CommandManager
** Author : Dong
** Date : 2015/7/8 星期三 上午 2:42:13
** Summary : CommandManager
***********************************************************************/

using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class CommandManager : ICommandManager
    {
		private Stack<ICommand> _stackUndo;// = new Stack<ICommand>();
		private Stack<ICommand> _stackRedo;// = new Stack<ICommand>();

        private bool _undoable;
        private bool _redoable;

		public void Init () {
			_stackUndo = new Stack<ICommand> ();
			_stackRedo = new Stack<ICommand> ();
			_undoable = false;
			_redoable = false;
		}

		public void Clear () {
			ClearAllCommands ();
			_stackUndo = null;
			_stackRedo = null;
		}

        public bool Execute(ICommand command,Vector2 mousePos)
        {
            if (command != null)
            {
                if (command.Execute(mousePos))
                {
                    PushUndoCommond(command);
                    DeleteRedoCommands();
                    CheckUndoRedoState();
                    return true;
                }
            }
            return false;
        }

        public void ClearAllCommands()
        {
            DeleteRedoCommands();
            DeleteUndoCommands();
            CheckUndoRedoState();
        }

        public void Undo()
        {
            var command = PopUndoCommand();
            if (command != null)
            {
                if (command.Undo())
                {
                    PushRedoCommond(command);
                }
                CheckUndoRedoState();
            }
        }

        public void Redo()
        {
            var command = PopRedoCommand();
            if (command != null)
            {
                if (command.Redo())
                {
                    PushUndoCommond(command);
                }
                CheckUndoRedoState();
            }
        }

        public bool CanUndo()
        {
            return _stackUndo.Count > 0;
        }

        public bool CanRedo()
        {
            return _stackRedo.Count > 0;
        }

        private void PushUndoCommond(ICommand command)
        {
            if (command != null)
            {
                //Debug.Log("PushUndoCommond" + command.ToString());
                _stackUndo.Push(command);
            }
        }

        private ICommand PopUndoCommand()
        {
            return CanUndo() ? _stackUndo.Pop() : null;
        }

        private void PushRedoCommond(ICommand command)
        {
            if (command != null)
            {
                _stackRedo.Push(command);
            }
        }

        private ICommand PopRedoCommand()
        {
            return CanRedo() ? _stackRedo.Pop() : null;
        }

        private void DeleteUndoCommands()
        {
            _stackUndo.Clear();
        }

        private void DeleteRedoCommands()
        {
            _stackRedo.Clear();
        }

        private void CheckUndoRedoState()
        {
            var canUndo = CanUndo();
            if (_undoable != canUndo)
            {
                _undoable = canUndo;
                Messenger<bool>.Broadcast(EMessengerType.OnUndoChanged, _undoable);
            }
            var canRedo = CanRedo();
            if (_redoable != canRedo)
            {
                _redoable = canRedo;
                Messenger<bool>.Broadcast(EMessengerType.OnRedoChanged, _redoable);
            }
        }
    }
}