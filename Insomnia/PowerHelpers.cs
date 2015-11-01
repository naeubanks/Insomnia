using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;


namespace Insomnia
{
    class PowerHelpers
    {
        public static void EnableInsomnia()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_AWAYMODE_REQUIRED);
        }

        public static void TurnOffMonitor()
        {
            PostMessage(new IntPtr(HWND_TOPMOST), WM_SYSCOMMAND, new IntPtr(SC_MONITORPOWER), new IntPtr(MONITOR_OFF));
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }

        [DllImport("user32.dll")]
        static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        static readonly int HWND_TOPMOST = -1;
        static readonly int SC_MONITORPOWER = 0xF170;
        static readonly uint WM_SYSCOMMAND = 0x0112;
        static readonly int MONITOR_OFF = 2;
    }
}

