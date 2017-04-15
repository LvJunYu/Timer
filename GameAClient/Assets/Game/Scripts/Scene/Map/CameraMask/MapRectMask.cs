/********************************************************************
** Filename : MapRectMask  
** Author : ake
** Date : 11/8/2016 3:09:10 PM
** Summary : MapRectMask  
***********************************************************************/


using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
	public class MapRectMask:MonoBehaviour
	{
		private Transform _trans;

		public Transform Trans
		{
			get
			{
				if (_trans == null)
				{
					_trans = transform;
				}
				return _trans;
			}
		}

		private void Awake()
		{
			_trans = transform;
		}

		public void SetSortOrdering(int value)
		{
			Renderer render = GetComponent<Renderer>();
			if (render != null)
			{
				render.sortingOrder = value;
			}
			else
			{
				LogHelper.Error("MapRectMask.SetSortOrdering called but GetComponent<Renderer>() is null! value is {0}", value);
			}
		}


		public void SetLocalScale(float x, float y)
		{
			Vector3 scale = Vector3.one;
			scale.x = x;
			scale.y = y;
			_trans.localScale = scale;
		}
	}
}
