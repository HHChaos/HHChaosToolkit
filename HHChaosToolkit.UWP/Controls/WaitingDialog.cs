﻿using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace HHChaosToolkit.UWP.Controls
{
    public class WaitingDialog : Control
    {
        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(string), typeof(WaitingDialog), new PropertyMetadata(0));

        private readonly Popup _popup;

        private AppViewBackButtonVisibility _appViewBackButtonBak;

        public WaitingDialog(string content)
        {
            DefaultStyleKey = typeof(WaitingDialog);
            Content = content;
            Width = Window.Current.Bounds.Width;
            Height = Window.Current.Bounds.Height;
            _popup = new Popup
            {
                Child = this
            };
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        public string Content
        {
            get => (string) GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            Width = Window.Current.Bounds.Width;
            Height = Window.Current.Bounds.Height;
        }

        public void Show()
        {
            _appViewBackButtonBak =
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Disabled;
            _popup.IsOpen = true;
        }

        public void Close()
        {
            _popup.IsOpen = false;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = _appViewBackButtonBak;
            Window.Current.SizeChanged -= Current_SizeChanged;
        }
    }
}