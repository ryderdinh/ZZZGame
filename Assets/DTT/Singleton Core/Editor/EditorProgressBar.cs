#if UNITY_EDITOR


using System;
using System.Threading;
using UnityEditor;

namespace DTT.Singletons.Editor
{
    /// <summary>
    /// A utility class that provides methods for showing a progress bar window in the editor.
    /// </summary>
    public static class EditorProgressBar
    {
        /// <summary>
        /// The thread sleep time used to show the mock progress by default.
        /// </summary>
        private const int THREAD_SLEEP_TIME = 250;

        /// <summary>
        /// Shows a mock progressbar that doesn't change but can be shown to provide
        /// userfeedback on heavy editor operations.
        /// </summary>
        /// <param name="title">The title of the window.</param>
        /// <param name="info">The info text of the window.</param>
        /// <param name="mockProgress">The progress to be shown.</param>
        /// <param name="action">The operation to be done.</param>
        public static void ShowMock(string title, string info, float mockProgress, Action action)
            => ShowMock(title, info, mockProgress, THREAD_SLEEP_TIME, action);

        /// <summary>
        /// Shows a mock progressbar that doesn't change but can be shown to provide
        /// userfeedback on heavy editor operations.
        /// </summary>
        /// <param name="title">The title of the window.</param>
        /// <param name="info">The info text of the window.</param>
        /// <param name="mockProgress">The progress to be shown.</param>
        /// <param name="showTime">The total show time of the window in miliseconds.</param>
        /// <param name="action">The operation to be done.</param>
        public static void ShowMock(string title, string info, float mockProgress, int showTime, Action action)
        {
            EditorUtility.DisplayProgressBar(title, info, mockProgress);
            action.Invoke();
            Thread.Sleep(showTime);
            EditorUtility.ClearProgressBar();
        }
    }
}
#endif