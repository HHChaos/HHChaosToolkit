using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using HHChaosToolkit.UWP.SubWindows;

namespace HHChaosToolkit.UWP.Services
{
    public class SubWindowsService
    {
        private readonly Dictionary<string, Type> _pages = new Dictionary<string, Type>();
        private readonly Dictionary<string, SubWindow> _subWindows = new Dictionary<string, SubWindow>();
        private readonly Canvas _subWindowsPanel= new Canvas { Name="SubWindowsPanel"};
        private readonly Popup _subWindowsPopup = new Popup();

        public SubWindowsService()
        {
            _subWindowsPanel.ChildrenTransitions = new TransitionCollection
            {
                new PopupThemeTransition()
            };
            _subWindowsPopup.Child = _subWindowsPanel;
            _subWindowsPopup.IsOpen = true;
        }

        public void Configure(string key, Type pageType)
        {
            lock (_pages)
            {
                if (_pages.ContainsKey(key))
                    throw new ArgumentException(string.Format("The key {0} is already configured in SubWindowsService",
                        key));

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
                throw new ArgumentException(string.Format("The page '{0}' is unknown by the SubWindowsService",
                    page.Name));
            }
        }

        public string OpenWindow(string pageKey, Point position, object parameter = null)
        {
            Type page;
            lock (_pages)
            {
                if (!_pages.TryGetValue(pageKey, out page))
                    throw new ArgumentException(
                        string.Format("Page not found: {0}. Did you forget to call SubWindowsService.Configure?",
                            pageKey),
                        nameof(pageKey));
            }

            var subWindow = new SubWindow
            {
                X = position.X,
                Y = position.Y
            };
            _subWindowsPanel.Children.Add(subWindow);
            subWindow.Show();
            subWindow.Loaded += (sender, e) => { subWindow.Navigate(page, parameter); };
            subWindow.Closed += SubWindow_Closed;
            _subWindows.Add(subWindow.Id, subWindow);
            return subWindow.Id;
        }

        private void SubWindow_Closed(object sender, EventArgs e)
        {
            if (sender is SubWindow subWindow && _subWindows.ContainsKey(subWindow.Id))
            {
                subWindow.Closed -= SubWindow_Closed;
                _subWindows.Remove(subWindow.Id);
            }
        }

        public void Navigate(string subWinKey, string pageKey, object parameter = null)
        {
            if (_subWindows.ContainsKey(subWinKey))
            {
                var subWin = _subWindows[subWinKey];
                Type page;
                lock (_pages)
                {
                    if (!_pages.TryGetValue(pageKey, out page))
                        throw new ArgumentException(
                            string.Format("Page not found: {0}. Did you forget to call SubWindowsService.Configure?",
                                pageKey),
                            nameof(pageKey));
                }

                subWin.Navigate(page, parameter);
            }
        }

        /// <summary>
        ///     判断窗口是否存活
        /// </summary>
        /// <param name="subWinKey"></param>
        /// <returns></returns>
        public bool IsAliveWindow(string subWinKey)
        {
            return _subWindows.ContainsKey(subWinKey);
        }

        /// <summary>
        ///     获取窗口坐标
        /// </summary>
        /// <param name="subWinKey"></param>
        /// <returns></returns>
        public Point GetWindowPosition(string subWinKey)
        {
            if (_subWindows.ContainsKey(subWinKey))
            {
                var subWin = _subWindows[subWinKey];
                return new Point(subWin.X, subWin.Y);
            }

            return new Point();
        }

        public void HideAllWindows()
        {
            foreach (var subWin in _subWindows.Values) subWin.Hide();
        }

        public void ShowAllWindows()
        {
            foreach (var subWin in _subWindows.Values) subWin.Show();
        }

        public void CloseWindow(string subWinKey)
        {
            if (_subWindows.ContainsKey(subWinKey))
            {
                var subWin = _subWindows[subWinKey];
                subWin.Close();
            }
        }

        public void GoBack(string subWinKey)
        {
            if (_subWindows.ContainsKey(subWinKey))
            {
                var subWin = _subWindows[subWinKey];
                if (subWin.CanGoBack) subWin.GoBack();
            }
        }

        public void GoForward(string subWinKey)
        {
            if (_subWindows.ContainsKey(subWinKey))
            {
                var subWin = _subWindows[subWinKey];
                if (subWin.CanGoForward) subWin.GoForward();
            }
        }
    }
}
