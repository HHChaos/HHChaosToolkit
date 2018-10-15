using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml.Navigation;
using HHChaosToolkit.UWP.Mvvm;

namespace HHChaosToolkit.Sample.ViewModels.TestViewModels
{
    public class TestColorPickerViewModel: ObjectPickerBase<Color>
    {
        private Color _pickedColor;

        public Color PickedColor
        {
            get => _pickedColor;
            set => Set(ref _pickedColor, value);
        }
        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Color color)
            {
                PickedColor = color;
            }

            base.OnNavigatedTo(e);
        }
        public ICommand PickColorCommand => new RelayCommand(() =>
        {
            SetResult(PickedColor);
        });
    }
}
