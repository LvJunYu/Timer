using System;
using SoyEngine.Proto;

namespace GameA
{
    public partial class UnitPreinstall
    {
        public void Save(Action successAction = null, Action failAction = null)
        {
            Msg_Preinstall msg = new Msg_Preinstall();
            msg.Rotation = PreinstallData.Rotation;
            msg.Name = PreinstallData.Name;
            msg.UnitId = PreinstallData.UnitId;
            msg.Data = _preinstallData.Data;
            RemoteCommands.UpdateUnitPreinstall(PreinstallId, msg, unitMsg =>
                {
                    if (unitMsg.ResultCode == (int) EUnitPreinstallOperateResult.UPOR_Success)
                    {
                        CopyMsgData(unitMsg.UnitPreinstallData);
                        if (successAction != null)
                        {
                            successAction.Invoke();
                        }
                    }
                    else
                    {
                        UPCtrlUnitPropertyEditPreinstall.ShowFailDialog((EUnitPreinstallOperateResult) unitMsg.ResultCode);
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