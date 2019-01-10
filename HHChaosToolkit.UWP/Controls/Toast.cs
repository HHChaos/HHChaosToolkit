using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;

namespace HHChaosToolkit.UWP.Controls
{
    public sealed class Toast : Control
    {
        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(string), typeof(Toast), new PropertyMetadata(0));

        // Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(Toast),
                new PropertyMetadata(TimeSpan.FromSeconds(2.0)));

        public Toast(string content)
        {
            DefaultStyleKey = typeof(Toast);
            Content = content;
            Width = Window.Current.Bounds.Width;
            Height = Window.Current.Bounds.Height;
            Transitions = new TransitionCollection
            {
                new EntranceThemeTransition()
            };
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        public TimeSpan Duration
        {
            get => (TimeSpan) GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
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

        public async void Show()
        {
            var popup = new Popup
            {
                IsOpen = true,
                Child = this
            };
            await Task.Delay(Duration);
            popup.Child = null;
            popup.IsOpen = false;
            Window.Current.SizeChanged -= Current_SizeChanged;
        }
    }
}