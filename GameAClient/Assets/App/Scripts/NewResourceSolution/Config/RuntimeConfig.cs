using System;
using UnityEngine;

namespace NewResourceSolution
{
    public class RuntimeConfig : MonoBehaviour
	{
        #region fields
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
        #endregion

        #region methods
        void Awake ()
        {
            _instance = this;
        }
		#endregion

	}
}