using System;
using AAG.Global.Enums;

namespace AAG.Global.Threading
{
    public sealed class ThreadCompletedEventArgs : EventArgs
    {
        public string ThreadName { get; }

        public ProcessType ProcessType { get; }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="threadName"></param>
        public ThreadCompletedEventArgs(string threadName)
            => ThreadName = threadName;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="processType"></param>
        public ThreadCompletedEventArgs(ProcessType processType)
            => ProcessType = processType;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="threadName"></param>
        /// <param name="processType"></param>
        public ThreadCompletedEventArgs(
              string threadName
            , ProcessType processType)
        {
            ThreadName = threadName;
            ProcessType = processType;
        }
    }
}