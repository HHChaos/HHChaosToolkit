using Windows.UI.Xaml.Navigation;
using HHChaosToolkit.UWP.Mvvm;
using HHChaosToolkit.UWP.Services.Navigation;

namespace HHChaosToolkit.Sample.ViewModels.TestViewModels
{
    public class TestNavigationViewModel1 : ViewModelBase
    {
        private string _navigatedSource;
        public string NavigatedSource
        {
            get => _navigatedSource;
            set => Set(ref _navigatedSource, value);
        }

        private string _lastNavigatedTo;
        public string LastNavigatedTo
        {
            get => _lastNavigatedTo;
            set => Set(ref _lastNavigatedTo, value);
        }
        private string _navigatedParameter;
        public string NavigatedParameter
        {
            get => _navigatedParameter;
            set => Set(ref _navigatedParameter, value);
        }
        private bool _canLeaveThisPage=true;
        public bool CanLeaveThisPage
        {
            get => _canLeaveThisPage;
            set => Set(ref _canLeaveThisPage, value);
        }
        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigatedSource = e.SourcePageType.ToString();
            NavigatedParameter = e.Parameter?.ToString();
            base.OnNavigatedTo(e);
        }
        public override void OnNavigatedFrom(NavigatedFromEventArgs e)
        {
            //on navigated to other page
            LastNavigatedTo = e.Content?.ToString();
            base.OnNavigatedFrom(e);
        }
        public override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            e.Cancel = !CanLeaveThisPage;
            base.OnNavigatingFrom(e);
        }
    }
}
