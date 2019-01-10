using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using CommonServiceLocator;
using HHChaosToolkit.Sample.ViewModels.TestViewModels;
using HHChaosToolkit.UWP.Controls;
using HHChaosToolkit.UWP.Mvvm;
using HHChaosToolkit.UWP.Services;
using HHChaosToolkit.Sample.Helpers;

namespace HHChaosToolkit.Sample.ViewModels
{
    public class ToastSampleViewModel : ViewModelBase
    {
        public ICommand ShowSampleToastCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ToastHelper.SendToast("This is a sample toast.");
                });
            }
        }

        public ICommand ShowSampleToastForLongTimeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ToastHelper.SendToast("This is a long time sample toast.",TimeSpan.FromSeconds(5));
                });
            }
        }
        
        public ICommand ShowCustomToastCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ToastHelper.SendCustomToast("This is a custom toast.", TimeSpan.FromSeconds(3));
                });
            }
        }

        public ICommand ShowWaitingDialogCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var dialog = new WaitingDialog("It will disappear in 3 seconds.");
                    dialog.Show();
                    await Task.Delay(1000);
                    dialog.Content = "It will disappear in 2 seconds.";
                    await Task.Delay(1000);
                    dialog.Content = "It will disappear in 1 seconds.";
                    await Task.Delay(1000);
                    dialog.Close();
                });
            }
        }
    }
}
