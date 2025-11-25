/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;
using IEnumerator = System.Collections.IEnumerator;

namespace AuroraFPSRuntime.CoreModules.Coroutines
{
    public abstract class AuroraYieldInstruction : IEnumerator, IAuroraYieldInstruction
    {
        // IEnumerator implementation.
        private AuroraYieldInstruction current;
        object IEnumerator.Current
        {
            get
            {
                return current;
            }
        }

        // CoroutineInstruction actions.
        private bool isExecuting;
        private bool isPaused;
        private bool isStopped;

        void IEnumerator.Reset()
        {
            isPaused = false;
            isStopped = false;
        }

        bool IEnumerator.MoveNext()
        {
            if (isStopped)
            {
                (this as IEnumerator).Reset();
                return false;
            }

            if (!isExecuting)
            {
                isExecuting = true;

                OnStarted();
                OnStartedCallback?.Invoke(this);
            }

            if (isPaused)
                return true;

            if (!Update())
            {
                OnDone();
                OnDoneCallback?.Invoke(this);

                isStopped = true;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Set instruction on pause.
        /// </summary>
        public void Pause()
        {
            if (isExecuting && !isPaused)
            {
                isPaused = true;

                OnPaused();
                OnPausedCallback?.Invoke(this);
            }
        }

        /// <summary>
        /// Resume instruction.
        /// </summary>
        public void Resume()
        {
            isPaused = false;
            OnResumed();
        }

        /// <summary>
        /// Terminate instruction.
        /// </summary>
        public void Terminate()
        {
            if (Stop())
            {
                OnTerminated();
                OnTerminatedCallback?.Invoke(this);
            }
        }

        /// <summary>
        /// Stop instruction processing.
        /// </summary>
        private bool Stop()
        {
            if (isExecuting)
            {
                (this as IEnumerator).Reset();

                return isStopped = true;
            }

            return false;
        }

        /// <summary>
        /// Terminate and reset instruction.
        /// </summary>
        public void Reset()
        {
            Terminate();

            OnStartedCallback = null;
            OnPausedCallback = null;
            OnTerminatedCallback = null;
            OnDoneCallback = null;
        }

        /// <summary>
        /// Called when instruction firstly start.
        /// </summary>
        protected virtual void OnStarted() { }

        /// <summary>
        /// Called when instruction is paused.
        /// </summary>
        protected virtual void OnPaused() { }

        /// <summary>
        /// Called when instruction resumed.
        /// </summary>
        protected virtual void OnResumed() { }

        /// <summary>
        /// Called when instruction is terminated.
        /// </summary>
        protected virtual void OnTerminated() { }

        /// <summary>
        /// Called when instruction is done
        /// </summary>
        protected virtual void OnDone() { }

        /// <summary>
        /// Called every move next.
        /// </summary>
        protected abstract bool Update();

        #region [Event callback functions]
        /// <summary>
        /// On started event callback function.
        /// OnStartedCallback called when instruction firstly started.
        /// </summary>
        /// <param name="CoroutineInstruction">CoroutineInstruction instance.</param>
        public event Action<AuroraYieldInstruction> OnStartedCallback;

        /// <summary>
        /// On paused event callback function.
        /// OnPausedCallback called when instruction is paused.
        /// </summary>
        /// <param name="CoroutineInstruction">CoroutineInstruction instance.</param>
        public event Action<AuroraYieldInstruction> OnPausedCallback;

        /// <summary>
        /// On terminated event callback function.
        /// OnTerminatedCallback called when instruction is terminated.
        /// </summary>
        /// <param name="CoroutineInstruction">CoroutineInstruction instance.</param>
        public event Action<AuroraYieldInstruction> OnTerminatedCallback;

        /// <summary>
        /// On done event callback function.
        /// OnDoneCallback called when instruction is done.
        /// </summary>
        /// <param name="CoroutineInstruction">CoroutineInstruction instance.</param>
        public event Action<AuroraYieldInstruction> OnDoneCallback;
        #endregion

        #region [Getter / Setter]
        protected AuroraYieldInstruction GetCurrent()
        {
            return current;
        }

        protected void SetCurrent(AuroraYieldInstruction value)
        {
            current = value;
        }

        /// <summary>
        /// Instruction is executing.
        /// </summary>
        public bool IsExecuting()
        {
            return isExecuting;
        }

        /// <summary>
        /// Instruction is paused.
        /// </summary>
        public bool IsPaused()
        {
            return isPaused;
        }

        /// <summary>
        /// Instruction is stoped.
        /// </summary>
        protected bool IsStopped()
        {
            return isStopped;
        }
        #endregion
    }
}