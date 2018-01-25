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

        private Sprite _sprite;
        private int _targetIndex;
        private ENpcTargetType _type;
        public NpcTaskTargetDynamic TargetData { get; private set; }
        public DictionaryListObject TaskData { get; private set; }
        private string _unitIconName;
        private Sprite _unitIcon;
        private Table_Unit _uint;

        public USViewUnitNpcTaskTarget View
        {
            get { return _cachedView; }
        }

        public void SetSelectTarget(NpcTaskTargetDynamic targetData, DictionaryListObject taskData,
            SeletTaskTargetType callback, UnityAction refresh)
        {
            TargetData = targetData;
            TaskData = taskData;

            switch ((ENpcTargetType) targetData.TaskType)
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
            _cachedView.ChoseBtn.onClick.AddListener(() => { callback.Invoke(targetData); });
            _cachedView.DelteBtn.onClick.AddListener(() =>
            {
                int index = 0;
                for (int i = 0; i < taskData.Count; i++)
                {
                    if (taskData.ToList<NpcTaskTargetDynamic>()[i].Equals(targetData))
                    {
                        index = i;
                    }
                }
                taskData.Remove(index);
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