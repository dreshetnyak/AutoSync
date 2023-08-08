using System;
using System.Windows.Input;

namespace AutoSyncTool.Commands
{
    internal class RelayCommand : ICommand
    {
        public Action<object?> Action { get; }

        public RelayCommand(Action<object?> action, bool canExecute = true)
        {
            Action = action;
            _canBeExecuted = canExecute;
        }

        public void Execute(object? parameter)
        {
            try { Action.Invoke(parameter); }
            catch { /* ignore */ }
        }

        #region CanExecute Implementation
        public event EventHandler? CanExecuteChanged;
        private bool _canBeExecuted;
        public bool CanBeExecuted
        {
            get => _canBeExecuted;
            set
            {
                if (_canBeExecuted == value) 
                    return;
                _canBeExecuted = value;
                OnCanExecuteChanged();
            }
        }
        public bool CanExecute(object? parameter) => _canBeExecuted;
        protected virtual void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}
