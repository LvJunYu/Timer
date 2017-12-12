using System;
using SoyEngine.Proto;

namespace GameA
{
    public partial class UnitPreinstall
    {
        public void Save(Action successAction = null, Action failAction = null)
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
            msg.MaxHp = PreinstallData.MaxHp;
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
            msg.Drops.AddRange(PreinstallData.Drops);
            msg.KnockbackForces.AddRange(PreinstallData.KnockbackForces);
            msg.AddStates.AddRange(PreinstallData.AddStates);
            msg.Name = PreinstallData.Name;
            msg.UnitId = PreinstallData.UnitId;
            RemoteCommands.UpdateUnitPreinstall(PreinstallId, msg, unitMsg =>
                {
                    if (unitMsg.ResultCode == (int) EUnitPreinstallOperateResult.UPOR_Success)
                    {
                        if (successAction != null)
                        {
                            successAction.Invoke();
                        }
                    }
                    else
                    {
                        if (failAction != null)
                        {
                            failAction.Invoke();
                        }
                    }
                },
                res =>
                {
                    if (failAction != null)
                    {
                        failAction.Invoke();
                    }
                });
        }
    }
}