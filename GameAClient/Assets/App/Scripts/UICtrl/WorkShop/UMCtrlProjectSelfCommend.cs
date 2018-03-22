using DG.Tweening;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlProjectSelfCommend : UMCtrlBase<UMViewProjectSelfCommend>, IDataItemRenderer
    {
        private CardDataRendererWrapper<UserSelfRecommendProject> _wrapper;
        private int _index;
        public int Index { get; set; }
        private const string NewProject = "创建新关卡";
        private const string NoName = "未命名";
        private const string NoProject = "可添加";
        private const string UnLockText = "未解锁";
        private GridDataScroller _gridDataScroller;
        private int _allowIndex;

        private static readonly Color LockColor =
            new Color(211.0f / 255.0f, 189.0f / 255.0f, 165.0f / 255.0f, 125.0f / 255.0f);

        private static readonly Color DefaultColor =
            new Color(211.0f / 255.0f, 189.0f / 255.0f, 165.0f / 255.0f, 255.0f / 255.0f);

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public object Data
        {
            get { return _wrapper.Content.ProjectData; }
        }

        private Vector2 _orgPos = Vector2.zero;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.AddProjectBtn.onClick.AddListener(SocialGUIManager.Instance.GetUI<UICtrlWorkShop>()
                .OpenAddSelfRecommendPanel);
            Messenger.AddListener(EMessengerType.OnWorkShopSelfRecommendEditBtn, ResponseRemoveBtn);
            _cachedView.SelectBtn.onClick.AddListener(OnSelectBtn);
            _cachedView.UnSelectBtn.onClick.AddListener(OnUnSelectBtn);
            _orgPos = _cachedView.ProjectRect.anchoredPosition;
        }

        protected override void OnDestroy()
        {
            if (_wrapper != null)
            {
                _wrapper.OnDataChanged -= RefreshView;
            }

            base.OnDestroy();
        }

        private void OnCardClick()
        {
            _wrapper.FireOnClick();
        }

        public void Set(object obj)
        {
            _cachedView.ProjectRect.anchoredPosition = _orgPos;
            if (_wrapper != null)
            {
                _wrapper.OnDataChanged -= RefreshView;
            }

            _wrapper = obj as CardDataRendererWrapper<UserSelfRecommendProject>;
            if (_wrapper != null)
            {
                _wrapper.OnDataChanged += RefreshView;
            }

            RefreshView();
        }

        public void Unload()
        {
            Debug.Log("Unload");
        }

        public void RefreshView()
        {
            if (_wrapper == null)
            {
                return;
            }

            _allowIndex = LocalUser.Instance.UserSelfRecommendProjectStatistic.TotalCount;
            if (_wrapper.Content.SlotInx >= _allowIndex)
            {
                RefreshUnLock();
            }
            else
            {
                RefreshProjectView();
            }
        }

        private void RefreshProjectView()
        {
            bool emptyProject = _wrapper.Content.ProjectData == null;
            _cachedView.IndexText.text = (_wrapper.Content.SlotInx + 1).ToString();
            if (!emptyProject)
            {
                _cachedView.ProjectObj.SetActiveEx(true);
                _cachedView.AddProjectBtn.SetActiveEx(false);
                _cachedView.LastTimeText.SetActiveEx(false);
                _cachedView.SelectBtn.SetActiveEx(false);
                _cachedView.UnSelectBtn.SetActiveEx(false);
                _cachedView.LockImage.SetActiveEx(false);
                Project p = _wrapper.Content.ProjectData;
                if (string.IsNullOrEmpty(p.Name))
                {
                    DictionaryTools.SetContentText(_cachedView.TileText, NoName);
                }
                else
                {
                    DictionaryTools.SetContentText(_cachedView.TileText, p.Name);
                }

                ImageResourceManager.Instance.SetDynamicImage(_cachedView.ProjectBgImage, p.IconPath,
                    _cachedView.DefualtTexture);
            }
            else
            {
                RefreshNoProject();
            }
        }

        private void RefreshNoProject()
        {
            _cachedView.ProjectObj.SetActiveEx(false);
            _cachedView.LockImage.SetActiveEx(false);
            _cachedView.AddProjectBtn.SetActiveEx(true);
            _cachedView.SelectBtn.SetActiveEx(false);
            _cachedView.UnSelectBtn.SetActiveEx(false);
            _cachedView.IndexText.text = (_wrapper.Content.SlotInx + 1).ToString();
            _cachedView.TileText.text = NoProject;
            _cachedView.BgImage.color = DefaultColor;
        }

        private void RefreshUnLock()
        {
            _cachedView.ProjectObj.SetActiveEx(false);
            _cachedView.LockImage.SetActiveEx(true);
            _cachedView.AddProjectBtn.SetActiveEx(false);
            _cachedView.SelectBtn.SetActiveEx(false);
            _cachedView.UnSelectBtn.SetActiveEx(false);
            _cachedView.TileText.text = UnLockText;
            _cachedView.BgImage.color = LockColor;
        }

        private void ResponseRemoveBtn()
        {
            if (_wrapper != null)
            {
                if (_wrapper.Content.ProjectData != null)
                {
                    _cachedView.SelectBtn.SetActiveEx(true);
                }
            }
        }

        private void OnSelectBtn()
        {
            if (SocialGUIManager.Instance.GetUI<UICtrlWorkShop>().OnSelectBtn(_wrapper.Content))
            {
                _cachedView.UnSelectBtn.SetActiveEx(true);
            }
        }

        private void OnUnSelectBtn()
        {
            if (SocialGUIManager.Instance.GetUI<UICtrlWorkShop>().OnUnSelectBtn(_wrapper.Content))
            {
                _cachedView.UnSelectBtn.SetActiveEx(false);
            }
        }

        public void SetScrollRect(GridDataScroller dataScrollRect)
        {
            _gridDataScroller = dataScrollRect;
            _cachedView.ProjectDragHelper.ScrollRect = dataScrollRect.ScrollRect;
            _cachedView.ProjectDragHelper.OnBeginDragAction = OnDragBengin;
            _cachedView.ProjectDragHelper.OnDragAction = OnDrag;
            _cachedView.ProjectDragHelper.OnEndDragAction = OnDragEnd;
        }

        private int _benginIndex;

        private void OnDragBengin()
        {
            _benginIndex = Index;
            _cachedView.ProjectRect.parent = _gridDataScroller.ScrollRect.content;
            _cachedView.TileText.text = NoProject;
            _cachedView.AddProjectBtn.SetActiveEx(true);
        }

        private int newindex = -1;

        private void OnDrag()
        {
            int temptindex = _gridDataScroller.GetItemIndexByPos(GetProjectToScrollPos());
            if (newindex != temptindex)
            {
                newindex = temptindex;
                if (newindex < _allowIndex)
                {
                    _gridDataScroller.OnItemDragMovePos(_benginIndex, newindex);
                    _cachedView.ProjectRect.SetAsLastSibling();
                }
            }
        }

        private void OnDragEnd()
        {
            if (newindex >= _allowIndex)
            {
                newindex = _allowIndex - 1;
            }

            if (newindex != -1 && _benginIndex != newindex)
            {
                Vector2 targerpos = GetLocalPos(_gridDataScroller.GetPosByIndex(newindex));
                _cachedView.ProjectRect.anchoredPosition = targerpos;
                _gridDataScroller.EndTween();
                SocialGUIManager.Instance.GetUI<UICtrlWorkShop>().OnUmProjectDragEnd(_benginIndex, newindex);
            }
            else
            {
                _gridDataScroller.EndTween();
                Vector2 targerpos = _gridDataScroller.GetPosByIndex(_benginIndex);
                _cachedView.Trans.anchoredPosition = targerpos;
                SocialGUIManager.Instance.GetUI<UICtrlWorkShop>().OnUmProjectDragEnd(0, 0);
            }

            _cachedView.ProjectRect.parent = _cachedView.Trans;
            _cachedView.ProjectRect.anchoredPosition = _orgPos;
        }

        private Tween _tween;

        public override void MoveByIndex(int beginIndex, int judgeindex)
        {
            if (beginIndex == Index)
            {
                return;
            }

            int dragIndex = Index;
            if (Index > beginIndex)
            {
                dragIndex = Index - 1;
            }

            if (_tween != null)
            {
                if (_tween.IsPlaying())
                {
                    _tween.Complete();
                }
            }

            if (dragIndex < judgeindex)
            {
                Vector2 targerpos = GetLocalPos(_gridDataScroller.GetPosByIndex(dragIndex));
                _tween = _cachedView.ProjectRect.DOAnchorPos(targerpos, 1.0f);
                _cachedView.ProjectRect.parent = _gridDataScroller.ScrollRect.content;
                if (!_cachedView.LockImage.IsActive())
                {
                    _cachedView.TileText.text = NoProject;
                    _cachedView.AddProjectBtn.SetActiveEx(true);
                }

                _tween.Play();
            }
            else
            {
                Vector2 targerpos = GetLocalPos(_gridDataScroller.GetPosByIndex(dragIndex + 1));
                _tween = _cachedView.ProjectRect.DOAnchorPos(targerpos, 1.0f);
                _cachedView.ProjectRect.parent = _gridDataScroller.ScrollRect.content;
                if (!_cachedView.LockImage.IsActive())
                {
                    _cachedView.TileText.text = NoProject;
                    _cachedView.AddProjectBtn.SetActiveEx(true);
                }

                _tween.Play();
            }
        }

        public override void EndTween()
        {
            if (_tween != null && _tween.IsPlaying())
            {
                _tween.Complete();
            }

            _cachedView.ProjectRect.parent = _cachedView.Trans;
            _cachedView.ProjectRect.anchoredPosition = _orgPos;
        }

        private Vector2 GetProjectToScrollPos()
        {
            Vector2 pos = Vector2.down;
            pos = _gridDataScroller.ScrollRect.content.InverseTransformPoint(_cachedView.ProjectRect.position);
            return pos;
        }

        private Vector2 GetLocalPos(Vector2 postoScroll)
        {
            Vector2 localpos = Vector2.zero;
//            Vector2 worldpos = _gridDataScroller.ScrollRect.content.TransformPoint(postoScroll);
//            localpos = _cachedView.Trans.InverseTransformPoint(worldpos);
            localpos = _orgPos + postoScroll;
            return localpos;
        }
    }
}