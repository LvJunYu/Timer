using System;
using System.Threading;
using System.Collections.Generic;
using SoyEngine;

namespace NewResourceSolution
{
    public class ThreadManager : Singleton<ThreadManager>
    {
        private class ThreadContainer
        {
            public enum EThreadContainerState
            {
                UnInited,
                Working,
                Idle,
                Closed,
            }
            public int ContainerIdx;
            // only write by child thread
            public EThreadContainerState State;
            public bool IsTimeout;

            public Thread InternalThread;
            public ManualResetEvent WaitEvent;

            private ThreadLoopDelegate _threadLoop;

            // only write by child thread
            private ThreadAction _currentWorkingAction;
            private long _timeoutTime;
            #region called from main thread
            public ThreadContainer (int containerIdx, ThreadLoopDelegate threadLoop)
            {
                ContainerIdx = containerIdx;
                WaitEvent = new ManualResetEvent(true);
                State = EThreadContainerState.UnInited;
                IsTimeout = false;
                _timeoutTime = long.MaxValue;
                _threadLoop = threadLoop;
                StartThread();
            }

            ~ThreadContainer ()
            {
                Clear();
            }

            public void Restart ()
            {
//                UnityEngine.Debug.Log ("Restart thread container " + ContainerIdx);
                WaitEvent = new ManualResetEvent(true);
                State = EThreadContainerState.UnInited;
                IsTimeout = false;
                _timeoutTime = long.MaxValue;
                StartThread();
            }

            public void Clear ()
            {
                if (null != InternalThread)
                {
                    // 危险
                    InternalThread.Abort ();
                }
            }

            public void HandleAction ()
            {
                WaitEvent.Set ();
            }

            public void CheckTimeout (long currentTimeInMilliSecond)
            {
                if (!IsTimeout && _timeoutTime < currentTimeInMilliSecond)
                {
//                    UnityEngine.Debug.Log ("Container timeout id: " + ContainerIdx);
                    IsTimeout = true;
                    _currentWorkingAction.FinishWithTimeout ();
                }
            }
            #endregion

            #region called from child thread
            public void ActionExceptionCatched (ThreadAction exceptionOwner)
            {
                if (exceptionOwner != _currentWorkingAction)
                    return;
                State = EThreadContainerState.Closed;
                _currentWorkingAction = null;
//                UnityEngine.Debug.Log ("ActionExceptionCatched, id: " + ContainerIdx);
            }
            public void FinishAction (ThreadAction lastFinishedAction)
            {
                if (lastFinishedAction != _currentWorkingAction)
                    return;
                if (EThreadContainerState.Working != State)
                    return;
                State = EThreadContainerState.Idle;
                _timeoutTime = long.MaxValue;
                _currentWorkingAction = null;
//                UnityEngine.Debug.Log ("FinishAction, id: " + ContainerIdx);
                WaitEvent.Reset ();
            }
            public void BeginAction (ThreadAction currentWorkingAction)
            {
//                UnityEngine.Debug.Log ("BeginAction, id: " + ContainerIdx);
				State = EThreadContainerState.Working;
                _currentWorkingAction = currentWorkingAction;
                long currentTimeInMilliSecond = DateTimeUtil.GetNowTicks() / 10000;
                _timeoutTime = long.MaxValue;
                if (Timeout.Infinite != currentWorkingAction.Timeout)
                {
                    _timeoutTime = currentTimeInMilliSecond + _currentWorkingAction.Timeout;
                }
            }
            #endregion

            private void StartThread ()
            {
                if (null != InternalThread)
                {
                    InternalThread.Abort ();
                }
                ThreadStart threadStart = new ThreadStart (
                    delegate
                    {
                        _threadLoop(this);
                    }
                );
                State = EThreadContainerState.Idle;
                WaitEvent.Set ();
                InternalThread = new Thread (threadStart);
                InternalThread.Start ();
            }
        }

        private delegate void ThreadLoopDelegate (ThreadContainer container);

        #region fields
        private static int s_maxThreadNum = 4;
        private int _threadCnt = 1;
        private ThreadContainer _ioThreadContainer;
        private List<ThreadContainer> _normalThreadContainerList = new List<ThreadContainer> ();

        private Queue<ThreadAction> _normalActionQueue = new Queue<ThreadAction>();
        private Queue<ThreadAction> _ioActionQueue = new Queue<ThreadAction>();
        private Object _normalQueueLock = new object();
        private Object _ioQueueLock = new object();
        #endregion

        #region properties
        #endregion

        #region methods
        public ThreadManager ()
        {
            _ioThreadContainer = new ThreadContainer (0, IOThreadLoop);
        }

        ~ThreadManager ()
        {
            Clear ();
        }

        public void Clear ()
        {
            for (int i = 0; i < _normalThreadContainerList.Count; i++)
            {
                var container = _normalThreadContainerList [i];
                container.Clear ();
            }
            _normalThreadContainerList.Clear ();
            _ioThreadContainer.Clear ();
            _threadCnt = 1;
        }

		/// <summary>
		/// todo 优化 主线程中连续多次调用该方法，会造成前两个入队的任务压到同一个线程中执行
		/// </summary>
		/// <param name="threadAction">Thread action.</param>
		/// <param name="ioAction">如果为true，则只会排队到主子线程</param>
        public void EnqueueAction (ThreadAction threadAction, bool ioAction = false)
        {
//            LogHelper.Info ("EnqueueAction");
            if (null == threadAction || null == threadAction.Action)
                return;

            if (ioAction)
            {
                lock (_ioQueueLock)
                {
                    _ioActionQueue.Enqueue (threadAction);
                    if (ThreadContainer.EThreadContainerState.Idle == _ioThreadContainer.State)
                    {
                        _ioThreadContainer.HandleAction ();
                    }
                }
            }
            else
            {
                int idleThreadCnt = 0;
                lock (_normalQueueLock)
                {
                    _normalActionQueue.Enqueue (threadAction);
                    for (int i = 0; i < _normalThreadContainerList.Count; i++)
                    {
                        if (ThreadContainer.EThreadContainerState.Idle == _normalThreadContainerList [i].State)
                        {
                            idleThreadCnt++;
                            _normalThreadContainerList [i].HandleAction ();
                            break;
                        }
                    }
                }
                int queuedActionCnt = _normalActionQueue.Count - idleThreadCnt;
//                UnityEngine.Debug.Log ("idleThreadCnt: " + idleThreadCnt + " _normalActionQueue.Count: " + _normalActionQueue.Count);
                while (queuedActionCnt > 0 && _threadCnt < s_maxThreadNum)
                {
//                    UnityEngine.Debug.Log ("new Container, id: " + _threadCnt);
                    queuedActionCnt--;
                    _normalThreadContainerList.Add (new ThreadContainer (_threadCnt++, ThreadLoop));
                }
            }
        }

        public void Update ()
        {
            long currentTimeInMilliSecond = DateTimeUtil.GetNowTicks() / 10000;
            for (int i = 0; i < _normalThreadContainerList.Count; i++)
            {
                _normalThreadContainerList [i].CheckTimeout (currentTimeInMilliSecond);
            }
            _ioThreadContainer.CheckTimeout (currentTimeInMilliSecond);
            if (_ioThreadContainer.IsTimeout)
            {
                _ioThreadContainer.Restart ();
            }
        }

        public string DebugInfo ()
        {
			return string.Format("Current threadCnt: {0}, IO queue count: {1}, normal queue count: {2}", _threadCnt, _ioActionQueue.Count, _normalActionQueue.Count);
        }

        private void ThreadLoop (ThreadContainer container)
        {
            while (true)
            {
                ThreadAction threadAction = null;
                while (true)
                {
                    lock (_normalQueueLock)
                    {
                        if (_normalActionQueue.Count == 0)
                            break;
                        threadAction = _normalActionQueue.Dequeue ();
                    }
                    if (null != threadAction.Action)
                    {
                        try
                        {
                            container.BeginAction(threadAction);
                            threadAction.Start();
                            threadAction.Action.Invoke();
//							UnityEngine.Debug.Log("Finish action, container id: " + container.ContainerIdx);
                        }
                        catch (Exception e)
                        {
                            threadAction.CatchException (e);
                            container.ActionExceptionCatched (threadAction);
                        }
                        finally
                        {
                            threadAction.Finish ();
                        }
                    }
                }
                lock (_normalQueueLock)
                {
                    if (_normalActionQueue.Count == 0)
                    {
                        container.FinishAction(threadAction);
                    }
                }
                container.WaitEvent.WaitOne ();
            }
        }

        private void IOThreadLoop (ThreadContainer container)
        {
            while (true)
            {
                ThreadAction threadAction = null;
                while (true)
                {
                    lock (_ioQueueLock)
                    {
                        if (_ioActionQueue.Count == 0)
                            break;
                        threadAction = _ioActionQueue.Dequeue ();
                    }
                    if (null != threadAction.Action)
                    {
                        try
                        {
                            container.BeginAction(threadAction);
                            threadAction.Start();
                            threadAction.Action.Invoke();
//                            UnityEngine.Debug.Log("Finish action, container id: " + container.ContainerIdx);
                        }
                        catch (Exception e)
                        {
                            threadAction.CatchException (e);
                            container.ActionExceptionCatched (threadAction);
                        }
                        finally
                        {
                            threadAction.Finish ();
                        }
                    }
                }
                lock (_ioQueueLock)
                {
                    if (_ioActionQueue.Count == 0)
                    {
                        container.FinishAction(threadAction);
                    }
                }
                container.WaitEvent.WaitOne ();
            }
        }
        #endregion
    }
}