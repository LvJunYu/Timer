/********************************************************************
** Filename : LoadingItem  
** Author : ake
** Date : 9/18/2016 4:15:38 PM
** Summary : LoadingItem  
***********************************************************************/


using System.IO;
using System.Text;
using UnityEngine;

namespace SoyEngine
{
	public class LoadingItem
	{

		private enum ELoadItemState
		{
			Loading,
			LoadComplete,
			LoadFailed,
		}

		public const float MaxItemLoadTiem = 50f;
		public const int MaxRetryDownloadTime = 3;


		public WWW Content;
		public string Url;
		public GameResourceLoaderEx.EWebDownItemType ItemType;
		public EGameResourceLoadingResult Result;
		public string ResourceName;
		public string LoadFailedDetail;
		public string Md5;
		public bool IsBuildInRes;
		public byte[] BuildInResBytes;

		private ELoadItemState _curState;
		private float _startLoadTime;

		private int _size;

		private int _reTryDownloadTimes;




		public int HasDownloadSize
		{
			get
			{
				if (HasLoadComplete)
				{
					return _size;
				}
				if (IsBuildInRes && Application.platform == RuntimePlatform.IPhonePlayer)
				{
					return 0;
				}
				return (int)(Content.progress*_size);
			}
			
		}


		public string ContentText
		{
			get
			{
				if (BuildInResBytes == null)
				{
					return "";
				}
				return Encoding.ASCII.GetString(BuildInResBytes); ;
			}
		}

		public byte[] ContentBytes
		{
			get
			{
				return BuildInResBytes;
			}
		}

		public bool HasLoadComplete
		{
			get
			{
				return _curState == ELoadItemState.LoadComplete;
			}
		}

		public bool HasLoadFailed
		{
			get { return _curState == ELoadItemState.LoadFailed; }
		}


		public void Update()
		{
			if (_curState != ELoadItemState.Loading)
			{
				return;
			}
			if (IsBuildInRes && Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (File.Exists(Url))
				{
					BuildInResBytes = File.ReadAllBytes(Url);
					_curState = ELoadItemState.LoadComplete;
				}
				else
				{
					_curState = ELoadItemState.LoadFailed;
					LoadFailedDetail = string.Format("file {0} doesn't exist!", Url);
				}
			}
			else
			{
				bool noError = string.IsNullOrEmpty(Content.error);
				if (Content.isDone && noError)
				{
					BuildInResBytes = Content.bytes;
					_curState = ELoadItemState.LoadComplete;
				}
				else if (!noError)
				{
					DoLoadFailed(string.Format("Load {0} failed，error is {1}", Url, Content.error));
				}
				else if ((Time.realtimeSinceStartup - _startLoadTime) > MaxItemLoadTiem)
				{
					DoLoadFailed(string.Format("Load {0} failed，error overtime", Url));
				}

				if (_curState != ELoadItemState.Loading)
				{
					ClearWWW();
				}
			}
		}

		private void DoLoadFailed(string msg)
		{
			if (_reTryDownloadTimes > MaxRetryDownloadTime)
			{
				_curState = ELoadItemState.LoadFailed;
				LoadFailedDetail = msg;
				LogHelper.Error("Retry {0} times,load failed.error msg {1},url is {2}", _reTryDownloadTimes, msg,Url);
			}
			else
			{
				RetryDownload();
				LogHelper.Error("Retry download {0} ,last error msg is {1}",Url, msg);
				_reTryDownloadTimes++;
			}
		}
		/// <summary>
		/// 只有ｗｗｗ加载会掉
		/// </summary>
		private void RetryDownload()
		{
			_startLoadTime = Time.realtimeSinceStartup;
			Content.Dispose();
			Content = new WWW(Url);
		}

		public LoadingItem()
		{
			_startLoadTime = Time.realtimeSinceStartup;
		}


		private void ClearWWW()
		{
			if (Content != null)
			{
				Content.Dispose();
				Content = null;
			}
		}


		public static LoadingItem CreateLoadItem(string gameName,string fileName, string md5, GameResourceLoaderEx.EWebDownItemType itemType,int size, bool isBuildRes = false)
		{
			LoadingItem res = new LoadingItem();
			res.ResourceName = "";
			res.Result = EGameResourceLoadingResult.None;
			res.LoadFailedDetail = "";
			res.IsBuildInRes = false;
			res.BuildInResBytes = null;
			res.Url = null;

			res.ResourceName = fileName;
			res.ItemType = itemType;
			res.Md5 = md5;
			res.IsBuildInRes = isBuildRes;
			res._size = size;

			string wholeName = GameResourcePathManager.GetFileWholeNameWithMd5(res.ResourceName, res.Md5);
			if (res.ItemType == GameResourceLoaderEx.EWebDownItemType.Resource)
			{
				res.Url = GameResourcePathManager.Instance.GetWebGameResUrl(wholeName, gameName, res.IsBuildInRes);
			}
			else if(res.ItemType == GameResourceLoaderEx.EWebDownItemType.Config)
			{
				res.Url = GameResourcePathManager.Instance.GetWebGameConfigUrl(wholeName, gameName, res.IsBuildInRes);
			}
			else
			{
				res.Url = GameResourcePathManager.Instance.GetLocalePackUrl(string.Format("{0}.{1}", fileName, md5));
			}
			if (!res.IsBuildInRes || Application.platform != RuntimePlatform.IPhonePlayer)
			{
				res.Content = new WWW(res.Url);
			}

			return res;
		}
	}
}
