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
                    var toast = new Toast("This is a sample toast.");
                    toast.Show();
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
                    await Task.Delay(3000);
                    dialog.Close();
                });
            }
        }
    }
}
