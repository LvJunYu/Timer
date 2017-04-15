using System;

namespace SevenZip
{
    class FuckCodeProgress : ICodeProgress
    {
        #region 常亮与字段

        private float _progress = 0;
        private Action _valueChangeCallback ;

        #endregion

        #region  属性

        public float Progress
        {
            get { return _progress; }
        }

        #endregion

        public FuckCodeProgress(Action callback)
        {
            _valueChangeCallback = callback;
        }

        public void SetProgress(long inSize, long outSize)
        {
            long outSizeEx = outSize;
            if (outSize == 0)
            {
                outSizeEx = 100;
            }
            _progress = 1f*inSize/outSizeEx;
            if (_valueChangeCallback != null)
            {
                _valueChangeCallback();
            }
        }
    }
}
