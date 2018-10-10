using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HHChaosToolkit.UWP.Utilities;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace HHChaosToolkit.UWP.SubWindows
{
    public sealed class SubWindow : Control
    {
        private const string LayoutGridName = "PART_LayoutGrid";
        private const string ContentFrameName = "PART_ContentFrame";
        private const string TitleGridName = "PART_TitleGrid";
        private const string CloseButtonName = "PART_CloseButton";

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof(object),
                typeof(SubWindow),
                new PropertyMetadata(null));

        private Button _closeButton;

        private Frame _contentFrame;

        private Grid _layoutGrid;

        private CompositeTransform _layoutGridTransform;

        private FrameworkElement _parent;
        private Grid _titleGrid;

        public SubWindow()
        {
            DefaultStyleKey = typeof(SubWindow);
            Loaded += SubWindow_Loaded;
            Unloaded += SubWindow_Unloaded;
            Closed += SubWindow_Closed;
        }

        public string Id { get; } = Guid.NewGuid().ToString();

        public object Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public event EventHandler Closed;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UnhookEvents();
            _layoutGrid = GetTemplateChild(LayoutGridName) as Grid;
            _contentFrame = GetTemplateChild(ContentFrameName) as Frame;
            _titleGrid = GetTemplateChild(TitleGridName) as Grid;
            _closeButton = GetTemplateChild(CloseButtonName) as Button;
            HookUpEvents();
        }

        private void DragSource_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Activate();
        }

        private void DragSource_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            X += e.Delta.Translation.X;
            Y += e.Delta.Translation.Y;
        }

        private void DragSource_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            Activate();
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void Activate()
        {
            var parent = this.GetVisualParent();
            if (parent != null)
            {
                var maxZIndex = parent.GetVisualChildren().Max(Canvas.GetZIndex);
                if (Canvas.GetZIndex(this) < maxZIndex)
                    Canvas.SetZIndex(this, maxZIndex + 1);
            }
        }

        private void SubWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_parent != null)
                _parent.SizeChanged -= OnParentSizeChanged;
            _parent = null;
        }

        private void SubWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _parent = Parent as FrameworkElement;
            if (_parent != null)
                _parent.SizeChanged += OnParentSizeChanged;
        }

        private void OnParentSizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        public void Close()
        {
            var parentPanel = Parent as Panel;
            parentPanel?.Children.Remove(this);
            Closed?.Invoke(this, EventArgs.Empty);
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        #region HookEvent

        private void HookUpEvents()
        {
            if (_layoutGrid != null)
            {
                _layoutGrid.RenderTransformOrigin = new Point(0.5, 0.5);
                _layoutGrid.RenderTransform = _layoutGridTransform = new CompositeTransform();
                _layoutGridTransform.TranslateX = X;
                _layoutGridTransform.TranslateY = Y;
            }

            if (_contentFrame != null) _contentFrame.Navigated += ContentFrame_Navigated;
            var dragSource = _titleGrid;
            if (dragSource != null)
            {
                dragSource.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
                dragSource.Tapped += DragSource_Tapped;
                dragSource.ManipulationDelta += DragSource_ManipulationDelta;
                dragSource.ManipulationStarted += DragSource_ManipulationStarted;
            }

            if (_closeButton != null) _closeButton.Click += OnCloseButtonClick;
        }

        private void UnhookEvents()
        {
            if (_contentFrame != null) _contentFrame.Navigated -= ContentFrame_Navigated;
            var dragSource = _titleGrid;
            if (dragSource != null)
            {
                dragSource.Tapped -= DragSource_Tapped;
                dragSource.ManipulationDelta -= DragSource_ManipulationDelta;
                dragSource.ManipulationStarted -= DragSource_ManipulationStarted;
            }

            if (_closeButton != null) _closeButton.Click -= OnCloseButtonClick;
        }

        #endregion

        #region Navigate ViewModel

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (_contentFrame.Content is Page page)
            {
                var titleBinding = new Binding
                {
                    Mode = BindingMode.OneWay,
                    Path = new PropertyPath("Title"),
                    Source = page.DataContext
                };
                SetBinding(TitleProperty, titleBinding);
                if (page.DataContext is ISubWindow viewModel)
                {
                    viewModel.WindowClosed += ViewModel_WindowClosed;
                    viewModel.SubWinKey = Id;
                }
            }
        }

        public bool CanGoBack => _contentFrame.CanGoBack;

        public bool CanGoForward => _contentFrame.CanGoForward;

        public void GoBack()
        {
            _contentFrame.GoBack();
        }

        public void GoForward()
        {
            _contentFrame.GoForward();
        }

        public bool Navigate(Type sourcePageType, object parameter = null)
        {
            if (_contentFrame.Content is Page page)
                if (page.DataContext is ISubWindow viewModel)
                    viewModel.WindowClosed -= ViewModel_WindowClosed;
            return _contentFrame.Navigate(sourcePageType, parameter);
        }

        private void ViewModel_WindowClosed(object sender, EventArgs e)
        {
            Close();
        }

        private void SubWindow_Closed(object sender, EventArgs e)
        {
            if (_contentFrame.Content is Page page)
                if (page.DataContext is ISubWindow viewModel)
                {
                    viewModel.WindowClosed -= ViewModel_WindowClosed;
                    viewModel.RaiseWindowClosedEvent();
                }

            _contentFrame.Navigated -= ContentFrame_Navigated;
        }

        #endregion

        #region X

        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register(
                "X",
                typeof(double),
                typeof(SubWindow),
                new PropertyMetadata(0d, OnXChanged));

        public double X
        {
            get => (double) GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        private static void OnXChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (SubWindow) d;
            var oldX = (double) e.OldValue;
            var newX = target.X;
            target.OnXChanged(oldX, newX);
        }

        private void OnXChanged(
            double oldX, double newX)
        {
            if (_layoutGridTransform != null) _layoutGridTransform.TranslateX = newX;
        }

        #endregion

        #region Y

        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register(
                "Y",
                typeof(double),
                typeof(SubWindow),
                new PropertyMetadata(0d, OnYChanged));

        public double Y
        {
            get => (double) GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        private static void OnYChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (SubWindow) d;
            var oldY = (double) e.OldValue;
            var newY = target.Y;
            target.OnYChanged(oldY, newY);
        }

        private void OnYChanged(
            double oldY, double newY)
        {
            if (_layoutGridTransform != null) _layoutGridTransform.TranslateY = newY;
        }

        #endregion
    }
}