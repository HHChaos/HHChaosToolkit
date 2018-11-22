using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HHChaosToolkit.UWP.Services.Navigation;
using HHChaosToolkit.UWP.Utilities;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace HHChaosToolkit.UWP.SubWindows
{
    [TemplatePart(Name = LayoutGridName, Type = typeof(Grid))]
    [TemplatePart(Name = ContentFrameName, Type = typeof(Frame))]
    [TemplatePart(Name = TitleGridName, Type = typeof(Grid))]
    [TemplatePart(Name = CloseButtonName, Type = typeof(Button))]
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

        private Grid _layoutGrid;

        private CompositeTransform _layoutGridTransform;
        private NavigationService _navigationService;

        private Grid _titleGrid;

        public SubWindow()
        {
            DefaultStyleKey = typeof(SubWindow);
            Loaded += SubWindow_Loaded;
            Unloaded += SubWindow_Unloaded;
            Closed += SubWindow_Closed;
        }

        private object FrameContent => _navigationService?.Frame?.Content;

        public string Id { get; } = Guid.NewGuid().ToString();

        public double MoveAreaMargin { set; get; } = 20;

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
            if (GetTemplateChild(ContentFrameName) is Frame contentFrame)
                _navigationService = new NavigationService {Frame = contentFrame};
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
            var windowRect = new Rect(0, 0, Window.Current.Bounds.Width - MoveAreaMargin,
                Window.Current.Bounds.Height - MoveAreaMargin);
            var newPoint = new Point(X + e.Delta.Translation.X, Y + e.Delta.Translation.Y);
            if (windowRect.Contains(newPoint))
            {
                X = newPoint.X;
                Y = newPoint.Y;
            }
            else if (windowRect.Contains(new Point(0, newPoint.Y)))
            {
                Y = newPoint.Y;
            }
            else if (windowRect.Contains(new Point(newPoint.X, 0)))
            {
                X = newPoint.X;
            }
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
                if (maxZIndex == 0 || Canvas.GetZIndex(this) < maxZIndex)
                    Canvas.SetZIndex(this, maxZIndex + 1);
            }
        }

        private void SubWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= Current_SizeChanged;
        }

        private void SubWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += Current_SizeChanged;
            Activate();
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            var windowWidth = Window.Current.Bounds.Width - MoveAreaMargin;
            var windowHeight = Window.Current.Bounds.Height - MoveAreaMargin;
            var position = $"{(X < windowWidth ? 'L' : 'R')}{(Y < windowHeight ? 'T' : 'B')}";
            var point = new Point(X, Y);
            switch (position)
            {
                case "LT":
                    //Show();
                    return;
                case "LB":
                    point.Y = windowHeight;
                    break;
                case "RT":
                    point.X = windowWidth;
                    break;
                case "RB":
                    point.X = windowWidth;
                    point.Y = windowHeight;
                    break;
            }

            //Hide();
            X = point.X;
            Y = point.Y;
        }

        public void Close()
        {
            UnhookEvents();
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

            if (_navigationService != null) _navigationService.Navigated += ContentFrame_Navigated;
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
            if (_navigationService != null) _navigationService.Navigated -= ContentFrame_Navigated;
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
            if (FrameContent is Page page)
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

        public bool CanGoBack => _navigationService.CanGoBack;

        public bool CanGoForward => _navigationService.CanGoForward;

        public void GoBack()
        {
            _navigationService.GoBack();
        }

        public void GoForward()
        {
            _navigationService.GoForward();
        }

        public bool Navigate(Type sourcePageType, object parameter = null)
        {
            if (_navigationService == null)
                return false;
            if (FrameContent is Page page)
                if (page.DataContext is ISubWindow viewModel)
                    viewModel.WindowClosed -= ViewModel_WindowClosed;
            return _navigationService.Navigate(sourcePageType, parameter);
        }

        private void ViewModel_WindowClosed(object sender, EventArgs e)
        {
            Close();
        }

        private void SubWindow_Closed(object sender, EventArgs e)
        {
            if (FrameContent is Page page)
                if (page.DataContext is ISubWindow viewModel)
                {
                    viewModel.WindowClosed -= ViewModel_WindowClosed;
                    viewModel.RaiseWindowClosedEvent();
                }

            _navigationService.Navigated -= ContentFrame_Navigated;
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