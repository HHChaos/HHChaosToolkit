﻿using HHChaosToolkit.UWP.Services.Navigation;
using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace HHChaosToolkit.UWP.Picker
{
    public class ObjectPicker<T> : ContentControl
    {
        private readonly NavigationService _navigationService = new NavigationService { Frame = new Frame() };

        private Popup _popup;
        private Grid _rootGrid;
        private TaskCompletionSource<PickResult<T>> _taskSource;

        public ObjectPicker()
        {
            Content = _navigationService.Frame;
            HookUpEvents();
        }

        private IObjectPicker<T> ViewModel =>
            (_navigationService?.Frame?.Content as Page)?.DataContext as IObjectPicker<T>;

        public PickerOpenOption PickerOpenOption { get; set; } = new PickerOpenOption();

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
            if (PickerOpenOption.EnableTapBlackAreaExit) _rootGrid.Tapped += RootGrid_Tapped;

            _popup = new Popup
            {
                Child = _rootGrid
            };
            _rootGrid.Children.Add(this);
            _navigationService.Navigate(sourcePageType, parameter);

            var appViewBackButtonBak =
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Disabled;
            _popup.IsOpen = true;
            await FocusPicker();
            var result = await _taskSource.Task;
            Close();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = appViewBackButtonBak;
            return result;
        }

        private async Task FocusPicker()
        {
            var element = FocusManager.FindFirstFocusableElement(_rootGrid);
            if (element != null)
                await FocusManager.TryFocusAsync(element, FocusState.Programmatic);
        }

        private void Close()
        {
            if (_rootGrid != null)
            {
                _rootGrid.Tapped -= RootGrid_Tapped;
                _rootGrid.Children.Remove(this);
            }
        }

        #region HookEvent

        private void HookUpEvents()
        {
            Unloaded += ObjectPicker_Unloaded;
            _navigationService.Navigated += Frame_Navigated;
            Window.Current.SizeChanged += Window_SizeChanged;
        }

        private void UnhookEvents()
        {
            Unloaded -= ObjectPicker_Unloaded;
            _navigationService.Navigated -= Frame_Navigated;
            Window.Current.SizeChanged -= Window_SizeChanged;
        }

        private void HookUpViewModelEvents()
        {
            if (ViewModel != null)
            {
                ViewModel.ObjectPicked += ViewModel_ObjectPicked;
                ViewModel.Canceled += ViewModel_Canceled;
            }
        }

        private void UnhookViewModelEvents()
        {
            if (ViewModel != null)
            {
                ViewModel.ObjectPicked -= ViewModel_ObjectPicked;
                ViewModel.Canceled -= ViewModel_Canceled;
            }
        }

        #endregion

        #region PickerEvent handlers

        private void ObjectPicker_Unloaded(object sender, RoutedEventArgs e)
        {
            UnhookViewModelEvents();
            UnhookEvents();
            _popup.IsOpen = false;
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            HookUpViewModelEvents();
        }

        private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            if (_rootGrid != null)
            {
                _rootGrid.Width = Window.Current.Bounds.Width;
                _rootGrid.Height = Window.Current.Bounds.Height;
            }
        }

        private void RootGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (e.OriginalSource == _rootGrid)
                _taskSource.SetResult(new PickResult<T>
                {
                    Canceled = true
                });
        }

        #endregion

        #region ViewModelEvent handlers

        private void ViewModel_ObjectPicked(object sender, ObjectPickedEventArgs<T> e)
        {
            _taskSource.SetResult(new PickResult<T>
            {
                Result = e.Result
            });
        }

        private void ViewModel_Canceled(object sender, EventArgs e)
        {
            _taskSource.SetResult(new PickResult<T>
            {
                Canceled = true
            });
        }

        #endregion
    }
}