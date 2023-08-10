using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AutoSyncTool.Models;

namespace AutoSyncTool.ViewModels
{
    internal class SettingsViewModel : INotifyPropertyChanged
    {
        #region Application Settings
        public bool StartWithWindows
        {
            get => Configuration.App.StartWithWindows;
            set
            {
                Configuration.App.StartWithWindows = value;
                OnPropertyChanged();
            }
        }

        public bool StartMinimizedToTray
        {
            get => Configuration.App.StartMinimizedToTray;
            set
            {
                Configuration.App.StartMinimizedToTray = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Scan Settings

        public Configuration.PeriodicityType ScanType
        {
            get => Configuration.Scan.Type;
            set
            {
                Configuration.Scan.Type = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan ScanFrequency
        {
            get => Configuration.Scan.Frequency;
            set
            {
                Configuration.Scan.Frequency = value;
                OnPropertyChanged();
            }
        }

        public TimeOnly ScanTime
        {
            get => Configuration.Scan.Time;
            set
            {
                Configuration.Scan.Time = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
