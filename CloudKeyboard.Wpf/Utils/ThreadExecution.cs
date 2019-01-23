using System;
using System.Runtime.InteropServices;

namespace Walterlv.CloudTyping.Utils
{
    /// <summary>
    /// 包含控制系统休眠状态的方法。
    /// </summary>
    public static class ThreadExecution
    {
        /// <summary>
        /// 设置此线程此时开始一直将处于运行状态，此时不应该进入睡眠状态或者关闭屏幕。
        /// 此线程退出后，设置将失效。
        /// 如果需要恢复，请调用 <see cref="RestoreSleeping"/> 方法。
        /// </summary>
        /// <param name="isUI">
        /// 表示此任务是否是 UI 任务，UI 任务（例如播放视频或者演示文档）运行时不应该关闭屏幕。
        /// </param>
        public static void PreventSleeping(bool isUI = true)
        {
            SetThreadExecutionState(ExecutionState.SystemRequired | ExecutionState.DisplayRequired |
                                    ExecutionState.Continuous);
        }

        /// <summary>
        /// 恢复此线程的运行状态，操作系统现在可以正常进入睡眠状态和关闭屏幕。
        /// </summary>
        public static void RestoreSleeping()
        {
            SetThreadExecutionState(ExecutionState.Continuous);
        }

        /// <summary>
        /// 通知操作系统应用现在正在持续运行，睡眠或者关闭屏幕的计时需要重置。
        /// </summary>
        /// <param name="isUI">
        /// 表示此任务是否是 UI 任务，UI 任务（例如播放视频或者演示文档）运行时不应该关闭屏幕。
        /// </param>
        public static void InformRunning(bool isUI = true)
        {
            SetThreadExecutionState(isUI
                ? ExecutionState.SystemRequired | ExecutionState.DisplayRequired
                : ExecutionState.SystemRequired);
        }

        /// <summary>
        /// 文档请参阅：https://docs.microsoft.com/en-us/windows/desktop/api/winbase/nf-winbase-setthreadexecutionstate
        /// </summary>
        [DllImport("kernel32")]
        private static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

        [Flags]
        private enum ExecutionState: uint
        {
            /// <summary>
            /// Forces the system to be in the working state by resetting the system idle timer.
            /// </summary>
            SystemRequired = 0x01,

            /// <summary>
            /// Forces the display to be on by resetting the display idle timer.
            /// </summary>
            DisplayRequired = 0x02,

            /// <summary>
            /// This value is not supported.
            /// If ES_USER_PRESENT is combined with other esFlags values,
            /// the call will fail and none of the specified states will be set.
            /// </summary>
            [Obsolete("This value is not supported.")]
            UserPresent = 0x04,

            /// <summary>
            /// Enables away mode. This value must be specified with ES_CONTINUOUS.
            /// <para />
            /// Away mode should be used only by media-recording and media-distribution applications that must perform
            /// critical background processing on desktop computers while the computer appears to be sleeping.See Remarks.
            /// </summary>
            AwaymodeRequired = 0x40,

            /// <summary>
            /// Informs the system that the state being set should remain in effect until the next call that
            /// uses ES_CONTINUOUS and one of the other state flags is cleared.
            /// </summary>
            Continuous = 0x80000000,
        }
    }
}
