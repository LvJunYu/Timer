using System;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.Events;

namespace GameA
{
    public class USCtrlUnitNpcTaskTargetBtn : USCtrlBase<USViewUnitNpcTaskTarget>
    {
        private const string ControlSpriteName = "icon_edit_kongzhi";
        private const string MonsterSpriteName = "icon_edit_daguai";
        private const string ColltionSpriteName = "icon_edit_daoju";
        private const string DialogSpriteName = "icon_edit_chuanhua";
        private const string QianzhiTaskSpriteName = "icon_edit_qianzhi";
        private Sprite _sprite;
        private int _targetIndex;
        private ENpcTargetType _type;
        public NpcTaskTargetDynamic TargetData;
        public DictionaryListObject TaskTargetListData;
        private string _unitIconName;
        private Sprite _unitIcon;
        private Table_Unit _uint;
        private IntVec3 _guid;
        private NpcTaskDynamic _taskData;

        public USViewUnitNpcTaskTarget View
        {
            get { return _cachedView; }
        }

        public void Clear()
        {
            _cachedView.ChoseBtn.onClick.RemoveAllListeners();
            _cachedView.DelteBtn.onClick.RemoveAllListeners();
        }

        public void SetSelectTarget(IntVec3 npcguid, NpcTaskTargetDynamic targetData, DictionaryListObject taskData,
            SeletTaskTargetType callback, UnityAction refresh)
        {
            Clear();
            _guid = npcguid;
            TargetData = targetData;
            TaskTargetListData = taskData;

            switch ((ENpcTargetType) TargetData.TaskType)
            {
                case ENpcTargetType.Colltion:
                    _unitIconName = ColltionSpriteName;
                    break;
                case ENpcTargetType.Moster:
                    _unitIconName = MonsterSpriteName;
                    break;
                case ENpcTargetType.Dialog:
                    _unitIconName = DialogSpriteName;
                    break;
                case ENpcTargetType.Contorl:
                    _unitIconName = ControlSpriteName;
                    break;
            }
            if (JoyResManager.Instance.TryGetSprite(_unitIconName, out _unitIcon))
            {
                _cachedView.IconImage.sprite = _unitIcon;
            }
            _cachedView.ChoseBtn.onClick.AddListener(() => { callback.Invoke(TargetData); });
            _cachedView.DelteBtn.onClick.AddListener(() =>
            {
                int index = 0;
                for (int i = 0; i < TaskTargetListData.Count; i++)
                {
                    if (TaskTargetListData.Get<NpcTaskTargetDynamic>(i).Equals(TargetData))
                    {
                        index = i;
                        break;
                    }
                }
                if (TargetData.TaskType == (int) ENpcTargetType.Contorl)
                {
                    IntVec3 unitGuid = TargetData.TargetGuid;
                    DataScene2D.CurScene.UnbindSwitch(_guid, unitGuid);
                }
                TaskTargetListData.RemoveAt(index);

                refresh.Invoke();
            });
            _cachedView.RightClick.RightMouseCallback = (() =>
            {
                _cachedView.DelteBtn.gameObject.SetActiveEx(!_cachedView.DelteBtn.gameObject.activeSelf);
            });
        }

        public void SetTriggerCondition(NpcTaskDynamic taskData,
            Action<TrrigerTaskType> callback, UnityAction refresh)
        {
            _taskData = taskData;

            switch ((TrrigerTaskType) _taskData.TriggerType)
            {
                case TrrigerTaskType.Colltion:
                    _unitIconName = ColltionSpriteName;
                    break;
                case TrrigerTaskType.Kill:
                    _unitIconName = MonsterSpriteName;
                    break;
                case TrrigerTaskType.FinishOtherTask:
                    _unitIconName = QianzhiTaskSpriteName;
                    break;
            }
            if (JoyResManager.Instance.TryGetSprite(_unitIconName, out _unitIcon))
            {
                _cachedView.IconImage.sprite = _unitIcon;
            }
            _cachedView.ChoseBtn.onClick.AddListener(
                () => { callback.Invoke((TrrigerTaskType) _taskData.TriggerType); });
            _cachedView.DelteBtn.onClick.AddListener(() =>
            {
                _taskData.TriggerType = (byte) TrrigerTaskType.None;
                refresh.Invoke();
            });
            _cachedView.RightClick.RightMouseCallback = (() =>
            {
                _cachedView.DelteBtn.gameObject.SetActiveEx(!_cachedView.DelteBtn.gameObject.activeSelf);
            });
        }

        public void AddNewTarget(UnityAction callback)
        {
            _cachedView.ChoseBtn.onClick.AddListener(callback);
        }

        public void SetEnable(bool enable)
        {
            _cachedView.SetActiveEx(enable);
        }
    }
}