using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UISoyPublishedProjectList : MonoBehaviour
    {
        private const long RequestInterval = 5 * GameTimer.Minute2Ticks;
        private const int ShowCount = 4;
        private User _user;
        private GameTimer _requestTimer;
        private bool _isRequest = false;
        private readonly List<UMCtrlProjectCardSimple> _itemList = new List<UMCtrlProjectCardSimple>();
        private List<CardDataRendererWrapper<Project>> _content = new List<CardDataRendererWrapper<Project>>();
        [SerializeField]
        private GameObject _contentDock;
        [SerializeField]
        private GameObject _emptyDock;
        [SerializeField]
        private GameObject _loadingDock;
        [SerializeField]
        private RectTransform _itemDock;
        [SerializeField]
        private Button _moreBtn;

        private void Awake()
        {
            _moreBtn.onClick.AddListener(OnMoreBtnClick);
        }

        private void RequestData()
        {
//            if(_isRequest)
//            {
//                return;
//            }
//            if(_user == null)
//            {
//                return;
//            }
//            _isRequest = true;
//            Msg_CA_RequestPublishedProject msg = new Msg_CA_RequestPublishedProject();
//            msg.MaxCount = ShowCount;
//            msg.UserGuid = _user.UserGuid;
//            User user = _user;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_ProjectList>(SoyHttpApiPath.GetPublishedProjectList, msg, ret=>{
//                _isRequest = false;
////                user.OnSyncUserPublishedProjectList(ret);
//                if(_user == user)
//                {
//                    _requestTimer.Reset();
//                    SetData();
//                }
//            }, (code, msgStr)=>{
//                _isRequest = false;
//                if(_user == user)
//                {
//                    _contentDock.SetActive(false);
//                    _emptyDock.SetActive(false);
//                    _loadingDock.SetActive(false);
//                }
//            });
        }

        public void SetUser(User user)
        {
            _user = user;
            _requestTimer = _user.GetPublishedPrjectRequestTimer();
        }

        public void Refresh()
        {
            var pList = _user.GetPublishedProjectList();
            if(pList == null)
            {
                _contentDock.SetActive(false);
                _emptyDock.SetActive(false);
                _loadingDock.SetActive(true);
                RequestData();
            }
            else
            {
                if(_requestTimer.GetInterval() > RequestInterval)
                {
                    RequestData();
                }
            }
            SetData();
        }

        private void SetData()
        {
            _content.Clear();
            var pList = _user.GetPublishedProjectList();
            if(pList != null)
            {
                for(int i=0, maxLen = Mathf.Min(pList.Count, ShowCount); i<maxLen; i++)
                {
                    var wrapper = new CardDataRendererWrapper<Project>(pList[i], OnItemClick);
                    wrapper.CardMode = ECardMode.Normal;
                    wrapper.IsSelected = false;
                    _content.Add(wrapper);
                }
                if(pList.Count == 0)
                {
                    _contentDock.SetActive(false);
                    _emptyDock.SetActive(true);
                    _loadingDock.SetActive(false);
                }
                else
                {
                    _contentDock.SetActive(true);
                    _emptyDock.SetActive(false);
                    _loadingDock.SetActive(false);
                }
            }

            SetItemListLength(_content.Count);
            for (var i = 0; i < _content.Count; i++)
            {
                _itemList[i].Set(_content[i]);
            }
        }

        private void SetItemListLength(int len)
        {
            if (_itemList.Capacity < len)
            {
                _itemList.Capacity = len;
            }
            for (var i = _itemList.Count - 1; i >= len; i--)
            {
                _itemList[i].Destroy();
                _itemList.RemoveAt(i);
            }
            for (var i = _itemList.Count; i < len; i++)
            {
                UMCtrlProjectCardSimple card = new UMCtrlProjectCardSimple();
                card.Init(_itemDock);
                _itemList.Add(card);
            }
        }

        private void OnItemClick(CardDataRendererWrapper<Project> item)
        {
            ProjectParams param = new ProjectParams(){
                Type = EProjectParamType.Project,
                Project = item.Content
            };
            SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(param);
        }

        private void OnMoreBtnClick()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlPublishedProjects>(_user);
        }
    }
}