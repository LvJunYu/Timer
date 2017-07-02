namespace NewResourceSolution
{
    public class ThreadAction
    {
        #region fields
        private static string s_TimeoutErrorMsg = "ThreadAction timeout after {0} milliseconds";

        private bool _isDone;
        private bool _isStart;
        private System.Exception _exception;
        private string _error;
        private System.Action _action;
        private int _timeout;
        private bool _throwException;
        private bool _isTimeout;
        #endregion

        #region properties
        public bool IsDone
        {
            get
            {
                if (_throwException && null != _exception) throw _exception;
                return _isStart && _isDone; 
            }
        }
        public bool IsTimeout
        {
            get { return _isTimeout; }
        }
        public string Error
        {
            get { return _error; }
        }
        public System.Action Action
        {
            get { return _action; }
        }
        public int Timeout
        {
            get { return _timeout; } 
        }
        #endregion

        #region methods
        public ThreadAction (
            System.Action action,
            bool ioAction = false,
            int timeout = System.Threading.Timeout.Infinite,
            bool throwException = false
        )
        {
            _action = action;
            _timeout = timeout;
            _throwException = throwException;
            _isTimeout = false;
            ThreadManager.Instance.EnqueueAction (this, ioAction);
        }

        public void Start ()
        {
            _isStart = true;
        }

        public void FinishWithTimeout ()
        {
            _isDone = true;
            _isTimeout = true;
			_error = StringUtil.Format (s_TimeoutErrorMsg, _timeout.ToString());
        }

        public void Finish ()
        {
            _isDone = true;
        }

        public void CatchException (System.Exception exception)
        {
            _exception = exception;
            _error = exception.ToString ();
        }
        #endregion
    }
}