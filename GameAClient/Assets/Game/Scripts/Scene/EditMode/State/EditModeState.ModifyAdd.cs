using HedgehogTeam.EasyTouch;
using SoyEngine;

namespace GameA.Game
{
    public partial class EditModeState
    {
        public class ModifyAdd : ModifyGenericBase<ModifyAdd>
        {
            public override void Enter(EditMode owner)
            {
                UpdateMaskEffects();
            }

            public override void OnTap(Gesture gesture)
            {
                var boardData = GetBlackBoard();
                UnitDesc touchedUnitDesc;
                if (EditHelper.TryGetUnitDesc(GM2DTools.ScreenToWorldPoint(gesture.position), boardData.EditorLayer, out touchedUnitDesc))
                {
                    return;
                }
                else
                {
                    if (boardData.CurrentSelectedUnitId == 0)
                    {
                        return;
                    }
                }
                UnitDesc createUnitDesc;
                if (!EditHelper.TryGetCreateKey(GM2DTools.ScreenToWorldPoint(gesture.position),
                    boardData.CurrentSelectedUnitId, out createUnitDesc))
                {
                    return;
                }
                
                var tableUnit = UnitManager.Instance.GetTableUnit(createUnitDesc.Id);
                var grid = tableUnit.GetBaseDataGrid(createUnitDesc.Guid.x, createUnitDesc.Guid.y);
                int layerMask = EnvManager.UnitLayerWithoutEffect;
                SceneNode outHit;
                if (DataScene2D.GridCast(grid, out outHit, layerMask))
                {
                    return;
                }
                if (CheckCanModifyAdd(createUnitDesc))
                {
                    if (EditMode.Instance.AddUnitWithCheck(createUnitDesc)) {
                        GameAudioManager.Instance.PlaySoundsEffects (AudioNameConstDefineGM2D.EditLayItem);
                        var unitEditData = new UnitEditData(createUnitDesc,
                            DataScene2D.Instance.GetUnitExtra(createUnitDesc.Guid));
                        OnModifyAdd(unitEditData);
                    }
                }
            }
            
            /// <summary>
            /// 撤销改造添加
            /// </summary>
            public void UndoModifyAdd(int idx)
            {
                if (idx >= AddedUnits.Count)
                {
                    LogHelper.Error("Try to undo the {0}'s add action out of range");
                    return;
                }
                ModifyData data = AddedUnits[idx];
                AddedUnits.RemoveAt(idx);
                if (!EditMode.Instance.DeleteUnitWithCheck(data.ModifiedUnit.UnitDesc))
                {
                    ModifiedUnits.Insert(idx, data);
                    LogHelper.Error("Can't undo the {0}'s modify action when delete unit, unitdesc: {1}", idx,
                        data.ModifiedUnit);
                    return;
                }

                Messenger.Broadcast(EMessengerType.OnModifyUnitChanged);
                UpdateMaskEffects();
            }

            
            protected void OnModifyAdd(UnitEditData orig)
            {
                AddedUnits.Add(new ModifyData(orig, orig));
                Messenger.Broadcast(EMessengerType.OnModifyUnitChanged);
                UpdateMaskEffects();
            }
            
            /// <summary>
            /// 检查是否可以改造添加
            /// </summary>
            /// <returns><c>true</c>, if can modify add was checked, <c>false</c> otherwise.</returns>
            public bool CheckCanModifyAdd(UnitDesc unitDesc)
            {
                // 检查是否是删除的物体
                for (int i = 0, n = RemovedUnits.Count; i < n; i++)
                {
                    if (RemovedUnits[i].OrigUnit.UnitDesc.Guid == unitDesc.Guid)
                    {
                        // 出错的才会到这里
                        Messenger<string>.Broadcast(EMessengerType.GameLog, "不能在删除物体的原位置添加");
                        return false;
                    }
                }
                // 检查是否是改动的物体
                for (int i = 0, n = ModifiedUnits.Count; i < n; i++)
                {
                    if (ModifiedUnits[i].OrigUnit.UnitDesc.Guid == unitDesc.Guid ||
                        ModifiedUnits[i].ModifiedUnit.UnitDesc.Guid == unitDesc.Guid)
                    {
                        Messenger<string>.Broadcast(EMessengerType.GameLog, "不能在变换物体的原位置添加");
                        return false;
                    }
                }
                // 检查是否达到了删除数量上限
                // todo 限制数量从玩家属性中取
                if (AddedUnits.Count >= LocalUser.Instance.MatchUserData.ReformAddUnitCapacity)
                {
                    Messenger<string>.Broadcast(EMessengerType.GameLog, "添加次数已用完");
                    return false;
                }

                // 检查地块数量是否够
                if (LocalUser.Instance.MatchUserData.GetCanAddUnitNumOfId(unitDesc.Id) <= 0)
                {
                    Messenger<string>.Broadcast(EMessengerType.GameLog, "这种地块已用完");
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
                for (int i = 0; i < AddedUnits.Count; i++) {
                    SetMaskEffectPos (GetUnusedYellowMask (yellowCnt++), AddedUnits [i].ModifiedUnit.UnitDesc.Guid);
                }
                // 所有删除位置、改动位置为红色
                for (int i = 0; i < RemovedUnits.Count; i++) {
                    SetMaskEffectPos (GetUnusedRedMask (redCnt++), RemovedUnits [i].OrigUnit.UnitDesc.Guid);
                }
                for (int i = 0; i < ModifiedUnits.Count; i++) {
                    if (ModifiedUnits [i].OrigUnit.UnitDesc.Guid != ModifiedUnits [i].ModifiedUnit.UnitDesc.Guid) {
                        SetMaskEffectPos (GetUnusedRedMask (redCnt++), ModifiedUnits [i].OrigUnit.UnitDesc.Guid);
                    }
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