using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using HHChaosToolkit.UWP.Mvvm;

namespace HHChaosToolkit.UWP.Services.Navigation
{
    public class NavigationService
    {
        private readonly Dictionary<string, Type> _pages = new Dictionary<string, Type>();

        private Frame _frame;

        public Frame Frame
        {
            get
            {
                if (_frame == null)
                {
                    _frame = Window.Current.Content as Frame;
                    RegisterFrameEvents();
                }

                return _frame;
            }

            set
            {
                UnregisterFrameEvents();
                _frame = value;
                RegisterFrameEvents();
            }
        }

        public bool CanGoBack => Frame.CanGoBack;
        public bool CanGoForward => Frame.CanGoForward;

        public event NavigationFailedEventHandler NavigationFailed;

        public void GoBack()
        {
            var currentPage = _frame.Content as Page;
            Frame.GoBack();
            if (currentPage?.DataContext is INavigable navigable)
                navigable.OnNavigatedFrom(new NavigatedFromEventArgs
                {
                    Content = _frame.Content,
                    NavigationMode = NavigationMode.Back
                });
        }

        public void GoForward()
        {
            var currentPage = _frame.Content as Page;
            Frame.GoForward();
            if (currentPage?.DataContext is INavigable navigable)
                navigable.OnNavigatedFrom(new NavigatedFromEventArgs
                {
                    Content = _frame.Content,
                    NavigationMode = NavigationMode.Forward
                });
        }
        public bool Navigate(string pageKey, object parameter = null, NavigationTransitionInfo infoOverride = null)
        {
            Type page;
            lock (_pages)
            {
                if (!_pages.TryGetValue(pageKey, out page))
                    throw new ArgumentException(
                        string.Format("Page not found: {0}. Did you forget to call NavigationService.Configure?",
                            pageKey), nameof(pageKey));
            }

            return this.Navigate(page, parameter, infoOverride);
        }
        public bool Navigate<T>(object parameter = null, NavigationTransitionInfo infoOverride = null)
            where T : ViewModelBase
            => Navigate(typeof(T).FullName, parameter, infoOverride);

        internal bool Navigate(Type pageType,object parameter = null, NavigationTransitionInfo infoOverride = null)
        {
            if (Frame.Content?.GetType() != pageType || parameter != null)
            {
                var currentPage = _frame.Content as Page;
                var navigationResult = Frame.Navigate(pageType, parameter, infoOverride);
                if (navigationResult && currentPage?.DataContext is INavigable navigable)
                    navigable.OnNavigatedFrom(new NavigatedFromEventArgs
                    {
                        Content = _frame.Content,
                        NavigationMode = NavigationMode.New
                    });

                return navigationResult;
            }

            return false;
        }
        public void Configure(string key, Type pageType)
        {
            lock (_pages)
            {
                if (_pages.ContainsKey(key))
                    throw new ArgumentException(
                        string.Format("The key {0} is already configured in NavigationService", key));

                if (_pages.Any(p => p.Value == pageType))
                    throw new ArgumentException(string.Format("This type is already configured with key {0}",
                        _pages.First(p => p.Value == pageType).Key));

                _pages.Add(key, pageType);
            }
        }

        public string GetNameOfRegisteredPage(Type page)
        {
            lock (_pages)
            {
                if (_pages.ContainsValue(page))
                    return _pages.FirstOrDefault(p => p.Value == page).Key;
                throw new ArgumentException(string.Format("The page '{0}' is unknown by the NavigationService",
                    page.Name));
            }
        }

        private void RegisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigated += Frame_Navigated;
                _frame.Navigating += Frame_Navigating;
                _frame.NavigationFailed += Frame_NavigationFailed;
            }
        }

        private void UnregisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigated -= Frame_Navigated;
                _frame.Navigating -= Frame_Navigating;
                _frame.NavigationFailed += Frame_NavigationFailed;
            }
        }

        private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            var page = _frame?.Content as Page;
            if (page?.DataContext is INavigable navigable) navigable.OnNavigatingFrom(e);
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            var page = e.Content as Page;
            if (page?.DataContext is INavigable navigable) navigable.OnNavigatedTo(e);
        }

        private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            NavigationFailed?.Invoke(sender, e);
        }
    }
}