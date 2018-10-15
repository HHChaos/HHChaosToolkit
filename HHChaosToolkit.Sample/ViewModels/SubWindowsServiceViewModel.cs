using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI.Xaml.Navigation;
using CommonServiceLocator;
using HHChaosToolkit.Sample.ViewModels.TestViewModels;
using HHChaosToolkit.UWP.Mvvm;
using HHChaosToolkit.UWP.Services;
using HHChaosToolkit.UWP.Services.Navigation;

namespace HHChaosToolkit.Sample.ViewModels
{
    public class SubWindowsServiceViewModel : ViewModelBase
    {
        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            ShowAllSubWindows?.Execute(null);
            base.OnNavigatedTo(e);
        }
        public override void OnNavigatedFrom(NavigatedFromEventArgs e)
        {
            HideAllSubWindows?.Execute(null);
            base.OnNavigatedFrom(e);
        }
        public ICommand OpenSampleSubWindowCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var subWinService = ServiceLocator.Current.GetInstance<SubWindowsService>();
                    var key = subWinService.OpenWindow(typeof(TestSampleSubWindowViewModel).FullName, new Point(100, 100));
                });
            }
        }
        public ICommand HideAllSubWindows
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var subWinService = ServiceLocator.Current.GetInstance<SubWindowsService>();
                    subWinService.HideAllWindows();
                });
            }
        }
        public ICommand ShowAllSubWindows
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var subWinService = ServiceLocator.Current.GetInstance<SubWindowsService>();
                    subWinService.ShowAllWindows();
                });
            }
        }
    }
}
