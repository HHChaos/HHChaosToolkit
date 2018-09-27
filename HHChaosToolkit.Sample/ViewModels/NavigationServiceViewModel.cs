using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using HHChaosToolkit.Sample.ViewModels.TestViewModels;
using HHChaosToolkit.UWP.Mvvm;
using HHChaosToolkit.UWP.Services.Navigation;

namespace HHChaosToolkit.Sample.ViewModels
{
    public class NavigationServiceViewModel : ViewModelBase
    {
        public static readonly string ContentNavigationServiceKey = "NavigationServiceViewModel_Content";

        private string _parameter = "test parameter";

        public NavigationService ContentService =>
            NavigationServiceList.Instance.IsRegistered(ContentNavigationServiceKey)
                ? NavigationServiceList.Instance[ContentNavigationServiceKey]
                : null;

        public string Parameter
        {
            get => _parameter;
            set => Set(ref _parameter, value);
        }

        public ICommand GoPage1Command
        {
            get
            {
                return new RelayCommand(
                    () => { ContentService?.Navigate<TestNavigationViewModel1>(Parameter); });
            }
        }

        public ICommand GoPage2Command
        {
            get
            {
                return new RelayCommand(
                    () => { ContentService?.Navigate<TestNavigationViewModel2>(Parameter); });
            }
        }

        public ICommand GoBackCommand
        {
            get
            {
                return new RelayCommand(
                    () =>
                    {
                        if (ContentService?.CanGoBack == true) ContentService?.GoBack();
                    });
            }
        }

        public ICommand GoForwardCommand
        {
            get
            {
                return new RelayCommand(
                    () =>
                    {
                        if (ContentService?.CanGoForward == true) ContentService?.GoForward();
                    });
            }
        }

        public void Initialize(Frame frame)
        {
            NavigationServiceList.Instance.RegisterOrUpdateFrame(ContentNavigationServiceKey, frame);
            ContentService?.Navigate<TestNavigationViewModel1>(Parameter);
        }
    }
}