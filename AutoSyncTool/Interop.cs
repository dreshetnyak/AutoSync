using System;
using System.Runtime.InteropServices;

namespace AutoSyncTool
{
    internal static partial class Interop
    {
        #region Types Definition
        // ReSharper disable IdentifierTypo InconsistentNaming
        public static class WindowAttributes
        {
            public const int GWL_WNDPROC = -4;
            public const int GWL_HINSTANCE = -6;
            public const int GWL_HWNDPARENT = -8;
            public const int GWL_STYLE = -16;
            public const int GWL_EXSTYLE = -20;
            public const int GWL_USERDATA = -21;
            public const int GWL_ID = -12;
        }

        public static class ShowWindowActions
        {
            public const int SW_HIDE = 0;
            public const int SW_SHOWNORMAL = 1;
            public const int SW_NORMAL = 1;
            public const int SW_SHOWMINIMIZED = 2;
            public const int SW_SHOWMAXIMIZED = 3;
            public const int SW_MAXIMIZE = 3;
            public const int SW_SHOWNOACTIVATE = 4;
            public const int SW_SHOW = 5;
            public const int SW_MINIMIZE = 6;
            public const int SW_SHOWMINNOACTIVE = 7;
            public const int SW_SHOWNA = 8;
            public const int SW_RESTORE = 9;
            public const int SW_SHOWDEFAULT = 10;
            public const int SW_FORCEMINIMIZE = 11;
        }

        public static class WindowStyles
        {
            public const int WS_EX_TOOLWINDOW = 0x00000080;
        }

        // ReSharper restore IdentifierTypo InconsistentNaming
        #endregion

        [LibraryImport("user32.dll", EntryPoint = "GetWindowLongW")]
        public static partial IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [LibraryImport("user32.dll", EntryPoint = "SetWindowLongW")]
        public static partial int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
