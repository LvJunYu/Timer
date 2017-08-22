/********************************************************************
** Filename : UMCtrlItem
** Author : Dong
** Date : 2015/7/29 星期三 下午 6:45:32
** Summary : UMCtrlItem
***********************************************************************/


using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameA
{
    [Poolable(MinPoolSize = 1, PreferedPoolSize = 10, MaxPoolSize = 1000)]
    public class UMCtrlItem : UMCtrlBase<UMViewItem>
    {
        private Table_Unit _table;
        private bool _selected;
        private static ECheckBehaviour _checkBehaviour;
        private static Vector2 _startPos;
        private static readonly Vector2 CheckDelta = new Vector2(5f, 20f);

        public void Show()
        {
            _cachedView.gameObject.SetActive(true);
            Messenger<ushort>.AddListener (EMessengerType.OnSelectedItemChanged, OnSelectedItemChanged);
            Messenger<int>.AddListener (EMessengerType.OnUnitAddedInEditMode, OnUnitAddedInEditMode);
            Messenger<int>.AddListener (EMessengerType.OnUnitDeletedInEditMode, OnUnitAddedInEditMode);
        }

        public void Hide()
        {
            _cachedView.gameObject.SetActive(false);
            Messenger<ushort>.RemoveListener (EMessengerType.OnSelectedItemChanged, OnSelectedItemChanged);
            Messenger<int>.RemoveListener (EMessengerType.OnUnitAddedInEditMode, OnUnitAddedInEditMode);
            Messenger<int>.RemoveListener (EMessengerType.OnUnitDeletedInEditMode, OnUnitAddedInEditMode);
        }

        public void OnDestroyObject()
        {
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            
            _cachedView.EventTrigger.AddListener(EventTriggerType.PointerClick, OnClick);
            _cachedView.EventTrigger.AddListener(EventTriggerType.BeginDrag, OnBeginDrag);
            _cachedView.EventTrigger.AddListener(EventTriggerType.Drag, OnDrag);
            _cachedView.EventTrigger.AddListener(EventTriggerType.EndDrag, OnEndDrag);
            
        }

        private void OnBeginDrag(BaseEventData eventData)
        {
//            Debug.Log("OnBeginDrag");
            PointerEventData pointerEventData = eventData as PointerEventData;
            if (null == pointerEventData)
            {
                return;
            }
            _checkBehaviour = ECheckBehaviour.None;
            _startPos = pointerEventData.position;
        }

        private void OnDrag(BaseEventData eventData)
        {
//            Debug.Log("OnDrag");
            PointerEventData pointerEventData = eventData as PointerEventData;
            if (null == pointerEventData)
            {
                return;
            }
            if (ECheckBehaviour.None == _checkBehaviour)
            {
                var delta = pointerEventData.position - _startPos;
//                Debug.Log("Delta: " + delta);
                if (delta.y > CheckDelta.y)
                {
                    _checkBehaviour = ECheckBehaviour.Drag;
                    var current = EditHelper.GetUnitOrigDirOrRot(_table);
                    EDirectionType rotate = EDirectionType.Up;
                    UnitExtra unitExtra = UnitExtra.zero;
                    if (_table.CanRotate)
                    {
                        rotate = (EDirectionType) current;
                    }
                    var mouseWorldPos = GM2DTools.ScreenToWorldPoint(pointerEventData.position);
                    EditMode.Instance.StartDragUnit(mouseWorldPos, mouseWorldPos, _table.Id, rotate, ref unitExtra);
                }
                else if (Mathf.Abs(delta.x) > CheckDelta.x)
                {
                    _checkBehaviour = ECheckBehaviour.Scroll;
                    _cachedView.SendMessageUpwards("OnBeginDrag", eventData, SendMessageOptions.DontRequireReceiver);
                }
            }
            else if (ECheckBehaviour.Scroll == _checkBehaviour)
            {
                _cachedView.SendMessageUpwards("OnDrag", eventData, SendMessageOptions.DontRequireReceiver);
            }
        }

        private void OnEndDrag(BaseEventData eventData)
        {
//            Debug.Log("OnEndDrag");
            PointerEventData pointerEventData = eventData as PointerEventData;
            if (null == pointerEventData)
            {
                return;
            }
            if (ECheckBehaviour.Scroll == _checkBehaviour)
            {
                _cachedView.SendMessageUpwards("OnEndDrag", eventData, SendMessageOptions.DontRequireReceiver);
            }
        }

        private void OnClick(BaseEventData eventData)
        {
//            Debug.Log("OnClick");
            PointerEventData pointerEventData = eventData as PointerEventData;
            if (null == pointerEventData || pointerEventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            OnItem();
        }


        private void OnItem()
        {
            if (_table.CanRotate || _table.CanMove || _table.Id == UnitDefine.RollerId)
            {
                if (_selected)
                {
                    EditHelper.ChangeUnitOrigDirOrRot(_table);
                    RefreshArrowRotation();
                }
            }
            SocialGUIManager.Instance.GetUI<UICtrlItem>().SelectItem(_table);
        }

        internal void Set(Table_Unit tableUnit, bool selected)
        {
            _table = tableUnit;
            _selected = selected;
            if (!_isViewCreated)
            {
                return;
            }
            _cachedView.SpriteIcon.sprite = null;
            _cachedView.SpriteIcon.SetActiveEx(true);
            Sprite texture;
            if (ResourcesManager.Instance.TryGetSprite(tableUnit.Icon, out texture))
            {
                _cachedView.SpriteIcon.sprite = texture;
            }
            else
            {
                LogHelper.Error("tableUnit {0} icon {1} invalid! tableUnit.EGeneratedType is {2}", tableUnit.Id,
                    tableUnit.Icon, tableUnit.EGeneratedType);
            }
            if (_selected)
            {
                _cachedView.SpriteIcon.transform.transform.localPosition = Vector3.up*15;

                // 除了主角，所有能旋转，能移动，还有传送带 都需要显示箭头
                if (_table.CanRotate || _table.CanMove || _table.Id == UnitDefine.RollerId)
                {
                    _cachedView.Arrow.SetActive(true);
                    RefreshArrowRotation();
                }
            }
            else
            {
                _cachedView.SpriteIcon.transform.transform.localPosition = Vector3.zero;
            }

            int currentCnt = EditHelper.GetUnitCnt(_table.Id);
            int limit = LocalUser.Instance.UserWorkshopUnitData.GetUnitLimt(_table.Id);
            int number = limit - currentCnt;
            if (number < 0) number = 0;
            if (number > 999)
            {
                //_cachedView.Number.gameObject.SetActive(false);
                //_cachedView.Unlimited.SetActive(true);
                //                    _cachedView.Number.gameObject.SetActive(false);
                //                    _cachedView.Unlimited.SetActive(true);
                _cachedView.Number.text = "999+";
            }
            else
            {
                //                    _cachedView.Unlimited.SetActive(false);
                _cachedView.Number.text = number.ToString();
            }
            _cachedView.Number.gameObject.SetActive(true);
        }


        private void OnSelectedItemChanged (ushort id)
        {
            if (id == _table.Id) {
                _selected = true;
                _cachedView.SpriteIcon.transform.transform.localPosition = Vector3.up * 15;
                _cachedView.ShadowTrans.localScale = Vector3.one * 0.7f;

                // 除了主角，所有能旋转，能移动，还有传送带 都需要显示箭头
                if (_table.CanRotate || _table.CanMove || _table.Id == UnitDefine.RollerId)
                {
                    _cachedView.Arrow.SetActive(true);
                    RefreshArrowRotation();
                }
            } else {
                _selected = false;
                _cachedView.SpriteIcon.transform.transform.localPosition = Vector3.zero;
                _cachedView.ShadowTrans.localScale = Vector3.one;
                
                _cachedView.Arrow.SetActive(false);
            }
        }

        private void OnUnitAddedInEditMode(int id)
        {
            if (id == _table.Id)
            {
                int currentCnt = EditHelper.GetUnitCnt(_table.Id);
                int limit = LocalUser.Instance.UserWorkshopUnitData.GetUnitLimt(_table.Id);
                int number = limit - currentCnt;
                if (number < 0) number = 0;
                if (number > 999)
                {
//                    _cachedView.Number.gameObject.SetActive(false);
//                    _cachedView.Unlimited.SetActive(true);
                    _cachedView.Number.text = "999+";
                }
                else
                {
//                    _cachedView.Unlimited.SetActive(false);
                    _cachedView.Number.text = number.ToString();
                }
                _cachedView.Number.gameObject.SetActive(true);
            }
        }

        private void RefreshArrowRotation()
        {
            var current = EditHelper.GetUnitOrigDirOrRot(_table);
            if (_table.CanMove || _table.Id == UnitDefine.RollerId)
            {
                _cachedView.Arrow.transform.localEulerAngles = new Vector3(0, 0, -90 * (byte)(current - 1));
            }
            else if (_table.CanRotate)
            {
                _cachedView.Arrow.transform.localEulerAngles = new Vector3(0, 0, -90 * (byte)(current));
            }
        }

        private enum ECheckBehaviour
        {
            None,
            Scroll,
            Drag,
        }
    }
}