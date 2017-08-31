using System.Collections.Generic;
using HedgehogTeam.EasyTouch;
using SoyEngine;

namespace GameA.Game
{
    public partial class EditModeState
    {
        public class ModifyRemove : ModifyGenericBase<ModifyRemove>
        {
            public override void Enter(EditMode owner)
            {
                UpdateMaskEffects();
            }

            public override void OnTap(Gesture gesture)
            {
                UnitDesc touchedUnitDesc;
                if (!EditHelper.TryGetUnitDesc(GM2DTools.ScreenToWorldPoint(gesture.position), GetBlackBoard().EditorLayer, out touchedUnitDesc))
                {
                    return;
                }
                if (!CheckCanModifyErase(touchedUnitDesc))
                {
                    return;
                }
                GameAudioManager.Instance.PlaySoundsEffects (AudioNameConstDefineGM2D.EditLayItem);
                
                UnitExtra unitExtra = DataScene2D.Instance.GetUnitExtra(touchedUnitDesc.Guid);
                if (EditMode.Instance.DeleteUnitWithCheck(touchedUnitDesc))
                {
                    OnModifyDelete(new UnitEditData(touchedUnitDesc, unitExtra));
                }
            }
            

            /// <summary>
            /// 撤销改造擦除
            /// </summary>
            public void UndoModifyErase(int idx)
            {
                if (idx >= RemovedUnits.Count)
                {
                    LogHelper.Error("Try to undo the {0}'s erase action out of range");
                    return;
                }
                ModifyData data = RemovedUnits[idx];
                RemovedUnits.RemoveAt(idx);
                if (!EditMode.Instance.AddUnitWithCheck(data.OrigUnit.UnitDesc))
                {
                    RemovedUnits.Insert(idx, data);
                    LogHelper.Error("Can't undo the {0}'s erase action when add unit, unitdesc: {1}", idx,
                        data.OrigUnit);
                }
                else
                {
                    //				DataScene2D.Instance.ProcessUnitExtra(data.OrigUnit.UnitDesc.Guid, data.OrigUnit.UnitExtra);
                }
                Messenger.Broadcast(EMessengerType.OnModifyUnitChanged);
                UpdateMaskEffects();
            }
            
            
            protected void OnModifyDelete(UnitEditData orig)
            {
                RemovedUnits.Add(new ModifyData(orig, orig));
                Messenger.Broadcast(EMessengerType.OnModifyUnitChanged);
                UpdateMaskEffects();
            }

            /// <summary>
            /// 检查是否可以改造擦除
            /// </summary>
            /// <returns><c>true</c>, if can modify erase was checked, <c>false</c> otherwise.</returns>
            public bool CheckCanModifyErase(UnitDesc unitDesc)
            {
                // 检查是否是不能删除的特殊物体
                if (unitDesc.Id == UnitDefine.PlayerTableId)
                {
                    Messenger<string>.Broadcast(EMessengerType.GameLog, "不能删除主角");
                    return false;
                }
                else if (unitDesc.Id == UnitDefine.FinalDoorId)
                {
                    Messenger<string>.Broadcast(EMessengerType.GameLog, "不能删除终点");
                    return false;
                }

                // 检查是否是删除的物体
                for (int i = 0, n = RemovedUnits.Count; i < n; i++)
                {
                    if (RemovedUnits[i].OrigUnit.UnitDesc.Guid == unitDesc.Guid)
                    {
                        // 出错的才会到这里
                        Messenger<string>.Broadcast(EMessengerType.GameLog, "不能重复删除物体");
                        return false;
                    }
                }
                // 检查是否是改动的物体
                for (int i = 0, n = ModifiedUnits.Count; i < n; i++)
                {
                    if (ModifiedUnits[i].OrigUnit.UnitDesc.Guid == unitDesc.Guid ||
                        ModifiedUnits[i].ModifiedUnit.UnitDesc.Guid == unitDesc.Guid)
                    {
                        Messenger<string>.Broadcast(EMessengerType.GameLog, "不能删除变换过的物体");
                        return false;
                    }
                }
                // 检查是否是添加的物体
                for (int i = 0, n = AddedUnits.Count; i < n; i++)
                {
                    if (AddedUnits[i].ModifiedUnit.UnitDesc.Guid == unitDesc.Guid)
                    {
                        Messenger<string>.Broadcast(EMessengerType.GameLog, "不能删除改造添加的物体");
                        return false;
                    }
                }
                // 检查是否达到了删除数量上限
                // todo 限制数量从玩家属性中取
                if (RemovedUnits.Count >= LocalUser.Instance.MatchUserData.ReformDeleteUnitCapacity)
                {
                    Messenger<string>.Broadcast(EMessengerType.GameLog, "擦除次数已用完");
                    return false;
                }
                return true;
            }
            
            
            /// <summary>
            /// 更新表示当前改造操作已影响物体的蒙版特效
            /// </summary>
            protected void UpdateMaskEffects()
            {
                var data = GetBlackBoard().GetStateData<Data>();
                int redCnt = 0;
                int yellowCnt = 0;
                for (int i = 0; i < RemovedUnits.Count; i++) {
                    SetMaskEffectPos (GetUnusedYellowMask (yellowCnt++), RemovedUnits [i].OrigUnit.UnitDesc.Guid);
                }
                // 所有添加位置、改动位置为红色
                for (int i = 0; i < AddedUnits.Count; i++) {
                    SetMaskEffectPos (GetUnusedRedMask (redCnt++), AddedUnits [i].ModifiedUnit.UnitDesc.Guid);
                }
                for (int i = 0; i < ModifiedUnits.Count; i++) {
                    SetMaskEffectPos (GetUnusedRedMask (redCnt++), ModifiedUnits [i].ModifiedUnit.UnitDesc.Guid);
                }
                for (; yellowCnt < data.YellowMaskEffectCache.Count; yellowCnt++)
                {
                    data.YellowMaskEffectCache[yellowCnt].Stop();
                }
                for (; redCnt < data.RedMaskEffectCache.Count; redCnt++)
                {
                    data.RedMaskEffectCache[redCnt].Stop();
                }
            }
        }
    }
}