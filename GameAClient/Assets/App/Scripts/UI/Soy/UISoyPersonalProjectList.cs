using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UISoyPersonalProjectList : MonoBehaviour
    {
        private readonly List<CardDataRendererWrapper<Project>> _content = new List<CardDataRendererWrapper<Project>>();
        [SerializeField]
        private RectTransform _dock;
        [SerializeField]
        private Text _workshopSizeText;
        [SerializeField]
        private Text _publishCountText;
        [SerializeField]
        private GridDataScroller _gridDataScroller;
        [SerializeField]
        private ScrollRect _scrollRect;
        [SerializeField]
        private LayoutElement _layoutElement;
        [SerializeField]
        private GameObject _topMenuDock;
        [SerializeField]
        private Text _topSelectedInfoText;
        [SerializeField]
        private Button _topDeleteButton;
        [SerializeField]
        private Button _topCancelButton;

        private UICtrlMatrixDetail _uiCtrlMatrixDetail;
        private int _currentSelectedCount;
        private UICtrlMatrixDetail.EMode _mode = UICtrlMatrixDetail.EMode.None;
        private bool _hasInit = false;
        private bool _load;
        private void Awake()
        {
            _topMenuDock.SetActive(false);
        }

        public void SetUICtrl(UICtrlMatrixDetail ui)
        {
            if(_hasInit)
            {
                return;
            }
            _hasInit = true;
            _uiCtrlMatrixDetail = ui;

            _gridDataScroller.SetCallback(OnItemRefresh, GetItemRenderer);

            _topDeleteButton.onClick.AddListener(OnDeleteClick);
            _topCancelButton.onClick.AddListener(OnCancelClick);
        }
        private void OnDeleteClick()
        {
            if(_currentSelectedCount == 0)
            {
                return;
            }
            _uiCtrlMatrixDetail.ProcessDelete();
        }
        private void OnCancelClick()
        {
            _uiCtrlMatrixDetail.SetMode(UICtrlMatrixDetail.EMode.Normal);
        }

        public void SetCardMode(UICtrlMatrixDetail.EMode mode)
        {
            for(int i=0; i<_content.Count; i++)
            {
                _content[i].CardMode = mode==UICtrlMatrixDetail.EMode.Edit ? ECardMode.Selectable : ECardMode.Normal;
                _content[i].IsSelected = false;
                _content[i].BroadcastDataChanged();
            }
            if(_mode != mode)
            {
                _mode = mode;
                RefreshScrollRectSize();
            }
            if(mode == UICtrlMatrixDetail.EMode.Edit)
            {
                _topMenuDock.SetActive(true);
            }
            else
            {
                _topMenuDock.SetActive(false);
            }
            RefreshMenu();
        }

        public List<Project> GetSelectedProjectList()
        {
            List<Project> list = new List<Project>(_currentSelectedCount);
            for(int i=0; i<_content.Count; i++)
            {
                if(_content[i].IsSelected)
                {
                    list.Add(_content[i].Content);
                }
            }
            return list;
        }

        public void Refresh()
        {
            if(LocalUser.Instance.User == null)
            {
                return;
            }
            _load = true;
            RequestData();
            RefreshView();
        }

        public void OnClose()
        {
            _load = false;
            _gridDataScroller.RefreshCurrent();
        }

        private void RequestData()
        {
            MatrixProjectTools.PreparePersonalProjectData(()=>{
                RefreshView();
            }, ()=>{
                
            });
        }

        private void RefreshView()
        {
//            var userMatrixData = AppData.Instance.UserMatrixData.GetData(_matrix.MatrixGuid);
            _content.Clear();
            int totalSize = 0;
            int pCount = 0;
//            if(userMatrixData != null)
//            {
                var pList = LocalUser.Instance.User.GetSavedProjectList();
                for(int i=0, maxLen = pList.Count; i<maxLen; i++)
                {
                    var wrapper = new CardDataRendererWrapper<Project>(pList[i], OnItemClick);
                    wrapper.CardMode = ECardMode.Normal;
                    wrapper.IsSelected = false;
                    _content.Add(wrapper);
                }
            totalSize = pList.Count;
                pCount = pList.Count;
//            }

            for(var i=_content.Count; i<totalSize; i++)
            {
                var wrapper = new CardDataRendererWrapper<Project>(null, OnItemClick);
                wrapper.CardMode = ECardMode.Normal;
                wrapper.IsSelected = false;
                _content.Add(wrapper);
            }
//            if(userMatrixData == null)
//            {
                _workshopSizeText.text = "-";
                _publishCountText.text = "-";
//            }
//            else
//            {
//                _workshopSizeText.text = string.Format("{0}/{1}", pCount, userMatrixData.PersonalProjectWorkshopSize);
//                _publishCountText.text = string.Format("{0}/{1}", userMatrixData.TodayPublishCount, userMatrixData.PublishCountEachDay);
//            }
            _gridDataScroller.SetItemCount(Mathf.Max(pCount, totalSize));
            _uiCtrlMatrixDetail.SetMode(UICtrlMatrixDetail.EMode.Normal);
            _currentSelectedCount = 0;
            _uiCtrlMatrixDetail.SetPersonalProjectCount(_content.Count);
            RefreshMenu();
            RefreshScrollRectSize();
        }


        private void RefreshMenu()
        {
            DictionaryTools.SetContentText(_topSelectedInfoText, string.Format("{0}/{1}", _currentSelectedCount, _content.Count));
        }

        private void RefreshScrollRectSize()
        {
            int titleHeight = 66;
            int menuHeight = 40;
            GridLayoutGroup grid = _gridDataScroller.GridLayoutGroup;
            int layoutHeight = grid.padding.top + (_content.Count+1)/2 * (int)(grid.spacing.y + grid.cellSize.y) + grid.padding.bottom - (int)grid.spacing.y;
            int pageHeight = (int)_uiCtrlMatrixDetail.UITrans.rect.height - titleHeight - (_mode==UICtrlMatrixDetail.EMode.Edit? menuHeight : 0);
            _layoutElement.preferredHeight = _layoutElement.minHeight = Mathf.Min(layoutHeight, pageHeight);
            _gridDataScroller.OnViewportSizeChanged();
        }

        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlPersonalProjectCard();
            item.Init(parent, Vector3.zero);
            return item;
        }

        private void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if(_content == null)
            {
                return;
            }
            if(inx >= _content.Count)
            {
                LogHelper.Error("OnItemRefresh Error Inx > count");
                return;
            }
            if(!_load)
            {
                item.Unload();
                return;
            }
            item.Set(_content[inx]);
        }

        private void OnItemClick(CardDataRendererWrapper<Project> item)
        {
            if(_mode == UICtrlMatrixDetail.EMode.Edit)
            {
                if(item.Content == null)
                {
                    return;
                }
                else
                {
                    item.IsSelected = !item.IsSelected;
                    item.BroadcastDataChanged();
                    if(item.IsSelected)
                    {
                        _currentSelectedCount++;
                        RefreshMenu();
                    }
                    else
                    {
                        _currentSelectedCount--;
                        RefreshMenu();
                    }
                }
            }
            else
            {
                if(item.Content == null)
                {
                    _uiCtrlMatrixDetail.ShowCreateCategoryMask ();
                }
                else
                {
                    AppLogicUtil.EditPersonalProject(item.Content);
                }
            }
        }
    }
}