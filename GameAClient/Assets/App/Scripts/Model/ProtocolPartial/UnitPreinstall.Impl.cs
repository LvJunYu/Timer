using SoyEngine.Proto;

namespace GameA
{
    public partial class UnitPreinstall
    {
        public void Save()
        {
            Msg_Preinstall msg = new Msg_Preinstall();
            msg.MoveDirection = PreinstallData.MoveDirection;
            msg.Active = PreinstallData.Active;
            msg.ChildId = PreinstallData.ChildId;
            msg.ChildRotation = PreinstallData.ChildRotation;
            msg.RotateMode = PreinstallData.RotateMode;
            msg.RotateValue = PreinstallData.RotateValue;
            msg.TimeDelay = PreinstallData.TimeDelay;
            msg.TimeInterval = PreinstallData.TimeInterval;
            msg.Msg = PreinstallData.Msg;
            msg.JumpAbility = PreinstallData.JumpAbility;
            msg.TeamId = PreinstallData.TeamId;
            msg.Life = PreinstallData.Life;
            msg.AttackPower = PreinstallData.AttackPower;
            msg.MoveSpeed = PreinstallData.MoveSpeed;
            msg.EffectRange = PreinstallData.EffectRange;
            msg.CastRange = PreinstallData.CastRange;
            msg.ViewRange = PreinstallData.ViewRange;
            msg.BulletCount = PreinstallData.BulletCount;
            msg.CastSpeed = PreinstallData.CastSpeed;
            msg.ChargeTime = PreinstallData.ChargeTime;
            msg.InjuredReduce = PreinstallData.InjuredReduce;
            msg.CureIncrease = PreinstallData.CureIncrease;
//            msg.Drops= PreinstallData.Drops
//            msg.KnockbackForces.AddRange(KnockbackForces.ToList());
//            msg.AddStates.AddRange(AddStates.ToList());
            
//            msg.Name = PreinstallData.Name;
//            msg.Name = _dataList[_curIndex].PreinstallData.Name;
//            msg.UnitId = _mainCtrl.EditData.UnitDesc.Id;
//            RemoteCommands.UpdateUnitPreinstall(_dataList[_curIndex].PreinstallId, msg, unitMsg =>
//                {
//                    if (unitMsg.ResultCode == (int) EUnitPreinstallOperateResult.UPOR_Success)
//                    {
//                        _dataList[_curIndex] = new UnitPreinstall(unitMsg.UnitPreinstallData);
//                        HasChanged = false;
//                    }
//                    //todo
//                },
//                res => { SocialGUIManager.ShowPopupDialog("保存预设失败"); });
        }
    }
}