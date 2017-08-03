namespace GameA.Game
{
    public partial class EditModeState
    {
        /// <summary>
        /// 地块变换
        /// </summary>
        public class UnitClick : GenericBase<UnitClick>
        {
            private struct Context
            {
                public UnitDesc CurrentTouchUnitDesc;
                public Table_Unit CurrentTouchTableUnit;
                public UnitExtra CurrentTouchUnitExtra;
            }
            public override void Enter(EditMode2 owner)
            {
                DoClickOperator();
                owner.StateMachine.RevertToPreviousState();
            }

            private bool DoClickOperator()
            {
                var board = EditMode2.Instance.BoardData;
                if (board.CurrentTouchUnitDesc == UnitDesc.zero)
                {
                    return false;
                }
                var context = new Context();
                context.CurrentTouchUnitDesc = board.CurrentTouchUnitDesc;
                context.CurrentTouchTableUnit = TableManager.Instance.GetUnit(board.CurrentTouchUnitDesc.Id);
                context.CurrentTouchUnitExtra = DataScene2D.Instance.GetUnitExtra(board.CurrentTouchUnitDesc.Guid);
                if (context.CurrentTouchTableUnit.CanMove || context.CurrentTouchTableUnit.OriginMagicDirection != 0)
                {
                    if (context.CurrentTouchUnitExtra.MoveDirection != 0)
                    {
                        return DoMove(ref context);
                    }
                }
                if (UnitDefine.IsWeaponPool(context.CurrentTouchTableUnit.Id))
                {
                    return DoWeapon(ref context);
                }
                if (UnitDefine.IsJet(context.CurrentTouchTableUnit.Id))
                {
                    return DoJet(ref context);
                }
                if (context.CurrentTouchTableUnit.CanRotate)
                {
                    return DoRotate(ref context);
                }
                if (context.CurrentTouchUnitDesc.Id == UnitDefine.BillboardId)
                {
                    return DoAddMsg(ref context);
                }
                if (context.CurrentTouchUnitDesc.Id == UnitDefine.RollerId)
                {
                    return DoRoller(ref context);
                }
                if (UnitDefine.IsEarth(context.CurrentTouchUnitDesc.Id))
                {
                    return DoEarth(ref context);
                }
                return false;
            }
    
            private bool DoWeapon(ref Context context)
            {
                context.CurrentTouchUnitExtra.UnitValue++;
                if (context.CurrentTouchUnitExtra.UnitValue >= (int)EWeaponType.Max)
                {
                    context.CurrentTouchUnitExtra.UnitValue = 2;
                }
                DataScene2D.Instance.ProcessUnitExtra(context.CurrentTouchUnitDesc, context.CurrentTouchUnitExtra);
                return true;
            }
    
            private bool DoJet(ref Context context)
            {
                context.CurrentTouchUnitExtra.UnitValue++;
                if (context.CurrentTouchUnitExtra.UnitValue >= (int)EJetWeaponType.Max)
                {
                    context.CurrentTouchUnitExtra.UnitValue = 1;
                }
                DataScene2D.Instance.ProcessUnitExtra(context.CurrentTouchUnitDesc, context.CurrentTouchUnitExtra);
                return true;
            }
            
            private bool DoAddMsg(ref Context context)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGameItemAddMessage>(context.CurrentTouchUnitDesc);
                return false;
            }
    
            private bool DoRotate(ref Context context)
            {
                byte dir;
                if (!EditHelper.CalculateNextDir(context.CurrentTouchUnitDesc.Rotation,
                    context.CurrentTouchTableUnit.RotationMask, out dir))
                {
                    return false;
                }
                context.CurrentTouchUnitDesc.Rotation = dir;
                DataScene2D.Instance.ProcessUnitExtra(context.CurrentTouchUnitDesc, context.CurrentTouchUnitExtra);
                return false;
            }
    
            private bool DoRoller(ref Context context)
            {
                byte dir;
                if (!EditHelper.CalculateNextDir((byte)(context.CurrentTouchUnitExtra.RollerDirection - 1), 10, out dir))
                {
                    return false;
                }
                context.CurrentTouchUnitExtra.RollerDirection = (EMoveDirection)(dir + 1);
                DataScene2D.Instance.ProcessUnitExtra(context.CurrentTouchUnitDesc, context.CurrentTouchUnitExtra);
                return true;
            }
    
            private bool DoMove(ref Context context)
            {
                byte dir;
                if (!EditHelper.CalculateNextDir((byte)(context.CurrentTouchUnitExtra.MoveDirection - 1), 
                   context.CurrentTouchTableUnit.MoveDirectionMask, out dir))
                {
                    return false;
                }
                context.CurrentTouchUnitExtra.MoveDirection = (EMoveDirection) (dir + 1);
                DataScene2D.Instance.ProcessUnitExtra(context.CurrentTouchUnitDesc, context.CurrentTouchUnitExtra);
                return true;
            }
            
            private bool DoEarth(ref Context context)
            {
                context.CurrentTouchUnitExtra.UnitValue++;
                if (context.CurrentTouchUnitExtra.UnitValue > 2)
                {
                    context.CurrentTouchUnitExtra.UnitValue = 0;
                }
                context.CurrentTouchUnitExtra.UnitValue = context.CurrentTouchUnitExtra.UnitValue;
                DataScene2D.Instance.ProcessUnitExtra(context.CurrentTouchUnitDesc, context.CurrentTouchUnitExtra);
                return true;
            }
        }
    }
}