using SoyEngine;

namespace GameA
{
    public class UPCtrlWorldNewest : UPCtrlWorldPanelBase
    {
        private UPCtrlWorldNewestPanelBase _curMenuCtrl;
        private UPCtrlWorldNewestPanelBase[] _menuCtrlArray;
        
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            var upCtrlWorldAllNewestProject = new UPCtrlWorldAllNewestProject();
            upCtrlWorldAllNewestProject.Set(_resScenary);
            upCtrlWorldAllNewestProject.SetMenu(EMenu.All);
            upCtrlWorldAllNewestProject.Init(_mainCtrl, _cachedView);
            _menuCtrlArray[(int) EMenu.All] = upCtrlWorldAllNewestProject;

            var upCtrlWorldFollowedUserProject = new UPCtrlWorldFollowedUserProject();
            upCtrlWorldFollowedUserProject.Set(_resScenary);
            upCtrlWorldFollowedUserProject.SetMenu(EMenu.Follow);
            upCtrlWorldFollowedUserProject.Init(_mainCtrl, _cachedView);
            _menuCtrlArray[(int) EMenu.Follow] = upCtrlWorldFollowedUserProject;
        }

        public override void Open()
        {
            base.Open();
            
        }

        public override void RequestData(bool append = false)
        {
            
        }

        protected override void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            
        }


        public enum EMenu
        {
            None = -1,
            All,
            Follow,
            Max
        }
    }
}