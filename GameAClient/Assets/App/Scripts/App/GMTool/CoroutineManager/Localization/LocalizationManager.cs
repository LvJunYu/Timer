using SoyEngine;

namespace NewResourceSolution
{
	public class LocalizationManager : Singleton<LocalizationManager>
	{
		#region fields

		/// <summary>
		/// 当前地区
		/// </summary>
		protected ELocale _currentLocale;
		#endregion

		#region properties
		/// <summary>
		/// 当前地区
		/// </summary>
		public ELocale CurrentLocale {
			get {
				return this._currentLocale;
			}
		}
		#endregion

		#region methods
		public void Init ()
		{
			_currentLocale = RuntimeConfig.Instance.DefaultLocale;
		}

		#endregion
	}
}