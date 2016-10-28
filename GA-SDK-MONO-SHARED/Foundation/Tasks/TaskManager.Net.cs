# if !WINDOWS_WSA && !UNITY_WEBGL && !WINDOWS_UWP && !UNITY_TIZEN
using System.Threading;

namespace Foundation.Tasks
{
    public  partial class TaskManager
    {       /// <summary>
        /// Checks if this is the main thread
        /// </summary>
        public static bool IsMainThread
        {
            get { return Thread.CurrentThread == MainThread; }
        }

        /// <summary>
        /// The Main Thread
        /// </summary>
        public static Thread MainThread { get; protected set; }
        
        /// <summary>
        /// The Current Thread
        /// </summary>
        public static Thread CurrentThread
        {
            get
            {
                return Thread.CurrentThread;
            }
        }
    }
}
#endif
