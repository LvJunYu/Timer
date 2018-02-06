using HedgehogTeam.EasyTouch;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public partial class EditModeState
    {
        public class ModifyModify : ModifyGenericBase<ModifyModify>
        {
            public override void Enter(EditMode owner)
            {
                UpdateMaskEffects();
            }

            public override void Exit(EditMode owner)
            {
                Drop();
                base.Exit(owner);
            }

            public override void Execute(EditMode owner)
            {
                var boardData = GetBlackBoard();
                if (!boardData.DragInCurrentState)
                {
                    return;
                }
                Drag(Input.mousePosition);
            }

            public override void OnDragStart(Gesture gesture)
            {
                var boardData = GetBlackBoard();
                boardData.DragInCurrentState = false;
                UnitDesc touchedUnitDesc;
                Vector2 mousePos = gesture.startPosition;
                var mouseWorldPos = GM2DTools.ScreenToWorldPoint(mousePos);
                if (!EditHelper.TryGetUnitDesc(mouseWorldPos, boardData.EditorLayer, out touchedUnitDesc))
                {
                    return;
                }
                if (!CheckCanModifyModify(touchedUnitDesc, touchedUnitDesc))
                {
                    return;
                }
                var data = boardData.GetStateData<Data>();
                if (null != data.MovingRoot)
                {
                    Object.Destroy(data.MovingRoot.gameObject);
                    data.MovingRoot = null;
                }
                var unitPos = mouseWorldPos;
                UnitBase unitBase;
                if (ColliderScene2D.CurScene.TryGetUnit(touchedUnitDesc.Guid, out unitBase))
                {
                    unitPos = GM2DTools.TileToWorld(unitBase.CenterPos);
                }
                var unitExtra = DataScene2D.CurScene.GetUnitExtra(touchedUnitDesc.Guid);
                var rootGo = EditHelper.CreateDragRoot(unitPos, touchedUnitDesc.Id,
                    (EDirectionType) touchedUnitDesc.Rotation, out unitBase);
                data.CurrentMovingUnitBase = unitBase;
                data.DragUnitExtra = unitExtra;
                data.MovingRoot = rootGo.transform;
                data.MouseObjectOffsetInWorld = unitPos - mouseWorldPos;
                boardData.CurrentTouchUnitDesc = touchedUnitDesc;
                boardData.DragInCurrentState = true;
                data.MouseActualPos = mousePos;
               
            }

            public override void OnDragEnd(Gesture gesture)
            {
                Drop();
            }

            public override void OnTap(Gesture gesture)
            {
                UnitDesc touchedUnitDesc;
                if (!EditHelper.TryGetUnitDesc(GM2DTools.ScreenToWorldPoint(gesture.position),
                    GetBlackBoard().EditorLayer, out touchedUnitDesc))
                {
                    return;
                }
                if (!CheckCanModifyModify(touchedUnitDesc, touchedUnitDesc))
                {
                    return;
                }
                var touchedUnitExtra = DataScene2D.CurScene.GetUnitExtra(touchedUnitDesc.Guid);
                if (EditHelper.TryEditUnitData(touchedUnitDesc))
                {
                    UnitBase unit;
                    if (ColliderScene2D.CurScene.TryGetUnit(touchedUnitDesc.Guid, out unit))
                    {
                        UnitDesc newUnitDesc = unit.UnitDesc;
                        var newUnitExtra = DataScene2D.CurScene.GetUnitExtra(newUnitDesc.Guid);
                        OnModifyModify(new UnitEditData(touchedUnitDesc, touchedUnitExtra),
                            new UnitEditData(newUnitDesc, newUnitExtra));
                    }
                }
            }

            private void Drag(Vector2 mousePos)
            {
                var boardData = GetBlackBoard();
                if (!boardData.DragInCurrentState)
                {
                    return;
                }
                var stateData = boardData.GetStateData<Data>();
                stateData.MouseActualPos = Vector2.Lerp(stateData.MouseActualPos, mousePos, 20 * Time.deltaTime);
                Vector3 realMousePos = GM2DTools.ScreenToWorldPoint(stateData.MouseActualPos);
                // 把物体放在摄像机裁剪范围内
                realMousePos.z = -50;
                stateData.MovingRoot.position = realMousePos + stateData.MouseObjectOffsetInWorld;

                // 摇晃和缩放被拖拽物体
                Vector2 delta = stateData.MouseActualPos - mousePos;
                stateData.MovingRoot.eulerAngles = new Vector3(0, 0, Mathf.Clamp(delta.x * 0.5f, -45f, 45f));
                if (delta.y > 0)
                {
                    if (delta.y < 15)
                    {
                        delta.y = 0;
                    }
                    else
                    {
                        delta.y -= 15;
                    }
                }
                else if (delta.y < 0)
                {
                    if (delta.y > -15)
                    {
                        delta.y = 0;
                    }
                    else
                    {
                        delta.y += 15;
                    }
                }
                stateData.MovingRoot.localScale = new Vector3(
                    Mathf.Clamp(1f + delta.y * 0.0025f, 0.8f, 1.2f),
                    Mathf.Clamp(1f - delta.y * 0.005f, 0.8f, 1.2f),
                    1f);
            }

            private void Drop()
            {
                var boardData = GetBlackBoard();
                if (!boardData.DragInCurrentState)
                {
                    return;
                }
                var stateData = boardData.GetStateData<Data>();

                ProcessDrop(boardData, stateData);

                boardData.DragInCurrentState = false;
                if (null != stateData.CurrentMovingUnitBase)
                {
                    UnitManager.Instance.FreeUnitView(stateData.CurrentMovingUnitBase);
                }
                if (null != stateData.MovingRoot)
                {
                    Object.Destroy(stateData.MovingRoot.gameObject);
                    stateData.MovingRoot = null;
                }
                boardData.CurrentTouchUnitDesc = UnitDesc.zero;
                stateData.DragUnitExtra = null;
            }

            private void ProcessDrop(EditMode.BlackBoard boardData, Data stateData)
            {
                Vector3 mouseWorldPos = GM2DTools.ScreenToWorldPoint(stateData.MouseActualPos);
                mouseWorldPos += stateData.MouseObjectOffsetInWorld;
                UnitDesc target;
                if (!EditHelper.TryGetCreateKey(mouseWorldPos, stateData.CurrentMovingUnitBase.Id, out target))
                {
                    return;
                }
                target.Scale = stateData.CurrentMovingUnitBase.Scale;
                target.Rotation = stateData.CurrentMovingUnitBase.Rotation;
                float minDepth, maxDepth;
                EditHelper.GetMinMaxDepth(boardData.EditorLayer, out minDepth, out maxDepth);
                var coverUnits =
                    DataScene2D.GridCastAllReturnUnits(target, JoyPhysics2D.LayMaskAll, minDepth, maxDepth);
                if (coverUnits != null && coverUnits.Count > 0)
                {
                    Messenger<string>.Broadcast(EMessengerType.GameLog, "只能移动或变换，不能覆盖");
                    return;
                }
                if (!CheckCanModifyModify(boardData.CurrentTouchUnitDesc, target))
                {
                    return;
                }
                if (EditMode.Instance.AddUnitWithCheck(target, stateData.DragUnitExtra))
                {
                    GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.EditLayItem);
                    OnModifyModify(new UnitEditData(boardData.CurrentTouchUnitDesc, stateData.DragUnitExtra),
                        new UnitEditData(target, stateData.DragUnitExtra));
                }
            }

            /// <summary>
            /// 检查是否可以改造修改
            /// </summary>
            /// <returns><c>true</c>, if can modify modify was chacked, <c>false</c> otherwise.</returns>
            /// <param name="orig">Original.</param>
            /// <param name="modified">Modified.</param>
            public bool CheckCanModifyModify(UnitDesc orig, UnitDesc modified)
            {
                // 检查目标位置是否存在删除物体
                for (int i = 0, n = RemovedUnits.Count; i < n; i++)
                {
                    if (RemovedUnits[i].OrigUnit.UnitDesc.Guid == modified.Guid)
                    {
                        Messenger<string>.Broadcast(EMessengerType.GameLog, "不能覆盖改造删除的物体");
                        return false;
                    }
                }
                // 检查原物体是否是添加物体
                for (int i = 0, n = AddedUnits.Count; i < n; i++)
                {
                    if (AddedUnits[i].ModifiedUnit.UnitDesc.Guid == orig.Guid)
                    {
                        Messenger<string>.Broadcast(EMessengerType.GameLog, "不能覆盖改造添加的物体");
                        return false;
                    }
                }
                // 检查目标位置是否是已修改物体的原位置
                if (orig != modified)
                {
                    for (int i = 0, n = ModifiedUnits.Count; i < n; i++)
                    {
                        if (ModifiedUnits[i].OrigUnit.UnitDesc.Guid == modified.Guid &&
                            ModifiedUnits[i].ModifiedUnit.UnitDesc.Guid != orig.Guid)
                        {
                            Messenger<string>.Broadcast(EMessengerType.GameLog, "不能移动到已移动物体的原位置");
                            return false;
                        }
                    }
                }
                // 检查数量前检查是不是修改之前的已修改地块
                bool reModify = false;
                for (int i = ModifiedUnits.Count - 1; i >= 0; i--)
                {
                    if (ModifiedUnits[i].ModifiedUnit.UnitDesc.Guid == orig.Guid)
                    {
                        reModify = true;
                    }
                }
                if (!reModify && ModifiedUnits.Count >= LocalUser.Instance.MatchUserData.ReformModifyUnitCapacity)
                {
                    Messenger<string>.Broadcast(EMessengerType.GameLog, "改变次数已用完");
                    return false;
                }
                return true;
            }


            protected void OnModifyModify(UnitEditData orig, UnitEditData modified)
            {
                // 检查是否是对已修改地块再次进行修改
                for (int i = ModifiedUnits.Count - 1; i >= 0; i--)
                {
                    if (ModifiedUnits[i].ModifiedUnit.UnitDesc.Guid == orig.UnitDesc.Guid)
                    {
                        // 如果经过变换又回到了初始状态
                        if (ModifiedUnits[i].OrigUnit == modified)
                        {
                            ModifiedUnits.RemoveAt(i);
                            Messenger.Broadcast(EMessengerType.OnModifyUnitChanged);
                        }
                        else
                        {
                            ModifyData data = ModifiedUnits[i];
                            data.ModifiedUnit = modified;
                            ModifiedUnits[i] = data;
                        }
                        UpdateMaskEffects();
                        return;
                    }
                }
                ModifiedUnits.Add(new ModifyData(orig, modified));
                UpdateMaskEffects();
                Messenger.Broadcast(EMessengerType.OnModifyUnitChanged);
            }

            /// <summary>
            /// 撤销改造修改
            /// </summary>
            public void UndoModifModify(int idx)
            {
                if (idx >= ModifiedUnits.Count)
                {
                    LogHelper.Error("Try to undo the {0}'s modify action out of range");
                    return;
                }
                ModifyData data = ModifiedUnits[idx];
                ModifiedUnits.RemoveAt(idx);
                //			bool success = true;

                if (!EditMode.Instance.DeleteUnitWithCheck(data.ModifiedUnit.UnitDesc))
                {
                    ModifiedUnits.Insert(idx, data);
                    LogHelper.Error("Can't undo the {0}'s modify action when delete unit, unitdesc: {1}", idx,
                        data.ModifiedUnit);
                    return;
                }
                else
                {
                    //                DataScene2D.Instance.ProcessUnitExtra(data.OrigUnit.UnitDesc.Guid, UnitExtra.zero);
                }
                if (!EditMode.Instance.AddUnitWithCheck(data.OrigUnit.UnitDesc, data.OrigUnit.UnitExtra))
                {
                    ModifiedUnits.Insert(idx, data);
                    LogHelper.Error("Can't undo the {0}'s modify action when add unit, unitdesc: {1}", idx,
                        data.OrigUnit);
                    return;
                }
                else
                {
                    //				DataScene2D.Instance.ProcessUnitExtra(data.OrigUnit.UnitDesc.Guid, data.OrigUnit.UnitExtra);
                }
                Messenger.Broadcast(EMessengerType.OnModifyUnitChanged);
                UpdateMaskEffects();
            }

            /// <summary>
            /// 更新表示当前改造操作已影响物体的蒙版特效
            /// </summary>
            protected virtual void UpdateMaskEffects()
            {
                var data = GetBlackBoard().GetStateData<Data>();
                int redCnt = 0;
                int yellowCnt = 0;
                for (int i = 0; i < ModifiedUnits.Count; i++)
                {
                    SetMaskEffectPos(GetUnusedYellowMask(yellowCnt++), ModifiedUnits[i].OrigUnit.UnitDesc.Guid);
                    if (ModifiedUnits[i].OrigUnit.UnitDesc.Guid != ModifiedUnits[i].ModifiedUnit.UnitDesc.Guid)
                    {
                        SetMaskEffectPos(GetUnusedYellowMask(yellowCnt++), ModifiedUnits[i].ModifiedUnit.UnitDesc.Guid);
                    }
                }
                // 所有添加位置为红色
                for (int i = 0; i < AddedUnits.Count; i++)
                {
                    SetMaskEffectPos(GetUnusedRedMask(redCnt++), AddedUnits[i].ModifiedUnit.UnitDesc.Guid);
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