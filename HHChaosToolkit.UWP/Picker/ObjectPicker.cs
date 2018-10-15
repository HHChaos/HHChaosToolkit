using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

namespace HHChaosToolkit.UWP.Picker
{
    public class ObjectPicker<T> : ContentControl
    {
        private readonly Frame _frame = new Frame();
        
        private Popup _popup;
        private Grid _rootGrid;
        private TaskCompletionSource<PickResult<T>> _taskSource;

        public ObjectPicker()
        {
            Content = _frame;
            _frame.Navigated += Frame_Navigated;
            Unloaded += ObjectPicker_Unloaded;
        }

        public PickerOpenOption PickerOpenOption { get; set; } = new PickerOpenOption();

        private void ObjectPicker_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_frame.Content is Page page)
                if (page.DataContext is IObjectPicker<T> viewModel)
                {
                    viewModel.ObjectPicked -= ViewModel_ObjectPicked;
                    viewModel.Canceled -= ViewModel_Canceled;
                }

            Unloaded -= ObjectPicker_Unloaded;
            _frame.Navigated -= Frame_Navigated;
            _popup.IsOpen = false;
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (_frame.Content is Page page)
                if (page.DataContext is IObjectPicker<T> viewModel)
                {
                    viewModel.ObjectPicked += ViewModel_ObjectPicked;
                    viewModel.Canceled += ViewModel_Canceled;
                }
        }

        private void ClearViewModelEvent(IObjectPicker<T> viewModel)
        {
            if (viewModel != null)
            {
                viewModel.ObjectPicked -= ViewModel_ObjectPicked;
                viewModel.Canceled -= ViewModel_Canceled;
            }
        }

        private void ViewModel_ObjectPicked(object sender, ObjectPickedEventArgs<T> e)
        {
            _taskSource.SetResult(new PickResult<T>
            {
                Result = e.Result
            });
            ClearViewModelEvent(sender as IObjectPicker<T>);
        }

        private void ViewModel_Canceled(object sender, EventArgs e)
        {
            _taskSource.SetResult(new PickResult<T>
            {
                Canceled = true
            });
            ClearViewModelEvent(sender as IObjectPicker<T>);
        }

        public async Task<PickResult<T>> PickSingleObjectAsync(Type sourcePageType, object parameter = null)
        {
            _taskSource = new TaskCompletionSource<PickResult<T>>();
            HorizontalContentAlignment = PickerOpenOption.HorizontalAlignment;
            VerticalContentAlignment = PickerOpenOption.VerticalAlignment;
            Margin = PickerOpenOption.Margin;
            _rootGrid = new Grid
            {
                Background = PickerOpenOption.Background,
                Width = Window.Current.Bounds.Width,
                Height = Window.Current.Bounds.Height,
                ChildrenTransitions = PickerOpenOption.Transitions
            };
            if (PickerOpenOption.EnableTapBlackAreaExit)
            {
                _rootGrid.Tapped += _rootGrid_Tapped;
            }
            _popup = new Popup
            {
                Child = _rootGrid
            };
            _rootGrid.Children.Add(this);
            _frame.Navigate(sourcePageType, parameter);
            Window.Current.SizeChanged += Current_SizeChanged;
            _popup.IsOpen = true;
            var result = await _taskSource.Task;
            Close();
            return result;
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            if (_rootGrid != null)
            {
                _rootGrid.Width = Window.Current.Bounds.Width;
                _rootGrid.Height = Window.Current.Bounds.Height;
            }
        }

        private void _rootGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (e.OriginalSource == _rootGrid)
                ViewModel_Canceled(this, EventArgs.Empty);
        }

        private void Close()
        {
            Window.Current.SizeChanged -= Current_SizeChanged;
            if (_rootGrid != null)
            {
                _rootGrid.Tapped -= _rootGrid_Tapped;
                _rootGrid.Children.Remove(this);
            }
        }
    }
}
