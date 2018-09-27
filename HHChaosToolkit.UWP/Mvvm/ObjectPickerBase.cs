using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HHChaosToolkit.UWP.Picker;

namespace HHChaosToolkit.UWP.Mvvm
{
    public abstract class ObjectPickerBase<T> : ViewModelBase, IObjectPicker<T>
    {
        public event EventHandler<ObjectPickedEventArgs<T>> ObjectPicked;
        public event EventHandler Canceled;

        public void SetResult(T result)
        {
            ObjectPicked?.Invoke(this, new ObjectPickedEventArgs<T>(result));
        }
        public void Exit()
        {
            Canceled?.Invoke(this, EventArgs.Empty);
        }
        public RelayCommand ExitCommand => new RelayCommand(Exit);
    }
}
