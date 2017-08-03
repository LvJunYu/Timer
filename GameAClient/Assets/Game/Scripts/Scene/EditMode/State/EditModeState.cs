using SoyEngine.FSM;
using UnityEngine;

namespace GameA.Game
{
    public partial class EditModeState
    {
        public abstract class Base : State<EditMode>
        {
            public virtual void Init() { }
            public override void Enter(EditMode owner) { }
            public override void Execute(EditMode owner) { }
            public override void Exit(EditMode owner) { }
            public virtual void Dispose() { }

            protected EditMode.BlackBoard GetBlackBoard()
            {
                return EditMode.Instance.BoardData;
            }
            
            public virtual void OnPinch(Gesture gesture) { }
            public virtual void OnPinchEnd(Gesture gesture) { }
            public virtual void OnDragStart(Gesture gesture)  { }
            public virtual void OnDrag(Gesture gesture)  { }
            public virtual void OnDragEnd(Gesture gesture)  { }
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