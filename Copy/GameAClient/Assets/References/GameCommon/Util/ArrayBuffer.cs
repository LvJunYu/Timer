/********************************************************************
** Filename : ArrayBuffer  
** Author : ake
** Date : 5/19/2016 3:13:10 PM
** Summary : ArrayBuffer  
***********************************************************************/


using UnityEngine;

namespace SoyEngine
{
	public class ArrayBuffer<T> where T : class
	{
		private T[] _bufferArray;

		private int _curIndex;

		private int _size;

		public int Count
		{
			get { return _curIndex; }
		}

		public ArrayBuffer(int size)
		{
			_size = size;
			_curIndex = 0;
			_bufferArray = new T[_size];
		}

		public T GetItem(int index)
		{
			if (index >= _curIndex)
			{
				LogHelper.Error("ArrayIndexOutOfBoundsException index is {0} CurCount is {1}",index,_curIndex);
				return null;
			}
			return _bufferArray[index];
		}

		public bool Add(T item)
		{
			if (item == null)
			{
				return false;
			}
			if (_curIndex >= _size)
			{
				return false;
			}
			_bufferArray[_curIndex] = item;
			_curIndex++;
			return true;
		}

		public bool Remove(T item)
		{
			if (item == null)
			{
				return false;
			}
			if (_curIndex == 0)
			{
				return false;
			}
			int index = -1;
			for (int i = 0; i < _curIndex; i++)
			{
				if (_bufferArray[i] == item)
				{
					index = i;
					break;
				}
			}
			return RemoveAt(index);
		}

		public bool RemoveAt(int index)
		{
			if (index < 0)
			{
				return false;
			}
			for (int i = index; i < _curIndex - 1; i++)
			{
				_bufferArray[i] = _bufferArray[i + 1];
			}
			_bufferArray[_curIndex - 1] = null;
			_curIndex--;
			return true;
		}

		public void Clear()
		{
			for (int i = 0; i < _size; i++)
			{
				_bufferArray[i] = null;
			}
			_curIndex = 0;
		}
	}
}