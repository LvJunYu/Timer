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
    public class UMCtrlItem : UMCtrlBase<UMViewItem>
    {
        private Table_Unit _table;
        private bool _selected;
        private static ECheckBehaviour _checkBehaviour;
        private static Vector2 _startPos;
        private static readonly Vector2 CheckDelta = new Vector2(5f, 20f);
        private UMCtrlUnitProperty _umCtrlUnitProperty;
        private bool _isShow;

        public bool IsShow
        {
            get { return _isShow; }
        }

        public int UnitId
        {
            get { return _table.Id; }
        }

        public void Show()
        {
            if (IsShow)
            {
                return;
            }
            _isShow = true;
            _cachedView.gameObject.SetActive(true);
            Messenger<ushort>.AddListener(EMessengerType.OnSelectedItemChanged, OnSelectedItemChanged);
            Messenger<int>.AddListener(EMessengerType.OnUnitAddedInEditMode, OnUnitAddedInEditMode);
            Messenger<int>.AddListener(EMessengerType.OnUnitDeletedInEditMode, OnUnitAddedInEditMode);
            Messenger<int>.AddListener(EMessengerType.OnEditUnitDefaultDataChange, OnEditUnitDefaultDataChange);
            if (UnitDefine.IsSpawn(_table.Id))
            {
                Messenger.AddListener(EMessengerType.OnTeamChanged, RefreshSprite);
            }
        }

        public void Hide()
        {
            if (!IsShow)
            {
                return;
            }
            _isShow = false;
            _cachedView.gameObject.SetActive(false);
            Messenger<ushort>.RemoveListener(EMessengerType.OnSelectedItemChanged, OnSelectedItemChanged);
            Messenger<int>.RemoveListener(EMessengerType.OnUnitAddedInEditMode, OnUnitAddedInEditMode);
            Messenger<int>.RemoveListener(EMessengerType.OnUnitDeletedInEditMode, OnUnitAddedInEditMode);
            Messenger<int>.RemoveListener(EMessengerType.OnEditUnitDefaultDataChange, OnEditUnitDefaultDataChange);
            if (UnitDefine.IsSpawn(_table.Id))
            {
                Messenger.RemoveListener(EMessengerType.OnTeamChanged, RefreshSprite);
            }
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
                    var current = EditHelper.GetUnitDefaultData(_table.Id);
                    EDirectionType rotate = EDirectionType.Up;
                    UnitExtra unitExtra = current.UnitExtra;
                    if (_table.CanEdit(EEditType.Direction))
                    {
                        rotate = (EDirectionType) current.UnitDesc.Rotation;
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
            if (_selected)
            {
                if (EditMode.Instance.IsInState(EditModeState.Add.Instance)
                    || EditMode.Instance.IsInState(EditModeState.ModifyAdd.Instance))
                {
                    UnitDesc unitDesc = EditHelper.GetUnitDefaultData(_table.Id).UnitDesc;
                    EditHelper.TryEditUnitData(unitDesc);
                }
                else
                {
                    SocialGUIManager.Instance.GetUI<UICtrlItem>().SelectItem(_table);
                }
            }
            else
            {
                SocialGUIManager.Instance.GetUI<UICtrlItem>().SelectItem(_table);
            }
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
            RefreshSprite();
            if (_selected)
            {
                _cachedView.SpriteIcon.transform.transform.localPosition = Vector3.up * 15;
                _cachedView.ShadowTrans.localScale = Vector3.one * 0.7f;
            }
            else
            {
                _cachedView.SpriteIcon.transform.transform.localPosition = Vector3.zero;
                _cachedView.ShadowTrans.localScale = Vector3.one;
            }
            RefreshUnitProperty();

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

        private void RefreshSprite()
        {
            if (UnitDefine.IsSpawn(_table.Id))
            {
                var define = EditHelper.GetUnitDefaultData(UnitDefine.SpawnId);
                _cachedView.SpriteIcon.sprite = TeamManager.GetSpawnSprite(define.UnitExtra.TeamId);
            }
            else
            {
                Sprite texture;
                if (JoyResManager.Instance.TryGetSprite(_table.Icon, out texture))
                {
                    _cachedView.SpriteIcon.sprite = texture;
                }
                else
                {
                    LogHelper.Error("tableUnit {0} icon {1} invalid! tableUnit.EGeneratedType is {2}", _table.Id,
                        _table.Icon, _table.EGeneratedType);
                }
            }
        }

        private void OnSelectedItemChanged(ushort id)
        {
            if (id == _table.Id)
            {
                Set(_table, true);
            }
            else
            {
                if (_selected)
                {
                    Set(_table, false);
                }
            }
        }

        private void OnEditUnitDefaultDataChange(int id)
        {
            if (id == _table.Id)
            {
                RefreshUnitProperty();
            }
        }

        private void RefreshUnitProperty()
        {
            if (!_selected || !EditHelper.CheckCanEdit(_table.Id))
            {
                SocialGUIManager.Instance.GetUI<UICtrlItem>()
                    .ReturnUmCtrlUnitProperty(_cachedView.Trans, _umCtrlUnitProperty);
                return;
            }
            _umCtrlUnitProperty = SocialGUIManager.Instance.GetUI<UICtrlItem>().GetUmCtrlUnitProperty();
            _umCtrlUnitProperty.UITran.SetParent(_cachedView.Trans, false);
            _umCtrlUnitProperty.UITran.localPosition = Vector3.up * 15;
            _umCtrlUnitProperty.UITran.localScale = Vector3.one * 0.47f;
            var unitDefaultData = EditHelper.GetUnitDefaultData(_table.Id);
            _umCtrlUnitProperty.SetData(ref unitDefaultData.UnitDesc, ref unitDefaultData.UnitExtra);
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

        private enum ECheckBehaviour
        {
            None,
            Scroll,
            Drag
        }
    }
}