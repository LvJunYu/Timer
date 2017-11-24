/********************************************************************
** Filename : AppData
** Author : quansiwei
** Date : 16/4/18 下午7:12:54
** Summary : AppData
***********************************************************************/

using System;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
	public partial class AppData
	{
        #region field

        private bool _hasInit = false;
        private int _appResVersion = 1;
        private AdventureData _adventureData;
        private WorldData _worldData;

		private static AppData _instance = new AppData();

        #endregion field

        #region property

		public static AppData Instance {
			get { return _instance; }
		}

        public int AppResVersion
        {
            get { return _appResVersion; }
        }

        public bool HasInit
        {
            get { return _hasInit; }
        }

        public AdventureData AdventureData
        {
            get { return _adventureData; }
        }

        public WorldData WorldData
        {
            get { return _worldData; }
        }

        #endregion property

        #region methond

        public void Init()
        {
            _adventureData = new AdventureData();
            _worldData = new WorldData();
            Messenger.AddListener(SoyEngine.EMessengerType.OnAccountLoginStateChanged, OnAccountLoginStateChanged);
        }

        public void LoadAppData(Action successCallback, Action<ENetResultCode> failedCallback)
        {
			Request(0, ()=>{
				SoyPath.Instance.FileUrlRoot = _fileUrlRoot;
				SoyPath.Instance.ImageUrlRoot = _imageUrlRoot;
				if (_serverTime != 0)
				{
					DateTimeUtil.SyncServerTime(_serverTime);
				}
				if(successCallback != null)
				{
					successCallback.Invoke();
				}
			}, failedCallback);
        }

        private void OnAccountLoginStateChanged()
        {
            
        }
        #endregion method
    }
}