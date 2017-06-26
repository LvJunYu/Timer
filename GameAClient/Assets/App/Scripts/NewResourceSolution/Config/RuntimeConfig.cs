using System;
using UnityEngine;

namespace NewResourceSolution
{
    public class RuntimeConfig : MonoBehaviour
	{
		#region fields
        /// <summary>
        /// 资源服务器地址
        /// </summary>
        [SerializeField]
        private string _resourceServerAddress;
        /// <summary>
        /// 程序版本
        /// </summary>
        [SerializeField]
        private string _version;
		/// <summary>
		/// 默认地区
		/// </summary>
		[SerializeField]
		private ELocale _defaultLocale;
		/// <summary>
		/// 是否使用AssetBundle资源
		/// </summary>
		[SerializeField]
		private bool _useAssetBundleRes;
		/// <summary>
		/// 屏幕宽
		/// </summary>
        [SerializeField]
        private int _screenWidth;
		/// <summary>
		/// 屏幕高
		/// </summary>
        [SerializeField]
        private int _screenHeight;
		/// <summary>
		/// splash显示时长
		/// </summary>
        [SerializeField]
        private int _splashShowTime;
		/// <summary>
		/// 逻辑帧率
		/// </summary>
        [SerializeField]
        private int _logicFrameRate;

        private static RuntimeConfig _instance;
		#endregion
		#region properties
        public static RuntimeConfig Instance
        {
            get { return _instance; }
        }
        /// <summary>
        /// 程序版本
        /// </summary>
        public string Version
        {
            get { return _version; }
        }
        /// <summary>
        /// 资源服务器地址
        /// </summary>
        public string ResourceServerAddress
        {
            get { return _resourceServerAddress; }
        }
        /// <summary>
        /// 默认地区
        /// </summary>
		public ELocale DefaultLocale {
			get {
				return this._defaultLocale;
			}
		}
		/// <summary>
		/// 是否使用AssetBundle资源
		/// </summary>
		public bool UseAssetBundleRes
		{
			get
			{
				#if UNITY_EDITOR
				return _useAssetBundleRes;
				#else
				return true;
				#endif
			}
		}
		/// <summary>
		/// 屏幕宽
		/// </summary>
        public int ScreenWidth {
            get {
                return this._screenWidth;
            }
        }
		/// <summary>
		/// 屏幕高
		/// </summary>
        public int ScreenHeight {
            get {
                return this._screenHeight;
            }
        }
		/// <summary>
		/// splash显示时长
		/// </summary>
        public int SplashShowTime
        {
            get { return _splashShowTime; }
        }
		/// <summary>
		/// 逻辑帧率
		/// </summary>
        public int LogicFrameRate
        {
            get { return _logicFrameRate; }
        }
        void Awake ()
        {
            _instance = this;
        }
		#endregion

	}
}