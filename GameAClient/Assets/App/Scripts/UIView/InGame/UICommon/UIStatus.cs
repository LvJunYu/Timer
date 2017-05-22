/********************************************************************
** Filename : UIStatusData  
** Author : ake
** Date : 6/6/2016 6:32:11 PM
** Summary : UIStatusData  
***********************************************************************/


using System;
using UnityEngine;

namespace SoyEngine
{
	[Serializable]
	public class UIStatusData
	{
		public string key = "";

		public virtual void ApplyStatus(GameObject go)
		{ }
	}

	public static class UIStatus
	{
		public static void SetStatus(GameObject root, string key)
		{
			if (root == null) return;
			foreach (UITransformStatus s in root.GetComponentsInChildren<UITransformStatus>(includeInactive: true))
			{
				s.SetStatus(key, false);
			}
		}

		public static void SetStatus(GameObject root, int index)
		{
			if (root == null) return;
			foreach (UITransformStatus s in root.GetComponentsInChildren<UITransformStatus>(includeInactive: true))
			{
				s.SetStatus(index, false);
			}
		}
	}

	public interface IUIStatus
	{
		void SetStatus(int value, bool recursive);
		void SetStatus(string key, bool recursive);
	}

	public abstract class UIStatus<T> : MonoBehaviour, IUIStatus
		where T : UIStatusData, new()
	{
		[SerializeField]
		protected T[] statuses;
		[SerializeField]
		protected int currentIndex;

		public int StatusCount
		{
			get { return statuses == null ? 0 : statuses.Length; }
			set
			{
				if (statuses == null || value != statuses.Length)
				{
					statuses = CreateArray(statuses, value);
				}
			}
		}

		public int CurrentIndex
		{
			get
			{
				return currentIndex;
			}
			set
			{
				SetStatus(value, true);
			}
		}

		public T this[int i]
		{
			get
			{
				if (i >= 0 && i < StatusCount)
				{
					if (statuses[i] == null) statuses[i] = new T();
					return statuses[i];
				}
				return null;
			}
		}

		public T CurrentStatus
		{
			get { return this[CurrentIndex]; }
		}

		public static void SetStatus(GameObject root, string key)
		{
			var children = root.GetComponentsInChildren(typeof(IUIStatus), true);
			if (children == null)
				return;
			for (int i = 0, imax = children.Length; i < imax; i++)
			{
				var child = children[i] as IUIStatus;
				if (child != null)
					child.SetStatus(key, false);
			}
		}

		public static void SetStatus(GameObject root, int index)
		{
			var children = root.GetComponentsInChildren(typeof(IUIStatus), true);
			if (children == null)
				return;
			for (int i = 0, imax = children.Length; i < imax; i++)
			{
				var child = children[i] as IUIStatus;
				if (child != null)
					child.SetStatus(index, false);
			}
		}

		public void SetStatus(string key, bool recursive)
		{
			var index = FindStatus(key);
			if (index >= 0 && currentIndex != index)
			{
				currentIndex = index;
				T s = statuses[currentIndex];
				s.ApplyStatus(gameObject);

				if (recursive)
					SetStatus(gameObject, key);
			}
		}

		public void SetStatus(int value, bool recursive)
		{
			if (value >= StatusCount)
			{
				return;
			}
			if (currentIndex != value)
			{
				currentIndex = value;
				if (currentIndex >= 0)
				{
					T s = statuses[currentIndex];
					if (s != null)
					{
						s.ApplyStatus(gameObject);
					}
				}
				if (recursive)
					SetStatus(gameObject, value);
			}
		}

		protected abstract T CreateDefaultStatus();

		private int FindStatus(string key)
		{
			if (statuses != null && !string.IsNullOrEmpty(key))
			{
				for (int i = 0; i < statuses.Length; i++)
				{
					if (statuses[i] != null && statuses[i].key == key)
						return i;
				}
			}
			return -1;
		}

		private T[] CreateArray(T[] old, int newlength)
		{
			T[] arr = new T[newlength];
			if (old != null)
			{
				int i = 0;
				for (; i < arr.Length && i < old.Length; i++)
				{
					arr[i] = old[i];
				}
				for (; i < arr.Length; i++)
				{
					arr[i] = CreateDefaultStatus() ?? new T();
				}
			}
			return arr;
		}
	}
}
