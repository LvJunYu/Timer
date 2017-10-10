using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;

namespace NewResourceSolution
{
	public class CHResManifest
	{
		#region fields
		[Newtonsoft.Json.JsonProperty]
		protected Version _version;
		[Newtonsoft.Json.JsonProperty]
		protected List<CHResBundle> _bundles;

        [Newtonsoft.Json.JsonProperty]
        protected List<string> _adamBundleNameList;

		protected EFileLocation _fileLocation;

		#endregion

		#region properties
        [Newtonsoft.Json.JsonIgnore]
		public Version Ver
		{
			get
			{
				return _version;
			}
		}
        [Newtonsoft.Json.JsonIgnore]
		public List<CHResBundle> Bundles
		{
			get
			{
				return _bundles;
			}
		}
        [Newtonsoft.Json.JsonIgnore]
        public List<string> AdamBundleNameList
        {
            get
            {
                return _adamBundleNameList;
            }
        }
		[Newtonsoft.Json.JsonIgnore]
		public EFileLocation FileLocation
		{
			get
			{
				return _fileLocation;
			}
			set
			{
				_fileLocation = value;
			}
		}
		#endregion

		#region methods
        public CHResManifest() {}
		public CHResManifest(Version version)
		{
			_version = version;
			_bundles = new List<CHResBundle>();
            _adamBundleNameList = new List<string> ();
            var manifestBundle = new CHResBundle();
			manifestBundle.GroupId = ResDefine.ResGroupInPackage;
			manifestBundle.CompressType = EAssetBundleCompressType.NoCompress;
			manifestBundle.FileLocation = EFileLocation.StreamingAsset;
			manifestBundle.AssetNames = new string[] { "AssetBundleManifest" };
			manifestBundle.AssetBundleName = ResDefine.UnityManifestBundleName;
			_bundles.Add(manifestBundle);
		}
		#endregion
	}

    /// <summary>
    /// 资源打包时使用的Manifest
    /// </summary>
	public class CHBuildingResManifest : CHResManifest
	{
//		private Dictionary<string, CHResBundle> _guidInxDic;
        private List<string> _allAssetNameList = new List<string>();

		public CHBuildingResManifest(Version version) : base(version)
		{
//			_guidInxDic = new Dictionary<string, CHResBundle>();
		}

        /// <summary>
        /// 添加bundle并检查assetName是否重复
        /// </summary>
        /// <returns><c>true</c>, if bundle was added, <c>false</c> otherwise.</returns>
        /// <param name="bundle">Bundle.</param>
        public bool AddBundle(CHResBundle bundle)
        {
            bundle.IsLocaleRes = bundle.AssetBundleName.ToCharArray()[0] == ResDefine.LocaleResBundleNameFirstChar;
            if (bundle.IsLocaleRes)
            {
                bundle.LocaleName = bundle.AssetBundleName.Substring(1).Split(ResDefine.ReplaceSplashCharInAssetBundleName)[0].ToUpper();
            }
			for (int i = 0; i<bundle.AssetNames.Length; i++)
            {
                string registAssetName = bundle.IsLocaleRes ?
					StringUtil.Format(StringFormat.TwoLevelPath, bundle.LocaleName, bundle.AssetNames[i]) :
					bundle.AssetNames[i];

                if (_allAssetNameList.Contains(registAssetName))
                {
					LogHelper.Error("Asset <{0}> name dumplicated", registAssetName);
                }
                else
                {
                    _allAssetNameList.Add(registAssetName);
                }
            }
            _bundles.Add(bundle);
            return true;
        }

//		public bool TryGetBundleByGuid(string guid, out CHResBundle bundle)
//		{
//			if (null == _guidInxDic)
//			{
//				bundle = null;
//				return false;
//			}
//			return _guidInxDic.TryGetValue(guid, out bundle);
//		}

        /// <summary>
        /// 从包内mainfest状态切换到资源服务器的manifest状态，以便序列化为资源服务器上的文件
        /// </summary>
        public void SwitchToResServerManifest ()
        {
            for (int i = 0; i < _bundles.Count; i++)
            {
                _bundles [i].FileLocation = EFileLocation.Server;
                _bundles [i].CompressType = EAssetBundleCompressType.LZMA;
            }
            _adamBundleNameList.Clear ();
//            if (null != _manifestBundle)
//            {
//                _manifestBundle.FileLocation = EFileLocation.Server;
//            }
        }
	}
}