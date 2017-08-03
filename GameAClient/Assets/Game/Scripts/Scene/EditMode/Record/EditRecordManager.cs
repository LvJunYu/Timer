using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    public class EditRecordManager
    {
		private Stack<EditRecordBatch> _stackUndo;
		private Stack<EditRecordBatch> _stackRedo;

        public bool CanUndo
        {
            get { return _stackUndo != null && _stackUndo.Count > 0; }
        }

        public bool CanRedo
        {
            get { return _stackRedo != null && _stackRedo.Count > 0; }
        }

		public void Init () {
			_stackUndo = new Stack<EditRecordBatch> (128);
			_stackRedo = new Stack<EditRecordBatch> (16);
		}

		public void Clear () {
			ClearAllCommands ();
			_stackUndo = null;
			_stackRedo = null;
		}
        
        public void AddNewRecord(EditRecordBatch record)
        {
            _stackUndo.Push(record);
            DeleteRedoCommands();
        }

        public void ClearAllCommands()
        {
            DeleteRedoCommands();
            DeleteUndoCommands();
        }

        public void Undo()
        {
            if (_stackUndo.Count > 0)
            {
                var command = _stackUndo.Pop();
                command.Undo();
                _stackRedo.Push(command);
                if (_stackUndo.Count == 0)
                {
                    Messenger<bool>.Broadcast(EMessengerType.OnUndoChanged, CanUndo);
                }
                if (_stackRedo.Count == 1)
                {
                    Messenger<bool>.Broadcast(EMessengerType.OnRedoChanged, CanRedo);
                }
            }
        }

        public void Redo()
        {
            if (_stackRedo.Count > 0)
            {
                var command = _stackRedo.Pop();
                command.Redo();
                _stackUndo.Push(command);
                if (_stackRedo.Count == 0)
                {
                    Messenger<bool>.Broadcast(EMessengerType.OnRedoChanged, CanRedo);
                }
                if (_stackUndo.Count == 1)
                {
                    Messenger<bool>.Broadcast(EMessengerType.OnUndoChanged, CanUndo);
                }
            }
        }

        private void DeleteUndoCommands()
        {
            if (_stackUndo != null)
            {
                _stackUndo.Clear();
            }
        }

        private void DeleteRedoCommands()
        {
            if (_stackRedo != null)
            {
                _stackRedo.Clear();
            }
        }
    }
}