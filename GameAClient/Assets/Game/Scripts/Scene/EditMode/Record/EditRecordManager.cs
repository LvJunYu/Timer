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
			ClearAllRecordBatch ();
			_stackUndo = null;
			_stackRedo = null;
		}
        
        public void CommitRecord(EditRecordBatch record)
        {
            _stackUndo.Push(record);
            DeleteRedoRecord();
            if (_stackUndo.Count == 1)
            {
                Messenger<bool>.Broadcast(EMessengerType.OnUndoChanged, CanUndo);
            }
        }

        public void ClearAllRecordBatch()
        {
            DeleteRedoRecord();
            DeleteUndoRecord();
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

        private void DeleteUndoRecord()
        {
            if (_stackUndo != null)
            {
                _stackUndo.Clear();
            }
        }

        private void DeleteRedoRecord()
        {
            if (_stackRedo != null)
            {
                _stackRedo.Clear();
            }
        }
    }
}