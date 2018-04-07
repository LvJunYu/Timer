  /********************************************************************
  ** Filename : IOnChangeHandler.cs
  ** Author : quan
  ** Date : 6/15/2017 8:02 PM
  ** Summary : IOnChangeHandler.cs
  ***********************************************************************/
using System;

namespace SoyEngine
{
    public interface IOnChangeHandler<T>
    {
        void OnChangeHandler(T val);
    }
}

