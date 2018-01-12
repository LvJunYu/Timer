using System;
using SoyEngine.Proto;

namespace GameA
{
    public partial class UnitPreinstall
    {
        public void Save(Action successAction = null, Action failAction = null)
        {
            Msg_Preinstall msg = new Msg_Preinstall();
            msg.Name = PreinstallData.Name;
            msg.UnitId = PreinstallData.UnitId;
            msg.Rotation = PreinstallData.Rotation;
            msg.Data = PreinstallData.Data;
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