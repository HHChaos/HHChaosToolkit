using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace HHChaosToolkit.UWP.Controls
{
    public sealed class WaitingDialog : Control
    {
        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(string), typeof(WaitingDialog), new PropertyMetadata(0));

        private readonly Popup _popup;

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

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            Width = Window.Current.Bounds.Width;
            Height = Window.Current.Bounds.Height;
        }

        public string Content
        {
            get => (string)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public void Show()
        {
            _popup.IsOpen = true;
        }

        public void Close()
        {
            _popup.IsOpen = false;
            Window.Current.SizeChanged -= Current_SizeChanged;
        }
    }
}
