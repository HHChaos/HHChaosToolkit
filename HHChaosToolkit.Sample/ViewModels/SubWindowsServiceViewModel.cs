using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using CommonServiceLocator;
using HHChaosToolkit.Sample.ViewModels.TestViewModels;
using HHChaosToolkit.UWP.Mvvm;
using HHChaosToolkit.UWP.Services;

namespace HHChaosToolkit.Sample.ViewModels
{
    public class SubWindowsServiceViewModel : ViewModelBase
    {
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
