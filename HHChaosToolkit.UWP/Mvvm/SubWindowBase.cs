using System;
using HHChaosToolkit.UWP.SubWindows;

namespace HHChaosToolkit.UWP.Mvvm
{
    public abstract class SubWindowBase : ViewModelBase, ISubWindow
    {
        public event EventHandler WindowClosed;

        private string _title;

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public void RaiseWindowClosedEvent()
        {
            WindowClosed?.Invoke(this, EventArgs.Empty);
        }

        public string SubWinKey { get; set; }

        public void Close()
        {
            RaiseWindowClosedEvent();
        }

        public RelayCommand CloseCommand => new RelayCommand(Close);
    }
}
