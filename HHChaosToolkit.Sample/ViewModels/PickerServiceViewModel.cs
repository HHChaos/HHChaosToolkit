using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using CommonServiceLocator;
using HHChaosToolkit.Sample.ViewModels.TestViewModels;
using HHChaosToolkit.UWP.Mvvm;
using HHChaosToolkit.UWP.Picker;
using HHChaosToolkit.UWP.Services;

namespace HHChaosToolkit.Sample.ViewModels
{
    public class PickerServiceViewModel : ViewModelBase
    {
        private Color _pickedColor;

        public Color PickedColor
        {
            get => _pickedColor;
            set => Set(ref _pickedColor, value);
        }

        public ICommand PickBackgroundColorCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var pickerService = ServiceLocator.Current.GetInstance<ObjectPickerService>();
                    var ret = await pickerService.PickSingleObjectAsync<Color>(typeof(TestColorPickerViewModel)
                        .FullName);
                    if (!ret.Canceled)
                        PickedColor = ret.Result;
                });
            }
        }

        public ICommand PickBackgroundColorWithCustomOptionCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var pickerService = ServiceLocator.Current.GetInstance<ObjectPickerService>();
                    var openOption = new PickerOpenOption
                    {
                        EnableTapBlackAreaExit = true,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Background = new AcrylicBrush
                        {
                            TintOpacity = 0.1
                        },
                        Transitions = new TransitionCollection
                        {
                            new EdgeUIThemeTransition
                            {
                                Edge = EdgeTransitionLocation.Right
                            }
                        }
                    };
                    var ret = await pickerService.PickSingleObjectAsync<Color>(typeof(TestColorPickerViewModel)
                        .FullName, null, openOption);
                    if (!ret.Canceled)
                        PickedColor = ret.Result;
                });
            }
        }

        public ICommand OpenInputDialogCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var pickerService = ServiceLocator.Current.GetInstance<ObjectPickerService>();
                    var ret = await pickerService.PickSingleObjectAsync<string>(
                        typeof(TestInputDialogViewModel).FullName, null, new PickerOpenOption
                        {
                            HorizontalAlignment = HorizontalAlignment.Stretch
                        });
                });
            }
        }

        public ICommand OpenInputDialogWithCustomOptionCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var pickerService = ServiceLocator.Current.GetInstance<ObjectPickerService>();
                    var openOption = new PickerOpenOption
                    {
                        EnableTapBlackAreaExit = true,
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Background = new AcrylicBrush
                        {
                            BackgroundSource = AcrylicBackgroundSource.HostBackdrop,
                            TintOpacity = 0.1,
                            FallbackColor = Color.FromArgb(0x7f, 0x00, 0x00, 0x00)
                        },
                        Margin = new Thickness(0, 50, 0, 0),
                        Transitions = new TransitionCollection
                        {
                            new EdgeUIThemeTransition
                            {
                                Edge = EdgeTransitionLocation.Top
                            }
                        }
                    };
                    var ret = await pickerService.PickSingleObjectAsync<string>(
                        typeof(TestInputDialogViewModel).FullName, null, openOption);
                });
            }
        }
    }
}