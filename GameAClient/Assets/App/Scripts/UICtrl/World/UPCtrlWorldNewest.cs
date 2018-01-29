//using SoyEngine;
//
//namespace GameA
//{
//    public class UPCtrlWorldNewest : UPCtrlWorldPanelBase
//    {
//        private EMenu _curMenu = EMenu.None;
//        private UPCtrlWorldNewestPanelBase _curMenuCtrl;
//        private UPCtrlWorldNewestPanelBase[] _menuCtrlArray;
//
//        protected override void OnViewCreated()
//        {
//            base.OnViewCreated();
//            _menuCtrlArray = new UPCtrlWorldNewestPanelBase[(int) EMenu.Max];
//            
//            var upCtrlWorldAllNewestProject = new UPCtrlWorldAllNewestProject();
//            upCtrlWorldAllNewestProject.Set(_resScenary);
//            upCtrlWorldAllNewestProject.SetMenu(EMenu.All);
//            upCtrlWorldAllNewestProject.Init(_mainCtrl, _cachedView);
//            _menuCtrlArray[(int) EMenu.All] = upCtrlWorldAllNewestProject;
//
//            var upCtrlWorldFollowedUserProject = new UPCtrlWorldFollowedUserProject();
//            upCtrlWorldFollowedUserProject.Set(_resScenary);
//            upCtrlWorldFollowedUserProject.SetMenu(EMenu.Follow);
//            upCtrlWorldFollowedUserProject.Init(_mainCtrl, _cachedView);
//            _menuCtrlArray[(int) EMenu.Follow] = upCtrlWorldFollowedUserProject;
//
//            for (int i = 0; i < _cachedView.NewestButtonAry.Length; i++)
//            {
//                var inx = i;
//                _cachedView.NewestTabGroup.AddButton(_cachedView.NewestButtonAry[i], _cachedView.NewestSelectedButtonAry[i],
//                    b => ClickMenu(inx, b));
//                if (i < _menuCtrlArray.Length && null != _menuCtrlArray[i])
//                {
//                    _menuCtrlArray[i].Close();
//                }
//            }
//        }
//
//        public override void Open()
//        {
//            base.Open();
//            if (_curMenu == EMenu.None)
//            {
//                _cachedView.NewestTabGroup.SelectIndex((int) EMenu.All, true);
//            }
//            else
//            {
//                _cachedView.NewestTabGroup.SelectIndex((int) _curMenu, true);
//            }
//        }
//
//        public override void RequestData(bool append = false)
//        {
//        }
//
//        protected override void OnItemRefresh(IDataItemRenderer item, int inx)
//        {
//        }
//
//        public override void Clear()
//        {
//            base.Clear();
//            for (int i = 0; i < _menuCtrlArray.Length; i++)
//            {
//                if (_menuCtrlArray[i] != null)
//                {
//                    _menuCtrlArray[i].Clear();
//                }
//            }
//        }
//
//        private void ChangeMenu(EMenu menu)
//        {
//            _cachedView.SearchPannelRtf.SetActiveEx(false);
//            if (_curMenuCtrl != null)
//            {
//                _curMenuCtrl.Close();
//            }
//            _curMenu = menu;
//            var inx = (int) _curMenu;
//            if (inx < _menuCtrlArray.Length)
//            {
//                _curMenuCtrl = _menuCtrlArray[inx];
//            }
//            if (_curMenuCtrl != null)
//            {
//                _curMenuCtrl.Open();
//            }
//        }
//
//        private void ClickMenu(int selectInx, bool open)
//        {
//            if (open)
//            {
//                ChangeMenu((EMenu) selectInx);
//            }
//        }
//
//        public enum EMenu
//        {
//            None = -1,
//            All,
//            Follow,
//            Max
//        }
//    }
//}