using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HHChaosToolkit.UWP.Mvvm;

namespace HHChaosToolkit.Sample.ViewModels.TestViewModels
{
    public class TestInputDialogViewModel : ObjectPickerBase<string>
    {
        public ICommand SubmitCommand => new RelayCommand<string>(SetResult);
    }
}
