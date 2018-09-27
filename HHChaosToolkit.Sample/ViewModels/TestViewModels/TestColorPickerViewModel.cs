using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI;
using HHChaosToolkit.UWP.Mvvm;

namespace HHChaosToolkit.Sample.ViewModels.TestViewModels
{
    public class TestColorPickerViewModel: ObjectPickerBase<Color>
    {
        public ICommand PickColorCommand => new RelayCommand<Color>(SetResult);
    }
}
