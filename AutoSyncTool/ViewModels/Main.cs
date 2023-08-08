using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using AutoSyncTool.Commands;
using static AutoSyncTool.Interop;

namespace AutoSyncTool.ViewModels
{
    internal class Main
    {
        public ICommand Hide { get; }
        public ICommand Show { get; }
        public ICommand Exit { get; }

        public Main()
        {
            Hide = new RelayCommand(OnHide);
            Show = new RelayCommand(OnShow);
            Exit = new RelayCommand(OnExit);
        }

        private void OnHide(object? obj)
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow == null)
                return;

            var hWnd = new WindowInteropHelper(mainWindow).EnsureHandle();

            var windowLong = (int)GetWindowLong(hWnd, WindowAttributes.GWL_EXSTYLE);
            windowLong &= WindowStyles.WS_EX_TOOLWINDOW; 

            _ = SetWindowLong(hWnd, WindowAttributes.GWL_EXSTYLE, windowLong);
            ShowWindow(hWnd, ShowWindowActions.SW_HIDE);
        }

        private void OnShow(object? obj)
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow == null)
                return;

            var hWnd = new WindowInteropHelper(mainWindow).EnsureHandle();

            var windowLong = (int)GetWindowLong(hWnd, WindowAttributes.GWL_EXSTYLE);
            windowLong &= ~WindowStyles.WS_EX_TOOLWINDOW; // Remove tool window bit

            _ = SetWindowLong(hWnd, WindowAttributes.GWL_EXSTYLE, windowLong);
            ShowWindow(hWnd, ShowWindowActions.SW_SHOW);
        }

        private void OnExit(object? obj)
        {
            Application.Current.Shutdown();
        }
    }
}