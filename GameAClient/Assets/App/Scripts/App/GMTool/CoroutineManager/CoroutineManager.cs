using System.Collections;
using UnityEngine;
using SoyEngine;

namespace NewResourceSolution
{
	public class CoroutineManager : Singleton<CoroutineManager>
	{
		#region fields
		private static MonoBehaviour _handler;
		#endregion

		#region propeties
		#endregion

		#region methods
		public void Init (MonoBehaviour handler)
		{
			_handler = handler;
		}

		public static void StartCoroutine (IEnumerator routine)
		{
			_handler.StartCoroutine(routine);
		}
		#endregion
	}
}