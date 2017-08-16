﻿using HedgehogTeam.EasyTouch;
using SoyEngine.FSM;
using UnityEngine;

namespace GameA.Game
{
    public partial class EditModeState
    {
        public abstract class Base : State<EditMode>
        {
            private EditRecordBatch _recordBatch;
            public virtual void Init() { }

            public override void Enter(EditMode owner)
            {
                if (owner == null || owner.BoardData == null)
                {
                    return;
                }
                owner.BoardData.DragInCurrentState = false;
            }
            public override void Execute(EditMode owner) { }

            public override void Exit(EditMode owner)
            {
                if (owner == null || owner.BoardData == null)
                {
                    return;
                }
                owner.BoardData.DragInCurrentState = false;
                CommitRecordBatch();
            }
            public virtual void Dispose() { }

            protected EditMode.BlackBoard GetBlackBoard()
            {
                return EditMode.Instance.BoardData;
            }

            protected EditRecordBatch GetRecordBatch()
            {
                if (null == _recordBatch)
                {
                    _recordBatch = new EditRecordBatch();
                }
                return _recordBatch;
            }

            protected void CommitRecordBatch()
            {
                if (null == _recordBatch || _recordBatch.IsEmpty)
                {
                    return;
                }
                EditMode.Instance.CommitRecordBatch(_recordBatch);
                _recordBatch = null;
            }

            public virtual bool CanRevertTo()
            {
                return true;
            }
            
            public virtual void OnPinch(Gesture gesture) { }
            public virtual void OnPinchEnd(Gesture gesture) { }
            public virtual void OnDragStart(Gesture gesture)  { }
            public virtual void OnDrag(Gesture gesture)  { }
            public virtual void OnDragEnd(Gesture gesture)  { }
            public virtual void OnDragStartTwoFingers(Gesture gesture)  { }
            public virtual void OnDragTwoFingers(Gesture gesture)  { }
            public virtual void OnDragEndTwoFingers(Gesture gesture)  { }
            public virtual void OnTap(Gesture gesture)  { }
            public virtual void OnMouseWheelChange(Vector3 pos, Vector2 delta)  { }
            public virtual void OnMouseRightButtonDragStart(Vector3 pos)  { }
            public virtual void OnMouseRightButtonDrag(Vector3 pos, Vector2 delta)  { }
            public virtual void OnMouseRightButtonDragEnd(Vector3 pos, Vector2 delta)  { }
        }
        
        public abstract class GenericBase<T> : Base where T : class, new()
        {
            private static T _internalInstance;
	
            public static T Instance
            {
                get { return _internalInstance ?? (_internalInstance = new T()); }
            }
	
            public static void Release()
            {
                _internalInstance = null;
            }
        }
        
        public class None : GenericBase<None>
        {
        }
    }
}