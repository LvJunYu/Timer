/********************************************************************
** Filename : UIParticleItem  
** Author : ake
** Date : 3/2/2017 4:36:14 PM
** Summary : UIParticleItem  
***********************************************************************/


namespace SoyEngine
{
	public class UIParticleItem : IUISortingOrderExtendItem
	{
		private UnityNativeParticleItem _particle;
		private int _index;
		private SoyUIGroup _beloneGroup;

		public UnityNativeParticleItem Particle
		{
			get { return _particle; }
		}

		public UIParticleItem(UnityNativeParticleItem item,SoyUIGroup g)
		{
			_particle = item;
			_beloneGroup = g;
			_particle.OnFreeEvent = OnFree;
		}

		public int GetIndex()
		{
			return _index;
		}

		public void SetIndex(int v)
		{
			_index = v;
		}

		public void SetSortingOrder(int value)
		{
			if (_particle != null)
			{
				_particle.SetSortingOrder(value);
			}
		}

		public void Clear()
		{
			_particle = null;
			_beloneGroup = null;
		}

		private void OnFree()
		{
			if (_particle != null)
			{
				_particle.OnFreeEvent = null;
			}
			_particle = null;
			if (_beloneGroup != null)
			{
				_beloneGroup.RemoveSortingLayerItem(this);
			}
		}

	}
}

